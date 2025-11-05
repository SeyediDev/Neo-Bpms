using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

public interface IMessageCatchRuntime
{
    ProcessVersionRuntime ProcessVersion { get; }
    FlowNode flowNode { get; }
    MessageEventDefinition MessageDefinition { get; set; }
    bool MessageReceived(ExecutionInstance execution, MessageEventDefinition message, LocalParameters messageParams);
}

public class MessageCatchRuntime(ProcessVersionRuntime processVersion, CatchEvent catchEvent,
    MessageEventDefinition messageDefinition) : CatchRuntime(processVersion, catchEvent), IMessageCatchRuntime
{
    public MessageEventDefinition MessageDefinition { get; set; } = messageDefinition;

    public bool MessageReceived(ExecutionInstance execution, MessageEventDefinition message,
        LocalParameters messageParams)
    {
        var catchingMessage = new CatchingMessage(this, message, execution,
            null, null, messageParams);
        return catchingMessage.EventReceived(CatchEvent.Location);
    }
}
