using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Entities.WorkManagement;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;


namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract class UserTaskDefinition<TForm> : TaskDefinition<UserTask>
    where TForm : FormDefinition
{
    protected override UserTask FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new UserTask(flowElementsContainer, ElementId, Name,
            typeof(TForm).Name, null, null);
    }

    protected void SetAssociationIndex<TIndexForm>(string associationFieldId)
        where TIndexForm : FormDefinition
    {
        Element.IndexFormId = typeof(TIndexForm).Name;
        Element.AssociationFieldId = associationFieldId;
    }
    /// <summary>
    /// Sets the work distribution policy.
    /// </summary>
    /// <param name="policy">The policy.</param>
    /// <param name="allocationOrOfferingAlgorithm">The allocation or offering algorithm.</param>
    /// <param name="rankingFormula"></param>
    /// <param name="needsAllocationBeforePerform">if set to <c>true</c> [needs allocation before perform].</param>
    /// <param name="maxGroupOfferedUsers">The maximum group offered users.</param>
    /// <returns></returns>
    public void SetWorkDistributionPolicy(
        eWorkAllocationPolicy policy = eWorkAllocationPolicy.AllocateToUser,
        eAllocationOrOfferingAlgorithm allocationOrOfferingAlgorithm = eAllocationOrOfferingAlgorithm.Rotational,
        string rankingFormula = null, bool needsAllocationBeforePerform = false,
        int maxGroupOfferedUsers = 5)
    {
        Element.WorkDistributionPolicy = new WorkDistributionPolicy(policy, allocationOrOfferingAlgorithm,
            rankingFormula, needsAllocationBeforePerform, maxGroupOfferedUsers);
    }
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the user task.
    /// </summary>
    /// <param name="id">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="formId">The form identifier.</param>
    /// <param name="indexFormId">The index form Id form bulk create process</param>
    /// <param name="associationFieldId">The association Field Identifier between process entity and index form</param>
    /// <returns></returns>
    protected UserTask AddUserTask(string id, string name, object outputStateId,
        Type formId, Type indexFormId = null, string associationFieldId = null)
    {
        if (definitions == null || process == null) return null;
        if (formId != null)
            CheckFormType(formId, "formId");
        else
            throw new Exception("Please determine form in the formId argument.");
        if (indexFormId != null)
            CheckFormType(indexFormId, "indexFormId");
        var task = new UserTask(process, id, name, formId.Name, indexFormId?.Name, associationFieldId);
        AddTask(_currentLane, outputStateId, task);
        return task;
    }

    private static void CheckFormType(Type formId, string argName)
    {
        var b = formId.BaseType;
        while (b != null && b != typeof(FormDefinition))
        {
            if (b == typeof(ReportDefinition) || b == typeof(DashboardDefinition))
                throw new Exception($"Please use form in the {argName} argument. Not use report.");
            b = b.BaseType;
        }

        if (b == null)
            throw new Exception($"Please use form in the {argName} argument.");
    }

    /// <summary>
    /// Sets the work distribution policy.
    /// </summary>
    /// <param name="policy">The policy.</param>
    /// <param name="allocationOrOfferingAlgorithm">The allocation or offering algorithm.</param>
    /// <param name="rankingFormula"></param>
    /// <param name="needsAllocationBeforePerform">if set to <c>true</c> [needs allocation before perform].</param>
    /// <param name="maxGroupOfferedUsers">The maximum group offered users.</param>
    /// <returns></returns>
    protected WorkDistributionPolicy SetWorkDistributionPolicy(
        eWorkAllocationPolicy policy = eWorkAllocationPolicy.AllocateToUser,
        eAllocationOrOfferingAlgorithm allocationOrOfferingAlgorithm = eAllocationOrOfferingAlgorithm.Rotational,
        string rankingFormula = null,
        bool needsAllocationBeforePerform = false,
        int maxGroupOfferedUsers = 5)
    {
        if (_currentActivity is not UserTask currentUserTask) return null;
        var w = new WorkDistributionPolicy(policy, allocationOrOfferingAlgorithm, rankingFormula,
            needsAllocationBeforePerform, maxGroupOfferedUsers);
        currentUserTask.WorkDistributionPolicy = w;
        return w;
    }
}