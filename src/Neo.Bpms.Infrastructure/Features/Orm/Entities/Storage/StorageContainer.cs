using Neo.Bpms.Domain.Models.Cmmn.Storage;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Storage;
public abstract class StorageContainer
{
    public abstract List<string> Errors { get; }

    protected static List<string> ErrorsOfStorage(params IMemoryStorageBase[] storageList)
    {
        var errors = new List<string>();
        foreach (var storage in storageList)
        {
            if (storage.Errors != null)
                errors.AddRange(storage.Errors.SelectMany(e => e.Value));
        }

        return errors;
    }

    protected static List<string> ErrorsOfStorage(params StorageContainer[] storageList)
    {
        var errors = new List<string>();
        foreach (var storage in storageList)
        {
            if (storage.Errors != null)
                errors.AddRange(storage.Errors);
        }

        return errors;
    }

    public void Init()
    {
        Load();
        AfterLoad();
    }

    public abstract void Load();
    public abstract void AfterLoad();
    protected void AfterLoad(params IMemoryStorageBase[] storageList)
    {
        foreach (var storage in storageList)
        {
            storage?.AfterLoad();
        }
    }
}
