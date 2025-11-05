namespace Neo.Bpms.Domain.Models.Base;

public interface IBaseClassId<TKey>
{
    TKey Id { get; set; }
}
