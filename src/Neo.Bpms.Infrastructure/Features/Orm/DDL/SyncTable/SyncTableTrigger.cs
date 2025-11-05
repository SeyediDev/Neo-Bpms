using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Engine.DDL;

abstract partial class DDLManager
{
    private void ReadTablesTriggers()
    {
        var sql = @"SELECT 
     sysobjects.name AS trigger_name 
    ,USER_NAME(sysobjects.uid) AS trigger_owner 
    ,s.name AS table_schema 
    ,OBJECT_NAME(parent_obj) AS table_name 
    ,OBJECTPROPERTY( id, 'ExecIsUpdateTrigger') AS isupdate 
    ,OBJECTPROPERTY( id, 'ExecIsDeleteTrigger') AS isdelete 
    ,OBJECTPROPERTY( id, 'ExecIsInsertTrigger') AS isinsert 
    ,OBJECTPROPERTY( id, 'ExecIsAfterTrigger') AS isafter 
    ,OBJECTPROPERTY( id, 'ExecIsInsteadOfTrigger') AS isinsteadof 
    ,OBJECTPROPERTY(id, 'ExecIsTriggerDisabled') AS [disabled] 
FROM sysobjects 
INNER JOIN sysusers 
    ON sysobjects.uid = sysusers.uid 
INNER JOIN sys.tables t 
    ON sysobjects.parent_obj = t.object_id 
INNER JOIN sys.schemas s 
    ON t.schema_id = s.schema_id 
WHERE sysobjects.type = 'TR' ";
        var dt = Select(sql, "10.0.30.2");
        if (!dt.Any()) return;
        DbTable dbTable = null;
        foreach (var item in dt)
        {
            var schemaName = item.GetString("table_schema");
            var tableName = item.GetString("table_name");
            var tableFullName = $"[{schemaName}].[{tableName}]";
            if (dbTable == null || (tableFullName != dbTable.FullName))
                _model.Tables.TryGetValue(tableFullName, out dbTable);
            if (dbTable == null)
                continue;
            var triggerName = item.GetString("trigger_name");
            dbTable.Triggers ??= [];
            dbTable.Triggers.Add(new DataTrigger
            {
                Name = triggerName,
                Owner = item.GetString("trigger_owner"),
                Schema = item.GetString("table_schema"),
                IsUpdate = item.GetBool("isupdate", false),
                IsDelete = item.GetBool("isdelete", false),
                IsInsert = item.GetBool("isinsert", false),
                IsAfter = item.GetBool("isafter", false),
                IsInsteadOf = item.GetString("isinsteadof"),
                Disabled = item.GetBool("disabled", false)
            });
        }
        dt = Select("select * from sys.triggers where name=\'SchemaAuditDDLTrigger\'", "10.0.30.3");
        if (!dt.Any()) return;
        sql = "DROP TRIGGER [SchemaAuditDDLTrigger] ON DATABASE";
        AddToCommandList(null, sql, false);
        DoSqlCommand(null, sql, "", "10.0.97.2");
    }
    private void SyncTriggers(Entity entity, DbTable dbTable)
    {
        //if (entity.audit == null)
        DropTriggers(entity, dbTable);
        //else 
        //    CheckTriggers(entity, dbTable);
    }

    private void DropTriggers(Entity entity, DbTable dbTable)
    {
        if (dbTable.Triggers == null) return;
        var triggers = dbTable.Triggers.ToList();
        foreach (var dbTrigger in triggers)
            DropTrigger(entity, dbTable, dbTrigger);
        dbTable.Triggers = null;
    }

    //        private void CheckTriggers(Entity entity, DbTable dbTable)
    //        {
    //            if (dbTable.Triggers == null)
    //            {
    //                if( entity.audit!=null && _options.CheckTriggers)
    //                    ReportEntityChanged(entity);
    //                return;
    //            }
    //            var triggers = dbTable.Triggers.ToList();
    //            dbTable.Triggers = new List<DataTrigger>();
    //            foreach (var dbTrigger in triggers)
    //            {
    //                if (!dbTrigger.Name.StartsWith(dbTable.Name))
    //                    DropTrigger(entity, dbTable, dbTrigger);
    //                else
    //                    dbTable.Triggers.Add(dbTrigger);
    //            }
    //        }

    private void DropTrigger(Entity entity, DbTable dbTable, DataTrigger dbTrigger)
    {
        var ddl = DDLGenerator.DropTrigger(dbTrigger.Schema, dbTrigger.Name);
        AddToCommandList(entity, ddl, false);
        var b = DoSqlCommand(entity, ddl, "", "10.0.97");
        if (b)
            dbTable.Triggers.Remove(dbTrigger);
    }
}
