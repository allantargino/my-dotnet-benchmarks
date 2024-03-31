using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

namespace roslyn_manual_tree_transversal_context_actions.Analyzers.V2;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1036
public sealed class OrchestratorAnalyzer4 : OrchestrationAnalyzer
#pragma warning restore RS1036
{
    public const string DiagnosticId = "DF1104";

#pragma warning disable RS1033, RS2008
    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, "Guid", "Guid", "Guid", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Description");
#pragma warning restore RS1033, RS2008

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    protected override void RegisterAdditionalCompilationStartAction(CompilationStartAnalysisContext context, ConcurrentDictionary<ISymbol, MethodDeclarationSyntax> methodsReachableByOrchestrators)
    {
        INamedTypeSymbol? guidSymbol = context.Compilation.GetTypeByMetadataName(typeof(Guid).FullName);
        Debug.Assert(guidSymbol != null);

        ConcurrentBag<(ISymbol method, IPropertyReferenceOperation operation)> guidUsage = new();

        // search for usages of DateTime.Now, DateTime.UtcNow, DateTime.Today and store them
        context.RegisterOperationAction(ctx =>
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var invocation = (IInvocationOperation)ctx.Operation;
            var targetMethod = invocation.TargetMethod;

            //if (property.ContainingSymbol.Equals(guidSymbol, SymbolEqualityComparer.Default) &&
            //    property.Name is nameof(DateTime.Now) or nameof(DateTime.UtcNow) or nameof(DateTime.Today))
            //{
            //    var method = ctx.ContainingSymbol;
            //    guidUsage.Add((method, operation));
            //}
        }, OperationKind.Invocation);

        // compare whether the found DateTime usages occur in methods reachable by orchestrators
        context.RegisterCompilationEndAction(ctx =>
        {
            foreach (var (method, operation) in guidUsage)
            {
                if (methodsReachableByOrchestrators.ContainsKey(method))
                {
                    ctx.ReportDiagnostic(Rule, operation, operation.Property.ToString()!);
                }
            }
        });
    }
}
