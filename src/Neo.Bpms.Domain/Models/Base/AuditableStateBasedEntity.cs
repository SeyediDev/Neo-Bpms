namespace Neo.Bpms.Domain.Models.Base;

[States(typeof(StateBaseEntityId))]
public abstract class AuditableStateBasedEntity : BaseAuditableEntity<long>
{
    [DisplayName("وضعیت")]
    public long StateId { get; set; }

    public static string ActiveFilter => $"{nameof(StateId)}=={StateBaseEntityId.Active:D}";
}
