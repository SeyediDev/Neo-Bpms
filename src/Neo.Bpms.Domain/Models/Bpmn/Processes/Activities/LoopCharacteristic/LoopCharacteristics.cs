namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;

/// <summary>
/// Activities MAY be repeated sequentially, essentially behaving like a loop. The presence of 
/// LoopCharacteristics signifies that the Activity has looping behavior. LoopCharacteristics is an abstract 
/// class. Concrete subclasses define specific kinds of looping behavior.
/// 
/// However, each Loop Activity instance has attributes whose values MAY be referenced by Expressions. 
/// These values are only available when the Loop Activity is being executed.
/// </summary>
public abstract class LoopCharacteristics(Activity activity, string id, LoopCharacteristics.eLoopType loopType) : BaseElement(activity, id)
{
    public eLoopType loopType = loopType;

    public enum eLoopType
    {
        Standard,
        MultiInstance
    }
}

//public class LoopActivityInstance
//{
//    /// <summary>
//    /// The LoopCounter attribute is used at runtime to count the number of loops and is automatically updated by the process engine.
//    /// </summary>
//    public int loopCounter;
//}

//public class MultiInstanceActivityInstance
//{
//    /// <summary>
//    /// This attribute is provided for each generated (inner) instance of the Activity. It contains the 
//    /// sequence number of the generated instance, i.e., if this value of some instance in n, the instance 
//    /// is the n-th instance that was generated.
//    /// </summary>
//    public int loopCounter;
//    /// <summary>
//    /// This attribute is provided for the outer instance of the Multi-Instance Activity only. 
//    /// This attribute contains the total number of inner instances created for the Multi-Instance Activity.
//    /// </summary>
//    public int numberOfInstances;
//    /// <summary>
//    /// This attribute is provided for the outer instance of the Multi-Instance Activity only. 
//    /// This attribute contains the number of currently active inner instances for the Multi-Instance Activity. 
//    /// In case of a sequential Multi-Instance Activity, this value can’t be greater than 1. For parallel 
//    /// Multi-Instance Activities, this value can’t be greater than the value contained in numberOfInstances
//    /// </summary>
//    public int numberOfActiveInstances;
//    /// <summary>
//    /// This attribute is provided for the outer instance of the Multi-Instance Activity only. 
//    /// This attribute contains the number of already completed inner instances for the Multi-Instance Activity.
//    /// </summary>
//    public int numberOfCompletedInstances;
//    /// <summary>
//    /// This attribute is provided for the outer instance of the Multi-Instance Activity only. 
//    /// This attribute contains the number of terminated inner instances for the Multi-Instance Activity. 
//    /// The sum of numberOfTerminatedInstances, numberOfCompletedInstances, and numberOfActiveInstances 
//    /// always sums up to numberOfInstances
//    /// </summary>
//    public int numberOfTerminatedInstances;
//}
