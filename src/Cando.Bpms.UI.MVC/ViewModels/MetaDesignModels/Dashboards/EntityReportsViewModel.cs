using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.Dashboards;

public class EntityReportsViewModel
{
    public EntityReportsViewModel(UiEntity entity)
    {
        entityId = entity.Id;
        entityName = entity.Name;
        reports = entity.GetReports()?.Select(report => new ReportRecognizer(report)) ?? [];
    }
    public string entityId { get; set; }
    public string entityName { get; set; }
    public IEnumerable<ReportRecognizer> reports { get; set; }
}
