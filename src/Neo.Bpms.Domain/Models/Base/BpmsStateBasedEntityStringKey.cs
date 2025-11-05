namespace Neo.Bpms.Domain.Models.Base;

[States(typeof(StateBaseEntityId))]
public abstract class BpmsStateBasedEntityStringKey : BpmsBaseEntityStringKey, IStateBasedEntityWithKey<string>
{
    protected BpmsStateBasedEntityStringKey()
    {
        StateId = StateBaseEntityId.Active.ToInt();
    }
    [DisplayName("وضعیت")]
    public long StateId { get; set; }

    public static string ActiveFilter => $"{nameof(StateId)}=={StateBaseEntityId.Active:D}";
}

[Schema(nameof(BpmsSchema.Cmmn))]
[FileGroup(nameof(BpmsSchema.Cmmn))]
public abstract class BaseCmmnStateBasedEntityStringKey : BpmsStateBasedEntityStringKey
{
}

[Schema(nameof(BpmsSchema.ProcessModel))]
[FileGroup(nameof(BpmsSchema.ProcessModel))]
public abstract class BaseProcessModelStateBasedEntityStringKey : BpmsStateBasedEntityStringKey
{
}

[Schema(nameof(BpmsSchema.ProcessData))]
[FileGroup(nameof(BpmsSchema.ProcessData))]
public abstract class BaseProcessDataStateBasedEntityStringKey : BpmsStateBasedEntityStringKey
{
}
