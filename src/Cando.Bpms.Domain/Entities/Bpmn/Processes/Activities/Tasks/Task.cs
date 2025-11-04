using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

/// <inheritdoc />
/// <summary>
/// A Task is an atomic Activity within a Process flow. 
/// A Task is used when the work in the Process cannot be broken down to a finer level of detail. 
/// Generally, an end-user and/or applications are used to perform the Task when it is executed.
/// BPMN specifies three types of markers for Task: a Loop marker or a Multi-Instance marker and a Compensation marker A Task MAY have one or two of these markers
/// The loop Marker or Multi-Instance MAY be used in combination with the compensation marker
/// There are different types of Tasks identified within BPMN to separate the types of inherent behavior that Tasks might represent. 
/// The list of Task types MAY be extended along with any corresponding indicators. A Task which is not further specified is called Abstract Task (this was referred to as the None Task in BPMN 1.2)
/// </summary>
public class Task(IFlowElementsContainer flowElementsContainer,
    string id, string name, Activity.eActivityType activityType) : Activity(flowElementsContainer, id, name, activityType)
{
    public double priorityLevel { get; set; }
    public string priorityLevelProperty { get; set; }
}
