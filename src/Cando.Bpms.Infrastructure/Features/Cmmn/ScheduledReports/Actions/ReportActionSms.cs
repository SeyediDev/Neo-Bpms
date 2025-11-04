namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public class ReportActionSms(/*ISmsService smsService*/) : ReportActionType
{
    public override async Task<bool> DoAction(LocalParameters outParameters)
    {
        //await smsService.SendAsync(new Neo.Domain.Features.Sms.Dto.SmsDto(
        //    "09127165496",
        //    (ReportAction.ScheduledReport.ActionName ?? ReportAction.ScheduledReportName) + "\n" + ReportAction.ReportView.Content));
        await Task.CompletedTask;
        return true;
    }
}
