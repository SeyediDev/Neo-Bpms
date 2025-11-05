using Microsoft.AspNetCore.Mvc.Razor;

namespace Neo.Bpms.UI.MVC.PartialModels.Report;

public class DrillDownHeadModel
{
    public ReportData reportInfo { get; set; }
    public bool canDesign { get; set; }
    public HelperResult inFormContent { get; set; }
}