namespace Neo.Bpms.UI.MVC.PartialModels.Report
{
    public class ChartScriptModel(string idx, ReportData reportInfo, string calendar, string chartTitle, bool shouldShowTitle)
    {
        public string Idx { get; } = idx;

        public ReportData ReportInfo { get; } = reportInfo;

        public string Calendar { get; } = calendar;

        public string ChartTitle { get; } = chartTitle;

        public bool ShouldShowTitle { get; } = shouldShowTitle;

        public ReportStructure Structure => ReportInfo?.Structure;
    }
}
