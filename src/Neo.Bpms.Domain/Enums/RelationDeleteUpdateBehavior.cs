namespace Neo.Bpms.Domain.Enums;

public enum RelationDeleteUpdateBehavior :byte
{
    Cascade,
    WarningCascade,
    WarningSetNull,
    Error,
    DontCheck
}
