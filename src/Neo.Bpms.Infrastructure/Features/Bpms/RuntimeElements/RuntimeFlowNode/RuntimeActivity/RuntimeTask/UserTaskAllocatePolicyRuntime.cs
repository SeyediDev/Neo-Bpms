using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Resources;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public partial class UserTaskRuntime
{
    private int _lastUserIndex;

    private void DoWorkDistribution(UserTaskInstance wi)
    {
        if (UserTask?.ActivityAndLaneResources == null)
        {
            return;
        }

        UserSelections users = [];
        List<PotentialOwner> potentialOwners = UserTask.ActivityAndLaneResources.OfType<PotentialOwner>().ToList();
        foreach (PotentialOwner resourceRole in potentialOwners.Where(r => r.ParentElement?.Id == UserTask.Id))
        {
            DistributeWorkToResource(wi, users, resourceRole);
        }

        if (!users.Any())
        {
            foreach (PotentialOwner resourceRole in potentialOwners.Where(r => r.ParentElement?.Id != UserTask.Id))
            {
                DistributeWorkToResource(wi, users, resourceRole);
            }
        }

        if (!users.Any())
        {
            AuditTrace($"Activity potential owners have not any user, activity : {wi.activity.Id}", wi.pi, wi);
            foreach (AuthorizedUser resourceManager in UserTask.ActivityAndLaneResources.OfType<AuthorizedUser>())
            {
                DistributeWorkToResource(wi, users, resourceManager);
            }
        }
        WorkDistributionPolicy pol = UserTask.WorkDistributionPolicy;
        eWorkAllocationPolicy workAllocationPolicy = pol?.policy ?? eWorkAllocationPolicy.AllocateToUser;
        eAllocationOrOfferingAlgorithm selectionMethod = pol?.algorithm ?? eAllocationOrOfferingAlgorithm.Rotational;
        AuditTrace($"Work allocation policy is {workAllocationPolicy}, activity : {wi.activity.Id}", wi.pi, wi);
        switch (workAllocationPolicy)
        {
            case eWorkAllocationPolicy.OfferToGroup:
            /*foreach (var user1 in users )
				{
					wi.OfferedToASingleResource(user1.Id);
					if (IsLogProcess(wi.pi))
						Trace($"Allocated to {user1.UserName}, activity : {wi.activity.id}, AI:{wi.Id}");
					break;//todo
				}
				break;*/
            case eWorkAllocationPolicy.AllocateToUser:
                if (users.Count > 0)
                {
                    UserSelection user;
                    switch (selectionMethod)
                    {
                        default: /*
							case eAllocationOrOfferingAlgorithm.LastUserWhoSuccessfullyPerformedTheWork:
								user = users.FirstOrDefault();
								break;
							case eAllocationOrOfferingAlgorithm.WithLeastAllocationToWork:
								user = users.FirstOrDefault();
								break;
							case eAllocationOrOfferingAlgorithm.WithHighestUnemployment:
								user = users.FirstOrDefault();
								break;
							case eAllocationOrOfferingAlgorithm.WithSmallestQueue:
								user = users.FirstOrDefault();
								break;*/
                            //case eAllocationOrOfferingAlgorithm.Rotational:
                            int idx = (_lastUserIndex + 1) % users.Count;
                            _lastUserIndex = idx;
                            user = users.Values.ToList()[idx];
                            break;
                        case eAllocationOrOfferingAlgorithm.Randomly:
                            Random rnd = new();
                            user = users.Values.ToList()[rnd.Next(users.Count)];
                            break;
                        case eAllocationOrOfferingAlgorithm.FirstUser:
                            user = users.Values.FirstOrDefault();
                            break;
                        case eAllocationOrOfferingAlgorithm.WithHighestRank:
                            double maxRank = 0;
                            user = users.Values.FirstOrDefault();
                            foreach (UserSelection user1 in users.Values)
                            {
                                double uRank = GetRank(user1.User, wi);
                                if (uRank <= maxRank)
                                {
                                    continue;
                                }

                                user = user1;
                                maxRank = uRank;
                            }
                            break;
                    }
                    if (user != null)
                    {
                        AllocateToUser(wi, user);
                    }
                }
                break;
            case eWorkAllocationPolicy.NoAllocationAndOffering:
                break;
        }
        if ((int)wi.userTaskState < 11 && workAllocationPolicy != eWorkAllocationPolicy.NoAllocationAndOffering)
        {
            foreach (ResourceRole notification in Activity.ActivityAndLaneResources?.OfType<NotificationResourceRole>().Where(not =>
                                             (not.notificationTimes & NotificationTimes.onNoResource) != 0) ??
                                         Enumerable.Empty<ResourceRole>())
            {
                Notificate(notification, wi, "No resource find to allocate or offering.");
            }

            AuditTrace($"Don`t Allocated to user, activity : {wi.activity.Id}", wi.pi, wi);
        }
    }

    private void AllocateToUser(UserTaskInstance wi, UserSelection user)
    {
        wi.AllocatedToASingleResource(user.User.Id, user.UserGroupId);

        AuditTrace($"Allocated to user:{user.User.UserName}, userGroup:{user.UserGroupId},{user.UserGroupName}, activity : {wi.activity.Id}", wi.pi, wi);

        if (Activity.notificationToOwnerOnAllocate)
        {
            Notificate(user.User, wi, "Allocated to you.");
        }

        foreach (ResourceRole notification in Activity.ActivityAndLaneResources?.OfType<NotificationResourceRole>().Where(not =>
                                         (not.notificationTimes & NotificationTimes.onAllocate) != 0) ??
                                     Enumerable.Empty<ResourceRole>())
        {
            Notificate(notification, wi, $"Allocated to {user.User.UserName}. Selected From {user.UserGroupName}");
        }
    }

    public void DistributeWorkToResource(UserTaskInstance wi, UserSelections users,
        ResourceRole resourceRole)
    {
        if (resourceRole?.resourceRef is not HumanResource resource)
        {
            return;
        }

        Entity resourceEntity = ProjectDefinition.Project.GetEntity(resource.NamespaceId, resource.EntityId);
        EntityField keyField = resourceEntity?.KeyFields?.FirstOrDefault();
        string id = null;
        if (keyField != null)
        {
            id = FetchResourceParameterValue(wi, resourceRole, resource, keyField.Id);
        }

        if (string.IsNullOrEmpty(id))
        {
            id = resource.Id;
        }

        List<UserClaimRestrictionValue> claimRestrictionValues = SetClaimRestrictionValues(wi, resourceRole, resource);
        switch (resource.type)
        {
            case HumanResourceType.User:
                FetchUsersOfUserResource(wi, users, resource, id, claimRestrictionValues);
                break;
            case HumanResourceType.UserGroup:
                FetchUsersOfUserGroupResource(wi, users, resource, id, claimRestrictionValues);
                break;
        }
    }

    private void FetchUsersOfUserGroupResource(UserTaskInstance wi, UserSelections users, HumanResource resource,
        string userGroupId, List<UserClaimRestrictionValue> claimRestrictionValues)
    {
        //IdentityGroup group = UserAndOrganizationStorage.Instance.GetUserGroupByCode(userGroupId);
        //AuditTrace(
        //    $"Distribute work to {resource.type} : {group?.Code ?? userGroupId} {(group == null ? " not exists!!!" : "")}, activity : {wi?.activity?.Id}", wi?.pi, wi);
        //if (group == null)
        //{
        //    AuditTrace($"User Group {userGroupId} not defined, activity : {wi?.activity?.Id}", wi?.pi, wi);
        //}
        //else
        //{
        //    List<IdentityUser> inGroupUsers = UserAndOrganizationStorage.Instance.UserGroupUsers(group.Id ?? 0).ToList();
        //    if (inGroupUsers.Count != 0)
        //    {
        //        List<IdentityUser> userGroupUsers = inGroupUsers.Where(user1 => CheckUserClaims(user1, resource, claimRestrictionValues))
        //            .ToList();
        //        AuditTrace(
        //            $"User Group {group.Id} {group.Code} {group.Name} has {inGroupUsers.Count} users that {userGroupUsers.Count} users are in claim, activity : {wi?.activity?.Id}",
        //            wi?.pi, wi);
        //        AddUserSelections(users, resource, claimRestrictionValues, userGroupUsers, group.Id ?? 0, group.Name);
        //    }
        //    else
        //    {
        //        AuditTrace(
        //            $"User Group {userGroupId} {group?.Code} {group?.Name ?? userGroupId} has not any user, activity : {wi?.activity?.Id}",
        //            wi?.pi, wi);
        //    }
        //}
    }

    //private static void AddUserSelections(UserSelections users, HumanResource resource,
    //    List<UserClaimRestrictionValue> claimRestrictionValues,
    //    IEnumerable<IdentityUser> userGroupUsers, long userGroupId, string userGroupName)
    //{
    //    foreach (IdentityUser user in userGroupUsers)
    //    {
    //        AddUserSelection(users, resource, claimRestrictionValues, userGroupId, userGroupName, user);
    //    }
    //}

    private static void AddUserSelection(UserSelections users, HumanResource resource,
        List<UserClaimRestrictionValue> claimRestrictionValues,
        long userGroupId, string userGroupName, IdentityUser user)
    {
        if (user == null || users.ContainsKey(user.Id))
        {
            return;
        }

        if (CheckUserClaims(user, resource, claimRestrictionValues))
        {
            users.Add(user.Id, new UserSelection
            {
                User = user,
                UserGroupId = userGroupId,
                UserGroupName = userGroupName
            });
        }
    }

    private void FetchUsersOfUserResource(UserTaskInstance wi, UserSelections users, HumanResource resource,
        string id, List<UserClaimRestrictionValue> claimRestrictionValues)
    {
        IdentityUser user = UserAndOrganizationStorage.Instance.Users.GetItem(id);
        AuditTrace($"Distribute work to {resource.type} : {user?.UserName ?? id}, activity : {wi.activity.Id}",
            wi.pi, wi);
        AddUserSelection(users, resource, claimRestrictionValues, 0, resource.Name, user);
    }

    private List<UserClaimRestrictionValue> SetClaimRestrictionValues(UserTaskInstance wi, ResourceRole resourceRole, HumanResource resource)
    {
        List<UserClaimRestrictionValue> claimRestrictionValues = [];
        List<string> userClaimRestrictions = resource.UserClaimRestrictions?.ToList();
        if (userClaimRestrictions != null)
        {
            claimRestrictionValues.AddRange(userClaimRestrictions.Select(claim =>
            {
                ResourceParameterBinding resourceParameterBinding = resourceRole.resourceParameterBindings?.FirstOrDefault(binding =>
                    binding.parameterRef != null && binding.parameterRef.Name == claim &&
                    binding.parameterRef.type == ResourceParameter.eType.Claim);
                string claimValueProperty = resourceParameterBinding?.FormalExpression?.Expression?.ExpressionString;
                string claimValue = claimValueProperty != null
                    ? wi.FetchData(claimValueProperty)
                    : $"Error : Can not find binding of Claim {claim}";
                AuditTrace(
                    $"User claim({claim}) restriction field({claimValueProperty}) value({claimValue}), activity : {wi.activity.Id}", wi.pi, wi);
                return new UserClaimRestrictionValue
                {
                    ClaimType = claim,
                    ClaimValue = claimValue
                };
            }));
        }

        return claimRestrictionValues;
    }

    private string FetchResourceParameterValue(UserTaskInstance ai, ResourceRole resourceRole, HumanResource resource, string name)
    {
        string id = null;
        ResourceParameterBinding resourceParameterBinding = resourceRole.resourceParameterBindings?.FirstOrDefault(binding =>
            binding.parameterRef != null &&
            binding.parameterRef.Name == name &&
            (binding.parameterRef.type == ResourceParameter.eType.UserField ||
             (binding.parameterRef.type == ResourceParameter.eType.EntityField && resource.type == HumanResourceType.User)));
        if (resourceParameterBinding?.FormalExpression?.Expression != null)
        {
            if (ai.GetData(resourceParameterBinding.FormalExpression.Expression.ExpressionString, out object obj))
            {
                id = obj?.ToString();
            }

            if (string.IsNullOrEmpty(id) && resourceParameterBinding.parameterRef.isRequired)
            {
                LogError(
                    $"value of field({resourceParameterBinding.FormalExpression.Expression.ExpressionString}) that determins resource is null, activity : {ai.activity.Id}", ai.pi, ai);
            }
        }
        return id;
    }

    private static bool CheckUserClaims(IdentityUser user, HumanResource resource,
        IEnumerable<UserClaimRestrictionValue> userClaimRestrictions)
    {
        if (user == null)
        {
            return false;
        }

        return false;
        //TODO
        //IEnumerable<IdentityUserClaim> userClaims = UserAndOrganizationStorage.Instance.UsersClaims.GetCategorizedItems("UserId", user.Id);
        //return resource.UserClaimRestrictions == null ||
        //        (userClaimRestrictions?.All(
        //            ucr => userClaims.Any(uc => uc.Type == ucr.ClaimType && uc.ClaimValue == ucr.ClaimValue)) ?? true);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    private double GetRank(IdentityUser user, UserTaskInstance wi)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        if (UserTask?.WorkDistributionPolicy?.rankingFormula == null)
        {
            return 0;
        }
        //todo eval userTaskRuntime?.TaskDefinition?.WorkDistributionPolicy?.rankingFormula
        return 1;
    }

    private double calcWorkAllocationFactor()
    {
        //TODO
        return 0;
    }

    public class UserClaimRestrictionValue
    {
        public string ClaimType;
        public string ClaimValue;
    }
}
