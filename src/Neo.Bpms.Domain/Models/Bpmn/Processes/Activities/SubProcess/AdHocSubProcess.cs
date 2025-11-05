namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;

/// <summary>
/// An Ad-Hoc Sub-Process is a specialized type of Sub-Process that is a group of Activities 
/// that have no REQUIRED sequence relationships. A set of Activities can be defined for the Process, 
/// but the sequence and number of performances for the Activities is determined by the performers of the Activities.
/// 
/// It is a challenge for a BPM engine to monitor the status of Ad-Hoc Sub-Processes, usually these kind of Processes 
/// are handled through groupware applications (such as e-mail), but BPMN allows modeling of Processes that are not 
/// necessarily executable, although there are some process engines that can follow an Ad-Hoc Sub-Process. Given this, 
/// at some point the Ad-Hoc Sub-Process will have complete and this can be determined by evaluating a 
/// completionCondition that evaluates Process attributes that will have been updated by an Activity in the Process
/// </summary>
public class AdHocSubProcess(IFlowElementsContainer flowElementsContainer, string id, string name) : SubProcess(flowElementsContainer, id, name, eActivityType.AdhocSubProcess)
{
    /// <summary>
    /// This attribute defines if the Activities within the Process can be performed in parallel or MUST be 
    /// performed sequentially. The default setting is parallel and the setting of sequential is a restriction 
    /// on the performance that can be needed due to shared resources. When the setting is sequential, then only
    /// one Activity can be performed at a time. When the setting is parallel, then zero (0) to all the Activities 
    /// of the Sub-Process can be performed in parallel.
    /// </summary>
    public AdHocOrdering ordering = AdHocOrdering.Parallel;
    /// <summary>
    /// This Expression defines the conditions when the Process will end. 
    /// When the Expression is evaluated to true, the Process will be terminated.
    /// </summary>
    public BpmnExpression completionCondition;
    /// <summary>
    /// This attribute is used only if ordering is parallel. It determines whether running
    /// instances are cancelled when the completionCondition becomes true.
    /// </summary>
    public bool cancelRemainingInstances = true;

    public enum AdHocOrdering { Parallel, Sequential }
}
