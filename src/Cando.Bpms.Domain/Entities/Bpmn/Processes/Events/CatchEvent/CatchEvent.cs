using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;

public abstract class CatchEvent(IFlowElementsContainer flowElementsContainer, string id, string name,
    CatchEventLocation location) : Event(flowElementsContainer, id, name), IDataOutputContainer, IDataOutputAssociationContainer
{
    /// <summary>
    /// The Data Outputs for the catch Event. This is an ordered set.
    /// </summary>
    public List<DataOutput> dataOutputs { get; set; }

    /// <summary>
    /// The OutputSet for the catch Event. maximum can be one row
    /// </summary>
    public List<OutputSet> outputSets { get; set; }

    /// <summary>
    /// The Data Associations of the catch Event. The dataOutputAssociation of a catch Event is used to assign data from the Event to a 
    /// data element that is in the scope of the Event. For a catch Multiple Event, multiple Data Associations might be REQUIRED, 
    /// depending on the individual triggers of the Event.
    /// </summary>
    public List<DataOutputAssociation> dataOutputAssociations { get; set; }

    /// <summary>
    /// This attribute is only relevant when the catch Event has more than EventDefinition (Multiple).
    /// If this value is true, then all of the types of triggers that are listed in the catch Event MUST be triggered before the Process is instantiated.
    /// </summary>
    public bool parallelMultiple = false;

    public CatchEventLocation Location { get; set; } = location;
}

public enum CatchEventLocation
{
    Start = 1,
    IntermediateCatch = 11,
    Boundary = 21,
}