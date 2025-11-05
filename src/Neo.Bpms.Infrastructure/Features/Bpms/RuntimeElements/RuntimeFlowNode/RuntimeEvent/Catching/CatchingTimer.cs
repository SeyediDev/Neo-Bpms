using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class CatchingTimer : CatchingEvent
{
    private TimerEventDefinition _timer => EventDefinition as TimerEventDefinition;

    internal CatchingTimer(FlowNodeRunTime flowNodeRuntime,
        TimerEventDefinition timerDefinition, ExecutionInstance execution,
        ProcessInstance pi, FlowNodeInstance ai, LocalParameters inputData)
        : base(flowNodeRuntime, timerDefinition, execution, pi, ai, inputData)
    {
    }

    internal static ProcessInstance TimerReceived(TimerCatchRuntime timerCatchRuntime)
    {
        var auditTrail = new AuditTrail(TriggerTypeId.Timer, timerCatchRuntime.CatchEvent.Name);
        var execution = new ExecutionInstance(auditTrail, null);
        var catchingTimer = new CatchingTimer(timerCatchRuntime, timerCatchRuntime.TimerEventDefinition,
            execution, null, null, null);
        if (catchingTimer.EventReceived(timerCatchRuntime.CatchEvent.Location))
        {
            DataStorage.SaveAudit(auditTrail);
            return catchingTimer.Pi;
        }

        return null;
    }

    protected override bool CatchingInStartEvent()
    {
        if (!flowNode.InSubProcess)
        {
            Pi = CreateProcessInstanceAndCatching((flowNode as IDataOutputContainer)?.dataOutputs, null);
        }
        else
        {
            var entityPkv = ProcessVersion.FetchEntityPkv(InputData);
            var pis = DataStorage.LoadProcessInstances(ProcessVersion, Execution, entityPkv);
            if (pis != null)
            {
                foreach (var pi in pis.Values)
                {
                    {
                        try
                        {
                            Pi = DataStorage.LockControl(pi, "catch Timer in event sub process start", out _);
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
            $"Catching condition in {_timer.Code} in {flowNodeRuntime.flowNode.GetType().Name} {flowNodeRuntime.flowNode.Id}");
    }

    protected override bool CatchingInBoundaryEvent(BoundaryEvent boundaryEvent)
    {
        return ProcessVersion.TryGetActivityRuntime(boundaryEvent.attachedToRef, out var activityRunTime) &&
               CatchCorrelatedFlowInstances(activityRunTime,
                   $"Catching condition in boundaryEvent {boundaryEvent.Name} in {_timer.Code} {activityRunTime.flowNode.Id}");
    }

    private bool CatchCorrelatedFlowInstances(FlowNodeRunTime runTime, string traceCode)
    {
        var ais = DataStorage.CorrelateFlowNodeInstances(Execution, runTime,
            new LocalParameters(Execution.AuditTrail.User));
        foreach (var ai in ais)
        {
            if (CheckTimeDuration(ai.pi, ai))
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

    private bool CheckTimeDuration(ProcessInstance pi, FlowNodeInstance ai)
    {
        var c = new TimerInInstance(pi, ai, _timer?.dueDuration);
        return c.CheckTimeDuration(DateTime.Now);
    }
}
