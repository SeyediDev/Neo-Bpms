namespace Neo.Bpms.Domain.Modeling.Entities.CmmnConfig;

[DisplayNameAndEnName("سابقه اجرای گزارش زمانبندی شده")]
[DontAudit]
public class ScheduledReportLog: BaseCmmnConfigEntity
{
    public long ScheduledReportId;

    [DisplayNameAndEnName("تاریخ")]
    public DateTime Date;

    [DisplayNameAndEnName("نام فایل")]
    [MaxLength(512)]
    public string FileName;

    [DisplayNameAndEnName("موفق")]
    [FAttr_Boolean("موفق", "ناموفق", "هر دو")]
    public bool Successfull;
}
