namespace Neo.Bpms.UI.MVC.PartialModels.Report;

public class DrillDownFilteredByModel
{
    public ReportStructure structure { get; set; }
    public IdentityUser user { get; set; }
    public string parentFilterValues { get; set; }
}