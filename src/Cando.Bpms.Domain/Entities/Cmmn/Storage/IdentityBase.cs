namespace Neo.Bpms.Domain.Entities.Cmmn.Storage;

[NotInMeta]
public abstract class IdentityBase<TKey> : IBaseClassId<TKey>
{
    public virtual TKey Id { get; set; }
}
