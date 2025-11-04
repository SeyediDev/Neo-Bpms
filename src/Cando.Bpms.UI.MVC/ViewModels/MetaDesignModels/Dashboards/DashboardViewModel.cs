namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.Dashboards;

public class DashboardViewModel
{
    public DashboardViewModel()
    {

    }
    public DashboardViewModel(Dashboard dashboard)
    {
        id = dashboard.Id;
        name = dashboard.Name;
        namespaceId = dashboard.NamespaceId;
        entityId = dashboard.EntityId;
        reports = dashboard.Reports?.Select(r => new DashboardReportViewModel(r));
    }

    public string id { get; set; }
    public string name { get; set; }
    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public IEnumerable<DashboardReportViewModel> reports { get; set; }

    internal void Modify(Dashboard dashboard)
    {
        dashboard.Id = id;
        dashboard.Name = name;
        dashboard.Reports = reports?.Select(r => r.ToDashboardReport()).ToList();
    }
}
