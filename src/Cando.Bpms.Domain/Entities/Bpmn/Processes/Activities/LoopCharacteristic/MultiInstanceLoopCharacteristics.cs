using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;

/// <summary>
/// The MultiInstanceLoopCharacteristics class allows for creation of a desired number of Activity instances.
/// The instances MAY execute in parallel or MAY be sequential. Either an Expression is used to specify or 
/// calculate the desired number of instances or a data driven setup can be used. In that case a data input 
/// can be specified, which is able to handle a collection of data. The number of items in the collection 
/// determines the number of Activity instances. This data input can be produced by an input Data Association. 
/// The modeler can also configure this loop to control the tokens produced
/// </summary>
public class MultiInstanceLoopCharacteristics(Activity activity, string id) : LoopCharacteristics(activity, id, eLoopType.MultiInstance),
    IBaseDataInputContainer, IBaseDataOutputContainer
{
    public bool isSequential = false;

    /// <summary>
    /// This attribute defines a boolean Expression that when evaluated to true, 
    /// cancels the remaining Activity instances and produces a token.
    /// </summary>
    public BpmnExpression completionCondition;

    /// <summary>
    /// A numeric Expression that controls the number of Activity instances that will be created. 
    /// This Expression MUST evaluate to an integer. This MAY be underspecified, meaning that the modeler 
    /// MAY simply document the condition. In such a case the loop cannot be formally executed.
    /// 
    /// In order to initialize a valid multi-instance, either the loopCardinality Expression or the 
    /// loopDataInput MUST be specified
    /// </summary>
    public BpmnExpression loopCardinality;

    /// <summary>
    /// This ItemAwareElement is used to determine the number of Activity instances, one Activity instance per 
    /// item in the collection of data stored in that ItemAwareElement element.
    /// For Tasks it is a reference to a Data Input which is part of the Activity’s InputOutputSpecification.
    /// For Sub-Processes it is a reference to a collection-valued Data Object in the context that is visible 
    /// to the Sub-Processes. In order to initialize a valid multi-instance, either the loopCardinality Expression or the loopDataInput MUST be specified.
    /// </summary>
    public IItemAwareElement loopDataInputRef;

    /// <summary>
    /// This ItemAwareElement specifies the collection of data, which will be produced by the multi-instance.
    /// For Tasks it is a reference to a Data Output which is part of the Activity’s InputOutputSpecification.
    /// For Sub-Processes it is a reference to a collection-valued Data Object in the context that is visible 
    /// to the Sub-Processes.
    /// </summary>
    public IItemAwareElement loopDataOutputRef; //todo not implementation

    /// <summary>
    /// A Data Input, representing for every Activity instance the single item of the collection stored in 
    /// the loopDataInput. This Data Input can be the source of DataInputAssociation to a data input of the 
    /// Activity’s InputOutputSpecification. The type of this Data Input MUST the scalar of the type defined for the loopDataInput.
    /// </summary>
    public DataInput inputDataItem;

    /// <summary>
    /// A Data Output, representing for every Activity instance the single item of the collection stored in 
    /// the loopDataOutput. This Data Output can be the target of DataOutputAssociation to a data output of 
    /// the Activity’s InputOutputSpecification. The type of this Data Output MUST the scalar of the type 
    /// defined for the loopDataOutput.
    /// </summary>
    public DataOutput outputDataItem;

    /// <summary>
    /// The attribute behavior acts as a shortcut for specifying when events SHALL be thrown from an Activity 
    /// instance that is about to complete. It can assume values of None, One, All, and Complex, resulting in 
    /// the following behavior:
    /// • None: the EventDefinition which is associated through the noneEvent association will be thrown for each instance completing.
    /// • One: the EventDefinition referenced through the oneEvent association will be thrown upon the first instance completing.
    /// • All: no Event is ever thrown; a token is produced after completion of all instances.
    /// • Complex: the complexBehaviorDefinitions are consulted to determine if and which Events to throw.
    /// For the behaviors of none and one, a default SignalEventDefinition will be thrown which automatically carries
    /// the current runtime attributes of the MI Activity. 
    /// Any thrown Events can be caught by boundary Events on the Multi-Instance Activity.
    /// </summary>
    public MultiInstanceBehavior behavior = MultiInstanceBehavior.All;

    /// <summary>
    /// Controls when and which Events are thrown in case behavior is set to complex
    /// </summary>
    public List<ComplexBehaviorDefinition> complexBehaviorDefinition;

    /// <summary>
    /// The EventDefinition which is thrown when behavior is set to one and the first internal 
    /// Activity instance has completed.
    /// </summary>
    public EventDefinition oneBehaviorEventRef;

    public EventDefinition noneBehaviorEventRef;

    public IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems)
    {
        return ItemAwareContainer.GetItemAwareElement(this, itemId, itemName, fromInputItems);
    }

    public enum MultiInstanceBehavior
    {
        None,
        One,
        All,
        Complex
    }
}
