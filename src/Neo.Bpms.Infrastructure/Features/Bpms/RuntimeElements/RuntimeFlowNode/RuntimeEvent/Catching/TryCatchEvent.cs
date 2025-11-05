using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway.RuntimeEventBasedGateway;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
internal class TryCatchEvent<T> where T : EventDefinition
{
    internal static void Try(string eventCode, LocalParameters messageParams,
        ProcessInstance pi, FlowNodeInstance2 ai)
    {
        var catchManager = new TryCatchEvent<T>
        {
            _eventCode = eventCode,
            _pi = pi,
            _ai = ai,
            _inputData = messageParams,
        };
        catchManager.TryingCatchEvent();
    }

    private LocalParameters _inputData;
    private FlowNodeInstance2 _ai;
    private ProcessInstance _pi;
    private string _eventCode;

    public void TryingCatchEvent()
    {
        if (!TryCatchEventInCurrentAndParents())
        {
            if (typeof(T) == typeof(ErrorEventDefinition))
                _pi.CheckAndClosePiAndParentAiIfNotExecuting();
        }
    }

    private bool TryCatchEventInCurrentAndParents()
    {
        var curPi = _pi;
        var curAi = _ai;
        while (curPi != null)
        {
            if (curAi != null)
            {
                if (TryCatchEventBoundary(curPi, curAi))
                    return true;
            }

            if (TryCatchEventSubProcess(curPi, curAi))
                return true;
            curAi = curPi.ParentAi;
            curPi = curAi?.pi;
        }

        return false;
    }

    private bool TryCatchEventBoundary(ProcessInstance pi, FlowNodeInstance ai)
    {
        var catchBoundaryRuntime = FetchBoundaryEvent(pi.Process, ai.FlowNode.Id, pi, out var caughtEventDefinition);
        if (catchBoundaryRuntime != null)
        {
            var catchingEvent = NewCatchingEvent(pi, ai, caughtEventDefinition, catchBoundaryRuntime, true);
            catchingEvent?.CatchAndSavePi();
            return catchingEvent != null;
        }

        return false;
    }

    private bool TryCatchEventSubProcess(ProcessInstance pi, FlowNodeInstance ai)
    {
        FlowNodeRunTime caughtRunTime = null;
        EventDefinition caughtEventDefinition = null;
        var matchEventCode = "";
        foreach (var subProcess in pi.Process.flowElements.Values?.OfType<SubProcess>().Where(sp => sp.triggeredByEvent) ??
                                   [])
        {
            if (subProcess.flowElements == null)
                continue;
            foreach (var subElement in subProcess.flowElements.Values)
            {
                if (!pi.ProcessVersion.TryGetCatchRuntime(subElement.Id, out var subRuntime))
                    continue;
                if (!subRuntime.IsStarting)
                    continue;
                CanBeCatch(subRuntime, ref matchEventCode, pi, out caughtRunTime, out caughtEventDefinition);
            }
        }

        if (caughtRunTime != null && caughtEventDefinition != null)
        {
            var catchingEvent = NewCatchingEvent(pi, ai, caughtEventDefinition, caughtRunTime, false);
            catchingEvent?.Catch();
            return catchingEvent != null;
        }

        return false;
    }

    private CatchingEvent NewCatchingEvent(ProcessInstance pi, FlowNodeInstance ai,
        EventDefinition caughtEventDefinition, FlowNodeRunTime caughtRunTime, bool inBoundary)
    {
        CatchingEvent catchingEvent = null;
        switch (caughtEventDefinition)
        {
            case CancelEventDefinition _:
                if (inBoundary)
                    catchingEvent = new CatchingEvent(caughtRunTime, caughtEventDefinition, pi.Execution, pi, ai, _inputData);
                break;
            case CompensateEventDefinition _:
            case ErrorEventDefinition _:
            case EscalationEventDefinition _:
                catchingEvent = new CatchingEvent(caughtRunTime, caughtEventDefinition, pi.Execution, pi, ai, _inputData);
                break;
            case ConditionalEventDefinition conditionalEventDefinition:
                catchingEvent =
                    new CatchingConditional(caughtRunTime, conditionalEventDefinition, pi.Execution, pi, ai, _inputData);
                break;
            case MessageEventDefinition messageEventDefinition:
                catchingEvent = new CatchingMessage(caughtRunTime, messageEventDefinition, pi.Execution, pi, ai, _inputData);
                break;
            case SignalEventDefinition signalEventDefinition:
                catchingEvent = new CatchingSignal(caughtRunTime, signalEventDefinition, pi.Execution, pi, ai, _inputData);
                break;
        }

        return catchingEvent;
    }

