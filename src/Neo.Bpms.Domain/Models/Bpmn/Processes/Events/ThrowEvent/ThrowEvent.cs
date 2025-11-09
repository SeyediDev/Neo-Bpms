using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;

public abstract class ThrowEvent(IFlowElementsContainer flowElementsContainer, string id, string name, ThrowEventLocation location) : Event(flowElementsContainer, id, name), IDataInputContainer, IDataInputAssociationContainer
{
    /// <summary>
    /// The Data Inputs for the throw Event. This is an ordered set.
    /// </summary>
    public List<DataInput> dataInputs { get; set; }

    /// <summary>
    /// The InputSet for the throw Event. maximum can be one row
    /// </summary>
    public List<InputSet> inputSets { get; set; }

    /// <summary>
    /// The Data Associations of the throw Event. The dataInputAssociation of a throw Event is responsible for the assignment of a data element 
    /// that is in scope of the Event to the Event data. For a throw Multiple Event, multiple Data Associations might be REQUIRED, depending on 
    /// the individual results of the Event.
    /// </summary>
    public List<DataInputAssociation> dataInputAssociations { get; set; }

    public ThrowEventLocation Location { get; set; } = location;
}
public enum ThrowEventLocation
{
    IntermediateThrow = 51,
    End = 61,
    Implicit = 71,
}