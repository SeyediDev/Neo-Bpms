using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI;
using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Security.AccessServices;

//TODO MRSH
public class NeoAccessServices : IAccessServices
{
    public bool CheckFormAccess(IdentityUser user, Form form, out long userGroupId)
    {
        userGroupId = 0;
        return true;
    }

    public bool CheckReportAccess(IdentityUser user, Report report)
    {
        ArgumentNullException.ThrowIfNull(user);
        return report != null;
    }

    public bool CheckDashboardAccess(IdentityUser user, Dashboard dashboard)
    {
        ArgumentNullException.ThrowIfNull(user);
        return dashboard != null;
    }

    public bool CheckControllerActionAccess(IdentityUser user, string controllerId, string action)
    {
        ArgumentNullException.ThrowIfNull(user);
        return true;
    }

    public bool CheckProcessAccess(IdentityUser user, string processId)
    {
        ArgumentNullException.ThrowIfNull(user);
        return ProjectDefinition.Project.ProcessExists(processId);
    }

    public bool CheckSystemFeatureAccess(IdentityUser user, SystemFeatureId systemFeature)
    {
        ArgumentNullException.ThrowIfNull(user);
        return true;
    }

    public bool InUserGroup(IdentityUser user, string userGroupCode)
    {
        ArgumentNullException.ThrowIfNull(user);
        return true;
    }
    public bool InUserGroup(string userName, string userGroupCode)
    {
        ArgumentNullException.ThrowIfNull(userName);
        return true;
    }

    /// <summary>
    /// کلیم های کاربر
    /// </summary>
    /// <param name="user">کاربر</param>
    /// <param name="claimType">نوع کلیم</param>
    /// <returns></returns>
    public List<string> UserClaims(IdentityUser user, string claimType)
    {
        return claimType switch
        {
            "Mobile" => [user.MobileNo],
            "NationalNumber" => [user.NationalNumber],
            //TODO
            _ => null,
        };
    }
    public List<string> UserClaims(string userName, string claimType)
    {
        //TODO MRSH
        return [];
    }

    public IdentityRole CheckUserGroupAccess(IdentityRole userGroup, eActivityType activityType,
        string namespaceId, string entityId, string entityItemId,
        long systemFeatureId, long formType, string formSubjectId,
        string controllerId, string action)
    {
        //TODO MRSH
        return null;
    }

    public IdentityRole GetUserGroupByCode(string userGroupName)
    {
        //TODO MRSH
        return null;
    }
}
