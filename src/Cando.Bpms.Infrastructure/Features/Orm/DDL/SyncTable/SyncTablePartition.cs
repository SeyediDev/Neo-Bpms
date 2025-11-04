using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadTablesDataSpaces()
    {
        var sql = @"SELECT tbl.name AS [Table_Name], s.name AS [Schema_Name], 
          CASE WHEN dsidx.type='FG' THEN dsidx.name ELSE '(Partitioned)' END AS [File_Group] ,
          CASE WHEN dsidx.type='PS' THEN dsidx.name ELSE '' END AS [Partition_Schema],
		  dsidx.type AS [ds_type]
FROM      sys.tables AS tbl 
JOIN      sys.indexes AS idx ON idx.object_id = tbl.object_id --AND idx.is_primary_key=1
JOIN      sys.schemas AS s ON tbl.schema_id = s.schema_id
LEFT JOIN sys.data_spaces AS dsidx 
ON        dsidx.data_space_id = idx.data_space_id";
        var dt = Select(sql, "10.1.1.17");
        if (dt == null) return;
        foreach (var item in dt)
        {
            if (!item.GetField("Table_Name", out var cvTableName))
                continue;
            if (!item.GetField("Schema_Name", out var cvSchemaName))
                continue;
            if (!_model.Tables.TryGetValue(cvSchemaName.ToString(), cvTableName.ToString()
                    , out var dbTable) || dbTable == null)
                continue;
            var dsType = item.GetString("ds_type");
            if (dsType == "FG")
                dbTable.FileGroup = item.GetString("File_Group");
            else
                dbTable.PartitionScheme = item.GetString("Partition_Schema");
        }
    }

    private void SyncTablePartition(Entity entity, DbTable dbTable)
    {
        if (dbTable.PartitionScheme != entity.PartitionScheme)
            //todo اگر فیلد پارتیشن اسکیما تعویض شود متوجه نمی شود که باید توسعه داد 
            ChangeTablePartitionScheme(entity, dbTable);
    }

    private void ChangeTablePartitionScheme(Entity entity, DbTable dbTable)
    {
        var partitionField = entity.GetField(entity.PartitionField);
        if (partitionField == null)
        {
            AddLog("99.0", Log.Warning, $"Invalid partitionField in entity {entity.Id}");
            return;
        }

        var clusteredIndex = dbTable.Indexes?.Values.FirstOrDefault(i => i.Clustered);
        var clusteredIndexField = clusteredIndex?.Fields?.FirstOrDefault(f => f.Name == partitionField.DbFieldName);
        if (clusteredIndexField != null && clusteredIndex.DataSpace?.Name == entity.PartitionScheme)
        {
            dbTable.PartitionScheme = entity.PartitionScheme;
            return; //is ok
        }

        RemoveAllTableDependency(dbTable, entity);
        var pkIndex = dbTable.PkIndex;
        if (pkIndex != null && clusteredIndex != pkIndex && pkIndex.Clustered)
            DeleteDbIndex(entity, dbTable, pkIndex);
        var ddl = $"CREATE CLUSTERED INDEX [{entity.PartitionField}] ON [{DbNameManager.GetDbSchemaName(entity)}].[" + dbTable.Name + "]([" +
                  entity.PartitionField + "]) WITH(DROP_EXISTING = " + (clusteredIndexField != null ? "ON" : "OFF") +
                  ", ONLINE = ON) ON [" +
                  entity.PartitionScheme + "]([" + entity.PartitionField + "])";
        AddToCommandList(entity, ddl, false);
        DoSqlCommand(entity, ddl, "", "44.3.3");
        dbTable.PartitionScheme = entity.PartitionScheme;
        var keys = FetchKeys(entity);
        ddl = $"ALTER TABLE {dbTable.FullName} ADD PRIMARY KEY NONCLUSTERED (" + keys +
              ") WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
        AddToCommandList(entity, ddl, false);
        DoSqlCommand(entity, ddl, "", "44.3.2");
    }
}
