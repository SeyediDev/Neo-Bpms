using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;

/// <summary>
/// An Activity is work that is performed within a Business Process. An Activity can be atomic or non-atomic
/// (compound). The types of Activities that are a part of a Process are: Task, Sub-Process, and Call Activity, which
/// allows the inclusion of re-usable Tasks and Processes in the diagram
/// Activities represent points in a Process flow where work is performed. 
/// They are the executable elements of a BPMN Process.
/// </summary>
public abstract class Activity(IFlowElementsContainer flowElementsContainer,
    string id, string name, Activity.eActivityType activityType) :
    FlowNode(flowElementsContainer, id, name, eFlowNodeType.Activity), IResourceRoleContainer, IPropertyContainer,
    IIoSpecificationContainer, IDataInputAssociationContainer, IDataOutputAssociationContainer,
    IDueTimeDurationContainer, IHasDefaultSequenceFlow
{
    /// <summary>
    /// A flag that identifies whether this Activity is intended for the purposes of compensation.
    /// If false, then this Activity executes as a result of normal execution flow.
    /// If true, this Activity is only activated when a Compensation Event is detected and initiated under Compensation Event visibility scope.
    /// </summary>
    public bool isForCompensation = false;

    /// <summary>
    /// An Activity MAY be performed once or MAY be repeated. If repeated, 
    /// the Activity MUST have loopCharacteristics that define the repetition
    /// criteria (if the isExecutable attribute of the Process is set to true).
    /// </summary>
    public LoopCharacteristics loopCharacteristics { get; set; }

    /// <summary>
    /// Defines the resource that will perform or will be responsible for the Activity. 
    /// The resource, e.g., a performer, can be specified in the form of a specific individual, a group, an organization role or position, or an organization.
    /// </summary>
    public List<ResourceRole> resources { get; set; }

    /// <summary>
    /// The Sequence Flow that will receive a token when none of the conditionExpressions on other outgoing Sequence Flows evaluate
    /// to true. The default Sequence Flow should not have a conditionExpression. 
    /// Any such Expression SHALL be ignored.
    /// </summary>
    public string defaultSequenceFlowId { get; set; }

    /// <summary>
    /// The InputOutputSpecification defines the inputs and outputs and the InputSets and OutputSets for the Activity.
    /// </summary>
    public InputOutputSpecification ioSpecification { get; set; }

    /// <summary>
    /// Modeler-defined properties MAY be added to an Activity. These properties are contained within the Activity.
    /// </summary>
    public List<Property> properties { get; set; }

    /// <summary>
    /// An optional reference to the DataInputAssociations. 
    /// A DataInputAssociation defines how the DataInput of the Activity’s InputOutputSpecification will be populated
    /// </summary>
    public List<DataInputAssociation> dataInputAssociations { get; set; }

    public List<DataOutputAssociation> dataOutputAssociations { get; set; }
    public int startQuantity = 1;
    public int completionQuantity = 1;

    public eActivityType ActivityType { get; set; } = activityType;

    public DueTimeDuration dueDuration { get; set; }
    public bool notificationToOwnerOnAllocate { get; set; }
    public bool notificationToOwnerOnExpiration { get; set; }
    public bool notificationToManagerOnExpiration { get; set; } //vertical
    public TimeSpan EmergencyTimeToDo { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan CriticalTimeToDo { get; set; } = TimeSpan.FromDays(7);

    public void AddResourceRole(ResourceRole resourceRole)
    {
        resources ??= [];
        resources.Add(resourceRole);
    }

    public void addProperty(Property property)
    {
        properties ??= [];
        properties.Add(property);
    }

    public IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems)
    {
        return ItemAwareContainer.GetItemAwareElement(this, itemId, itemName, fromInputItems);
    }

    private List<ResourceRole> _activityAndLaneResources;

    public List<ResourceRole> ActivityAndLaneResources
    {
        get
        {
            if (_activityAndLaneResources != null)
            {
                return _activityAndLaneResources;
            }

            _activityAndLaneResources = resources?.ToList() ?? [];
            Lane lane = FlowElementsContainer.FindLaneByFlowNodeId(Id);
            while (lane != null)
            {
                if (lane.defaultPerformer != null)
                {
                    _activityAndLaneResources.Add(lane.defaultPerformer);
                }
                //todo check parent subProcess
                lane = lane.Parent as Lane;
            }
            return _activityAndLaneResources;
        }
        set => _activityAndLaneResources = value;
    }

    List<Property> IPropertyContainer.properties { get; set; }

    public enum eActivityType
    {
        UserTask = 1,
        ManualTask,
        ServiceTask,
        SendTask,
        ScriptTask,
        BusinessRuleTask,
        Task,
        ReceiveTask = 21,

        EmbeddedSubProcess = 31,
        EventSubProcess,
        AdhocSubProcess,
        TransactionSubProcess,

        CallActivitySubProcess = 41,
        CallActivityGlobalTask
    }

    public enum CallActivityTypeId
    {
        SubProcess = 41,
        GlobalTask,
    }
}

///// <remarks>
///// If the Activity has multiple incoming Sequence Flows, then this is considered uncontrolled flow. 
///// This means that when a token arrives from one of the Paths, the Activity will be instantiated. 
///// It will not wait for the arrival of tokens from the other paths. 
///// If another token arrives from the same path or another path, then a separate instance of the Activity will be created. 
///// If the flow needs to be controlled, then the flow should converge on a Gateway that precedes the Activities.
///// </remarks>
//public class ActivityInstance
//{
//    /// <summary>
//    /// See Figure 13.2 ("The Lifecycle of a BPMN Activity") in Section 13.2.2 for permissible values.
//    /// </summary>
//    public string state = "None";
//}
