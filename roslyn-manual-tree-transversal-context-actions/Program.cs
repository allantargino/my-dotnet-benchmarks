using BenchmarkDotNet.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.DurableTask;
using roslyn_manual_tree_transversal_context_actions.Analyzers;
using System.Reflection;

namespace roslyn_manual_tree_transversal_context_actions
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<RoslynBenchmark>();

            const string sourceCode = @"
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace FuncPlayground;

public class Program
{
    public static void Main() {}
}

public static class DurableFunction
{
    [Function(nameof(Orchestrator))]
    public static Task Orchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        DateTime now = DateTime.Now;
        return Task.CompletedTask;
    }
}
";


            //var projectName = "Test";
            //var projectId = ProjectId.CreateNewId(projectName);

            ////MSBuildLocator.RegisterDefaults();  //select the recent SDK

            ////var workspace = MSBuildWorkspace.Create();
            //var workspace = new AdhocWorkspace();
            //var solution = workspace.CurrentSolution
            //    .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
            //    .AddDocument(DocumentId.CreateNewId(projectId), "Program.cs", sourceCode);




            //var referencedAssemblies = Assembly.GetEntryAssembly()!.GetReferencedAssemblies();


            //foreach (var referencedAssembly in referencedAssemblies)
            //{
            //    var loadedAssembly = Assembly.Load(referencedAssembly);
            //    references.Add(MetadataReference.CreateFromFile(loadedAssembly.Location));
            //}

            //var mscorlib = @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\8.0.3\";
            //var coreDir = Directory.GetParent(mscorlib);
            //references.Add(MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "mscorlib.dll"));

            //foreach (var reference in references)
            //{
            //    solution = solution.AddMetadataReference(projectId, reference);
            //}

            //var project = solution.GetProject(projectId);
            //Debug.Assert(project != null);

            //var compilation = await project.GetCompilationAsync();
            //Debug.Assert(compilation != null);

            //var c = compilation.WithAnalyzers([new DateTimeOrchestratorAnalyzer()]);

            //var diagsFromAnalyzers = await c.GetAnalyzerDiagnosticsAsync();

            //var diags = await c.GetAllDiagnosticsAsync();


            SyntaxTree code = CSharpSyntaxTree.ParseText(sourceCode);

            //MetadataReference corLib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),                         // mscorlib
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),   // netstandard
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=8.0.0.0").Location),
                MetadataReference.CreateFromFile(typeof(FunctionAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OrchestrationTriggerAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(TaskOrchestrationContext).Assembly.Location),
            };

            //var referencedAssemblies = Assembly.GetEntryAssembly()!.GetReferencedAssemblies();
            //foreach (var referencedAssembly in referencedAssemblies)
            //{
            //    var loadedAssembly = Assembly.Load(referencedAssembly);
            //    references.Add(MetadataReference.CreateFromFile(loadedAssembly.Location));
            //}

            CSharpCompilation compilation2 = CSharpCompilation.Create("MyAssembly", [code], references);

            var c2 = compilation2.WithAnalyzers([new DateTimeOrchestratorAnalyzer()]);
            var d1 = await c2.GetAnalyzerDiagnosticsAsync();
            var d2 = await c2.GetAllDiagnosticsAsync();
        }
    }
}

//public class SampleDiagnosticAnalyzer : DiagnosticAnalyzer
//{
//    public const string DiagnosticId = "Regex";
//    internal const string Title = "Regex error parsing string argument";
//    internal const string MessageFormat = "Regex error {0}";
//    internal const string Description = "Regex patterns should be syntactically valid.";
//    internal const string Category = "Syntax";

//    internal static DiagnosticDescriptor Rule =
//        new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

//    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

//    public override void Initialize(AnalysisContext context)
//    {
//        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
//        context.EnableConcurrentExecution();

//        context.RegisterSyntaxNodeAction(c =>
//        {
//            var diagnostic = Diagnostic.Create(Rule, c.Node.GetLocation(), "args from allan");
//            c.ReportDiagnostic(diagnostic);
//        }, SyntaxKind.ClassDeclaration);
//    }
//}

public class RoslynBenchmark
{
    [Params(
        "Short.cs",
        "Medium.cs",
        "Large.cs")]
    public string SourceCode { get; set; } = string.Empty;

    [GlobalSetup]
    public void GlobalSetup()
    {

    }


    [IterationSetup]
    public void IterationSetup()
    {

    }

    [IterationCleanup]
    public void IterationCleanup()
    {

    }


    [Benchmark(Baseline = true)]
    public void UsingManualTreeTransversal()
    {

    }

    [Benchmark]
    public void UsingContextActions()
    {

    }
}
