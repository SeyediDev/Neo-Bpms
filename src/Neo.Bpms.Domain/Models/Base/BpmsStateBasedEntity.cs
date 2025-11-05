namespace Neo.Bpms.Domain.Models.Base;

[States(typeof(StateBaseEntityId))]
public abstract class BpmsStateBasedEntity : BpmsBaseEntity, IStateBasedEntity
{
    protected BpmsStateBasedEntity()
    {
        StateId = StateBaseEntityId.Active.ToInt();
    }
    [DisplayName("وضعیت")]
    public long StateId { get; set; }

    public static string ActiveFilter => $"{nameof(StateId)}=={StateBaseEntityId.Active:D}";
}

[Schema(nameof(BpmsSchema.Cmmn))]
[FileGroup(nameof(BpmsSchema.Cmmn))]
public abstract class BaseCmmnStateBasedEntity : BpmsStateBasedEntity
{
}

[Schema(nameof(BpmsSchema.CmmnConfig))]
[FileGroup(nameof(BpmsSchema.CmmnConfig))]
public abstract class BaseCmmnConfigStateBasedEntity : BpmsStateBasedEntity
{
}

[Schema(nameof(BpmsSchema.ProcessModel))]
[FileGroup(nameof(BpmsSchema.ProcessModel))]
public abstract class BaseProcessModelStateBasedEntity : BpmsStateBasedEntity
{
}

[Schema(nameof(BpmsSchema.ProcessData))]
[FileGroup(nameof(BpmsSchema.ProcessData))]
public abstract class BaseProcessDataStateBasedEntity : BpmsStateBasedEntity
{
}
