using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadDataSpaces()
    {
        string sql = DDLGenerator.GetDataSpaces();
        _model.FileGroups = [];
        IEnumerable<ElasticObject> dt = Select(sql, "10.1.1.0");
        if (dt == null)
        {
            return;
        }

        foreach (ElasticObject item in dt)
        {
            DbFileGroup dataSpace = new()
            {
                Id = item.GetLong("data_space_id"),
                Name = item.GetString("name"),
                Type = item.GetString("type")
            };
            _model.FileGroups.Add(dataSpace.Name, dataSpace);
        }
    }

    private void SyncFileGroup(List<ModelNamespace> syncModels)
    {
        HashSet<string> uniqueFileGroups = [];
        uniqueFileGroups.UnionWith(syncModels.SelectMany(m => m.GetEntities().Values)
            .Select(e => e.FileGroup));
        uniqueFileGroups.UnionWith(syncModels.SelectMany(m => m.GetEntities().Values)
            .SelectMany(e => e.PartitionSchemeFileGroups ?? []));
        uniqueFileGroups.UnionWith(syncModels.SelectMany(m => m.PartitionSchemes ?? [])
            .SelectMany(pe => pe.Value.FileGroups ?? []));
        foreach (string fileGroup in uniqueFileGroups)
        {
            SyncFileGroup(fileGroup);
        }
    }

    private void SyncFileGroup(string fileGroup)
    {
        if (string.Equals(fileGroup, "PRIMARY", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        if (!_model.FileGroups.ContainsKey(fileGroup))
        {
            DbFileGroup dbFileGroup = _model.FileGroups.Values.FirstOrDefault(d =>
                string.Equals(d.Name, fileGroup, StringComparison.CurrentCultureIgnoreCase));
            if (dbFileGroup != null)
            {
                AddMessage(null, null, $"Modify File Group {dbFileGroup.Name} to {fileGroup}");
                string ddl = DDLGenerator.ModifyFileGroup(dbFileGroup.Name, fileGroup);
                AddToCommandList(null, ddl, false);
                DoSqlCommand(null, ddl, "", "44.12.1");
                _ = _model.FileGroups.Remove(dbFileGroup.Name);
            }
            else
            {
                AddMessage(null, null, "Create File Group " + fileGroup);
                string ddl = DDLGenerator.AddFileGroup(fileGroup);
                AddToCommandList(null, ddl, false);
                DoSqlCommand(null, ddl, "", "44.12.2");
            }

            _model.FileGroups.Add(fileGroup, new DbFileGroup { Name = fileGroup, Type = "FG" });
        }
        SyncDbFile(fileGroup);
    }

    private string GetFileGroupOrPartition(Entity entity, bool clustered)
    {
        try
        {
            string ddl = " ON ";
            bool usedPartitionScheme = false;
            if (entity.Partitioned && clustered)
            {
                EntityField partitionField = entity.GetField(entity.PartitionField);
                if (partitionField != null &&
                    (partitionField.FieldType == TVariableTypes.Date ||
                     partitionField.FieldType == TVariableTypes.DateTime) //todo is temporary check
                )
                {
                    if (_model.PartitionSchemes.ContainsKey(entity.PartitionScheme))
                    {
                        ddl += "[" + entity.PartitionScheme + "]([" + partitionField.DbFieldName + "])";
                        usedPartitionScheme = true;
                    }
                    else
                    {
                        AddLog("54.0", Log.Warning,
                            $"Partition Scheme {entity.PartitionScheme} for entity {entity.Id} not defined");
                    }
                }
            }

            if (!usedPartitionScheme)
            {
                if (_model.FileGroups.ContainsKey(entity.FileGroup))
                {
                    ddl += "[" + entity.FileGroup + "]";
                }
                else
                {
                    AddLog("54.1", Log.Warning, $"File Group {entity.FileGroup} for entity {entity.Id} not defined");
                    ddl += "[PRIMARY]";
                }
            }


            return ddl;
        }
        catch (Exception e)
        {
            AddError(entity.model, entity, e, "54.2");
            return "";
        }
    }
}
