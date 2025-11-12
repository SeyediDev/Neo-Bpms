using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.UI.MVC.PartialModels.Report
{
    public class ChartScriptModel
    {
        public ChartScriptModel(string idx, ReportData reportInfo, string calendar, string chartTitle, bool shouldShowTitle)
        {
            Idx = idx;
            ReportInfo = reportInfo;
            Calendar = calendar;
            ChartTitle = chartTitle;
            ShouldShowTitle = shouldShowTitle;
        }

        public string Idx { get; }

        public ReportData ReportInfo { get; }

        public string Calendar { get; }

        public string ChartTitle { get; }

        public bool ShouldShowTitle { get; }

        public ReportStructure Structure => ReportInfo?.structure;
    }
}
