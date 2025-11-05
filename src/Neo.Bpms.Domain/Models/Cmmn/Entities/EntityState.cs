namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

public class EntityState : BaseModelClass
{
    public EntityState() { }
    public EntityState(Entity entity, int id, string name, string enName, object category)
        : base(entity, "" + id, name)
    {
        this.enName = enName;
        this.category = Convert.ToUInt32(category);
    }

    public long LongId => Convert.ToInt64(Id);
    public string enName { get; set; }

    public uint category;

    public string stateFieldName { get; set; }
    public bool IsActive => (category & ((uint)eCategory.ActiveNode | (uint)eCategory.InProgress)) != 0 || category == 0;

    [Flags]
    public enum eCategory
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
}
