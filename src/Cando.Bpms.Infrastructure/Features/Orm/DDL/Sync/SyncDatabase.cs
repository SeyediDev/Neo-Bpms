using Neo.Bpms.Domain.Entities.Cmmn.Data;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private Entity _firstEntityInThisProvider;
    public void SyncDatabasesWithModel(MigrationOptions options, Func<string, string, string, bool> func = null)
    {
        if (Provider.DontSync) return;
        InitSyncDatabase(options, func);
        AddMessage(null, null, $"خواندن تعاریف {Provider.Name} پایگاه داده {DatabaseName}");

        List<ModelNamespace> modelNamespaces = FetchNamespaces();

        if (_pleaseStop) return;

        ReadDatabaseDefinitions();
        if (_pleaseStop) return;

        AddMessage(null, null, $"بررسی متادیتا و پایگاه داده {DatabaseName}");
        SyncNamespaces(modelNamespaces);
        //SetAutoAudit();
        //ReportToFile();
        AddMessage(null, null,
            $"اتمام یکسان سازی {DatabaseName} با تعداد {Commands.Count} دستور و {Logs.Count} خطا در متادیتا");
    }

    private List<ModelNamespace> FetchNamespaces()
    {
        var modelNamespaces = new List<ModelNamespace>();

        if (string.IsNullOrEmpty(Options.SpecificNamespace))
            modelNamespaces = [.. (ProjectDefinition.Project.Namespaces?.Values.Where(m =>
            {
                var f = GetEntitiesOfProvider(m).FirstOrDefault();
                if (f != null && _firstEntityInThisProvider == null)
                    _firstEntityInThisProvider = f;
                return f != null;
            })
                      ?? [])];
        else
        {
            ModelNamespace m = null;
            ProjectDefinition.Project.Namespaces?.TryGetValue(Options.SpecificNamespace, out m);
            var f = GetEntitiesOfProvider(m).FirstOrDefault();
            if (f != null)
            {
                _firstEntityInThisProvider = f;
                modelNamespaces.Add(m);
            }
        }

        return modelNamespaces;
    }

    public DatabaseModel ReadDataDictionary(MigrationOptions options, Func<string, string, string, bool> func = null)
    {
        InitSyncDatabase(options, func);
        AddMessage(null, null, $"خواندن تعاریف {Provider.Name} پایگاه داده {DatabaseName}");
        ReadDatabaseDefinitions();
        AddMessage(null, null, $"اتمام خواندن تعاریف{DatabaseName}");
        return _model;
    }

    public void Stop()
    {
        _pleaseStop = true;
    }

    private void InitSyncDatabase(MigrationOptions options, Func<string, string, string, bool> func)
    {
        Options = options;
        _messageFunction = func;
        Commands = [];
        _changedEntities = [];
    }

    private void CleanDatabaseTablesAndViews()
    {
        AddMessage(null, null, "حذف کلیه جداول قبلی");
        int delCount;
        do
        {
            if (_pleaseStop) return;
            delCount = 0;
            foreach (var dbTable in _model.Tables.Enumerable())
            {
                if (_pleaseStop) return;
                var ddl = $"DROP TABLE {dbTable.FullName}";
                AddToCommandList(null, ddl, true);
                delCount += DoSqlCommand(null, ddl, "", "45") ? 1 : 0;
            }
        } while (delCount != 0);

        _model.Tables.Set([]);
        AddMessage(null, null, "حذف کلیه ویوهای قبلی");
        foreach (var dbView in _model.Views.Enumerable())
        {
            if (_pleaseStop) return;
            var ddl = "DROP VIEW " + dbView.FullName;
            AddToCommandList(null, ddl, false);
            DoSqlCommand(null, ddl, "", "46");
        }

        _model.Views.Set([]);
    }

    private void ReportEntityChanged(Entity entity)
    {
        var key = entity.model.Id + "." + entity.Id;
        if (!string.IsNullOrEmpty(key) && !_changedEntities.ContainsKey(key))
            _changedEntities.Add(key, entity);
    }

    private static EntityField GetFieldViaDbName(Entity entity, string dbFieldName)
    {
        return entity.MappedEntityFields.FirstOrDefault(i => i.DbFieldName == dbFieldName);
    }

    private Entity GetEntityWithDbName(IList<ModelNamespace> models, string tableName)
    {
        foreach (var model in models)
        {
            foreach (var entity in GetEntitiesOfProvider(model))
                if (EntityDbNameManager.GetDbTableName(entity) == tableName)
                    return entity;
        }

        foreach (var model in models)
        {
            var entity = model.GetEntity(tableName);
            if (entity != null && EntityIsInThisProvider(entity))
                return entity;
        }

        foreach (var model in models)
        {
            foreach (var entity in GetEntitiesOfProvider(model))
                if (entity.OldDbTableNameMap == tableName)
                    return entity;
        }

        return null;
    }

    private IEnumerable<Entity> GetEntitiesOfProvider(ModelNamespace model)
    {
        return model.GetEntitiesOfProvider(ProviderName);
    }

    private bool EntityIsInThisProvider(Entity entity)
    {
        return entity.Provider == ProviderName;
    }
}
