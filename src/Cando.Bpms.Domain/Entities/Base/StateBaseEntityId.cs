namespace Neo.Bpms.Domain.Entities.Base;

public enum PublicEntityStateId
{
    Active = 1,
    Backup = 101,
    WaitForBackup = 102
}
public enum StateBaseEntityId
{
    [EAttr_State(1, "Active", "فعال", EntityStateCategory.ActiveNode)]
    Active = PublicEntityStateId.Active,
    [EAttr_State(101, "Backup", "بایگانی شده", EntityStateCategory.BackupNode)]
    Backup = PublicEntityStateId.Backup
}
[Flags]
public enum EntityStateCategory
{
    None = 0,

    BackupFromCascade = 2,
    BackupNode = 4,
    ActiveNode = 8,
    InProgress = 0x10,

    //AccountRegistered = 0x10,
    //MustbeCleanedAtYearEnd = 0x20,
    //Mandatory = 0x40,
    Rejected = 0x80,
    //WaitForSubProcess = 0x100,
    //Virtual = 0x200,
    //WaitForAutomaticProcess = 0x400,
    //Hidden = 0x800,
    //PackOnlyActives = 0x1000,

}
