namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
/// <summary>
/// Implicit Throw Event A sub-type of throw Event is the ImplicitThrowEvent. This is a non-graphical Event that is used for Multi-Instance Activities. 
/// The ImplicitThrowEvent element inherits the attributes and model associations of ThrowEvent (see Table 10.84), but does not have any additional 
/// attributes or model associations.
/// </summary>
public class ImplicitThrowEvent(IFlowElementsContainer flowElementsContainer, string id, string name) : ThrowEvent(flowElementsContainer, id, name, ThrowEventLocation.Implicit)
{
}