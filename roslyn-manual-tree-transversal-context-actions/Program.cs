using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using roslyn_manual_tree_transversal_context_actions.Analyzers.V2;
using System.Collections.Immutable;
using System.Reflection;

namespace roslyn_manual_tree_transversal_context_actions;

internal class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<RoslynBenchmark>();

        //string sourceCode = File.ReadAllText("Cases/Large.cs");

        //SyntaxTree code = CSharpSyntaxTree.ParseText(sourceCode);

        //var references = new List<MetadataReference>
        //{
        //    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),                         // mscorlib
        //    MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),   // netstandard
        //    MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=8.0.0.0").Location),
        //    MetadataReference.CreateFromFile(typeof(FunctionAttribute).Assembly.Location),
        //    MetadataReference.CreateFromFile(typeof(Microsoft.Azure.Functions.Worker.OrchestrationTriggerAttribute).Assembly.Location),
        //    MetadataReference.CreateFromFile(typeof(Microsoft.DurableTask.TaskOrchestrationContext).Assembly.Location),
        //    MetadataReference.CreateFromFile(typeof(Microsoft.Azure.WebJobs.Extensions.DurableTask.OrchestrationTriggerAttribute).Assembly.Location),
        //    MetadataReference.CreateFromFile(typeof(Microsoft.Azure.WebJobs.Extensions.DurableTask.IDurableOrchestrationContext).Assembly.Location),
        //};

        //CSharpCompilation compilation = CSharpCompilation.Create("MyAssembly", [code], references);

        //ImmutableArray<DiagnosticAnalyzer> analyzers = [new DateTimeOrchestratorAnalyzerV2(), new DeterministicMethodAnalyzer()];
        //CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        //ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
public class RoslynBenchmark
{
    [Params(
        "Cases/Short.cs",
        "Cases/Medium.cs",
        "Cases/Large.cs")]
    public string SourceCodeFilePath { get; set; } = string.Empty;

    private CSharpCompilation compilation = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        string sourceCode = File.ReadAllText(SourceCodeFilePath);

        SyntaxTree code = CSharpSyntaxTree.ParseText(sourceCode);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),                         // mscorlib
            MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),   // netstandard
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=8.0.0.0").Location),
            MetadataReference.CreateFromFile(typeof(FunctionAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Azure.Functions.Worker.OrchestrationTriggerAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.DurableTask.TaskOrchestrationContext).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Azure.WebJobs.Extensions.DurableTask.OrchestrationTriggerAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.Azure.WebJobs.Extensions.DurableTask.IDurableOrchestrationContext).Assembly.Location),
        };

        compilation = CSharpCompilation.Create("MyAssembly", [code], references);
    }


    [Benchmark(Baseline = true)]
    public async Task UsingManualTreeTransversal()
    {
        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers([new DeterministicMethodAnalyzer()]);
        ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    [Benchmark]
    public async Task UsingContextActions()
    {
        ImmutableArray<DiagnosticAnalyzer> analyzers = [
            new DateTimeOrchestratorAnalyzerV2(),
            new OrchestratorAnalyzer2(),
            new OrchestratorAnalyzer3(),
            new OrchestratorAnalyzer4(),
            new OrchestratorAnalyzer5(),
            new OrchestratorAnalyzer6(),
            new OrchestratorAnalyzer7(),
            new OrchestratorAnalyzer8(),
            new OrchestratorAnalyzer9(),
            new OrchestratorAnalyzer10()];
        CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
