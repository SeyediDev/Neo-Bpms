namespace Neo.Bpms.Domain.Entities.Base;

[States(typeof(StateBaseEntityId))]
public abstract class AuditableStateBasedEntity : BaseAuditableEntity<long>
{
    [DisplayName("وضعیت")]
    public long StateId { get; set; }

    public static string ActiveFilter => $"{nameof(StateId)}=={StateBaseEntityId.Active:D}";
}
