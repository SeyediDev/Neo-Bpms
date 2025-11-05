using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

/// <summary>
/// A Service Task is a Task that uses some sort of service, which could be a Web service or an automated application
/// The Service Task has exactly one inputSet and at most one outputSet. 
/// It has a single Data Input with an ItemDefinition equivalent to the one defined by the Message referenced by the inMessageRef attribute of the associated Operation. 
/// If the Operation defines output Messages, the Service Task has a single Data Output that has an ItemDefinition equivalent to the one defined by the Message referenced by the outMessageRef attribute of the associated Operation.
/// 
/// The actual Participant whose service is used can be identified by connecting the Service Task to a Participant using a Message Flows within the definitional Collaboration of the Process
/// </summary>
public class ServiceTask(IFlowElementsContainer flowElementsContainer,
    string id, string name, Operation operationRef) : Task(flowElementsContainer, id, name, eActivityType.ServiceTask), IOperationContainer
{
    /// <summary>
    /// This attribute specifies the technology that will be used to send and receive the Messages. 
    /// Valid values are "##unspecified" for leaving the implementation technology open, 
    /// "##WebService" for the Web service technology or a URI identifying any other technology or coordination protocol. 
    /// A Web service is the default technology.
    /// </summary>
    public string implementation = "##webService";
    /// <summary>
    /// This attribute specifies the operation that is invoked by the Service Task.
    /// </summary>
    public Operation operationRef { get; set; } = operationRef;
}
