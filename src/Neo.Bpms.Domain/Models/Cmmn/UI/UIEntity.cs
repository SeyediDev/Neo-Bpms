using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Models.Cmmn.UI;

/// <summary>
/// this is an extension to entity to manage ui components related to the entity
/// </summary>
public class UiEntity : Entity
{
    public UiEntity(ModelNamespace model, Type entityType, string name, string id, string enName,
        string schema = null, string providerName = null)
        : base(model, entityType, name, id, enName, schema, providerName)
    {
    }

    public UiEntity()
    {
    }

    #region forms

    [XmlIgnore] private ConcurrentDictionary<string, Form> _forms;

    public IEnumerable<Form> getForms()
    {
        return _forms?.Values;
    }

    public override object GetForm(string formId)
    {
        return getForm(formId);
    }

    public Form getForm(string formId)
    {
        if (_forms == null || formId == null) return null;
        _forms.TryGetValue(formId, out Form form);
        return form;
    }

    public Form getForm(string formId, Form.eFormType formType)
    {
        if (_forms == null) return null;
        if (string.IsNullOrEmpty(formId))
        {
            return _forms.Values.FirstOrDefault(item => item.FormType == formType);
        }

        _forms.TryGetValue(formId, out Form form);
        return form;
    }

    public Form GetEntityForm(string formId)
    {
        return getForm(formId);
    }

    public Form GetEntityForm(string formId, Form.eFormType? formType, string formSubjectId)
    {
        return getForm(formId, formType, formSubjectId);
    }

    public Form getForm(string formId, Form.eFormType? formType, string formSubjectId)
    {
        if (_forms == null) return null;
        if (string.IsNullOrEmpty(formId))
            return _forms.Values.FirstOrDefault(item =>
                item.FormType == formType && (item.FormSubjectId ?? "") == (formSubjectId ?? ""));
        _forms.TryGetValue(formId, out Form form);
        return form;
    }

    public void AddForm(Form form, bool replace = false)
    {
        _forms ??= [];
        if (_forms.ContainsKey(form.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate form:" + form.Id + " in entity :" + Id);
                return;
            }

            _forms?.TryRemove(form.Id, out _);
        }

        _forms.TryAdd(form.Id, form);
    }

    #endregion forms

    #region reports

    [XmlIgnore] private ConcurrentDictionary<string, Report> _reports;

    public IEnumerable<Report> GetReports()
    {
        return _reports?.Values;
    }

    public Report GetReport(string reportId)
    {
        if (_reports == null || reportId == null) return null;
        _reports.TryGetValue(reportId, out Report report);
        return report;
    }

    public void AddReport(Report report, bool replace = false)
    {
        _reports ??= new ConcurrentDictionary<string, Report>();
        if (_reports.ContainsKey(report.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate report :" + report.Id + " in entity :" + Id);
                return;
            }

            _reports?.TryRemove(report.Id, out _);
        }

        _reports.TryAdd(report.Id, report);
    }

    #endregion reports

    #region dashboards

    [XmlIgnore] 
    private ConcurrentDictionary<string, Dashboard> _dashboards;

    public bool Defined { get; set; }

    public void AddDashboard(Dashboard dashboard, bool replace = false)
    {
        _dashboards ??= new ConcurrentDictionary<string, Dashboard>();
        if (_dashboards.ContainsKey(dashboard.Id))
        {
            if (!replace)
            {
                System.Diagnostics.Debug.Assert(true, "Duplicate dashboard :" + dashboard.Id + " in entity :" + Id);
                return;
            }

            _dashboards?.TryRemove(dashboard.Id, out _);
        }

        _dashboards.TryAdd(dashboard.Id, dashboard);
    }

    public Dashboard GetDashboard(string dashboardId)
    {
        if (_dashboards == null || dashboardId == null) return null;
        _dashboards.TryGetValue(dashboardId, out Dashboard dashboard);
        return dashboard;
    }

    public ConcurrentDictionary<string, Dashboard> GetDashboards() => _dashboards;

    #endregion

    public void SetUiComponents(UiEntity entity)
    {
        _forms = entity._forms;
        _reports = entity._reports;
        _dashboards = entity._dashboards;
        SetUiComponentParent(entity, _forms?.Values);
        SetUiComponentParent(entity, _reports?.Values);
        SetUiComponentParent(entity, _dashboards?.Values);
    }

    private static void SetUiComponentParent(UiEntity entity, IEnumerable<BaseModelClass> uiComponents)
    {
        if (uiComponents == null)
            return;
        foreach (BaseModelClass uiComponent in uiComponents)
        {
            uiComponent.Parent = entity;
        }
    }

    public bool DeleteDashboard(string dashboardId, out Dashboard dashboard)
    {
        dashboard = null;
        return _dashboards?.TryRemove(dashboardId, out dashboard) ?? false;
    }

    public bool DeleteDashboard(string dashboardId)
    {
        return DeleteDashboard(dashboardId, out _);
    }

    public bool DeleteForm(string formId, out Form form)
    {
        form = null;
        return _forms?.TryRemove(formId, out form) ?? false;
    }

    public bool DeleteForm(string formId)
    {
        return DeleteForm(formId, out _);
    }

    public bool DeleteReport(string reportId, out Report report)
    {
        report = null;
        return _reports?.TryRemove(reportId, out report) ?? false;
    }

    public bool DeleteReport(string reportId)
    {
        return DeleteReport(reportId, out _);
    }
}
