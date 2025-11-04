using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class CatchingConditional : CatchingEvent
{
    private ConditionalEventDefinition _conditional => EventDefinition as ConditionalEventDefinition;

    internal CatchingConditional(FlowNodeRunTime flowNodeRuntime,
        ConditionalEventDefinition conditionalDefinition, ExecutionInstance execution,
        ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
        : base(flowNodeRuntime, conditionalDefinition, execution, pi, ai, inputData)
    {
    }

    protected override bool CatchingInStartEvent()
    {
        if (_conditional.condition is not FormalExpression condition || condition.Expression == null)
            return false;
        if (!flowNode.InSubProcess)
        {
            var q = DataStorage.QueryProcessData(ProcessVersion.definition);
            q.Where(condition.Expression.ExpressionString);
            q.ForEach(InputData, r =>
            {
                Pi = CreateProcessInstanceAndCatching(null, r.Id.ToString());
            });
        }
        else
        {
            var entityPkv = ProcessVersion.FetchEntityPkv(InputData);
            var pis = DataStorage.LoadProcessInstances(ProcessVersion, Execution, entityPkv);
            if (pis != null)
            {
                foreach (var pi in pis.Values)
                {
                    var expressionInInstance = new ExpressionInInstance(pi, null, InputData, _conditional.condition);
                    if (expressionInInstance.CheckCondition(_conditional.Code))
                    {
                        try
                        {
                            Pi = DataStorage.LockControl(pi, "catch Conditional in event sub process start", out _);
                            CatchingInStartEventInPi();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
        }

        return Pi != null;
    }

    protected override bool CatchingFlowNode(FlowNodeRunTime flowNodeRuntime)
    {
        return CatchCorrelatedFlowInstances(flowNodeRuntime,
            $"Catching condition in {_conditional.Code} in {flowNodeRuntime.flowNode.GetType().Name} {flowNodeRuntime.flowNode.Id}");
    }

    protected override bool CatchingInBoundaryEvent(BoundaryEvent boundaryEvent)
    {
        return ProcessVersion.TryGetActivityRuntime(boundaryEvent.attachedToRef, out var activityRunTime) &&
               CatchCorrelatedFlowInstances(activityRunTime,
                   $"Catching condition in boundaryEvent {boundaryEvent.Name} in {_conditional.Code} {activityRunTime.flowNode.Id}");
    }

    private bool CatchCorrelatedFlowInstances(FlowNodeRunTime runTime, string traceCode)
    {
        var ais = DataStorage.CorrelateFlowNodeInstances(Execution, runTime,
            new LocalParameters(Execution.AuditTrail.User));
        foreach (var ai in ais)
        {
            var expressionInInstance = new ExpressionInInstance(ai.pi, ai, InputData, _conditional.condition);
            if (expressionInInstance.CheckCondition(_conditional.Code))
            {
                Ai = DataStorage.LockControl(ai, traceCode);
                Pi = Ai?.pi;
                if (Pi == null)
                    continue;
                CatchAndSavePi();
            }
        }

        return true;
    }
}