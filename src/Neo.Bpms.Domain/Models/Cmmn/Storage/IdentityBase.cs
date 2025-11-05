namespace Neo.Bpms.Domain.Models.Cmmn.Storage;

[NotInMeta]
public abstract class IdentityBase<TKey> : IBaseClassId<TKey>
{
    public virtual TKey Id { get; set; }
}
