using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Task = System.Threading.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

public class ScriptTaskRuntime(ProcessVersionRuntime processVersion, ScriptTask task) : TaskRuntime(processVersion, task)
{
    public ScriptTask TaskDefinition => Activity as ScriptTask;
    private ExpressionNode _scriptExpressionTree;

    internal override async Task StartActivity(ActivityInstance ai, LocalParameters inputData)
    {
        await Task.Run(() =>
        {
            var outputData = Run(ai, inputData);
            Complete(ai, outputData);
        });
    }

    private LocalParameters Run(ActivityInstance ai, LocalParameters inputData)
    {
        _scriptExpressionTree ??= Parser.Parse(TaskDefinition.script);
        if (_scriptExpressionTree == null) return inputData;
        var data = ai.pi.Data.Clone();
        data.SetField("ai", ai.Data);

        //scriptExpressionTree.eval(pi.data, inputData);

        inputData ??= [];
        inputData.AddOrUpdate("q", data);
        try
        {
            var apply = new ApplyUtility(ai.pi.ModelId, ai.pi.EntityId, ai.pi.AuditTrail, inputData);
            apply.ApplyExpression(_scriptExpressionTree);
        }
        catch (Exception)
        {
            // ignored
        }

        return inputData;
    }
}
