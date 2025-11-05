using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    private void ReadTablesFieldsDefaultConstraint()
    {
        var sql = DDLGenerator.GetTablesFieldsDefaultConstraint(Options.SpecificEntity);

        var rs = Select(sql, "10.1.1.10");
        if (rs != null)
        {
            DbTable dbTable = null;
            foreach (var item in rs)
            {
                var schemaName = item.GetString("schemaName");
                var tableName = item.GetString("tableName");
                var tableFullName = $"[{schemaName}].[{tableName}]";
                if (dbTable == null || (tableFullName != dbTable.FullName))
                    _model.Tables.TryGetValue(tableFullName, out dbTable);
                if (dbTable != null)
                {
                    var colName = item.GetString("colName");
                    var dbField = dbTable.GetField(colName);
                    if (dbField != null)
                    {
                        dbField.Default = new DbField.DefaultConstraint
                        {
                            Name = item.GetString("constraintName"),
                            Value = item.GetString("val") //todo
                        };
                    }
                    else
                        AddLog("10.1.1.11", Log.Error, "پایگاه داده خراب است " + tableName + "." + colName);
                }
                else
                    AddLog("10.1.1.12", Log.Error, "پایگاه داده خراب است " + tableName);
            }
        }
        else
            AddLog("10.1.1.13", Log.Error, "پایگاه داده خراب است ");
    }

    private bool DropConstraint(Entity entity, DbTable dbTable, string constraintName)
    {
        DropTriggers(entity, dbTable);
        var ddl = $"ALTER TABLE {dbTable.FullName} DROP CONSTRAINT [" + constraintName + "]";
        AddToCommandList(entity, ddl, false);
        return DoSqlCommand(entity, ddl, "", "10.0.76");
    }
}
