using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadForeignKeys()
    {
        AddMessage(null, null, "خواندن روابط کلید خارجی های پایگاه داده");

        foreach (var dbForeignKey in GetForeignKeys())
        {
            var fkName = dbForeignKey.FKName;
            NormalizeForeignKey(dbForeignKey);
            _model.Tables.TryGetValue(dbForeignKey.ForeignTableSchema, dbForeignKey.ForeignTable, out var fkTable);
            var pForeignKey = fkTable?.GetForeignKey(fkName);
            if (pForeignKey == null && fkTable != null)
            {
                if (_model.Tables.TryGetValue(dbForeignKey.BaseTableSchema, dbForeignKey.BaseTable, out var pkTable) &&
                    pkTable != null)
                {
                    pForeignKey = fkTable.AddForeignKey(
                        fkName, dbForeignKey.DELETE_RULE, dbForeignKey.UPDATE_RULE, pkTable);
                }
            }

            pForeignKey?.AddColumn(dbForeignKey.ForeignKeyColumn, dbForeignKey.BaseKeyColumn);
        }
    }

    protected virtual void NormalizeForeignKey(ForeignKeyItem dbForeignKey)
    {
        dbForeignKey.UPDATE_RULE = "CASCADE";
    }

    protected virtual IEnumerable<ForeignKeyItem> GetForeignKeys()
    {
        yield break;
    }

    private void SyncForeignKey(IList<ModelNamespace> models)
    {
        if (!Options.CheckForeignKey) return;
        foreach (var model in models)
        {
            if (_pleaseStop) return;
            AddMessage(model, null, "...Checking foreign key");
            foreach (var entity in model.GetEntitiesOfProvider(ProviderName))
            {
                try
                {
                    if (_pleaseStop) return;
                    if (!CheckNeededToSync(entity))
                        continue;
                    if (NeededToSyncWithDbms(entity))
                        SyncForeignKey(entity);
                    if (Options.SpecificEntity != null)
                        break;
                }
                catch (Exception e)
                {
                    AddError(model, entity, e, "10.45.5");
                }
            }
        }
    }

    private void SyncForeignKey(Entity entity)
    {
        var tableName = EntityDbNameManager.GetTableDbFullName(entity);
        if (!_model.Tables.TryGetValue(tableName, out var dbTable) || dbTable == null)
            return;
        if (!entity.DerivedEntitiesExtendedMe)
            DropMoreForeignKey(entity, dbTable);
        foreach (var field in entity.MappedEntityFields)
        {
            if (_pleaseStop) return;
            if (CanSyncForeignKey(entity, field))
                SetForeignKey(entity, dbTable, field.AssociationEntity);
        }
    }

    private bool CanSyncForeignKey(Entity entity, EntityField field)
    {
        var association = field?.AssociationEntity;
        if (association == null) return false;
        if (field.NotMapped)
            return false;
        if (association.CheckFlag(EntityFieldFlags.NotMap) ||
            (association.Entity()?.DontSync ?? false))
            return false;
        if (association.CheckFlag(EntityFieldFlags.IsBitMask))
            return false;
        if (association.Entity() == null)
        {
            AddLog("9.0", Log.Warning,
                $"موجودیت {association.DestNamespaceId}.{association.DestEntityId} تعریف نشده است. استفاده شده در موجودیت {entity.model.Id}.{entity.Id}");
            return false;
        }

        if (association.Maps == null)
        {
            //// پیش نمی آید
            AddLog("9.3", Log.Warning,
                $"رابطه تعریف نشده از {entity.model.Id}.{entity.Id} به {association.DestNamespaceId}.{association.DestEntityId}");
            return false;
        }

        if (association.Maps.Any(m => !IsMappedInThisEntity(entity.GetField(m.SourceField))))
        {
            AddLog("9.1", Log.Warning,
                $"رابطه تعریف نشده از {entity.model.Id}.{entity.Id} به {association.DestNamespaceId}.{association.DestEntityId}");
            return false;
        }

        if (association.Maps.Any(m => !IsMappedInThisEntity(association.Entity()?.GetField(m.DestField))))
        {
            AddLog("9.2", Log.Warning,
                $"رابطه تعریف نشده از {entity.model.Id}.{entity.Id} به {association.DestNamespaceId}.{association.DestEntityId}");
            return false;
        }

        return true;
    }

    private void SetForeignKey(Entity entity, DbTable dbTable, Association association)
    {
        var fkName = DbNameManager.GetDbForeignKeyName(entity, association);
        var maps = (association.Maps ?? Enumerable.Empty<EntityRelationMap>()).ToList();
        var fkFields = string.Join(",", maps.Select(map =>
            entity.GetField(map.SourceField)?.DbFieldName));
        var pkFields = string.Join(",", maps.Select(map =>
            association.Entity().GetField(map.DestField)?.DbFieldName));
        var foreignKey = dbTable.GetForeignKey(fkName);
        var ruleOnDelete = GetRuleOnDelete(association);
        var ruleOnUpdate = GetRuleOnUpdate(association);
        if (foreignKey != null && foreignKey.DeleteRule == ruleOnDelete && foreignKey.UpdateRule == ruleOnUpdate &&
            string.Join(",", foreignKey.Columns.Select(s => s.ForeignKeyColumn).ToList()) == fkFields &&
            string.Join(",", foreignKey.Columns.Select(s => s.BaseKeyColumn).ToList()) == pkFields) return;
        _model.Tables.TryGetValue(EntityDbNameManager.GetDbTableName(association.Entity()), out var pkTable);
        if (pkTable == null)
            return;
        if (foreignKey != null)
        {
            if (!DropConstraint(entity, dbTable, fkName))
                return;
        }

        var ddl = CreateForeignKeyCommand(dbTable, fkName, fkFields, pkTable, pkFields, ruleOnDelete, ruleOnUpdate);
        AddToCommandList(entity, ddl, false);
        var bAdd = DoSqlCommand(entity, ddl, "", "10.0.60");
        if (!bAdd)
        {
            AddComment(ddl,
                "رابطه تعریف نشده به " + entity.Id + "->" + association.DestNamespaceId + "::" +
                association.DestEntityId);
            if (ruleOnDelete == ForeignKeyRule.Cascade)
                bAdd = DeleteExtraDataAndTryAgain(entity, dbTable, pkTable, association, ddl);
            else
                ReportWhyCannotSetForeignKey(entity, dbTable, pkTable, association);
        }

        if (!bAdd) return;
        if (foreignKey != null)
        {
            foreignKey.DeleteRule = ruleOnDelete;
            foreignKey.UpdateRule = ruleOnUpdate;
        }
        else
            AddForeignKeyToList(entity, dbTable, fkName, pkTable, association);
    }

    protected virtual string CreateForeignKeyCommand(DbTable dbTable, string fkName, string fkFields, DbTable pkTable, string pkFields,
        ForeignKeyRule ruleOnDelete, ForeignKeyRule ruleOnUpdate)
    {
        return "ALTER TABLE " + dbTable.FullName + " ADD CONSTRAINT [" + fkName + "]" +
                  " Foreign KEY (" + fkFields + ") REFERENCES [" + pkTable.Name + "](" + pkFields + ")" +
                  " ON DELETE " + AddRuleDdl(ruleOnDelete);
    }

    private void AddForeignKeyToList(Entity entity, DbTable dbTable, string fkName, DbTable pkTable,
        Association association)
    {
        var pForeignKey = dbTable.AddForeignKey(fkName, "CASCADE", "CASCADE", pkTable);
        if (pForeignKey != null)
        {
            foreach (var item in association.Maps)
            {
                var srcField = entity.GetField(item.SourceField);
                var dstField = association.Entity().GetField(item.DestField);
                pForeignKey.AddColumn(srcField.DbFieldName, dstField.DbFieldName);
            }
        }
        else
            AddLog("10.0.63", Log.Error, fkName);
    }

    private void ReportWhyCannotSetForeignKey(Entity entity, DbTable dbTable, DbTable pkTable,
        Association association)
    {
        var keys = string.Join(",", entity.KeyFields.Select(k => k.DbFieldName));
        var sources = string.Join(",",
            association.Maps.Select(item => entity.GetField(item.SourceField)?.DbFieldName));
        var where = string.Join(",",
            association.Maps.Select(item =>
                $"({dbTable.Name}.{entity.GetField(item.SourceField)?.DbFieldName}={pkTable.Name}.{association.Entity().GetField(item.DestField)?.DbFieldName})"));
        var sql =
            $"SELECT {keys}, {sources} FROM {dbTable.Name} WHERE {sources} is not null and NOT EXISTS( SELECT 1 FROM [{pkTable.Name}] WHERE {where})";
        var dml =
            $"UPDATE {dbTable.Name} SET {sources}=null WHERE {sources} is not null and NOT EXISTS( SELECT 1 FROM [{pkTable.Name}] WHERE {where})";
        AddLog("10.0.60.1", Log.Info, "این پرس و جو نشان می دهد که چرا محدودیت کلید خارجی با موفقیت انجام نشد");
        AddLog("", Log.Info, sql);
        AddLog("10.0.60.2", Log.Info, "دستور اصلاح زیر به شما کمک می کند تا اطلاعات را اصلاح کنید");
        AddLog("", Log.Info, dml);
    }

    private bool DeleteExtraDataAndTryAgain(Entity entity, DbTable dbTable,
        DbTable pkTable, Association association, string ddl)
    {
        var sources = string.Join(",",
            association.Maps.Select(item => entity.GetField(item.SourceField)?.DbFieldName));
        var where = string.Join(",",
            association.Maps.Select(item =>
                $"({dbTable.Name}.{entity.GetField(item.SourceField)?.DbFieldName}={pkTable.Name}.{association.Entity().GetField(item.DestField)?.DbFieldName})"));
        var sql =
            $"DELETE FROM {dbTable.Name} WHERE {sources} is not null and NOT EXISTS( SELECT 1 FROM [{pkTable.Name}] WHERE {where})";
        AddLog("10.0.201", Log.Info, "حذف رکورد های نامعتبر : رکورد هایی که فیلد ارجاعی حذف آبشاری نامعتبر دارد");
        AddLog("", Log.Info, sql);
        AddToCommandList(entity, sql, false);
        if (DoSqlCommand(entity, sql, "", "10.0.61"))
        {
            AddToCommandList(entity, ddl, false);
            return DoSqlCommand(entity, ddl, "", "10.0.62");
        }

        return false;
    }

    protected static string AddRuleDdl(ForeignKeyRule ruleOnDelete)
    {
        switch (ruleOnDelete)
        {
            case ForeignKeyRule.Cascade:
                return " CASCADE";
            case ForeignKeyRule.SetNull:
                return " SET NULL";
            case ForeignKeyRule.SetDefault:
                return " SET DEFAULT";
            case ForeignKeyRule.NoAction:
                return " NO ACTION";
            default:
                return " NO ACTION";
        }
    }

    private static ForeignKeyRule GetRuleOnUpdate(Association association)
    {
        ForeignKeyRule ruleOnUpdate;
        switch (association.OnUpdateBehaviour)
        {
            case RelationDeleteUpdateBehavior.Cascade:
                ruleOnUpdate = ForeignKeyRule.Cascade;
                break;
            case RelationDeleteUpdateBehavior.WarningCascade:
                ruleOnUpdate = ForeignKeyRule.Cascade;
                break;
            case RelationDeleteUpdateBehavior.WarningSetNull:
                ruleOnUpdate = ForeignKeyRule.SetNull;
                break;
            case RelationDeleteUpdateBehavior.Error:
                ruleOnUpdate = ForeignKeyRule.NoAction;
                break;
            default:
                ruleOnUpdate = ForeignKeyRule.NoAction;
                break;
        }

        return ruleOnUpdate;
    }

    private static ForeignKeyRule GetRuleOnDelete(Association association)
    {
        ForeignKeyRule ruleOnDelete;
        switch (association.OnDeleteBehaviour)
        {
            case RelationDeleteUpdateBehavior.Cascade:
                ruleOnDelete = ForeignKeyRule.Cascade;
                break;
            case RelationDeleteUpdateBehavior.WarningCascade:
                ruleOnDelete = ForeignKeyRule.Cascade;
                break;
            case RelationDeleteUpdateBehavior.WarningSetNull:
                ruleOnDelete = ForeignKeyRule.SetNull;
                break;
            case RelationDeleteUpdateBehavior.Error:
                ruleOnDelete = ForeignKeyRule.NoAction;
                break;
            default:
                ruleOnDelete = ForeignKeyRule.NoAction;
                break;
        }

        return ruleOnDelete;
    }

    private void DropMoreForeignKey(Entity entity, DbTable dbTable)
    {
        if (dbTable.ForeignKeys == null) return;
        foreach (var fk in dbTable.ForeignKeys.Values)
        {
            if (_pleaseStop) return;
            var drop = false;
            var relatedEntity =
                entity.Associations?.FirstOrDefault(i => DbNameManager.GetDbForeignKeyName(entity, i) == fk.Name);
            if (relatedEntity == null || relatedEntity.CheckFlag(EntityFieldFlags.NotMap))
                drop = true;
            else if (fk.ForeignTable.Name != dbTable.Name)
                drop = true;
            else
            {
                if (fk.Columns.Any(col =>
                    CheckColumnDeleted(col.ForeignKeyColumn) ||
                    CheckColumnDeleted(col.BaseKeyColumn)))
                {
                    drop = true;
                }
            }

            if (drop)
                DropConstraint(entity, dbTable, fk.Name);
        }
    }
}
