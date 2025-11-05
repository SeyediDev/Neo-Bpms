namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public class ReportActionEmail : ReportActionType
{
    public override async Task<bool> DoAction(LocalParameters outParameters)
    {
        string extension = GetFileExtension();
        outParameters.Add("FileName", (ReportAction.ScheduledReport.ActionName ?? ReportAction.ScheduledReportName) + "_" +
                       ReportAction.date.ToString("yyyy-MM-dd hh-mm") + extension);
        _ = ReportAction.ScheduledReport.ActionName ?? ReportAction.ScheduledReportName;
        _ = ReportAction.ReportView.Content as MemoryStream;
        if (ReportAction.ScheduledReport.UserGroupId > 0)
        {
            //EmailManager.SendFileToUserGroup(ReportAction.ScheduledReport.UserGroupId,
            //    emailTitle, "Please open the attachment", ReportAction.ScheduledReport.ContactFunctionTypeId,
            //    memoryStream, "report.xslt", "text", "xml", ReportAction.MessageGateway.EmailSender);
        }
        if (!string.IsNullOrEmpty(ReportAction.ScheduledReport.UserId))
        {
            //EmailManager.SendEmailWithAttatchedFileToUser(ReportAction.ScheduledReport.UserId,
            //    emailTitle, "Please open the attachment", ReportAction.ScheduledReport.ContactFunctionTypeId,
            //    memoryStream, "report.xslt", "text", "xml", ReportAction.MessageGateway.EmailSender);
        }
        await Task.CompletedTask;
        return true;
    }
}
