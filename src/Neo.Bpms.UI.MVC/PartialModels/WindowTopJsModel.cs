using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.UI.MVC.PartialModels;

public class WindowTopJsModel
{
    public List<ConfiguredReport.ConfiguredSubReport> drillDownSubReports { get; set; }
    public string reportKey { get; set; }
    public ReportData reportInfo { get; set; }
    public ElasticObject filterValues { get; set; }
}