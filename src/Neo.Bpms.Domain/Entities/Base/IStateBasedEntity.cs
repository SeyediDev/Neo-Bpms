namespace Neo.Bpms.Domain.Entities.Base;

public interface IStateBasedEntityBase
{
    long StateId { get; set; }
}

public interface IStateBasedEntityWithKey<TKey> : IStateBasedEntityBase, IEntity<TKey>
{
}

public interface IStateBasedEntity : IStateBasedEntityWithKey<long>
{
}
