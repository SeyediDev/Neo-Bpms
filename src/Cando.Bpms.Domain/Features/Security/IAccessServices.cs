using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Security.Authentication;
using Neo.Bpms.Domain.Entities.Security.Authorization;

namespace Neo.Bpms.Domain.Features.Security;

public interface IAccessServices
{
    bool CheckFormAccess(IdentityUser user, Form form, out long roleId);
    bool CheckReportAccess(IdentityUser user, Report report);
    bool CheckDashboardAccess(IdentityUser user, Dashboard dashboard);
    bool CheckSystemFeatureAccess(IdentityUser user, SystemFeatureId systemFeature);
    bool CheckControllerActionAccess(IdentityUser user, string controllerId, string action);
    bool CheckProcessAccess(IdentityUser user, string processId);
    bool InUserGroup(IdentityUser user, string userGroupCode);
    bool InUserGroup(string userName, string userGroupCode);
    List<string> UserClaims(IdentityUser user, string claimType);
    List<string> UserClaims(string userName, string claimType);

    IdentityRole CheckUserGroupAccess(IdentityRole userGroup, eActivityType activityType,
        string namespaceId, string entityId, string entityItemId,
        long systemFeatureId, long formType, string formSubjectId,
        string controllerId, string action);
    IdentityRole GetUserGroupByCode(string userGroupName);
}
