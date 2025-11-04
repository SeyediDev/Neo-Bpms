namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public class ReportActionNotification : ReportActionType
{
    public override void AddRows(IList<ReportRowInfo> rowList)
    {
        if (rowList == null) return;
        foreach (ReportRowInfo row in rowList)
        {
            LocalParameters values = [];
            foreach (ColumnFieldDefinition column in ReportAction.ReportView.ColumnList)
            {
                string val = ReportAction.ReportView.FetchColumnValue(column, row);
                values.AddOrUpdate(column.Label, val);
            }

            //NotifierEngine.AddNotification(
            //    ReportAction.ScheduledReport.ActionName ?? ReportAction.ScheduledReportName,
            //    values);
        }
    }

    public override async Task<bool> DoAction(LocalParameters outParameters)
    {
        await Task.CompletedTask;
        return true;
    }
}
