using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Hosting;

namespace FuncPlayground;

public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .Build();

        host.Run();
    }
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
