using Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.Dashboards;

public class EntityReportsViewModel(UiEntity entity)
{
    public string entityId { get; set; } = entity.Id;
    public string entityName { get; set; } = entity.Name;
    public IEnumerable<ReportRecognizer> reports { get; set; } = entity.GetReports()?.Select(report => new ReportRecognizer(report)) ?? [];
}
