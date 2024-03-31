using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace roslyn_manual_tree_transversal_context_actions.Analyzers.V2;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1036
public sealed class DateTimeOrchestratorAnalyzerV2 : OrchestrationAnalyzer
#pragma warning restore RS1036
{
    public const string DiagnosticId = "DF1101";

#pragma warning disable RS1033, RS2008
    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, "Orchestrator", "Orchestrator", "Orchestrator", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "Description");
#pragma warning restore RS1033, RS2008

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
                    ctx.ReportDiagnostic(Rule, operation, operation.Property.ToString()!);
                }
            }
        });
    }
}
