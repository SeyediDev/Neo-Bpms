using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Security;

public static class UserAuthorizationExtensions
{
    private static readonly IAccessServices AccessServices = DependencyInjectionHolder.Instance.AccessServices;
    public static bool CheckSystemFeatureAccess(this IdentityUser user, SystemFeatureId systemFeature)
    {
        ArgumentNullException.ThrowIfNull(user);
        return AccessServices.CheckSystemFeatureAccess(user, systemFeature);
    }

    public static bool CheckFormAccess(this IdentityUser user,
        string namespaceId, string entityId, Form.eFormType formType, string formSubjectId, string formId,
        Form form = null)
    {
        return user.CheckFormAccess(out _, namespaceId, entityId, formType, formSubjectId, formId, form);
    }

    public static bool CheckFormAccess(this IdentityUser user, out long userGroupId,
        string namespaceId, string entityId, Form.eFormType formType,
        string formSubjectId, string formId, Form form = null)
    {
        ArgumentNullException.ThrowIfNull(user);
        Form entityForm = form as Form;
        if (form == null)
        {
            entityForm = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?
                .GetEntityForm(formId, formType, formSubjectId);
        }

        userGroupId = 0;
        return entityForm != null && user.CheckAccess(entityForm, out userGroupId);
    }

    public static bool CheckAccess(this IdentityUser user, Form entityForm, out long userGroupId)
    {
        if (AccessServices.CheckFormAccess(user, entityForm, out userGroupId))
            return true;
        userGroupId = 0;
        return false;
    }

    public static bool CheckReportAccess(this IdentityUser user, string namespaceId, string entityId, string reportId, Report report = null)
    {
        ArgumentNullException.ThrowIfNull(user);
        report ??= ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?
                .GetReport(reportId);

        return report != null && AccessServices.CheckReportAccess(user, report);
    }

    public static bool CheckDashboardAccess(this IdentityUser user, string namespaceId, string entityId,
        string dashboardId, Dashboard dashboard = null)
    {
        ArgumentNullException.ThrowIfNull(user);
        dashboard ??= ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?.GetDashboard(dashboardId);

        return dashboard != null && AccessServices.CheckDashboardAccess(user, dashboard);
    }

    public static bool CheckControllerActionAccess(this IdentityUser user, string controllerId, string action)
    {
        ArgumentNullException.ThrowIfNull(user);
        return AccessServices.CheckControllerActionAccess(user, controllerId, action);
    }

    public static bool CheckProcessAccess(this IdentityUser user, string processId)
    {
        ArgumentNullException.ThrowIfNull(user);
        return ProjectDefinition.Project.ProcessExists(processId) && AccessServices.CheckProcessAccess(user, processId);
    }
}
