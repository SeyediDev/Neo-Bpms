using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Repository.Entities;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public abstract class EntityItemConfigBackupRestore<TConfiguredEntityItem>(IBpmsSubjectSettingRepository repository) :
    ItemConfigBackupRestore<TConfiguredEntityItem>()
    where TConfiguredEntityItem : ConfiguredEntityItem
{
    protected string SubjectId(TConfiguredEntityItem cr)
    {
        return SubjectId(cr.EntityItem.ItemType, cr.EntityItem.NamespaceId, cr.EntityItem.EntityId, cr.EntityItem.ItemId);
    }

    public async Task Save(TConfiguredEntityItem config, CancellationToken cancellationToken = default)
    {
        await repository.SaveAsync(SubjectTitle, SubjectId(config), config.ConfigId, config, cancellationToken);
    }

    public async Task RemoveConfig(TConfiguredEntityItem config, CancellationToken cancellationToken = default)
    {
        if (config.Id > 0)
        {
            await repository.RemoveAsync(config.Id, cancellationToken);
        }
        else
        {
            await repository.RemoveAsync(SubjectTitle, SubjectId(config), config.ConfigId, cancellationToken);
        }
    }

    public async Task RemoveConfig(string itemType, string namespaceId, string entityId, string itemId, string configId, CancellationToken cancellationToken = default)
    {
        await repository.RemoveAsync(SubjectTitle, SubjectId(itemType, namespaceId, entityId, itemId), configId, cancellationToken);
    }
    
    public async Task RemoveConfigs(string namespaceId, string entityId, string formId, string reportId, string dashboardId,
        string configId, CancellationToken cancellationToken = default)
    {
        List<TConfiguredEntityItem> configurations = await Configurations(namespaceId, entityId, formId, reportId, dashboardId, cancellationToken);
        foreach (TConfiguredEntityItem item in configurations ?? [])
        {
            if(!string.IsNullOrEmpty(configId) )
            {
                if (configId != item.ConfigId)
                    continue;
            }
            await RemoveConfig(item, cancellationToken);
        }
    }

    public async Task<TConfiguredEntityItem> GetConfig(string configId, CancellationToken cancellationToken = default)
    {
        return await repository.GetByKeyAsync<TConfiguredEntityItem>(SubjectTitle, configId, (config, setting) => config, cancellationToken);
    }

    public async Task<TConfiguredEntityItem> GetConfig(long id, CancellationToken cancellationToken = default)
    {
        return await repository.GetAsync<TConfiguredEntityItem>(id, (config, setting) => config, cancellationToken);
    }

    public async Task<TConfiguredEntityItem> GetConfig(string itemType, string namespaceId, string entityId, string itemId, string configId, CancellationToken cancellationToken = default)
    {
        return await repository.GetAsync<TConfiguredEntityItem>(SubjectTitle, SubjectId(itemType, namespaceId, entityId, itemId), configId, (config, setting) => config, cancellationToken);
    }

    public async Task<List<TConfiguredEntityItem>> Configurations(string itemType, string namespaceId, string entityId, string itemId, CancellationToken cancellationToken = default)
    {
        return await repository.GetAllConfigsAsync<TConfiguredEntityItem>(SubjectTitle, SubjectId(itemType, namespaceId, entityId, itemId), (config, setting) => config, cancellationToken);
    }

    public async Task<List<TConfiguredEntityItem>> Configurations(EntityItem entityItem, CancellationToken cancellationToken = default)
    {
        return await repository.GetAllConfigsAsync<TConfiguredEntityItem>(SubjectTitle, SubjectId(entityItem.ItemType, entityItem.NamespaceId, entityItem.EntityId, entityItem.ItemId), (config, setting) => config, cancellationToken);
    }
    
    public async Task<List<TConfiguredEntityItem>> Configurations(string namespaceId, string entityId, string formId, string reportId, string dashboardId, CancellationToken cancellationToken = default)
    {
        return await repository.GetAllConfigsAsync<TConfiguredEntityItem>(SubjectTitle,
            SubjectId(ItemType(formId, reportId, dashboardId), namespaceId, entityId,
                      ItemId(formId, reportId, dashboardId)),
                      (config, setting) => config, cancellationToken);
    }
}
