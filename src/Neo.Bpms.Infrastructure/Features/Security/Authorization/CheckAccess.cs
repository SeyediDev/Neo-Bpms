using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Security.Authorization;

public partial class AuthorizationManager
{
    public static bool CheckUserAccessToActivity(IdentityUser user, out long roleId, eActivityType activityType,
        string namespaceId, string entityId, string entityItemId,
        long systemFeatureId = 0, long formType = 0, string formSubjectId = null,
        string controllerId = null, string action = null)
    {
        if (user.IsAdmin)
        {
            roleId = 0; //todo admin default user role Id
            return true;
        }

        if (user.Disable)
        {
            roleId = 0;
            return false;
        }

        roleId = 0;
        return false;
    }

    public static bool CheckFormAccess(IdentityUser user,
        string namespaceId, string entityId, Form.eFormType? formType,
        string formSubjectId, string formId)
    {
        return CheckFormAccess(user, out _, namespaceId, entityId, formType,
            formSubjectId, formId);
    }

    public static bool CheckFormAccess(IdentityUser user, out long userGroupId,
        string namespaceId, string entityId, Form.eFormType? formType,
        string formSubjectId, string formId)
    {
        return CheckUserAccessToActivity(user, out userGroupId, eActivityType.Form,
                   namespaceId, entityId, formId,
                   0, formType.HasValue ? (int)formType : 0, formSubjectId) ||
               CheckSystemFeatureAccess(user, out userGroupId, SystemFeatureId.FormDesign);
    }

    public static bool CheckReportAccess(IdentityUser user, string namespaceId, string entityId, string reportId)
    {
        return CheckUserAccessToActivity(user, out _, eActivityType.Report, namespaceId, entityId, reportId) || CheckSystemFeatureAccess(user, SystemFeatureId.EntityDesign);
    }

    public static bool CheckDashboardAccess(IdentityUser user, string namespaceId, string entityId,
        string dashboardId)
    {
        return CheckUserAccessToActivity(user, out _, eActivityType.Dashboard, namespaceId, entityId, dashboardId) || CheckSystemFeatureAccess(user, SystemFeatureId.EntityDesign);
    }

    public static bool CheckSystemFeatureAccess(IdentityUser user, SystemFeatureId systemFeature)
    {
        return CheckSystemFeatureAccess(user, out _, systemFeature);
    }

    public static bool CheckSystemFeatureAccess(IdentityUser user, out long userGroupId, SystemFeatureId systemFeature)
    {
        return CheckUserAccessToActivity(user, out userGroupId, eActivityType.SystemFeature,
            null, null, null, (long)systemFeature);
    }

    public static bool CheckControllerActionAccess(IdentityUser user, string controllerId, string action)
    {
        return CheckUserAccessToActivity(user, out _, eActivityType.ControllerAction, null, null, null, 0, 0, null,
            controllerId, action);
    }

    public static bool CheckProcessAccess(IdentityUser user, string processId)
    {
        return CheckUserAccessToActivity(user, out _, eActivityType.Process, null, processId, null);
    }

}
