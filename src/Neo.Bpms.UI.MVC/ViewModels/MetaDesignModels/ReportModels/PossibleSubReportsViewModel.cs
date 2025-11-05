namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class PossibleSubReportsViewModel
{
    public string entityId { get; set; }
    public string entityName { get; set; }
    public string linkedField { get; set; }
    public IEnumerable<PossibleSubReportViewModel> subReports { get; set; }
}

public class PossibleSubReportViewModel
{
    public string id { get; set; }
    public string name { get; set; }
    public string entityName { get; set; }
    public string entityId { get; set; }
}
