using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;

public static class DurableFunction0
{
    [Microsoft.Azure.Functions.Worker.Function(nameof(OrchestratorIsolated))]
    public static Task OrchestratorIsolated([Microsoft.Azure.Functions.Worker.OrchestrationTrigger] TaskOrchestrationContext context)
    {
        DateTime now = DateTime.Now;
        return Task.CompletedTask;
    }

    [FunctionName(nameof(OrchestratorInProc))]
    public static Task OrchestratorInProc([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        DateTime now = DateTime.Now;
        return Task.CompletedTask;
    }
}
