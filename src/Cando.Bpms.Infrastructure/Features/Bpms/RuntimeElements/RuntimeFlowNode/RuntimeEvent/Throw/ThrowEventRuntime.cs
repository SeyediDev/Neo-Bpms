using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Throw;

public class ThrowRuntime(ProcessVersionRuntime processVersion, ThrowEvent eventDefinition) 
    : FlowNodeRunTime(processVersion, eventDefinition)
{
    public ThrowEvent ThrowEvent => flowNode as ThrowEvent;

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        LocalParameters lp = FetchInputData(pi);
        ThrowEventDefinitions(pi, lp, out bool termination);
        if (termination && ThrowEvent is EndEvent)
        {
            pi.TerminateProcessAndActivityInstances();
        }

        SendTokenToOutgoing(pi, lp, TokenPattern.Parallel);
    }

    private void ThrowEventDefinitions(ProcessInstance pi, LocalParameters inputData, out bool termination)
    {
        termination = false;
        foreach (var ev in ThrowEvent.eventDefinitions ?? Enumerable.Empty<EventDefinition>())
        {
            if (ev is not IThrowEventDefinition)
            {
                continue;
            }

            switch (ev)
            {
                case MessageEventDefinition messageEventDefinition:
                    pi.Execution.AddJob(new ThrowMessageJob
                    {
                        Message = messageEventDefinition.messageRef,
                        InputData = inputData,
                        AuditTrail = pi.AuditTrail
                    });
                    break;
                case SignalEventDefinition signalEventDefinition:
                    pi.Execution.AddJob(new DistributeSignalJob
                    {
                        Signal = signalEventDefinition.signalRef,
                        InputData = inputData?.Clone(),
                        Execution = pi.Execution
                    });
                    break;
                case LinkEventDefinition linkEventDefinition:
                    if (ThrowEvent is not IntermediateThrowEvent)
                    {
                        break;
                    }

                    Catch.CatchRuntime catchRuntime = ProcessVersion.GetCatches()
                        .Where(f => f.CatchEvent is IntermediateCatchEvent &&
                                    f.CatchEvent.eventDefinitions.OfType<LinkEventDefinition>()
                                        .Any(evd => evd.target == linkEventDefinition.target))
                        .ToList()
                        .FirstOrDefault();
                    catchRuntime?.Catch(pi, null, inputData);
                    break;
                case TerminateEventDefinition _:
                    if (ThrowEvent is not EndEvent)
                    {
                        break;
                    }

                    termination = true;
                    break;
                case EscalationEventDefinition escalationEventDefinition:
                    TryCatchEvent<EscalationEventDefinition>.Try(escalationEventDefinition.Code, inputData, pi, null);
                    break;
                case CancelEventDefinition cancelEventDefinition:
                    if (ThrowEvent is not EndEvent)
                    {
                        break;
                    }

                    TryCatchEvent<CancelEventDefinition>.Try(cancelEventDefinition.Code, inputData, pi, null);
                    break;
                case ErrorEventDefinition errorEventDefinition:
                    if (ThrowEvent is not EndEvent)
                    {
                        break;
                    }

                    termination = true;
                    TryCatchEvent<ErrorEventDefinition>.Try(errorEventDefinition.Code, inputData, pi, null);
                    break;
                case CompensateEventDefinition compensateEventDefinition:
                    TryCatchEvent<CompensateEventDefinition>.Try(compensateEventDefinition.Code, inputData, pi, null);
                    break;
            }
        }
    }

    private LocalParameters FetchInputData(ProcessInstance pi)
    {
        LocalParameters localParameters = [];
        if (ThrowEvent.dataInputAssociations != null)
        {
            foreach (var dia in ThrowEvent.dataInputAssociations)
            {
                RunDataAssociation(dia, null, localParameters, localParameters, pi);
            }
        }

        return localParameters;
    }
}
