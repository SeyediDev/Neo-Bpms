namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[UniqueIndex("StateId,Code")]
[DisplayNameAndEnName("گروه کاربری سیستم")]
public class SystemUserGroup : BaseCmmnStateBasedEntity, ICanBeDisable
{
    [DisplayNameAndEnName("نام گروه")]
    [MaxLength(80)]
    [InDisplayString]
    public string Name;

    [DisplayNameAndEnName("شرح")]
    [MaxLength(256)]
    public string? Description;

    [DisplayNameAndEnName("کد")]
    [MaxLength(256)]
    public string Code;

    [DisplayNameAndEnName("غیرفعال")]
    public bool Disable { get; set; }
}
