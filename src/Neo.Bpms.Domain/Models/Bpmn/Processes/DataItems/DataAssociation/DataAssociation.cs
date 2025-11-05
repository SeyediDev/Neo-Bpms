using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;

/// <summary>
/// Data Associations are used to move data between Data Objects, Properties, and inputs and outputs of Activities, Processes, and GlobalTasks. 
/// Tokens do not flow along a Data Association, and as a result they have no direct effect on the flow of the Process.
/// 
/// The purpose of retrieving data from Data Objects or Process Data Inputs is to fill the Activities inputs and later push the output 
/// values from the execution of the Activity back into Data Objects or Process Data Outputs.
/// 
/// The core concepts of a DataAssociation are that they have sources, a target, and an optional transformation.
/// When a data association is “executed,” data is copied to the target. What is copied depends if there is a transformation defined or not.
/// If there is no transformation defined or referenced, then only one source MUST be defined, and the contents of this source will be copied into the target.
/// </summary>
/// <remark>
/// In any case, sources are used to define if the data association can be “executed,” if any of the sources is in the state of “unavailable,” 
/// then the data association cannot be executed, and the Activity or Event where the data association is defined MUST wait until this condition is met.
/// 
/// Data Associations are always contained within another element that defines when these data associations are going to
/// be executed. Activities define two sets of data associations, while Events define only one.
///
/// For Events, there is only one set, but they are used differently for catch or throw Events. 
/// For a catch Event, data associations are used to push data from the Message received into Data Objects and properties. 
/// For a throw Event, data associations are used to fill the Message that is being thrown.
/// 
/// As DataAssociations are used in different stages of the Process and Activity lifecycle, the possible sources and targets vary according to that stage. 
/// This defines the scope of possible elements that can be referenced as source and target. For example: when an Activity starts executing, the scope of 
/// valid targets include the Activity data inputs, while at the end of the Activity execution, the scope of valid sources include Activity data outputs.
/// </remark>
public class DataAssociation : BaseElement
{
    public List<IItemAwareElement> sourceRef;
    public IItemAwareElement targetRef;
    public FormalExpression transformation;

    /// <summary>
    /// Specifies one or more data elements Assignments. By using an Assignment, single data structure elements can be assigned from the
    /// source structure to the target structure.
    /// </summary>
    public List<Assignment> assignment;

    public DataAssociation(IDataAssociationContainer parent, string id) :
        base(parent as BaseElement, id)
    {
    }

    public DataAssociation(IDataAssociationContainer parent,
        string id, IItemAwareElement sourceRef,
        IItemAwareElement targetRef)
        : base(parent as BaseElement, id)
    {
        if (sourceRef != null)
        {
            this.sourceRef = [sourceRef];
        }

        this.targetRef = targetRef;
        transformation = null;
    }

    public DataAssociation(IDataAssociationContainer parent,
        string id, IEnumerable<IItemAwareElement> sourceRef,
        IItemAwareElement targetRef,
        FormalExpression transformation)
        : base(parent as BaseElement, id)
    {
        if (sourceRef != null)
        {
            this.sourceRef = [.. sourceRef];
        }

        this.targetRef = targetRef;
        this.transformation = transformation;
    }

    public DataFlowElement TargetDataFlow => targetRef as DataFlowElement;
    public DataElement TargetDataElement => targetRef as DataElement;
    public BaseElement TargetRef => targetRef as BaseElement;
}

public interface IDataAssociationContainer : IItemAwareContainer
{
}
