using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void SyncNamespaces(IList<ModelNamespace> models)
    {
        RenameOrDropExtraTables(models);
        if (_pleaseStop)
        {
            return;
        }

        RenameOrDropExtraViews(models);
        if (_pleaseStop)
        {
            return;
        }

        List<ModelNamespace> syncModels = [.. models.Where(model => !model.DontSync)];

        HashSet<string> uniqueSchemas = [];
        uniqueSchemas.UnionWith(syncModels.Select(m => m.Schema));
        uniqueSchemas.UnionWith(syncModels.SelectMany(m => m.GetEntities().Values).Select(e => e.Schema));
        foreach (var schema in uniqueSchemas)
        {
            SyncSchema(schema);
        }

        SyncFileGroup(syncModels);

        foreach (ModelNamespace model in syncModels)
        {
            SyncNamespace(model);
        }

        if (_pleaseStop)
        {
            return;
        }

        SyncIndexes(syncModels);
        if (_pleaseStop)
        {
            return;
        }

        SyncForeignKey(syncModels);
        if (_pleaseStop)
        {
            return;
        }

        SyncEnumerationItems(syncModels);
    }

    private void SyncSchema(string schema)
    {
        if (_pleaseStop)
        {
            return;
        }

        if (schema == "dbo")
        {
            return;
        }

        AddMessage(null, null, $"...Checking Schema {schema}");
        if (!_model.Schemas.TryGetValue(schema, out DbSchema schemaObject) || schemaObject == null)
        {
            string ddl = $"CREATE SCHEMA [{schema}]";
            AddToCommandList(null, ddl, false);
            DoSqlCommand(null, ddl, "", "440.1");
        }
    }

    private void SyncNamespace(ModelNamespace model)
    {
        if (_pleaseStop)
        {
            return;
        }

        if (model.Id == "ProcessEntities")
        {
            return;
        }

        AddMessage(model, null, "...Checking Tables And Views");

        if (Options.SyncFileGroups)
        {
            SyncPartitionFunctions(model);
            if (_pleaseStop)
            {
                return;
            }
            SyncPartitionSchemes(model);
        }
        foreach (Entity entity in model.GetEntitiesOfProvider(ProviderName))
        {
            try
            {
                if (_pleaseStop)
                {
                    return;
                }

                if (!CheckNeededToSync(entity))
                {
                    continue;
                }

                if (entity.IsView)
                {
                    SyncView(entity);
                }
                else
                {
                    SyncTable(entity);
                }

                if (Options.SpecificEntity != null)
                {
                    break;
                }
            }
            catch (Exception e)
            {
                AddError(model, entity, e, "10.45.4");
            }
        }
    }
}
