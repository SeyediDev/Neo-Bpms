using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadIndexes()
    {
        AddMessage(null, null, "خواندن ایندکس های پایگاه داده");
        string sql = DDLGenerator.ReadIndexes(Options.SpecificEntity);
        IEnumerable<ElasticObject> items = Select(sql, "10.1.1.18");
        if (items == null)
        {
            return;
        }
        //WriteRegInfo(hKey,_T("StartState_Count"), (DWORD)(pRs.RecordCount) );
        foreach (ElasticObject item in items) //int i = 0; i < pRs.Rows.Count; i++
        {
            _ = item.GetField("TableName", out object cv);
            string tableName = cv.ToString();
            _ = item.GetField("SchemaName", out cv);
            string schemaName = cv.ToString();
            _ = _model.Tables.TryGetValue($"[{schemaName}].[{tableName}]", out DbTable dbTable);
            if (dbTable == null)
            {
                AddLog("10.4.1.0", Log.Error, "خطا در خواندن اطلاعات ایندکس ها");
                continue;
            }

            DbIndex.IndexType indexType = (DbIndex.IndexType)item.GetLong("IndexType");
            if (indexType == DbIndex.IndexType.Heap)
            {
                continue;
            }

            string indexName = item.GetString("IndexName") ?? "";
            bool isPk = item.GetBool("IsPK");
            DbIndex dbIndex = dbTable.GetIndex(indexName);
            if (dbIndex == null)
            {
                _ = item.GetField("IsUnique", out cv);
                string sIsUnique = cv.ToString();
                bool isUnique = DDLGenerator.IndexIsUnique(sIsUnique);
                long indexSpaceId = item.GetLong("IndexSpaceId");
                DbFileGroup dataSpace = _model.FileGroups.Values.FirstOrDefault(ds => ds.Id == indexSpaceId);
                dbIndex = dbTable.AddIndex(indexName, isUnique, indexType, dataSpace, isPk);
            }

            if (dbIndex == null)
            {
                continue;
            }

            string fieldName = item.GetString("FieldName");
            string strIsDescending = item.GetString("IsDescending");
            bool isDescending = DDLGenerator.IndexIsDescending(strIsDescending);
            string strIsIncluded = item.GetString("IsIncluded");
            bool isIncluded = DDLGenerator.IndexIsIncluded(strIsIncluded);
            //var partitionOrdinal = item.GetInt("partition_ordinal");
            long keyOrdinal = item.GetLong("key_ordinal");
            if (keyOrdinal != 0 || isIncluded)
            {
                _ = dbIndex.AddField(fieldName, isDescending, isIncluded);
            }
        }
    }

    private void SyncIndexes(IEnumerable<ModelNamespace> models)
    {
        if (!Options.CheckIndexes)
        {
            return;
        }

        foreach (ModelNamespace model in models)
        {
            if (_pleaseStop)
            {
                return;
            }

            AddMessage(model, null, "...Index Checking");
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

                    SyncEntityIndexes(entity);
                    if (!string.IsNullOrEmpty(Options.SpecificEntity))
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    AddError(model, entity, e, "10.45.2");
                }
            }
        }
    }

    private void SyncEntityIndexes(Entity entity)
    {
        if (entity.indexes == null)
        {
            return;
        }

        string tableFullName = EntityDbNameManager.GetTableDbFullName(entity);
        if(_model==null || _model.Tables==null || tableFullName==null)
        {
            return;
        }
        if (!_model.Tables.TryGetValue(tableFullName, out DbTable dbTable))
        {
            return;
        }

        bool stateIdIsInIndexes = false;
        foreach (EntityIndex entityIndex in entity.indexes)
        {
            if (_pleaseStop)
            {
                return;
            }

            if (entityIndex.Id == "StateId" || entityIndex.Fields.Any(f => f.FieldName == "StateId"))
            {
                stateIdIsInIndexes = true;
            }

            CheckDbIndex(entity, dbTable, entityIndex, DbNameManager.GetDbIndexName(entityIndex));
        }

        if (Options.CreateForeignKeyIndex)
        {
            CreateForeignKeyIndex(entity, dbTable, ref stateIdIsInIndexes);
        }

        if (entity.IsStateBase && !stateIdIsInIndexes && entity.GetField("StateId").MappedToDataInThisEntity)
        {
            CheckStateIdDbIndex(entity, dbTable);
        }
    }

    private static string FetchKeys(Entity entity)
    {
        string keys = string.Join(",", entity.KeyFields.Select(i => $"[{i.DbFieldName}]"));
        return keys;
    }

    private void CreateForeignKeyIndex(Entity entity, DbTable dbTable, ref bool stateIdIsInIndexes)
    {
        foreach (EntityField field in entity.MappedEntityFields)
        {
            if (_pleaseStop)
            {
                return;
            }

            if (!CanSyncForeignKey(entity, field))
            {
                continue;
            }

            if (field.AssociationEntity.Maps.Any(f => f.SourceField == "StateId"))
            {
                stateIdIsInIndexes = true;
            }

            CheckForeignKeyDbIndex(entity, dbTable, field.AssociationEntity);
        }
    }

    private void CheckStateIdDbIndex(Entity entity, DbTable dbTable)
    {
        EntityIndex entityIndex = new()
        {
            Id = "StateId",
            Name = "وضعیت",
            EnName = "StateId",
            Clustered = false,
            IsUnique = false,
            Fields =
                [
                    new IndexField
                    {
                        FieldName = "StateId"
                    }
                ]
        };
        CheckDbIndex(entity, dbTable, entityIndex, DbNameManager.GetDbIndexName(entityIndex));
    }

    private void CheckForeignKeyDbIndex(Entity entity, DbTable dbTable, Association association)
    {
        EntityIndex entityIndex = new()
        {
            Id = association.Id,
            Name = association.Name,
            EnName = association.EnName,
            Clustered = false,
            IsUnique = false,
            Fields = []
        };
        foreach (var map in association.Maps ?? Enumerable.Empty<EntityRelationMap>())
        {
            EntityField src = entity.GetField(map.SourceField);
            if (src == null || src.DonSync)
            {
                continue;
            }

            entityIndex.Fields.Add(new IndexField
            {
                FieldName = map.SourceField
            });
        }

        if (entityIndex.Fields.Count == 0)
        {
            return;
        }

        CheckDbIndex(entity, dbTable, entityIndex,
            DbNameManager.GetDbForeignKeyIndexName(association));
    }

    private void CheckDbIndex(Entity entity, DbTable dbTable,
        EntityIndex entityIndex, string dbIndexName)
    {
        DbIndex dbIndex = FetchDbIndex(entity, dbTable, entityIndex, dbIndexName);
        if (dbIndex != null)
        {
            bool changed = CheckDbIndexChanged(entity, entityIndex, dbIndex);
            if (changed && !entity.DerivedEntitiesExtendedMe)
            {
                DeleteDbIndex(entity, dbTable, dbIndex);
                dbIndex = null;
            }
        }

        if (dbIndex == null)
        {
            CreateIndex(entity, entityIndex, dbTable, ref dbIndex, dbIndexName);
        }
        else if (Options.RebuildIndex)
        {
            string ddl = DDLGenerator.GetRebuildIndexCommand(dbIndexName, dbTable.FullName);
            AddToCommandList(entity, ddl, false);
            _ = DoSqlCommand(entity, ddl, "", "10.4.1.1");
        }
    }

    private bool CheckDbIndexChanged(Entity entity, EntityIndex entityIndex, DbIndex dbIndex)
    {
        DbIndex.IndexType indexType = entityIndex.Clustered ? DbIndex.IndexType.Clustered : DbIndex.IndexType.NonClustered;
        if (entityIndex.IsUnique && !dbIndex.IsUnique)
        {
            return true;
        }

        if (!entityIndex.IsUnique && dbIndex.IsUnique)
        {
            return true;
        }

        if (indexType != dbIndex.Type)
        {
            return true;
        }

        if (!CheckDbIndexDataSpace(entity, dbIndex))
        {
            return true;
        }

        int iDbIndexField = 0;
        foreach (var entityIndexField in entityIndex.Fields)
        {
            if (CheckDbIndexFieldChanged(entity, dbIndex, entityIndexField, ref iDbIndexField))
            {
                return true;
            }
        }

        return iDbIndexField != dbIndex.Fields.Count ||
                 dbIndex.Fields.Any(col => CheckColumnDeleted(col.Name));
    }

    private bool CheckDbIndexFieldChanged(Entity entity, DbIndex dbIndex,
        IndexField entityIndexField, ref int iDbIndexField)
    {
        if (!FetchNextDbIndexField(dbIndex, ref iDbIndexField, out DbIndex.DbIndexField dbIndexField))
        {
            return true;
        }
        if (entity.Partitioned && iDbIndexField == 0 && entityIndexField.FieldName != entity.PartitionField)
        {
            AddLog("10.4.1.2", Log.Warning,
                $"مناسب است که در کل ایندکس های جداول پارتیشن بندی شده فیلد پارتیشن نیز در ایندکس بیاید . {entity.Name} {dbIndex.Name}");
        }

        EntityField eField = entity.GetField(entityIndexField.FieldName);
        if (eField == null)
        {
            return false;
        }

        if (eField.AssociationEntity == null)
        {
            if (CompareDbIndexField(entityIndexField, eField, dbIndexField))
            {
                return true;
            }
        }
        else
        {
            foreach (EntityRelationMap map in eField.AssociationEntity.Maps)
            {
                EntityField ef = entity.GetField(map.SourceField);
                if (ef == null)
                {
                    continue; //error in definition
                }

                if (CompareDbIndexField(entityIndexField, ef, dbIndexField))
                {
                    return true;
                }

                if (!FetchNextDbIndexField(dbIndex, ref iDbIndexField, out dbIndexField))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool CompareDbIndexField(IndexField entityIndexField, EntityField eField,
        DbIndex.DbIndexField dbIndexField)
    {
        return eField.DbFieldName != dbIndexField.Name ||
                 (entityIndexField.IsIncluded && !dbIndexField.IsIncluded) ||
                 (entityIndexField.IsDescending && !dbIndexField.IsDescending);
    }

    private static bool FetchNextDbIndexField(DbIndex dbIndex, ref int iDbIndexField,
        out DbIndex.DbIndexField dbIndexField)
    {
        dbIndexField = null;
        if (iDbIndexField >= dbIndex.Fields.Count)
        {
            return false;
        }

        dbIndexField = dbIndex.Fields[iDbIndexField++];
        return dbIndexField != null;
    }

    private static bool CheckDbIndexDataSpace(Entity entity, DbIndex dbIndex)
    {
        string dataSpaceName = /*!string.IsNullOrEmpty(entity.PartitionScheme)
            ? entity.PartitionScheme
            : */entity.FileGroup;
        return dataSpaceName == dbIndex.DataSpace?.Name;
    }

    private DbIndex FetchDbIndex(Entity entity, DbTable dbTable, EntityIndex entityIndex, string dbIndexName)
    {
        DbIndex dbIndex = null;
        if (dbTable.Indexes != null)
        {
            _ = dbTable.Indexes.TryGetValue(dbIndexName, out dbIndex);
            if (dbIndex == null)
            {
                _ = dbTable.Indexes.TryGetValue(entityIndex.Id, out dbIndex);
                if (dbIndex != null)
                {
                    RenameDbIndex(entity, dbTable, dbIndex, dbIndexName);
                }
            }
        }

        return dbIndex;
    }

    private void CreateIndex(Entity entity, EntityIndex entityIndex, DbTable dbTable,
        ref DbIndex dbIndex, string dbIndexName)
    {
        DbIndex.IndexType indexType = entityIndex.Clustered ? DbIndex.IndexType.Clustered : DbIndex.IndexType.NonClustered;
        string ddl = DDLGenerator.CreateIndex(entityIndex.Clustered, entityIndex.IsUnique, dbIndexName, dbTable.FullName);
        string fields = "";
        if (CreateIndexCommand(entity, entityIndex, ref ddl, ref fields))
        {
            return;
        }

        ddl += ") " + GetFileGroupOrPartition(entity, true);
        AddToCommandList(entity, ddl, false);
        if (!DoSqlCommand(entity, ddl, "", "10.4.1.4"))
        {
            if (entityIndex.IsUnique)
            {
                string sql = $"SELECT {fields}, count(*) FROM {dbTable.FullName} group by {fields} having count(*)>1";
                AddLog("10.4.1.5", Log.Info, "این پرس و جو نشان میدهد که چرا کلید یکتا با موفقیت انجام نشد");
                AddLog("", Log.Info, sql);
            }

            return;
        }

        dbIndex = dbTable.AddIndex(dbIndexName, entityIndex.IsUnique, indexType, null, false);
        if (dbIndex == null)
        {
            return;
        }

        foreach (var entityIndexField in entityIndex.Fields)
        {
            EntityField eField = entity.GetField(entityIndexField.FieldName);
            if (eField == null)
            {
                continue; //error in definition
            }

            if (eField.AssociationEntity == null)
            {
                dbIndex.AddField(eField.DbFieldName, entityIndexField.IsDescending, entityIndexField.IsIncluded);
            }
            else
            {
                foreach (EntityRelationMap map in eField.AssociationEntity.Maps)
                {
                    EntityField ef = entity.GetField(map.SourceField);
                    if (ef == null)
                    {
                        continue; //error in definition
                    }

                    dbIndex.AddField(eField.DbFieldName, entityIndexField.IsDescending, entityIndexField.IsIncluded);
                }
            }
        }
    }

    protected virtual bool CreateIndexCommand(Entity entity, EntityIndex entityIndex, ref string ddl, ref string fields)
    {
        bool bFirst = true;
        foreach (var entityIndexField in entityIndex.Fields)
        {
            EntityField eField = entity.GetField(entityIndexField.FieldName);
            if (eField == null)
            {
                AddLog("10.4.1.3", Log.Warning,
                    $"Field {entityIndexField.FieldName} not defined in index {entityIndex.Name} on entity {entity.Id}");
                continue;
            }

            if (eField.AssociationEntity == null)
            {
                ddl += (bFirst ? "" : ",") + $"[{eField.DbFieldName}]" + (entityIndexField.IsDescending ? " Desc" : "");
                fields += (bFirst ? "" : ",") + $"[{eField.DbFieldName}]";
                bFirst = false;
            }
            else
            {
                foreach (EntityRelationMap map in eField.AssociationEntity.Maps)
                {
                    EntityField ef = entity.GetField(map.SourceField);
                    if (ef == null)
                    {
                        continue; //error in definition
                    }

                    ddl += (bFirst ? "" : ",") + $"[{ef.DbFieldName}]" + (entityIndexField.IsDescending ? " Desc" : "");
                    fields += (bFirst ? "" : ",") + $"[{ef.DbFieldName}]";
                    bFirst = false;
                }
            }
        }
        return bFirst;
    }

    private void CreatePkIndex(Entity entity, DbTable dbTable, string keys)
    {
        if (string.IsNullOrEmpty(keys))
        {
            return;
        }

        SetPkFieldsNotNullForSafe(entity, dbTable);
        bool cluster = CheckPkIndexCanCluster(entity);
        string ddl = $"ALTER TABLE {dbTable.FullName} ADD CONSTRAINT [{GetPkConstraintName(entity, dbTable)}] PRIMARY KEY " +
            $"{(cluster ? "CLUSTERED" : "NONCLUSTERED")} ({keys}) ";
        ddl += AddIndexOptions();
        ddl += GetFileGroupOrPartition(entity, cluster);
        AddToCommandList(entity, ddl, false);
        if (DoSqlCommand(entity, ddl, "", "10.4.1.6"))
        {
            _ = dbTable.AddIndex(GetPkConstraintName(entity, dbTable), true,
                cluster ? DbIndex.IndexType.Clustered : DbIndex.IndexType.NonClustered, null, true);
            if (entity.Partitioned && cluster)
            {
                dbTable.PartitionScheme = entity.PartitionScheme;
            }
            else if (!entity.Partitioned)
            {
                dbTable.FileGroup = entity.FileGroup;
            }
        }
    }

    protected virtual string AddIndexOptions()
    {
        return "";
    }

    private static bool CheckPkIndexCanCluster(Entity entity)
    {
        bool bClustered = !entity.Partitioned;
        foreach (EntityField keyField in entity.KeyFields)
        {
            if (keyField.Id == entity.PartitionField)
            {
                bClustered = true;
            }
        }

        return bClustered;
    }

    private void SetPkFieldsNotNullForSafe(Entity entity, DbTable dbTable)
    {
        string collation = DbNameManager.GetDbTableCollation(entity);
        foreach (EntityField keyField in entity.KeyFields)
        {
            //if (_pleaseStop) return;
            DbField dbField = dbTable.GetField(keyField.DbFieldName);
            if (dbField != null && !dbField.IsIdentity)
            {
                SetDbFieldNotNull(entity, dbTable, dbField, keyField, collation);
            }
        }
    }

    private static string GetPkConstraintName(Entity entity, DbTable dbTable)
    {
        return "PK_" + DbNameManager.GetDbSchemaName(entity) + "." + dbTable.Name;
    }

    private void DropExtraIndex(DbTable dbTable, Entity entity)
    {
        Dictionary<string, DbIndex> dropIndexes = [];
        Dictionary<string, DbIndex> renameIndexes = [];
        foreach (DbIndex dbIndex in dbTable.Indexes?.Values ?? Enumerable.Empty<DbIndex>())
        {
            if (_pleaseStop)
            {
                return;
            }

            if (dbIndex.IsPrimaryKey)
            {
                continue;
            }

            if (entity.PartitionField?.ToLower() == dbIndex.Name.ToLower())
            {
                continue;
            }

            EntityIndex entityIndex = CheckIndexInEntityIndexes(entity, renameIndexes, dbIndex);
            if (entityIndex != null)
            {
                continue;
            }

            bool inForeignKeyIndexes = CheckIndexInForeignKeyIndexes(entity, renameIndexes, dbIndex);
            bool drop = inForeignKeyIndexes ? Options.DropForeignKeyIndex : Options.DropExtraIndex;
            if (!drop)
            {
                continue;
            }

            if (!(dbIndex.Name == "IX_StateId" && entity.IsStateBase))
            {
                dropIndexes.Add(dbIndex.Name, dbIndex);
            }
        }

        foreach (DbIndex dbIndex in dropIndexes.Values)
        {
            if (_pleaseStop)
            {
                return;
            }

            DeleteDbIndex(entity, dbTable, dbIndex);
        }

        foreach (KeyValuePair<string, DbIndex> dbIndex in renameIndexes)
        {
            if (_pleaseStop)
            {
                return;
            }

            RenameDbIndex(entity, dbTable, dbIndex.Value, dbIndex.Key);
        }
    }

    private static EntityIndex CheckIndexInEntityIndexes(Entity entity, Dictionary<string, DbIndex> renameIndexes,
        DbIndex dbIndex)
    {
        EntityIndex entityIndex =
            entity.indexes?.FirstOrDefault(i =>
                string.Equals(DbNameManager.GetDbIndexName(i).ToLower(), dbIndex.Name.ToLower(),
                    StringComparison.CurrentCultureIgnoreCase));
        if (entityIndex == null)
        {
            entityIndex = entity.indexes?.FirstOrDefault(i =>
                string.Equals(EntityDbNameManager.ToPascalCase(i.Id, false), dbIndex.Name,
                    StringComparison.CurrentCultureIgnoreCase));
            if (entityIndex != null)
            {
                renameIndexes.Add(DbNameManager.GetDbIndexName(entityIndex), dbIndex);
            }
        }
        else
        {
            string newName = DbNameManager.GetDbIndexName(entityIndex);
            if (newName != dbIndex.Name)
            {
                renameIndexes.Add(newName, dbIndex);
            }
        }

        return entityIndex;
    }

    private static bool CheckIndexInForeignKeyIndexes(Entity entity, IDictionary<string, DbIndex> renameIndexes,
        DbIndex dbIndex)
    {
        var relatedEntity = entity.Associations?.FirstOrDefault(i =>
            string.Equals(DbNameManager.GetDbForeignKeyIndexName(i), dbIndex.Name,
                StringComparison.CurrentCultureIgnoreCase));
        if (relatedEntity == null)
        {
            return false;
        }

        bool drop = true;
        string newName = DbNameManager.GetDbForeignKeyIndexName(relatedEntity);
        if (newName != dbIndex.Name)
        {
            renameIndexes.Add(newName, dbIndex);
        }
        else
        {
            drop = false;
        }

        return drop;
    }

    private void RenameDbIndex(Entity entity, DbTable dbTable, DbIndex dbIndex, string newName)
    {
        string ddl = DDLGenerator.RenameIndex(dbTable.FullName, dbIndex.Name, newName);
        AddToCommandList(entity, ddl, true);
        _ = DoSqlCommand(entity, ddl, "", "10.4.1.7");
        _ = dbTable.Indexes.Remove(dbIndex.Name);
        dbIndex.Name = newName;
        dbTable.Indexes.Add(dbIndex.Name, dbIndex);
    }

    private void DeleteDbIndex(Entity entity, DbTable dbTable, DbIndex dbIndex)
    {
        _ = (dbTable.Indexes?.Remove(dbIndex.Name));
        if (dbIndex.IsUnique || dbIndex.IsPrimaryKey)
        {
            if (DropConstraint(entity, dbTable, dbIndex.Name))
            {
                return;
            }
        }

        string ddl = DDLGenerator.DeleteIndex(dbIndex.Name, dbTable.FullName);
        AddToCommandList(entity, ddl, false);
        _ = DoSqlCommand(entity, ddl, "", "10.4.1.8");
    }
}
