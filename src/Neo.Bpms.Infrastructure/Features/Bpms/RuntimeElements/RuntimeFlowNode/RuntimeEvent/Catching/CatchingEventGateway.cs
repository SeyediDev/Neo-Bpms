using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
internal partial class CatchingEvent
{
    private void CatchingEventGatewayInStartInOnePi()
    {
        IncomingEventBasedGatewayRuntime.ReceiveToken(Pi, InputData);
        if (NeedToCompleteEventBasedGatewayInstance)
        {
            Execution.AddJob(new ActionJob(() =>
            {
                var gwi = DataStorage.FetchFlowNodeInstancesOfFlowNode(IncomingEventBasedGatewayRuntime, Execution, Pi)
                    ?.Values.OfType<GatewaySyncWaitingInstance>()
                    .FirstOrDefault();
                gwi?.CompleteAndSave();
            }));
        }

        FetchAndCatchAndSavePi();
    }

    private bool NeedToCompleteEventBasedGatewayInstance
    {
        get
        {
            if (!IncomingEventBasedGatewayRuntime.EventBasedGateway.instantiate)
                return false;
            if (IncomingEventBasedGatewayRuntime.EventBasedGateway.eventGatewayType ==
                EventBasedGateway.eEventBasedGatewayType.Exclusive)
                return true;
            switch (EventDefinition)
            {
                case ConditionalEventDefinition _:
                case ErrorEventDefinition _:
                    return true;
                case TimerEventDefinition _:
                    return true; //todo check cycle timers
            }

            return false;
        }
    }
}
