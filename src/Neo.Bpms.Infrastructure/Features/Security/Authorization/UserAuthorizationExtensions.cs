using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI;
using Neo.Bpms.Domain.Models.Security.Authorization;

namespace Neo.Bpms.Infrastructure.Features.Security.Authorization;

public static class UserAuthorizationExtensions
{
    public static bool CheckSystemFeatureAccess1(this IdentityUser user, SystemFeatureId systemFeature)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        return AuthorizationManager.CheckSystemFeatureAccess(user, systemFeature);
    }

    public static bool CheckFormAccess1(this IdentityUser user,
        string namespaceId, string entityId, Form.eFormType formType, string formSubjectId, string formId,
        Form form = null)
    {
        return user.CheckFormAccess1(out _, namespaceId, entityId, formType, formSubjectId, formId, form);
    }

    public static bool CheckFormAccess1(this IdentityUser user, out long userGroupId,
        string namespaceId, string entityId, Form.eFormType formType,
        string formSubjectId, string formId, Form form = null)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        var entityForm = form as Form;
        if (form == null)
        {
            entityForm = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?
                .GetEntityForm(formId, formType, formSubjectId);
        }

        userGroupId = 0;
        return entityForm != null && user.CheckAccess1(entityForm, out userGroupId);
    }

    public static bool CheckAccess1(this IdentityUser user, Form entityForm, out long userGroupId)
    {
        if (AuthorizationManager.CheckFormAccess(user, out userGroupId, entityForm.NamespaceId,
            entityForm.EntityId, entityForm.FormType, entityForm.FormSubjectId, entityForm.Id))
            return true;
        userGroupId = 0;
        return false;
    }

    public static bool CheckReportAccess1(this IdentityUser user, string namespaceId, string entityId, string reportId,
        Report report = null)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        report ??= ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?
                .GetReport(reportId);

        return report != null &&
                 AuthorizationManager.CheckReportAccess(user, namespaceId, entityId, reportId);
    }

    public static bool CheckDashboardAccess1(this IdentityUser user, string namespaceId, string entityId,
        string dashboardId, Dashboard dashboard = null)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        dashboard ??= ProjectDefinition.Project.GetUiEntity(namespaceId, entityId)?
                .GetDashboard(dashboardId);

        return dashboard != null &&
                 AuthorizationManager.CheckDashboardAccess(user, namespaceId, entityId, dashboardId);
    }

    public static bool CheckControllerActionAccess1(this IdentityUser user, string controllerId, string action)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        return AuthorizationManager.CheckControllerActionAccess(user, controllerId, action);
    }

    public static bool CheckProcessAccess1(this IdentityUser user, string processId)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        return ProjectDefinition.Project.ProcessExists(processId) &&
                 AuthorizationManager.CheckProcessAccess(user, processId);
    }
}
