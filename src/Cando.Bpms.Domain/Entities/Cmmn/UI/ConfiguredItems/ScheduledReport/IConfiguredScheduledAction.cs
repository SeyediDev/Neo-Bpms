namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;

public interface IConfiguredScheduledAction
{
    string ActionName { get; set; }
    ScheduledReportFileTypeId FileTypeId { get; set; }
    ScheduledReportOutputTypeId OutputTypeId { get; set; }
    string UserId { get; set; }
    long UserGroupId { get; set; }
    long? ContactFunctionTypeId { get; set; }
    long MaxRecordCount { get; set; }
    string DestinationPath { get; set; }
    string DestinationUserName { get; set; }
    string DestinationPassword { get; set; }
}