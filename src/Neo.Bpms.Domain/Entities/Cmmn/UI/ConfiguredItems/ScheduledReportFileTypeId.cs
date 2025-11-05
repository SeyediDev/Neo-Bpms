namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

[Description("نوع فایل")]
public enum ScheduledReportFileTypeId
{
    [Description("Html")]
    Html = 1,

    [Description("Excel")]
    Excel = 2,

    [Description("Text")]
    Text = 3,

    [Description("CSV")]
    CSV = 4
}