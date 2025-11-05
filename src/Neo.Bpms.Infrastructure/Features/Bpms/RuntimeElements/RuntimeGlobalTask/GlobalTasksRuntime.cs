using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;


namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeGlobalTask;

public class GlobalTaskRunTime
{
    public Dictionary<string, GlobalTaskVersionRuntime> versions = [];
}
public abstract class GlobalTaskVersionRuntime : CallableRunTime
{
    public GlobalTask defintion;
    public Repository repository;
    internal override void WithdrawObsolete()
    {
        //var auditTrail = new AuditTrail(TriggerTypeId.Revision, bpmnDefinitions.name, null);
        //DataStorage.SaveAudit(auditTrail);
    }
    internal abstract void StartActivity(ActivityInstance ai, LocalParameters inputData);

    internal void SendDataToOutputs(ActivityInstance ai, LocalParameters outputData)
    {
        if (ai.pi.ParentAi == null) return;
        var properties = (defintion.Parent as Process)?.properties;
        if (properties != null)
        {
            foreach (var item in outputData)
            {
                if (properties.Any(p => p.Name == item.Key))
                {
                    ai.pi.SetData(item.Key, item.Value);
                }
            }
        }
        DataStorage.SaveProcessInstance(ai.pi, ai, "GTR.0", true, DataStorage.LockChangeRequest.NoChange);
        ai.pi.ParentAi.ActivityRuntime?.RunDataOutputAssociations(ai.pi.ParentAi.pi, ai.pi.ParentAi, outputData);
    }
    internal void ActivityInstanceCompleted(ActivityInstance ai, LocalParameters outputData)
    {
        ai.Save("ActivityInstanceCompleted");
        ai.pi.ParentAi?.ActivityRuntime?.Complete(ai.pi.ParentAi, outputData, FlowNodeRunTime.TokenPattern.Parallel);
    }
}

public class GlobalManualTaskRuntime : GlobalTaskVersionRuntime
{
    public GlobalManualTask taskDefinition;
    internal override void StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var outputData = Run(ai, inputData);
        SendDataToOutputs(ai, outputData);
        ActivityInstanceCompleted(ai, outputData);
    }

    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        return inputData;
    }

}

public class GlobalScriptTaskRuntime : GlobalTaskVersionRuntime
{
    public GlobalScriptTask taskDefinition;
    private ExpressionNode scriptExpressionTree;
    internal override void StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var outputData = Run(ai, inputData);
        SendDataToOutputs(ai, outputData);
        ActivityInstanceCompleted(ai, outputData);
    }
    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        scriptExpressionTree ??= Parser.Parse(taskDefinition.script);
        var data = ai.pi.Data.Clone();
        data.SetField("ai", ai.Data);
        var lc = new LocalParameters(ai.pi.AuditTrail.User);
        lc.Set(inputData);
        scriptExpressionTree.Eval(data, lc);
        return inputData;
    }
}

public class GlobalUserTaskRuntime : GlobalTaskVersionRuntime
{
    public GlobalUserTask taskDefinition;
    internal override void StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
    }
}
public class GlobalBusinessRuleTaskRuntime : GlobalTaskVersionRuntime
{
    public GlobalBusinessRuleTask taskDefinition;
    internal override void StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        var outputData = Run(ai, inputData);
        SendDataToOutputs(ai, outputData);
        ActivityInstanceCompleted(ai, outputData);
    }
    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        //TODO: integration between process and rule engine.../service/url/
        return inputData;
    }
}
