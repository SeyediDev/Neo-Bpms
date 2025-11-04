namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.Dashboards;

public class DashboardReportViewModel
{
    public DashboardReportViewModel()
    {

    }
    public DashboardReportViewModel(Dashboard.DashboardReport dashboardReport)
    {
        namespaceId = dashboardReport.NamespaceId;
        entityId = dashboardReport.EntityId;
        id = dashboardReport.ReportId;
        name = ProjectDefinition.Project.GetUiEntity(dashboardReport.NamespaceId, dashboardReport.EntityId)
            ?.GetReport(dashboardReport.ReportId)?.Name;
    }
    public string id { get; set; }//todo rename
    public string name { get; set; }
    public string namespaceId { get; set; }
    public string entityId { get; set; }

    public Dashboard.DashboardReport ToDashboardReport()
    {
        return new Dashboard.DashboardReport
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            ReportId = id
        };
    }
}
