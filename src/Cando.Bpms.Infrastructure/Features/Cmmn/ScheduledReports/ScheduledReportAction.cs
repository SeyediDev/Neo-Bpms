using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports.ReportView;
using Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;

public class ScheduledReportAction
{
    public IConfiguredScheduledAction ScheduledReport;
    public long ScheduledReportId;
    public string ScheduledReportName;
    public DateTime date;

    public ReportViewGenerator ReportView { get; set; }
    public ReportActionType ReportActionType { get; set; }

    public void InitReport(CancellationToken cancellationToken, ReportStructure structure,
        List<ColumnFieldDefinition> columnList, string culture, string calendar)
    {
        ScheduledReportFileTypeId fileTypeId = FileTypeId();
        ReportView = fileTypeId switch
        {
            ScheduledReportFileTypeId.Html => new ReportToHtmlGenerator(cancellationToken, structure, columnList, culture, calendar),
            ScheduledReportFileTypeId.Excel => new ReportToExcelGenerator(cancellationToken, structure, columnList, culture, calendar),
            ScheduledReportFileTypeId.Text => new ReportToTextGenerator(cancellationToken, structure, columnList, culture, calendar),
            ScheduledReportFileTypeId.CSV => new ReportToCSVGenerator(cancellationToken, structure, columnList, culture, calendar),
            _ => throw new ArgumentOutOfRangeException(),
        };
        ReportActionType = ScheduledReport.OutputTypeId switch
        {
            ScheduledReportOutputTypeId.FileInfo => new ReportActionFileInfo(),
            ScheduledReportOutputTypeId.FileDirectory => new ReportActionFileDirectory(),
            ScheduledReportOutputTypeId.Email => new ReportActionEmail(),
            ScheduledReportOutputTypeId.Sms => new ReportActionSms(),
            ScheduledReportOutputTypeId.Notification => new ReportActionNotification(),
            _ => throw new ArgumentOutOfRangeException(),
        };
        ReportActionType.ReportAction = this;
    }

    public void AddRows(IList<ReportRowInfo> rowList)
    {
        ReportActionType.AddRows(rowList);
    }

    public async Task<(bool, LocalParameters)> DoAction()
    {
        LocalParameters localParameters = [];
        bool b = await ReportActionType.DoAction(localParameters);
        return (b, localParameters);
    }

    private ScheduledReportFileTypeId FileTypeId()
    {
        ScheduledReportFileTypeId fileTypeId =
            ScheduledReport.OutputTypeId == ScheduledReportOutputTypeId.Email
                ? ScheduledReportFileTypeId.Html
                : ScheduledReport.OutputTypeId == ScheduledReportOutputTypeId.Sms
                    ? ScheduledReportFileTypeId.Text
                    : ScheduledReport.FileTypeId;
        return fileTypeId;
    }
}
