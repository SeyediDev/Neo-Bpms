using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Cmmn.Data;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    public void SetEnumerationItems(MigrationOptions options, Func<string, string, string, bool> func = null)
    {
        if (Provider.DontSync) return;
        InitSyncDatabase(options, func);
        List<ModelNamespace> modelNamespaces = FetchNamespaces();

        if (_pleaseStop) return;

        SyncEnumerationItems(modelNamespaces);
        AddMessage(null, null,
            $"اتمام یکسان سازی {DatabaseName} با تعداد {Commands.Count} دستور و {Logs.Count} خطا در متادیتا");
    }
    private void SyncEnumerationItems(IList<ModelNamespace> models)
    {
        if (!Options.SetEnumerationItems || _pleaseStop) return;
        foreach (var model in models)
        {
            if (_pleaseStop) return;
            var enums = model.GetEnums();
            foreach (var myEnum in enums.Values)
            {
                if (_pleaseStop) return;
                var entity = ProjectDefinition.Project.GetEntity(myEnum.NamespaceId, myEnum.EntityId);
                if (entity == null || !EntityIsInThisProvider(entity)) continue;
                if (!NeededToSyncWithDbms(entity)) continue;
                if (myEnum.items.Count > 0)
                    SyncEnumerationItems(myEnum, entity);
            }
        }
    }

    private void SyncEnumerationItems(Enumeration myEnum, Entity entity)
    {
        var dbTableName = EntityDbNameManager.GetDbTableName(entity);
        AddMessage(entity.model, entity,
            $"{(dbTableName != entity.Id ? "-" + dbTableName : "")} ...SetEnumerationItems:{myEnum.items.Values.Count} ");
        var oldItems = QueryUtility.New(entity).ToDictionaryById();

        var activeStateId = (long?)StateBaseEntityId.Active.ToInt();
        var backupStateId = (long?)StateBaseEntityId.Backup.ToInt();
        if (entity.IsStateBase)
        {
            activeStateId = entity.FirstActiveStateId;
            backupStateId = entity.FirstBackupStateId;
        }

        var au = new ApplyUtility(entity);
        au.BeginBatch();
        foreach (var item in myEnum.items.Values)
        {
            if (_pleaseStop)
            {
                au.EndBatch();
                return;
            }

            var itemId = Convert.ToInt32(item.Id);
            oldItems.TryGetValue(itemId, out var oldItem);
            var changed = oldItem == null;
            if (oldItem != null)
            {
                if (entity.GetField(nameof(StringList.Name)) != null && (item.Name ?? "") != (oldItem[nameof(StringList.Name)]?.ToString() ?? ""))
                    changed = true;
                else if (entity.GetField(nameof(StringList.EnName)) != null &&
                         !string.IsNullOrEmpty(item.EnName) &&
                         (item.EnName ?? "") != (oldItem[nameof(StringList.EnName)]?.ToString() ?? ""))
                    changed = true;
                else if (entity.GetField(nameof(StringList.Description)) != null &&
                         !string.IsNullOrEmpty(item.Description) &&
                         (item.Description ?? "") != (oldItem[nameof(StringList.Description)]?.ToString() ?? ""))
                    changed = true;
                else if (entity.GetField(nameof(StringList.EnDescription)) != null &&
                         !string.IsNullOrEmpty(item.EnDescription) &&
                         (item.EnDescription ?? "") != (oldItem[nameof(StringList.EnDescription)]?.ToString() ?? ""))
                    changed = true;
                else if (entity.IsStateBase && entity.GetField(nameof(BpmsStateBasedEntity.StateId)) != null &&
                         activeStateId != oldItem.GetLong(nameof(BpmsStateBasedEntity.StateId)))
                    changed = true;
            }

            if (!changed) continue;
            var record = new ElasticObject();
            record.SetField(nameof(StringList.Id), item.Id);
            record.SetField(nameof(StringList.Name), item.Name);
            if (!string.IsNullOrEmpty(item.EnName))
                record.SetField(nameof(StringList.EnName), item.EnName);
            if (!string.IsNullOrEmpty(item.Description))
                record.SetField(nameof(StringList.Description), item.Description);
            if (!string.IsNullOrEmpty(item.EnDescription))
                record.SetField(nameof(StringList.EnDescription), item.EnDescription);
            if (entity.IsStateBase)
                record.SetField(nameof(BpmsStateBasedEntity.StateId), activeStateId);
            if (oldItem == null)
                au.Insert(record, true);
            else
                au.Update(record);
            AddToCommandList(au.Entity, au.CommandTxt, false);
        }

        if (myEnum.DeleteExtraItems)
        {
            foreach (var oldItem in oldItems)
            {
                if (_pleaseStop)
                {
                    au.EndBatch();
                    return;
                }

                if (!myEnum.items.ContainsKey(oldItem.Key.ToString()))
                {
                    if (entity.IsStateBase)
                    {
                        oldItem.Value.SetField(nameof(BpmsStateBasedEntity.StateId), backupStateId);
                        au.Update(oldItem.Value);
                    }
                    else
                        au.Delete(oldItem.Value);
                    AddToCommandList(au.Entity, au.CommandTxt, false);
                }
            }
        }

        au.EndBatch();
    }
}
