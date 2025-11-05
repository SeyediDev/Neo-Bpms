namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
public class BoundaryEvent(IFlowElementsContainer flowElementsContainer, string id, string name, string attachedToRef,
    bool cancelActivity) : CatchEvent(flowElementsContainer, id, name, CatchEventLocation.Boundary)
{
    /// <summary>
    /// Denotes whether the Activity should be cancelled or not, i.e., whether the boundary catch Event acts as an Error or an Escalation. 
    /// If the Activity is not cancelled, multiple instances of that handler can run concurrently.
    /// This attribute cannot be applied to Error, Cancel Events (where it’s always true), or Compensation Events (where it doesn’t apply).
    /// </summary>
    public bool cancelActivity { get; set; } = cancelActivity;

    public string attachedToRef { get; set; } = attachedToRef;
}
