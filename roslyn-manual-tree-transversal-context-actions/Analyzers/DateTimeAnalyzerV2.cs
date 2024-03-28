using Microsoft.Azure.Functions.Worker;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

namespace roslyn_manual_tree_transversal_context_actions.Analyzers;

/// <summary>
/// Provides a set of well-known types that are used by the analyzers.
/// Inspired by KnownTypeSymbols class in
/// <see href="https://github.com/dotnet/runtime/blob/2a846acb1a92e811427babe3ff3f047f98c5df02/src/libraries/System.Text.Json/gen/Helpers/KnownTypeSymbols.cs">System.Text.Json.SourceGeneration source code</see>.
/// </summary>
internal sealed class KnownTypeSymbols
{
    private readonly Compilation compilation;

    public KnownTypeSymbols(Compilation compilation)
    {
        this.compilation = compilation;
    }
    public INamedTypeSymbol? OrchestrationTriggerAttribute => GetOrResolveType(typeof(OrchestrationTriggerAttribute).FullName!, ref orchestrationTriggerAttribute);
    private Optional<INamedTypeSymbol?> orchestrationTriggerAttribute;

    private INamedTypeSymbol? GetOrResolveType(string fullyQualifiedName, ref Optional<INamedTypeSymbol?> field)
    {
        if (field.HasValue)
            return field.Value;

        INamedTypeSymbol? type = compilation.GetTypeByMetadataName(fullyQualifiedName);
        field = new(type);
        return type;
    }

    private readonly struct Optional<T>
    {
        public readonly bool HasValue;
        public readonly T Value;

        public Optional(T value)
        {
            HasValue = true;
            Value = value;
        }
    }
}

