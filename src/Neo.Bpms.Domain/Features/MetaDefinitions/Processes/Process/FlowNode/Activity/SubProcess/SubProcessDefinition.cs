namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the embedded sub process.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected SubProcess AddEmbeddedSubProcess(string actionId, string name, object outputStateId)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        SubProcess sp = new(process, actionId, name, false);
        _ = AddActivity(_currentLane, outputStateId, sp);
        return sp;
    }

    /// <summary>
    /// Adds the transaction sub process.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="method">The method.</param>
    /// <returns></returns>
    protected TransactionSubProcess AddTransactionSubProcess(string actionId, string name, object outputStateId,
        string method)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        TransactionSubProcess sp = new(process, actionId, name) { method = method };
        _ = AddActivity(_currentLane, outputStateId, sp);
        return sp;
    }

    /// <summary>
    /// Adds the event sub process.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected SubProcess AddEventSubProcess(string actionId, string name, object outputStateId)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        SubProcess sp = new(process, actionId, name, true);
        _ = AddActivity(_currentLane, outputStateId, sp);
        return sp;
    }

    /// <summary>
    /// Adds the ad-hoc sub process.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="ordering">The ordering.</param>
    /// <param name="completionCondition">The completion condition.</param>
    /// <param name="cancelRemainingInstances">if set to <c>true</c> [cancel remaining instances].</param>
    /// <returns></returns>
    protected AdHocSubProcess AddAdHocSubProcess(string actionId, string name, object outputStateId,
        AdHocSubProcess.AdHocOrdering ordering,
        FormalExpression completionCondition, bool cancelRemainingInstances = true)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        AdHocSubProcess sp = new(process, actionId, name)
        {
            ordering = ordering,
            cancelRemainingInstances = cancelRemainingInstances,
            completionCondition = completionCondition
        };
        _ = AddActivity(_currentLane, outputStateId, sp);
        return sp;
    }

    private SubProcess currentSubProcess;

    /// <summary>
    /// Enters the sub process.
    /// </summary>
    /// <param name="subProcess">new sub process</param>
    /// <returns>
    /// returns parent sub process or null if we are currently on process
    /// </returns>
    protected SubProcess EnterSubProcess(SubProcess subProcess)
    {
        SubProcess parentSp = currentSubProcess;
        currentSubProcess = subProcess;
        _flowElementsContainer = subProcess;
        currentBaseElement = subProcess;
        return parentSp;
    }

    /// <summary>
    /// Exits the sub process.
    /// </summary>
    /// <param name="parentSubProcess">The parent sub process.</param>
    protected void ExitSubProcess(SubProcess parentSubProcess)
    {
        currentSubProcess = parentSubProcess;
        currentBaseElement = parentSubProcess as BaseElement ?? process;
        _flowElementsContainer = currentBaseElement as IFlowElementsContainer;
    }
}
