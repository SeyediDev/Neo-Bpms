using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public abstract class ReportActionType
{
    public ScheduledReportAction ReportAction { get; set; }

    public virtual void AddRows(IList<ReportRowInfo> rowList)
    {
        ReportAction.ReportView.GenerateRows(rowList);
    }

    public abstract Task<bool> DoAction(LocalParameters outParameters);

    protected string GetFileExtension()
    {
        string extension = "";
        switch (ReportAction.ScheduledReport.FileTypeId)
        {
            case ScheduledReportFileTypeId.Html:
                extension = ".html";
                break;
            case ScheduledReportFileTypeId.Excel:
                extension = ".xlsx";
                break;
        }

        return extension;
    }
}
