namespace Neo.Bpms.Domain.Modeling.Entities.Cmmn;

[NotInMeta]
public abstract class MetaModelNamespacesDb : BaseCmmnStateBasedEntity
{
    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("فضای نامی")]
    public string MetaNamespaceId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    [InDisplayString]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("فضای نامی")]
public class MetaModelNamespace : MetaModelNamespacesDb
{
    [DisplayNameAndEnName("وضعیت")]
    public EntityStateName State;
}
