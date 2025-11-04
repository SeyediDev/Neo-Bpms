namespace Neo.Bpms.UI.MVC.Controls;

public class LinkDefinition(InputFieldDefinition controlDefinition, IUrlHelper urlHelper)
{
    public InputFieldDefinition ControlDefinition { get; } = controlDefinition;

    public string Id => ControlDefinition.FieldName;
    public string Title => ControlDefinition.Alias;

    /// <summary>
    /// For external links
    /// </summary>
    public string ExternalUrl { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.Url);

    public string NamespaceId { get; set; } = controlDefinition.NamespaceId;
    public string EntityId { get; set; } = controlDefinition.EntityId;
    public string SubjectId { get; } = controlDefinition.FormSubjectId;

    /// <summary>
    /// "Form"/"Report"/"Dashboard"
    /// </summary>
    public string PageType { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.PageType);

    /// <summary>
    /// e.g. "Edit" in Form/Edit
    /// </summary>
    public string PageSubType { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.PageSubType);

    /// <summary>
    /// FormId/ReportId/DashboardId
    /// </summary>
    public string PageId { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.EntityItemId);
    /// <summary>
    /// for Report/Dashboard
    /// </summary>
    public string ConfigurationId { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.ReportConfigurationId);
    public string ProcessId { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.ProcessId);
    public string TaskId { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.TaskId);

    public string Controller { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.LinkPathFirstPart);
    public string Action { get; set; } = controlDefinition.PropertyValue(eControlPropertyId.LinkPathLastPart);
    public IList<string> QueryParameters { get; set; } = [.. controlDefinition.GetProperties(eControlPropertyId.LinkParameter).Select(p => p.Value.ToString())];

    public bool IsAccessibleFor(IdentityUser user)
    {
        if (!string.IsNullOrEmpty(ExternalUrl))
        {
            return true;
        }

        if (!string.IsNullOrEmpty(Controller))
        {
            return DependencyInjectionHolder.Instance.AccessServices.CheckControllerActionAccess(user, Controller, Action);
        }

        switch (PageType)
        {
            case "Form":
                {
                    if (!string.IsNullOrEmpty(TaskId))
                    {
                        Form form = ProjectDefinition.Project.GetProcessTaskForm(ProcessId, TaskId);
                        return DependencyInjectionHolder.Instance.AccessServices.CheckFormAccess(user, form, out _);
                    }
                    UiEntity e = (UiEntity)ProjectDefinition.Project.GetModel(NamespaceId)?.GetEntity(EntityId);
                    Form eform = e?.getForm(PageId);
                    if (eform != null && DependencyInjectionHolder.Instance.AccessServices.CheckFormAccess(user, eform, out _ ))
                        return true;
                    if (string.IsNullOrEmpty(PageId))
                    {
                        eform = e?.getForms()?.FirstOrDefault(item => item.FormType == Form.eFormType.Index &&
                            item.FormSubjectId == SubjectId);
                        if (eform != null && DependencyInjectionHolder.Instance.AccessServices.CheckFormAccess(user, eform, out _))
                            return true;
                    }
                    return false;
                }
            case "Report":
                {
                    UiEntity e = (UiEntity)ProjectDefinition.Project.GetModel(NamespaceId)?.GetEntity(EntityId);
                    Report report = e?.GetReport(PageId);
                    return DependencyInjectionHolder.Instance.AccessServices.CheckReportAccess(user, report);
                }
            case "Dashboard":
                {
                    UiEntity e = (UiEntity)ProjectDefinition.Project.GetModel(NamespaceId)?.GetEntity(EntityId);
                    Dashboard dashboard = e.GetDashboard(PageId);
                    return DependencyInjectionHolder.Instance.AccessServices.CheckDashboardAccess(user, dashboard);
                }
            default:
                return false;
        }
    }

    public string GetUrl()
    {
        if (!string.IsNullOrEmpty(ExternalUrl))
        {
            return ExternalUrl;
        }

        return !string.IsNullOrEmpty(Controller)
            ? Url.Action(Action, Controller, urlHelper) + GetQueryString('?')
            : PageType switch
            {
                "Form" => string.IsNullOrEmpty(TaskId) ?
                                    Url.Action(PageSubType ?? "Index", "Form", urlHelper) + GetPageParameters() + GetQueryString('&') :
                                    Url.Action("Form", "Process", urlHelper, new
                                    {
                                        taskId = TaskId,
                                        __processId = ProcessId
                                    }),
                "Report" => Url.Action("Index", "Report", urlHelper) + GetPageParameters() + GetQueryString('&'),
                "Dashboard" => Url.Action("Index", "Dashboard", urlHelper) + GetPageParameters() + GetQueryString('&'),
                _ => null,
            };
    }

    private string GetPageParameters()
    {
        string result = $"?NamespaceId={NamespaceId}&EntityId={EntityId}";
        if (!string.IsNullOrEmpty(SubjectId))
        {
            result += $"&FormSubjectId={SubjectId}";
        }
        if (!string.IsNullOrEmpty(PageId))
        {
            result += $"&{PageType}Id={PageId}";
        }
        if (!string.IsNullOrEmpty(ConfigurationId))
        {
            result += $"&ConfigId={ConfigurationId}";
        }
        if (!string.IsNullOrEmpty(ProcessId))
        {
            result += $"&ProcessId={ProcessId}&TaskId={TaskId}";
        }
        return result;
    }

    private string GetQueryString(char startWith) => QueryParameters?.Any() ?? false
        ? (startWith + string.Join("&", QueryParameters))
        : string.Empty;
}
