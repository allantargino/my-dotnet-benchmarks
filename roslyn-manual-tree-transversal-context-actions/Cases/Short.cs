using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.DurableTask;

public class Program
{
    public static void Main() { }
}

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

public static class DurableFunction1
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

public static class DurableFunction2
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

public static class DurableFunction3
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

public static class DurableFunction4
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

public static class DurableFunction5
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

public static class DurableFunction6
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

public static class DurableFunction7
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

public static class DurableFunction8
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

public static class DurableFunction9
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