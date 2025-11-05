namespace Neo.Bpms.Domain.Enums;

[Flags]
public enum UserSecurityAccessFlags : uint
{
    None = 0,
    Create = 1,
    Read = 2,
    Update = 4,
    Delete = 8,
    All = Create | Read | Update | Delete
}
