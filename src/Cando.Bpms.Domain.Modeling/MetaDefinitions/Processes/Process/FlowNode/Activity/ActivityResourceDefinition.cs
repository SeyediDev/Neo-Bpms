using Neo.Bpms.Domain.Entities.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the potential owner user.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="humanResourceId">The human resource identifier.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    protected bool AddPotentialOwnerUser(string name, string humanResourceId, PotentialOwnerType type)
    {
        if (_currentActivity == null || _currentActivity.ActivityType != Activity.eActivityType.UserTask)
        {
            return false;
        }

        _currentResource = ProjectDefinition.Project.GetResource(humanResourceId);
        return CurrentHumanResource != null && AddActivityResourceRole(new PotentialOwner(_currentActivity,
                                                                            _currentFlowNode.Id + ".PotentialOwner." + humanResourceId,
                                                                            name, CurrentHumanResource, null /*todo*/, type));
    }

    /// <summary>
    /// Adds the authorized user.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="humanResourceId">The human resource identifier.</param>
    /// <returns></returns>
    protected bool AddAuthorizedUser(string name, string humanResourceId)
    {
        if (_currentActivity == null || _currentActivity.ActivityType != Activity.eActivityType.UserTask)
        {
            return false;
        }

        _currentResource = ProjectDefinition.Project.GetResource(humanResourceId);
        return CurrentHumanResource != null && AddActivityResourceRole(new AuthorizedUser(_currentActivity,
                                                                            _currentFlowNode.Id + ".AuthorizedUser." + humanResourceId,
                                                                            name, CurrentHumanResource, null /*todo*/));
    }

    /// <summary>
    /// Adds the confirming user.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="humanResourceId">The human resource identifier.</param>
    /// <param name="confirmStepId">The confirm step identifier.</param>
    /// <returns></returns>
    protected bool AddConfirmingUser(string name, string humanResourceId, int confirmStepId)
    {
        if (_currentActivity == null || _currentActivity.ActivityType != Activity.eActivityType.UserTask)
        {
            return false;
        }

        _currentResource = ProjectDefinition.Project.GetResource(humanResourceId);
        return CurrentHumanResource != null && AddActivityResourceRole(new Confirmer(_currentActivity,
                                                                    _currentFlowNode.Id + ".ConfirmingUser." + humanResourceId + "_" +
                                                                    confirmStepId, name, CurrentHumanResource, null /*todo*/,
                                                                    confirmStepId));
    }

    /// <summary>
    /// Adds the physical performer resource.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="resourceId">The resource identifier.</param>
    /// <returns></returns>
    protected bool AddPhysicalPerformerResource(string name, string resourceId)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        Resource rsc = ProjectDefinition.Project.GetResource(resourceId);
        return rsc != null && AddActivityResourceRole(new PhysicalPerformerResource(_currentActivity,
                                                                                        _currentFlowNode.Id + ".PhysicalPerformer." +
                                                                                        resourceId, name, rsc, null /*todo*/));
    }

    /// <summary>
    /// Adds the costing resource.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="resourceId">The resource identifier.</param>
    /// <param name="isDirect">if set to <c>true</c> [is direct].</param>
    /// <param name="quantity">The quantity.</param>
    /// <returns></returns>
    protected bool AddCostingResource(string name, string resourceId, bool isDirect, double quantity)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        Resource rsc = ProjectDefinition.Project.GetResource(resourceId);
        return rsc != null && AddActivityResourceRole(new CostingResource(_currentActivity,
                                                                            _currentFlowNode.Id + ".CostingResource." + resourceId, name,
                                                                            rsc, null /*todo*/, isDirect, quantity));
    }

    /// <summary>
    /// Adds the capacity constraint resource.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="resourceId">The resource identifier.</param>
    /// <param name="quantity">The quantity.</param>
    /// <returns></returns>
    protected bool AddCapacityConstraintResource(string name, string resourceId, double quantity)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        Resource rsc = ProjectDefinition.Project.GetResource(resourceId);
        return rsc != null && AddActivityResourceRole(
                                                new CapacityConstraintResource(_currentActivity,
                                                                                        _currentFlowNode.Id + ".CapacityConstraint." +
                                                                                        resourceId, name, rsc, null /*todo*/,
                                                                                        quantity));
    }


    /// <summary>
    /// Adds the activity resource role.
    /// </summary>
    /// <param name="resourceRole">The resource role.</param>
    /// <returns></returns>
    private bool AddActivityResourceRole(ResourceRole resourceRole)
    {
        if (_currentActivity == null)
        {
            throw new Exception("please add a resourceRole after activity");
        }

        _currentResourceRole = resourceRole;
        _currentActivity.AddResourceRole(resourceRole);
        return true;
    }
}
