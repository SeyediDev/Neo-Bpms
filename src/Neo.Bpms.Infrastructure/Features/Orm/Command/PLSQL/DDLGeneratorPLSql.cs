using Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
using Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.PLSQL;

public class DDLGeneratorPLSql(string databaseName) : DDLGeneratorSql(databaseName)
{
    public override string GetForeignKeys(string specificEntity)
    {
        var command = "select " +
                      $"a.constraint_name {nameof(ForeignKeyItem.FKName)}, a.table_name ForeignTable, b.column_name ForeignKeyColumn, " +
                      "c.table_name BaseTable, d.column_name BaseKeyColumn, a.delete_rule DELETE_RULE " +
                      "from " +
                      "user_constraints a, user_cons_columns b, user_constraints c, user_cons_columns d " +
                      "where " +
                      "a.constraint_name = b.constraint_name " +
                      "and a.r_constraint_name is not null " +
                      "and a.r_constraint_name=c.constraint_name " +
                      "and c.constraint_name=d.constraint_name ";
        //"and a.table_name LIKE 'M%%/_C%%'ESCAPE'/'";
        if (!string.IsNullOrEmpty(specificEntity))
            command += " and a.table_name LIKE '" + specificEntity + "'ESCAPE'/'";
        return command;
    }

    public override string GetTablesFieldsDefaultConstraint(string filterEntityDbTableName)
    {
        var command =
            "select c.definition val, c.name constraintName, col.name colName, o.name tableName \nfrom sys.default_constraints c \ninner join sys.columns col on col.default_object_id = c.object_id \n inner join sys.objects o  on o.object_id = c.parent_object_id \ninner join sys.schemas s on s.schema_id = o.schema_id ";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += $" and o.name = '{filterEntityDbTableName}'"; //todo
        return command;
    }

    public override string GetDefaultValue(string fullFieldType)
    {
        if (fullFieldType == "NUMBER" || fullFieldType == "FLOAT")
            return "0";
        return "\'\'";
    }

    public override bool IsStringType(string sqlFieldType)
    {
        throw new NotImplementedException();
    }

    public override string GetSqlFieldType(TVariableTypes fieldType)
    {
        throw new NotImplementedException();
    }

    public override string GetFullFieldType(TVariableTypes fieldType, int maxLen)
    {
        throw new NotImplementedException();
    }

    public override string GetFullFieldType(string fieldType, int maxLen)
    {
        throw new NotImplementedException();
    }

    public override string GetFieldType(TVariableTypes fieldType)
    {
        throw new NotImplementedException();
    }

    public override string DatabaseFiles()
    {
        return "select * from sys.database_files";
    }

    public override string ReadShemas()
    {
        throw new NotImplementedException();
    }

    public override string GetRebuildIndexCommand(string dbIndexName, string dbTableName)
    {
        throw new NotImplementedException();
    }

    public override string ModifyFileGroup(string dbName, string fileGroup)
    {
        throw new NotImplementedException();
    }

    public override string AddFileGroup(string fileGroup)
    {
        throw new NotImplementedException();
    }

    public override string AddFile(string fileDataName, string fileDataPhysicalName, string fileGroup)
    {
        throw new NotImplementedException();
    }

    public override string GetDataSpaces()
    {
        return "SELECT * FROM sys.data_spaces";
    }

    public override string GetPartitionFunctions()
    {
        return "SELECT * FROM sys.partition_functions"; //todo
    }

    public override string GetPartitionFunctionsParameters()
    {
        return "select * from sys.partition_parameters"; //todo
    }

    public override string GetPartitionFunctionsRanges()
    {
        return "select * from sys.partition_range_values"; //todo
    }

    public override string GetPartitionSchemes()
    {
        return "SELECT * FROM sys.partition_schemes"; //todo
    }

    public override string GetTables(string filterEntityDbTableName)
    {
        return "SELECT DISTINCT TABLE_NAME Name, TABLE_SCHEMA SchemaName FROM USER_TABLES "; //WHERE TABLE_NAME LIKE 'M%%/_C%%'ESCAPE'/'
    }

    public override string ModifyTable(string dbTableName, string newTableName)
    {
        return $"ALTER TABLE {dbTableName} RENAME TO {newTableName}";
    }

    public override string GetTablesFields(string filterEntityDbTableName)
    {
        return "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH FIELDLEN, NULLABLE, DATA_PRECISION, 'Persian_100_CI_AI' COLLATIONNAME FROM USER_TAB_COLS ";
        //WHERE TABLE_NAME LIKE 'M%%/_C%%'ESCAPE'/'
    }

