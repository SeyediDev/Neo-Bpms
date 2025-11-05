namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;

/// <summary>
/// The StandardLoopCharacteristics class defines looping behavior based on a boolean condition. The Activity
/// will loop as long as the boolean condition is true. The condition is evaluated for every loop iteration, and MAY be
/// evaluated at the beginning or at the end of the iteration. In addition, a numeric cap can be optionally specified. 
/// The number of iterations MAY NOT exceed this cap.
/// </summary>
public class StandardLoopCharacteristics(Activity activity, string id, BpmnExpression loopCondition) : LoopCharacteristics(activity, id, eLoopType.Standard)
{
    /// <summary>
    /// Flag that controls whether the loop condition is evaluated at the beginning
    /// (testBefore = true) or at the end (testBefore = false) of the loop iteration.
    /// </summary>
    public bool testBefore;

    /// <summary>
    /// Serves as a cap on the number of iterations
    /// </summary>
    public int loopMaximum;

    /// <summary>
    /// A boolean Expression that controls the loop. The Activity will only loop as long as this condition is 
    /// true. The looping behavior MAY be underspecified, meaning that the modeler can simply document the
    /// condition, in which case the loop cannot be formally executed.
    /// </summary>
    public BpmnExpression loopCondition = loopCondition;

    public StandardLoopCharacteristics(Activity activity, string id, BpmnExpression loopCondition, bool testBefore,
        int loopMaximum) : this(activity, id, loopCondition)
    {
        this.testBefore = testBefore;
        this.loopMaximum = loopMaximum;
    }
}