    private CatchRuntime FetchBoundaryEvent(IFlowElementsContainer container, string attachedToRef,
        ProcessInstance pi, out EventDefinition caughtEventDefinition)
    {
        var matchEventCode = "";
        CatchRuntime catchRuntime = null;
        caughtEventDefinition = null;
        foreach (var boundaryEvent in container?.flowElements?.Values
                                          .OfType<BoundaryEvent>()
                                          .Where(b => b.attachedToRef == attachedToRef)
                                      ?? [])
        {
            if (!pi.ProcessVersion.TryGetCatchRuntime(boundaryEvent.Id, out var boundaryEventRuntime))
                continue;
            if (CanBeCatch(boundaryEventRuntime, ref matchEventCode, pi,
                out var catchRuntime1, out var caughtEventDefinition1))
            {
                catchRuntime = (CatchRuntime)catchRuntime1;
                caughtEventDefinition = caughtEventDefinition1;
            }
        }

        return catchRuntime;
    }

    private bool CanBeCatch(FlowNodeRunTime flowNodeRuntime,
        ref string matchEventCode, ProcessInstance pi,
        out FlowNodeRunTime caughtRunTime, out EventDefinition caughtEventDefinition)

    {
        var find = false;
        caughtRunTime = null;
        caughtEventDefinition = null;
        switch (flowNodeRuntime)
        {
            case CatchRuntime catchRuntime:
                foreach (var eventDefinition in catchRuntime.CatchEvent.eventDefinitions?.OfType<T>()
                                                ?? [])
                {
                    switch (eventDefinition)
                    {
                        case ConditionalEventDefinition conditionalEvent:
                            var expressionInInstance = new ExpressionInInstance(pi, null, _inputData, conditionalEvent.condition);
                            if (expressionInInstance.CheckCondition(conditionalEvent.Name))
                            {
                                caughtRunTime = catchRuntime;
                                caughtEventDefinition = eventDefinition;
                                find = true;
                            }
                            break;
                        default:
                            if (MatchEventDefinition(ref matchEventCode, eventDefinition))
                            {
                                caughtRunTime = catchRuntime;
                                caughtEventDefinition = eventDefinition;
                                find = true;
                            }
                            break;
                    }
                }

                if ((catchRuntime.CatchEvent.eventDefinitions?.Count ?? 0) == 0)
                {
                    if (flowNodeRuntime.flowNode.outgoing != null)
                    {
                        foreach (var outgoing in flowNodeRuntime.flowNode.outgoing.Select(seq => seq.targetRef)
                            .Where(o => o is EventBasedGateway || o is IntermediateCatchEvent))
                        {
                            if (!pi.ProcessVersion.TryGetCatchRuntime(outgoing.Id, out var outgoingRuntime))
                                continue;
                            if (CanBeCatch(outgoingRuntime, ref matchEventCode, pi,
                                out var caughtRunTime1, out var caughtEventDefinition1))
                            {
                                caughtRunTime = caughtRunTime1;
                                caughtEventDefinition = caughtEventDefinition1;
                                find = true;
                            }
                        }
                    }
                }

                break;
            case EventBasedGatewayRuntime _:
                if (flowNodeRuntime.flowNode.outgoing != null)
                {
                    foreach (var outgoing in flowNodeRuntime.flowNode.outgoing.Select(seq => seq.targetRef))
                    {
                        if (!pi.ProcessVersion.TryGetCatchRuntime(outgoing.Id, out var outgoingRuntime))
                            continue;
                        if (CanBeCatch(outgoingRuntime, ref matchEventCode, pi,
                            out var caughtRunTime1, out var caughtEventDefinition1))
                        {
                            caughtRunTime = caughtRunTime1;
                            caughtEventDefinition = caughtEventDefinition1;
                            find = true;
                        }
                    }
                }

                break;
            case ReceiveTaskRuntime receiveTaskRuntime:
                if (typeof(T) != typeof(MessageEventDefinition))
                    break;
                if (MatchEventDefinition(ref matchEventCode, receiveTaskRuntime.MessageDefinition))
                {
                    caughtRunTime = receiveTaskRuntime;
                    caughtEventDefinition = receiveTaskRuntime.MessageDefinition;
                    find = true;
                }
                break;
        }

        return find;
    }

    private bool MatchEventDefinition(ref string matchEventCode,
        EventDefinition eventDefinition)
    {
        var eventDefCode = eventDefinition.Code;
        if (_eventCode.StartsWith(eventDefCode))
        {
            if (matchEventCode.Length < eventDefCode.Length)
            {
                matchEventCode = eventDefCode;
                return true;
            }
        }

        return false;
    }
}