    public override string AddField(string dbTableName, string fieldName, string fullFieldType, bool bNotNull, bool isIdentity)
    {
        return $"ALTER TABLE {dbTableName} ADD ({fieldName} {fullFieldType} " +
               (bNotNull ? "NOT " : "") + "NULL" +
               (isIdentity ? " IDENTITY (1, 1);" : bNotNull ? " default " + GetDefaultValue(fullFieldType) : "") + ")";
    }

    public override string DropField(string dbTableName, string dropFieldName)
    {
        return $"ALTER TABLE {dbTableName} DROP COLUMN \"{dropFieldName}\"";
    }

    public override string RenameField(string dbTableName, string oldName, string newName)
    {
        return $"ALTER TABLE {dbTableName} RENAME COLUMN \"{oldName}\" TO \"{newName}\"";
    }

    public override string SetColumnNullable(string dbTableName, string dbFieldName, string fullFieldType)
    {
        return $"ALTER TABLE {dbTableName} MODIFY ({dbFieldName} NULL)";
    }

    public override string SetColumnNotNull(string dbTableName, string dbFieldName, string fullFieldType)
    {
        return $"ALTER TABLE {dbTableName} MODIFY ({dbFieldName} NOT NULL)";
    }

    public override string SetNotNullColumnValue(string dbTableName, string dbFieldName, object value)
    {
        return $"UPDATE {dbTableName} SET [{dbFieldName}] = {value} WHERE [{dbFieldName}] IS NULL;";
    }

    public override string ChangeField(string dbTableName, string tempColumnName, string pFieldDbFieldName, string castFieldType)
    {
        return $"UPDATE {dbTableName} SET {tempColumnName}=CAST([{pFieldDbFieldName}] AS {castFieldType})";
    }

    public override string ReadIndexes(string filterEntityDbTableName)
    {
        //[IndexSpaceId], [IndexType], IsPK todo
        //todo Schema?
        var command = "select [dbo] SchemName, a.table_name TableName, a.index_name IndexName, b.uniqueness IsUnique, " +
                      "a.column_name FieldName, include_column IsIncluded, a.descend IsDescending " +
                      "from user_ind_columns a, user_indexes b " +
                      "where a.index_name=b.index_name ";
        //"AND a.table_name LIKE 'M%%/_C%%'ESCAPE'/' ";????????
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += " and a.table_name LIKE '" + filterEntityDbTableName + "'ESCAPE'/'";
        command += "order by a.table_name, a.index_name, a.column_position";
        return command;
    }

    public override bool IndexIsUnique(string sIsUnique)
    {
        return sIsUnique == "UNIQUE";
    }

    public override bool IndexIsDescending(string strIsDescending)
    {
        return strIsDescending == "DESC";
    }

    public override bool IndexIsIncluded(string strIsIncluded)
    {
        return strIsIncluded == "INCLUDE"; //???
    }

    public override string CreateIndex(bool entityIndexClustered, bool entityIndexIsUnique, string dbIndexName, string dbTableName)
    {
        return $"CREATE {(entityIndexClustered ? "CLUSTERED " : "")}{(entityIndexIsUnique ? "UNIQUE " : "")}INDEX {dbIndexName} ON {dbTableName} (";
    }

    public override string RenameIndex(string dbTableName, string dbIndexName, string newName)
    {
        return $"ALTER TABLE {dbTableName} RENAME Index \"{dbIndexName}\" TO \"{newName}\"";
    }

    public override string DeleteIndex(string dbIndexName, string dbTableName)
    {
        return $"DROP INDEX {dbIndexName}"; //??? WITH ( ONLINE = OFF ) , dbTable.name
    }

    public override string DropTrigger(string dbTriggerSchema, string dbTriggerName)
    {
        return $"DROP TRIGGER {dbTriggerName}";
    }

    public override string GetViews()
    {
        return "SELECT DISTINCT TABLE_NAME Name, TABLE_SCHEMA SchemaName, VIEW_DEFINITION FROM USER_VIEWS "; //todo //WHERE TABLE_NAME LIKE 'M%%/_C%%'ESCAPE'/'
    }

    public override string RenameView(string dbViewName, string newViewName)
    {
        return $"ALTER VIEW {dbViewName} RENAME TO {newViewName}";
    }
}
