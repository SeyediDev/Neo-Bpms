using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader.Dto;

internal class MessageCatches
{
    public Message Message { get; set; }
    public List<MessageCatchRuntimeLink> Catches { get; set; }
}

internal class MessageCatchRuntimeLink
{
    public MessageEventDefinition MessageEventDefinition { get; set; }
    public IMessageCatchRuntime MessageCatchRuntime { get; set; }
}
