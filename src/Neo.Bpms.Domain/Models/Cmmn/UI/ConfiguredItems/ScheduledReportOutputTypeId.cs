namespace Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

[Description("نوع خروجی")]
public enum ScheduledReportOutputTypeId
{
    [Description("دایرکتوری فایل")]
    FileDirectory = 1,

    [Description("ایمیل")]
    Email = 2,

    [Description("فایل سیستمی")]
    FileInfo = 3,

    [Description("پیامک")]
    Sms = 4,

    [Description("اطلاع رسانی")]
    Notification = 5
}