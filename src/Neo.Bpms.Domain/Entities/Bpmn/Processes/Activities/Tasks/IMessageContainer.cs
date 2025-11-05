using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

public interface IMessageContainer : IOperationContainer
{
    Message messageRef { get; set; }
}
