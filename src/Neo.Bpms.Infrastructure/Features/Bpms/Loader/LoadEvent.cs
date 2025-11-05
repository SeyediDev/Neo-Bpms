using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Infrastructure.Features.Bpms.Loader.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Throw;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    private FlowNodeRunTime AddThrowEvent(ProcessVersionRuntime processVersion, ThrowEvent throwEvent)
    {
        return new ThrowRuntime(processVersion, throwEvent);
    }

    private void AddCatchEvent(ProcessVersionRuntime processVersion, CatchEvent catchEvent)
    {
        foreach (EventDefinition eventDefinition in catchEvent.eventDefinitions ?? Enumerable.Empty<EventDefinition>())
        {
            if (eventDefinition is not ICatchEventDefinition)
            {
                continue;
            }

            FlowNodeRunTime result = null;
            switch (eventDefinition)
            {
                case CancelEventDefinition cancelEventDefinition:
                    if (catchEvent is BoundaryEvent boundary && boundary.cancelActivity)
                    {
                        result = new CatchRuntime(processVersion, catchEvent);
                    }
                    else
                    {
                        processVersion.AddError(
                            $"dont use {cancelEventDefinition.Code} in {catchEvent.Id} in process {processVersion.definition.Id}",
                            catchEvent.Id, "13.1.0", "cancel event can used in interrupting boundary event.");
                    }

                    break;
                case ConditionalEventDefinition conditionalEventDefinition:
                    ConditionalCatchRuntime condition =
                        new(processVersion, catchEvent, conditionalEventDefinition);
                    result = condition;
                    break;
                case ErrorEventDefinition errorEventDefinition:
                    if ((catchEvent is BoundaryEvent boundaryE && boundaryE.cancelActivity) ||
                        IsStartInEventSubProcess(catchEvent, true))
                    {
                        result = new CatchRuntime(processVersion, catchEvent);
                    }
                    else
                    {
                        processVersion.AddError(
                            $"dont use {errorEventDefinition.Code} in {catchEvent.Id} in process {processVersion.definition.Id}",
                            catchEvent.Id, "13.1.1",
                            "error event can used in interrupting boundary event or interrupting start in event sub process.");
                    }

                    break;
                case CompensateEventDefinition compensateEventDefinition:
                    if ((catchEvent is BoundaryEvent boundaryC && boundaryC.cancelActivity) ||
                        IsStartInEventSubProcess(catchEvent, true))
                    {
                        result = new CatchRuntime(processVersion, catchEvent);
                    }
                    else
                    {
                        processVersion.AddError(
                            $"dont use {compensateEventDefinition.Code} in {catchEvent.Id} in process {processVersion.definition.Id}",
                            catchEvent.Id, "13.1.2",
                            "escalation event can used in boundary event or start in event sub process.");
                    }

                    break;
                case EscalationEventDefinition escalationEventDefinition:
                    if (catchEvent is BoundaryEvent || IsStartInEventSubProcess(catchEvent, false))
                    {
                        result = new CatchRuntime(processVersion, catchEvent);
                    }
                    else
                    {
                        processVersion.AddError(
                            $"dont use {escalationEventDefinition.Code} in {catchEvent.Id} in process {processVersion.definition.Id}",
                            catchEvent.Id, "13.1.3",
                            "escalation event can used in boundary event or start in event sub process.");
                    }

                    break;
                case LinkEventDefinition linkEventDefinition:
                    if (catchEvent is IntermediateCatchEvent)
                    {
                        result = new CatchRuntime(processVersion, catchEvent);
                    }
                    else
                    {
                        processVersion.AddError(
                            $"dont use {linkEventDefinition.Code} in {catchEvent.Id} in process {processVersion.definition.Id}",
                            catchEvent.Id, "13.1.4", "link event can used in intermediate catch event.");
                    }

                    break;
                case MessageEventDefinition messageEventDefinition:
                    MessageCatchRuntime messageRuntime = new(processVersion, catchEvent, messageEventDefinition);
                    if (!AddToCatchMessage(messageRuntime.MessageDefinition, messageRuntime))
                    {
                        break;
                    }

                    result = messageRuntime;
                    break;
                case SignalEventDefinition signalEventDefinition:
                    SignalCatchRuntime signal = new(processVersion, catchEvent, signalEventDefinition);
                    if (!signalCatches.ContainsKey(catchEvent.Name))
                    {
                        signalCatches.Add(catchEvent.Name, []);
                    }

                    signalCatches[catchEvent.Name].Add(signal);
                    result = signal;
                    break;
                case TimerEventDefinition timerEventDefinition:
                    result = new TimerCatchRuntime(processVersion, catchEvent, timerEventDefinition);
                    break;
            }

            AddFlowElementRuntime(processVersion, catchEvent, result);
        }

        if (catchEvent is StartEvent startEvent &&
            startEvent.TriggerType == Event.eEventType.None &&
            (catchEvent.eventDefinitions?.Count ?? 0) == 0)
        {
            CatchRuntime result = new(processVersion, catchEvent);
            AddFlowElementRuntime(processVersion, catchEvent, result);
        }
    }

    private static bool IsStartInEventSubProcess(CatchEvent ev, bool checkInterrupting)
    {
        return ev is StartEvent startEvent &&
               startEvent.isInterrupting &&
               startEvent.SubProcess != null &&
               (startEvent.SubProcess.triggeredByEvent || !checkInterrupting);
    }

    private bool AddToCatchMessage(MessageEventDefinition messageEventDefinition, IMessageCatchRuntime catchRuntime)
    {
        if (messageEventDefinition?.messageRef == null)
        {
            Logger.LogError("Invalid message In {2} {0} in process {1}", catchRuntime.flowNode.Id,
                catchRuntime.flowNode.Process.Id, catchRuntime.flowNode.GetType().Name);
            return false;
        }

        if (!MessagesCatches.TryGetValue(messageEventDefinition.messageRef.Id, out MessageCatches messageCatch))
        {
            messageCatch = new MessageCatches
            {
                Message = messageEventDefinition.messageRef,
                Catches = []
            };
            MessagesCatches.Add(messageEventDefinition.messageRef.Id, messageCatch);
        }

        messageCatch.Catches.Add(new MessageCatchRuntimeLink
        {
            MessageEventDefinition = messageEventDefinition,
            MessageCatchRuntime = catchRuntime
        });
        return true;
    }
}