public abstract class OrchestrationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Register additional actions to be executed after the compilation has started.
    /// It is expected from a concrete implementation of <see cref="OrchestrationAnalyzer"/> to register a
    /// <see cref="CompilationStartAnalysisContext.RegisterCompilationEndAction"/>
    /// and then compare the found violations happened in any of the methods in <paramref name="methodsReachableByOrchestrations"/>.
    /// </summary>
    /// <param name="context">Context originally provided by <see cref="AnalysisContext.RegisterCompilationAction"/></param>
    /// <param name="methodsReachableByOrchestrations">Collection of Orchestration methods or methods that are invoked by them</param>
    protected abstract void RegisterAdditionalCompilationStartAction(CompilationStartAnalysisContext context, ConcurrentDictionary<ISymbol, MethodDeclarationSyntax> methodsReachableByOrchestrations);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(context =>
        {
            var knownSymbols = new KnownTypeSymbols(context.Compilation);

            if (knownSymbols.OrchestrationTriggerAttribute == null)
                return;

            ConcurrentDictionary<ISymbol, MethodDeclarationSyntax> methodsReachableByOrchestrations = new(SymbolEqualityComparer.Default);

            context.RegisterSyntaxNodeAction(ctx =>
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                // Checks whether the declared method is an orchestration
                ISymbol? methodSymbol = ctx.ContainingSymbol;
                if (!methodSymbol.ContainsAttributeInAnyMethodArguments(knownSymbols.OrchestrationTriggerAttribute))
                    return;

                var methodNode = (MethodDeclarationSyntax)ctx.Node;

                var added = methodsReachableByOrchestrations.TryAdd(methodSymbol!, methodNode);
                Debug.Assert(added, "an orchestration method declaration should not be visited twice");

                FindAndAddInvokedMethods(ctx.SemanticModel, methodNode, methodsReachableByOrchestrations);
            }, SyntaxKind.MethodDeclaration);

            // allows concrete implementations to register specific actions/analysis and then compare against methodsReachableByOrchestrations
            RegisterAdditionalCompilationStartAction(context, methodsReachableByOrchestrations);
        });
    }

    private void FindAndAddInvokedMethods(SemanticModel semanticModel, MethodDeclarationSyntax callerSyntax, ConcurrentDictionary<ISymbol, MethodDeclarationSyntax> methodsReachableByOrchestrations)
    {
        foreach (var invocation in callerSyntax.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var calleOperation = semanticModel.GetOperation(invocation);
            if (calleOperation is not IInvocationOperation calleInvocation)
                continue;

            var calleeSymbol = calleInvocation.TargetMethod;
            if (calleeSymbol == null)
                continue;

            // iterating over multiple syntax references is needed because the same method can be declared in multiple places (e.g. partial classes)
            foreach (var calleeSyntax in calleeSymbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax()).OfType<MethodDeclarationSyntax>())
            {
                // recursion base case: if the method was previously visited, skip it
                if (!methodsReachableByOrchestrations.TryAdd(calleeSymbol, calleeSyntax))
                    continue;

                FindAndAddInvokedMethods(semanticModel, calleeSyntax, methodsReachableByOrchestrations);
            }
        }
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DateTimeOrchestratorAnalyzer : OrchestrationAnalyzer
{
    public const string DiagnosticId = "DF1101";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, "Orchestrator", "Orchestrator", "Orchestrator", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "DateTime. Blah.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override void RegisterAdditionalCompilationStartAction(CompilationStartAnalysisContext context, ConcurrentDictionary<ISymbol, MethodDeclarationSyntax> methodsReachableByOrchestrators)
    {
        INamedTypeSymbol dateTimeSymbol = context.Compilation.GetSpecialType(SpecialType.System_DateTime);

        ConcurrentBag<(ISymbol method, IPropertyReferenceOperation operation)> dateTimeUsage = new();

        // search for usages of DateTime.Now, DateTime.UtcNow, DateTime.Today and store them
        context.RegisterOperationAction(ctx =>
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var operation = (IPropertyReferenceOperation)ctx.Operation;
            var property = operation.Property;

            if (property.ContainingSymbol.Equals(dateTimeSymbol, SymbolEqualityComparer.Default) &&
                property.Name is nameof(DateTime.Now) or nameof(DateTime.UtcNow) or nameof(DateTime.Today))
            {
                var method = ctx.ContainingSymbol;
                dateTimeUsage.Add((method, operation));
            }
        }, OperationKind.PropertyReference);

        // compare whether the found DateTime usages occur in methods reachable by orchestrators
        context.RegisterCompilationEndAction(ctx =>
        {
            foreach (var (method, operation) in dateTimeUsage)
            {
                if (methodsReachableByOrchestrators.ContainsKey(method))
                {
                    ctx.ReportDiagnostic(Rule, operation, operation.Property.ToString());
                }
            }
        });
    }
}

public static class RoslynExtensions
{
    //public static bool ContainsAttributeInAnyMethodArguments(this ISymbol? symbol, INamedTypeSymbol attributeSymbol)
    //{
    //    if (symbol is not IMethodSymbol methodSymbol)
    //        return false;


    //    var attributes = methodSymbol.Parameters.SelectMany(p => p.GetAttributes());
    //    foreach (var attribute in attributes)
    //    {
    //        var equal1 = attribute.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default);
    //        var equal2 = attribute.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.IncludeNullability);
    //    }

    //    return false;

    //    //return methodSymbol.Parameters
    //    //    .SelectMany(p => p.GetAttributes())
    //    //    .Any(a => a.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default));
    //}

    public static bool ContainsAttributeInAnyMethodArguments(this ISymbol? symbol, INamedTypeSymbol attributeSymbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            return false;

        return methodSymbol.Parameters
            .SelectMany(p => p.GetAttributes())
            .Any(a => a.AttributeClass!.Equals(attributeSymbol, SymbolEqualityComparer.Default));
    }


    public static void ReportDiagnostic(this CompilationAnalysisContext ctx, DiagnosticDescriptor descriptor, IOperation operation, params string[] messageArgs)
    {
        ctx.ReportDiagnostic(BuildDiagnostic(descriptor, operation.Syntax, messageArgs));
    }

    private static Diagnostic BuildDiagnostic(DiagnosticDescriptor descriptor, SyntaxNode syntaxNode, params string[] messageArgs)
    {
        return Diagnostic.Create(descriptor, syntaxNode.GetLocation(), messageArgs);
    }
}
