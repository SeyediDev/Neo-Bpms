namespace Neo.Bpms.Domain.Entities.Base;

public interface IBaseClassId<TKey>
{
    TKey Id { get; set; }
}
