using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Engine.Data.ADODotNet;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadTables()
    {
        string sql = DDLGenerator.GetTables(Options.SpecificEntity);
        IEnumerable<ElasticObject> dt = Select(sql, "10.1.1.8");
        _model.Tables.Set(dt?.Select(item => new DbTable(item.GetString("SchemaName"), item.GetString("Name")))
            .ToDictionary(d => d.FullName) ?? []);
        if (_model.Tables.Count == 0)
        {
            AddLog("10.1.1.9", Log.Error, sql);
        }
    }

    private void SyncTable(Entity entity)
    {
        (bool ret, bool _) = FetchOrCreateDbTable(entity, out DbTable dbTable);
        if (!ret)
        {
            return;
        }

        if (_pleaseStop)
        {
            return;
        }

        if (entity.Partitioned && Options.CheckIndexes)
        {
            SyncTablePartition(entity, dbTable);
        }
        else
        {
            SyncTableFileGroup(entity, dbTable);
        }

        if (_pleaseStop)
        {
            return;
        }

        SyncTable(entity, dbTable);
    }

    private (bool ret, bool created) FetchOrCreateDbTable(Entity entity, out DbTable dbTable)
    {
        string tableFullName = EntityDbNameManager.GetTableDbFullName(entity);
        bool created = false;
        if (!_model.Tables.TryGetValue(tableFullName, out dbTable) || dbTable == null)
        {
            if (!_model.Tables.TryGetValue(EntityDbNameManager.GetTableDbDeletedFullName(entity), out dbTable) || dbTable == null)
            {
                if (!string.IsNullOrEmpty(entity.OldDbTableNameMap))
                {
                    _model.Tables.TryGetValue(EntityDbNameManager.GetTableOldDbFullName(entity), out dbTable);
                }
            }

            dbTable ??= _model.Tables.FetchByLowerCase(tableFullName.ToLower());
            if (dbTable != null)
            {
                RenameDbTable(entity, dbTable, tableFullName, EntityDbNameManager.GetDbTableName(entity));
            }
            else
            {
                dbTable = CreateDbTable(entity);
                if (dbTable is not null)
                {
                    created = true;
                }
            }
        }

        if (dbTable == null)
        {
            AddLog("10.12.0", Log.Error, $"Unable to create db-table {tableFullName} for entity {entity.Id}.");
            return (false, false);
        }

        return (true, created);
    }

    private void SyncTable(Entity entity, DbTable dbTable)
    {
        if (string.IsNullOrEmpty(entity.DbTableNameMap))
        {

        }
        SyncTableSchema(entity, dbTable);
        SyncTablePkConstraint(entity, dbTable);
        if (_pleaseStop)
        {
            return;
        }

        SyncTableIdentity(entity, dbTable);
        if (_pleaseStop)
        {
            return;
        }

        SyncTriggers(entity, dbTable);
        if (_pleaseStop)
        {
            return;
        }

        SyncTableFields(entity, dbTable);
    }

    private void SyncTableSchema(Entity entity, DbTable dbTable)
    {
        if (entity.Schema != dbTable.Schema)
        {
            string ddl = $"ALTER SCHEMA [{entity.Schema}] TRANSFER {dbTable.Name}";
            AddToCommandList(null, ddl, false);
            DoSqlCommand(null, ddl, "", "441.1");
        }
    }

    private DbTable CreateDbTable(Entity entity)
    {
        if (!CreateDbTableCommand(entity, out bool pkIsDefined, out DbTable dbTable))
        {
            return null;
        }

        if (pkIsDefined)
        {
            var pk = dbTable.AddIndex(GetPkConstraintName(entity, dbTable), true,
                DbIndex.IndexType.Clustered, null, true);
            if (entity.Partitioned)
            {
                dbTable.PartitionScheme = entity.PartitionScheme;
            }
            else if (!entity.Partitioned)
            {
                dbTable.FileGroup = entity.FileGroup;
            }
            foreach (var pkField in entity.KeyFields)
            {
                pk.AddField(pkField.Id, false, true);
            }
        }

        _model.Tables.Add(dbTable.FullName, dbTable);
        return dbTable;
    }

    private bool CreateDbTableCommand(Entity entity,
        out bool pkIsDefined, out DbTable dbTable)
    {
        string collation = DbNameManager.GetDbTableCollation(entity);
        bool bFirst = true;
        pkIsDefined = false;
        string schema = EntityDbNameManager.GetSchema(entity);
        string tableName = EntityDbNameManager.GetDbTableName(entity);
        string ddl = $"CREATE TABLE [{schema}].[{tableName}] (";
        Dictionary<string, string> fields = [];
        EntityField partitionField = entity.GetField(entity.PartitionField);
        dbTable = new DbTable(schema, tableName)
        {
            FileGroup = entity.FileGroup,
            PartitionScheme = entity.PartitionScheme
        };
        foreach (EntityField field in entity.MappedEntityFields)
        {
            if (!fields.ContainsKey(field.DbFieldName))
            {
                fields.Add(field.DbFieldName, field.DbFieldName);
            }
            else
            {
                continue;
            }

            DbField dbField = new()
            {
                Name = field.DbFieldName,
                Type = GetSqlFullFieldType(field),
                Len = field.MaxLen,
                NotNull = field.Required
            };
            dbTable.AddField(dbField);
            var defaultValue = AdoDotNetDatabaseDataSource.GetSqlValue(field.DefaultValue, field, field.CSharpType, false);
            string fieldTypeStr = dbField.Type;
            if (field.IsTextParam())
            {
                fieldTypeStr += " COLLATE " + collation;
            }

            ddl += (bFirst ? "[" : ",[") + field.DbFieldName + "] " + fieldTypeStr;
            dbField.IsIdentity = IsIdentity(entity, field);
            if (!dbField.IsIdentity && !string.IsNullOrEmpty(defaultValue) && defaultValue!= "null")
            {
                ddl += field.FieldType == TVariableTypes.String
                    ? " DEFAULT '" + defaultValue + "'"
                    : " DEFAULT " + defaultValue;
            }

            if (field.IncludeInPkv && (!entity.Partitioned || partitionField?.Id == field.Id))
            {
                int pkFieldCount = entity.MappedEntityFields.Sum(f => f.IncludeInPkv ? 1 : 0);
                if (pkFieldCount == 1)
                {
                    ddl += " PRIMARY KEY";
                    pkIsDefined = true;
                }
            }
            else
            {
                ddl += field.Required ? " NOT NULL" : " NULL";
            }

            if (dbField.IsIdentity)
            {
                ddl += " IDENTITY(1, 1)"; // field.field.identity.seed +"," + field.field.identity.increment + ")";
            }

            bFirst = false;
        }

        ddl += ")";
        ddl += GetFileGroupOrPartition(entity, pkIsDefined);
        AddToCommandList(entity, ddl, true);
        return DoSqlCommand(entity, ddl, "", "10.0.8");
    }

    private static bool IsMappedInThisEntity(EntityField field)
    {
        return field != null && field.MappedToDataInThisEntity;
    }

    private void RenameOrDropExtraTables(IList<ModelNamespace> models)
    {
        if (!Options.DoDropUndefinedTables)
        {
            return;
        }

        AddMessage(null, null, "حذف و یا تغییر نام جداول اضافی پایگاه داده");
        List<DbTable> tables = [.. _model.Tables.Enumerable()];
        List<ModelNamespace> allModels = ProjectDefinition.Project.Namespaces?.Values.ToList();
        foreach (DbTable dbTable in tables)
        {
            if (_pleaseStop)
            {
                return;
            }

            if (!CheckNeededToSync(dbTable.Schema, dbTable.Name))
            {
                continue;
            }

            Entity entity = GetEntityWithDbName(models, dbTable.Name);
            if (entity == null)
            {
                entity = GetEntityWithDbName(allModels, dbTable.Name);
                if (entity == null)
                {
                    if (Options.DoRenameUndefinedTables || Options.DoDropUndefinedTables)
                    {
                        DeleteDbTable(null, dbTable);
                    }
                }
            }
            else if (!entity.DontSync)
            {
                if (entity.Schema != dbTable.Schema )
                {
                    string entityFullName = EntityDbNameManager.GetTableDbFullName(entity);
                    if (_model.Tables.ContainsKey(entityFullName))
                    {
                        if (Options.DoRenameUndefinedTables || Options.DoDropUndefinedTables)
                        {
                            DeleteDbTable(entity, dbTable);
                        }
                    }
                    continue;
                }
                if (dbTable.Name == entity.OldDbTableNameMap)
                {
                    RenameDbTable(entity, dbTable, EntityDbNameManager.GetTableDbFullName(entity), EntityDbNameManager.GetDbTableName(entity));
                }

                if ((Options.DropExtraIndex || Options.DropForeignKeyIndex) && !entity.DerivedEntitiesExtendedMe)
                {
                    DropExtraIndex(dbTable, entity);
                }

                foreach (DbField dbField in dbTable.Fields.Values)
                {
                    if (_pleaseStop)
                    {
                        return;
                    }

                    if (dbField.Deleted)
                    {
                        RemoveAllFieldDependency(dbTable, entity, dbField);
                    }
                }
            }

            if (_pleaseStop)
            {
                return;
            }

            if (Options.SpecificEntity != null)
            {
                break;
            }
        }
    }

    private void DeleteDbTable(Entity entity, DbTable dbTable)
    {
        string oldTableFullName = dbTable.FullName;
        RemoveAllTableDependency(dbTable, entity);
        if (Options.DoDropUndefinedTables)
        {
            DropDbTable(entity, dbTable.FullName);
        }
        else if (!CheckColumnDeleted(dbTable.Name))
        {
            string newTableName = EntityDbNameManager.GetTableDbDeletedFullName(entity);
            bool b = RenameDbTable(entity, dbTable, newTableName, EntityDbNameManager.GetTableDbDeletedName(entity));
            if (!b)
            {
                DropDbTable(entity, newTableName);
            }

            if (b)
            {
                AddLog("10.0.132", Log.Warning, "");
                dbTable.Name = EntityDbNameManager.GetTableDbDeletedName(entity);
                if (dbTable.Indexes != null)
                {
                    Dictionary<string, DbIndex> indexes = dbTable.Indexes;
                    dbTable.Indexes = null;
                    foreach (DbIndex dbIndex in indexes.Values)
                    {
                        if (_pleaseStop)
                        {
                            return;
                        }

                        DeleteDbIndex(entity, dbTable, dbIndex);
                    }
                }

                if (dbTable.InForeignKeys != null)
                {
                    Dictionary<string, DbForeignKey> inForeignKeys = dbTable.InForeignKeys;
                    dbTable.InForeignKeys = null;
                    foreach (DbForeignKey dbForeignKey in inForeignKeys.Values)
                    {
                        if (_pleaseStop)
                        {
                            return;
                        }

                        DropConstraint(entity, dbForeignKey.ForeignTable, dbForeignKey.Name);
                        dbForeignKey.ForeignTable.ForeignKeys?.Remove(dbForeignKey.Name);
                    }
                }
            }
        }

        _model.Tables.Remove(oldTableFullName);
    }

    private bool RenameDbTable(Entity entity, DbTable dbTable,
        string newTableFullName, string newTableName)
    {
        DropTriggers(entity, dbTable);
        string ddl = DDLGenerator.ModifyTable(dbTable.FullName, newTableName);
        AddToCommandList(entity, ddl, true);
        if (!DoSqlCommand(entity, ddl, "", "10.0.5"))
        {
            return false;
        }

        _model.Tables.Remove(dbTable.FullName);
        dbTable.Name = newTableName;
        if (_model.Tables.ContainsKey(newTableFullName))
        {
            _model.Tables.Set(newTableFullName, dbTable);
        }
        else
        {
            _model.Tables.Add(newTableFullName, dbTable);
        }

        return true;
    }

    private void DropDbTable(Entity entity, string tableName)
    {
        string ddl = $"DROP TABLE {tableName}";
        AddToCommandList(entity, ddl, true);
        DoSqlCommand(entity, ddl, "", "10.0.82");
    }

    private bool CheckNeededToSync(Entity entity)
    {
        return CheckNeededToSync(entity.Schema, entity.Id) && NeededToSyncWithDbms(entity);
    }

    private bool CheckNeededToSync(string schemaName, string tableName)
    {
        if (schemaName == "HangFire")
        {
            return false;
        }
            
        if (Options.SpecificEntity == null)
        {
            return true;
        }

        return tableName == Options.SpecificEntity; // todo DbTableName;
    }

    private static bool NeededToSyncWithDbms(Entity entity)
    {
        return !(entity.EntityType.IsAbstract || 
            entity == null ||
            entity.DontSync ||
            entity.NotMapped ||
            entity.BaseExtension != null ||
            entity?.EntityType?.IsAbstract is true || 
            entity.entityFields == null || 
            !entity.MappedEntityFields.Any());
    }
}
