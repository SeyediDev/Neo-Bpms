namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// عدم ثبت سابقه تغییرات برای موجودیت
/// </summary>
public class DontAudit : EAttr_Audit
{
    /// <summary>
    /// عدم ثبت سابقه تغییرات برای موجودیت
    /// </summary>
    public DontAudit()
    {
        dontAudit = true;
    }
}