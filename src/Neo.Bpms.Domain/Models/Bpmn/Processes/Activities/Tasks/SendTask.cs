using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

/// <summary>
/// A Send Task is a simple Task that is designed to send a Message to an external Participant (relative to the Process). 
/// Once the Message has been sent, the Task is completed.
/// 
/// The actual Participant which the Message is sent can be identified by connecting the Send Task to a Participant using a Message Flows within the definitional Collaboration of the Process.
/// </summary>
public class SendTask(IFlowElementsContainer flowElementsContainer,
    string id, string name) : Task(flowElementsContainer, id, name, eActivityType.SendTask), IMessageContainer
{
    /// <summary>
    /// A Message for the messageRef attribute MAY be entered. 
    /// This indicates that the Message will be sent by the Task. 
    /// The Message in this context is equivalent to an out-only message pattern (Web service). 
    /// One or more corresponding outgoing Message Flows MAY be shown on the diagram.
    /// However, the display of the Message Flows is NOT REQUIRED. 
    /// The Message is applied to all outgoing Message Flows and the Message will be sent down all outgoing Message Flows at the completion of a single instance of the Task.
    /// </summary>
    public Message messageRef { get; set; }

    /// <summary>
    /// This attribute specifies the operation that is invoked by the Send Task.
    /// </summary>
    public Operation operationRef { get; set; }
    Message IMessageContainer.messageRef { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <summary>
    /// This attribute specifies the technology that will be used to send and receive the
    /// Messages. Valid values are "##unspecified" for leaving the implementation
    /// technology open, "##WebService" for the Web service technology or a URI identifying any other technology 
    /// or coordination protocol A Web service is the default technology.
    /// </summary>
    public string implementation = "##webService";
}
