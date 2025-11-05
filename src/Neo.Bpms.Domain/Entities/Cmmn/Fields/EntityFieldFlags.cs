namespace Neo.Bpms.Domain.Entities.Cmmn.Fields;

[Flags]
public enum EntityFieldFlags
{
    None = 0,
    DerivedEntityBooleanField = 0x1,
    IsBitMask = 0x200,
    NotNull = 0x800,
    IncludeInPKV = 0x1000,
    NotMap = 0x2000,
    AuditField = 0x4000,
}
