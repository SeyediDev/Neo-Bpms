namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[DisplayNameAndEnName("فهرست پیشنهاد انجام فعالیت")]
[DontAudit]
public class ProcessOfferUser : BaseProcessDataEntity
{
    [DisplayNameAndEnName("کاربر")]
    [Range(0, 41)]
    [Required]
    public string UserId;
    [DisplayNameAndEnName("کاربر")]
    public SystemUser User;
}
