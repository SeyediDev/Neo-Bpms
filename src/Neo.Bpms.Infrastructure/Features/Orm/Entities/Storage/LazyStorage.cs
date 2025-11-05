namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;

public abstract class LazyStorage<T> : StorageContainer
    where T : new()
{
    private static readonly Lazy<T> Lazy = new(() => new T());

    public static T Instance => Lazy.Value;
}