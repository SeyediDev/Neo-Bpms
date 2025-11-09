namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

public interface IMessageContainer : IOperationContainer
{
    Message messageRef { get; set; }
}
