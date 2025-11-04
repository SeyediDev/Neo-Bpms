namespace Neo.Bpms.Domain.Modeling.Entities.CmmnConfig;

[DisplayNameAndEnName("وضعیت کاربر")]
public class UserPersistence: BaseStringListCmmnConfigEntity
{
    [EFAttr_Id(OldDbName = "F40", Name = "کاربر")]
    [MaxLength(80)]
    public string UserId;
}
