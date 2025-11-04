namespace Neo.Bpms.Domain.Entities.Base;

[States(typeof(StateBaseEntityId))]
public abstract class StateBasedEntityBase : BpmsBaseEntity, IStateBasedEntityBase
{
    protected StateBasedEntityBase()
    {
        StateId = StateBaseEntityId.Active.ToInt();
    }
    [DisplayName("وضعیت")]
    public long StateId { get; set; }

    public static string ActiveFilter => $"{nameof(StateId)}=={StateBaseEntityId.Active:D}";
}
