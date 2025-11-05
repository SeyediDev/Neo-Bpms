using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.CallActivity;

/// <summary>
/// A Call Activity identifies a point in the Process where a global Process or a Global Task is used. 
/// The Call Activity acts as a ‘wrapper’ for the invocation of a global Process or Global Task within the execution. 
/// The activation of a call Activity results in the transfer of control to the called global Process or Global Task
/// </summary>
/// <remarks>
/// When a Process with a definitional Collaboration, calls a Process that also has a definitional Collaboration, 
/// the Participants of the two Collaborations can be matched to each other using ParticipantAssociations of the
/// Collaboration of the calling Process.
/// 
/// A Call Activity MUST fulfill the data requirements, as well as return the data produced by the CallableElement
/// being invoked (see Figure 10.41). This means that the elements contained in the Call Activity’s
/// InputOutputSpecification MUST exactly match the elements contained in the referenced CallableElement.
/// This includes DataInputs, DataOutputs, InputSets, and OutputSets.
/// 
/// A Call Activity can override properties and attributes of the element being called, potentially changing the behavior of
/// the called element based on the calling context. For example, when the Call Activity defines one or more
/// ResourceRole elements, the elements defined by the CallableElement are ignored and the elements defined in the
/// Call Activity are used instead. Also, Events that are propagated along the hierarchy (errors and escalations) are
/// propagated from the called element to the Call Activity (and can be handled on its boundary).
/// </remarks>
public class CallActivity(IFlowElementsContainer flowElementsContainer,
    string id, string name, string calledElementId, Activity.CallActivityTypeId callActivityTypeId) : Activity(flowElementsContainer, id, name, (eActivityType)callActivityTypeId)
{
    /// <summary>
    /// The element to be called, which will be either a Process or a GlobalTask. 
    /// Other CallableElements, such as Choreography, GlobalChoreographyTask, Conversation, and 
    /// GlobalCommunication MUST NOT be called by the Call Conversation element.
    /// </summary>
    //public CallableElement calledElement;
    public string CalledElementId { get; set; } = calledElementId;
}
