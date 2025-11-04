using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;
public abstract partial class DDLManager
{
    private void SyncTablePkConstraint(Entity entity, DbTable dbTable)
    {
        if (dbTable == null)
        {
            return;
        }

        foreach (EntityField entityKeyField in entity.KeyFields)
        {
            if (entityKeyField.FieldType == TVariableTypes.String && entityKeyField.MaxLen == 0)
            {
                AddLog("10.1.1.21", Log.Error,
                    $"لازم است حداکثر طول برای فیلد کلید متنی مشخص شود. {entityKeyField.Name} {entity.Name}");
            }
        }

        var pkIndex = dbTable.PkIndex;
        string keys = FetchKeys(entity);
        if (pkIndex != null)
        {
            bool dropPk = string.IsNullOrEmpty(keys);
            if (!dropPk && string.Join(",", pkIndex.Fields.Select(f => $"[{f.Name}]")) != keys)
            {
                dropPk = true;
            }

            bool cluster = CheckPkIndexCanCluster(entity);
            if (pkIndex.Clustered != cluster)
            {
                dropPk = true;
            }

            if (!CheckDbIndexDataSpace(entity, pkIndex))
            {
                dropPk = true;
            }

            if (dropPk)
            {
                RemoveAllTableDependency(dbTable, entity);
                DeleteDbIndex(entity, dbTable, pkIndex);
                pkIndex = null;
            }
        }

        if (pkIndex == null)
        {
            CreatePkIndex(entity, dbTable, keys);
        }
    }

    private void SyncTableIdentity(Entity entity, DbTable dbTable)
    {
        if (!IsProviderSupportIdentity())
        {
            return;
        }

        AutoCalc field = entity.AutoCalcs?.Calculations.FirstOrDefault(auto =>
            !auto.RecalcOnAnyChange && auto.GenerationType == AutoCalc.eGenerationType.DBInsert);
        bool reCreate = false;
        if (field == null)
        {
            var identityDbField = dbTable.Fields.Values.FirstOrDefault(f => f.IsIdentity);
            if (identityDbField != null)
            {
                reCreate = true;
            }
        }
        else
        {
            EntityField entityField = entity.GetField(field.FieldId);
            DbField dbField = dbTable.GetField(entityField?.DbFieldName);
            if (dbField == null)
            {
                return;
            }

            if (dbField.IsIdentity)
            {
                return;
            }

            reCreate = true;
        }

        if (!reCreate)
        {
            return;
        }

        RemoveAllTableDependency(dbTable, entity);
        string tempName = $"_I_{dbTable.Name}";
        RenameDbTable(entity, dbTable, $"[{dbTable.Schema}].[{tempName}]", tempName);
        string dbTableFullName = EntityDbNameManager.GetTableDbFullName(entity);
        _ = CreateDbTable(entity);
        List<EntityField> fields = entity.MappedEntityFields.ToList();
        string newFields = string.Join(",", fields.Select(f => f.DbFieldName));
        string oldFields = string.Join(",", fields.Select(f => dbTable.Fields.ContainsKey(f.DbFieldName)
            ? f.DbFieldName : dbTable.Fields.ContainsKey(f?.OldDbFieldName ?? "") ? f.OldDbFieldName : null)
            .Select(fieldName => string.IsNullOrEmpty(fieldName) ? "null" : $"[{fieldName}]"));
        string ddl = "";
        if (field != null)
        {
            ddl += $@"SET IDENTITY_INSERT {dbTableFullName} ON;";
        }

        ddl += $@"INSERT INTO {dbTableFullName} ({newFields}) SELECT {oldFields} FROM {tempName};";
        if (field != null)
        {
            ddl += $@"SET IDENTITY_INSERT {dbTableFullName} OFF;";
        }

        if (!DoSqlCommandAndAddToList(entity, ddl, "10.0.186.2"))
        {
            DropDbTable(entity, dbTableFullName);
            RenameDbTable(entity, dbTable, dbTableFullName, EntityDbNameManager.GetDbTableName(entity));
        }
        else
        {
            DropDbTable(entity, tempName);
        }
    }

    protected abstract bool IsProviderSupportIdentity();
}
