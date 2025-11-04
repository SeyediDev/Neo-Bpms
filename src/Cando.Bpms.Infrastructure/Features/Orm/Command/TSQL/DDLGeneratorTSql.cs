using Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.TSQL;

public class DDLGeneratorTSql(string databaseName) : DDLGeneratorSql(databaseName)
{
    public override string GetDefaultValue(string fullFieldType)
    {
        if (fullFieldType == "bigint" || fullFieldType == "smallint" || fullFieldType == "int" ||
            fullFieldType == "float" || fullFieldType == "bit")
            return "0";
        return "\'\'";
    }

    public override string ModifyFileGroup(string dbName, string fileGroup)
    {
        return $"ALTER DATABASE [{DatabaseName}] MODIFY FILEGROUP [{dbName}] NAME = [{fileGroup}]";
    }

    public override string AddFileGroup(string fileGroup)
    {
        return "ALTER DATABASE [" + DatabaseName + "] ADD FILEGROUP [" + fileGroup + "]";
    }

    public override string AddFile(string fileDataName, string fileDataPhysicalName, string fileGroup)
    {
        return
            $"ALTER DATABASE [{DatabaseName}] ADD FILE ( NAME = N'{fileDataName}', FILENAME = N'{fileDataPhysicalName}' , SIZE = 512KB , FILEGROWTH = 1024KB ) TO FILEGROUP [{fileGroup}]";
    }

    public override bool IsStringType(string sqlFieldType)
    {
        return sqlFieldType.ToLower() == "nvarchar";
    }

    public override string GetSqlFieldType(TVariableTypes fieldType)
    {
        switch (fieldType)
        {
            //case TVariableTypes.varBOOL0:				return "bit";
            case TVariableTypes.BOOL:
                return "bit";
            case TVariableTypes.Char:
                return "smallint";
            case TVariableTypes.UChar:
                return "smallint";
            case TVariableTypes.Short:
                return "smallint";
            case TVariableTypes.Int:
                return "int";
            case TVariableTypes.Long:
                return "bigint";
            case TVariableTypes.ULong:
                return "bigint";
            case TVariableTypes.Decimal:
                return "money";
            case TVariableTypes.StringListItem:
                return "int";
            case TVariableTypes.StringListBitMask:
                return "bigint";
            case TVariableTypes.DayHourMinute:
                return "bigint";
            case TVariableTypes.DoubleMinuteSecond:
                return "bigint";
            case TVariableTypes.Association:
                return "bigint";
            case TVariableTypes.UShort:
                return "int";
            case TVariableTypes.HourMinute:
                return "int";
            case TVariableTypes.Double:
                return "float";
            case TVariableTypes.DurHourMinute:
                return "float";
            case TVariableTypes.String:
                return "nvarchar";
            case TVariableTypes.DateStr:
                return "nvarchar";
            case TVariableTypes.DateTime:
                return "datetime";
            case TVariableTypes.Date:
                return "datetime";
            case TVariableTypes.ByteArray:
                return "varbinary";
            default:
                return "nvarchar";
        }
    }

    public override string GetFullFieldType(TVariableTypes fieldType, int maxLen)
    {
        var sqlFieldType = GetFieldType(fieldType);
        switch (fieldType)
        {
            case TVariableTypes.BOOL:
                break;
            case TVariableTypes.Char:
                break;
            case TVariableTypes.UChar:
                break;
            case TVariableTypes.Short:
                break;
            case TVariableTypes.Int:
                break;
            case TVariableTypes.Long:
                break;
            case TVariableTypes.ULong:
                break;
            case TVariableTypes.Decimal:
                break;
            case TVariableTypes.StringListItem:
                break;
            case TVariableTypes.StringListBitMask:
                break;
            case TVariableTypes.DayHourMinute:
                break;
            case TVariableTypes.DoubleMinuteSecond:
                break;
            case TVariableTypes.Association:
                break;
            case TVariableTypes.UShort:
                break;
            case TVariableTypes.HourMinute:
                break;
            case TVariableTypes.Double:
                break;
            case TVariableTypes.DurHourMinute:
                break;
            case TVariableTypes.DateTime:
                break;
            case TVariableTypes.Date:
                break;
            //case TVariableTypes.String:
            //case TVariableTypes.PW:
            //case TVariableTypes.DateStr:
            //case TVariableTypes.ByteArray:
            default:
                {
                    var length = maxLen;
                    if (length <= 0 || length == 4000)
                        sqlFieldType += "(max)";
                    else
                        sqlFieldType += "(" + length + ")";
                    break;
                }
        }
        return sqlFieldType;
    }

    public override string GetFullFieldType(string fieldType, int maxLen)
    {
        var sqlFieldType = fieldType;
        if (fieldType is "nvarchar" or "varchar" or "varbinary")
        {
            var length = maxLen;
            if (length <= 0 || length >= 4000)
                sqlFieldType += "(max)";
            else
                sqlFieldType += "(" + length + ")";
        }
        return sqlFieldType;
    }

    public override string GetFieldType(TVariableTypes fieldType)
    {
        switch (fieldType)
        {
            //case TVariableTypes.varBOOL0:				return "bit";
            case TVariableTypes.BOOL:
                return "bit";
            case TVariableTypes.Char:
                return "smallint";
            case TVariableTypes.UChar:
                return "smallint";
            case TVariableTypes.Short:
                return "smallint";
            case TVariableTypes.Int:
                return "int";
            case TVariableTypes.Long:
                return "bigint";
            case TVariableTypes.ULong:
                return "bigint";
            case TVariableTypes.Decimal:
                return "money";
            case TVariableTypes.StringListItem:
                return "int";
            case TVariableTypes.StringListBitMask:
                return "bigint";
            case TVariableTypes.DayHourMinute:
                return "bigint";
            case TVariableTypes.DoubleMinuteSecond:
                return "bigint";
            case TVariableTypes.Association:
                return "bigint";
            case TVariableTypes.UShort:
                return "int";
            case TVariableTypes.HourMinute:
                return "int";
            case TVariableTypes.Double:
                return "float";
            case TVariableTypes.DurHourMinute:
                return "float";
            case TVariableTypes.DateTime:
                return "datetime";
            case TVariableTypes.Date:
                return "datetime";
            case TVariableTypes.ByteArray:
                return "varbinary";
            case TVariableTypes.String:
            case TVariableTypes.DateStr:
            default:
                return "nvarchar";
        }
    }

    public override string ReadShemas()
    {
        return "SELECT " +
               "SCHEMA_NAME Name, " +
               "SCHEMA_OWNER Owner, " +
               "DEFAULT_CHARACTER_SET_CATALOG DefaultCharacterSetCatalog, " +
               "DEFAULT_CHARACTER_SET_SCHEMA DefaultCharacterSetSchema, " +
               "DEFAULT_CHARACTER_SET_NAME DefaultCharacterSetName " +
               $"FROM INFORMATION_SCHEMA.SCHEMATA WHERE CATALOG_NAME = '{DatabaseName}'";
    }


    public override string GetRebuildIndexCommand(string indexName, string tableName)
    {
        return "ALTER INDEX " + indexName + " ON " + tableName + " REBUILD";
    }

    public override string DatabaseFiles()
    {
        return "select * from sys.database_files";
    }

    public override string GetDataSpaces()
    {
        return "SELECT * FROM sys.data_spaces";
    }

    public override string GetPartitionFunctions()
    {
        return "SELECT * FROM [sys].[partition_functions]";
    }

    public override string GetPartitionFunctionsParameters()
    {
        return "select * from sys.partition_parameters";
    }

    public override string GetPartitionFunctionsRanges()
    {
        return "select * from sys.partition_range_values order by boundary_id asc";
    }

    public override string GetPartitionSchemes()
    {
        return "SELECT * FROM [sys].[partition_schemes]";
    }

    public override string GetTables(string filterEntityDbTableName)
    {
        var command =
            "SELECT DISTINCT TABLE_NAME Name, TABLE_SCHEMA SchemaName FROM INFORMATION_SCHEMA.TABLES " +
            $"WHERE TABLE_TYPE='BASE TABLE' AND TABLE_CATALOG='{DatabaseName}' ";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += $" AND TABLE_NAME LIKE '{filterEntityDbTableName}'";
        command += "ORDER BY TABLE_NAME";
        return command;
    }

    public override string ModifyTable(string dbTableName, string newTableName)
    {
        return $"exec sp_rename '{dbTableName}', '{newTableName}'";
    }

    public override string GetTablesFieldsDefaultConstraint(string filterEntityDbTableName)
    {
        var command =
            "select c.definition val, c.name constraintName, col.name colName, o.name tableName, s.name schemaName " +
            "\nfrom sys.default_constraints c " +
            "\ninner join sys.columns col on col.default_object_id = c.object_id " +
            "\ninner join sys.objects o on o.object_id = c.parent_object_id " +
            "\ninner join sys.schemas s on s.schema_id = o.schema_id ";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += $" and o.name = '{filterEntityDbTableName}'";
        return command;
    }

    public override string GetTablesFields(string filterEntityDbTableName)
    {
        var command =
            $"SELECT COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') IsIdentity, " +
            $"TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FIELDLEN, IS_NULLABLE NULLABLE, COLLATION_NAME COLLATIONNAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='{DatabaseName}'";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += $" AND TABLE_NAME LIKE '{filterEntityDbTableName}'";
        return command;
    }

    public override string AddField(string dbTableName, string fieldName, string fullFieldType, bool bNotNull,
        bool isIdentity)
    {
        return $"ALTER TABLE {dbTableName} ADD [{fieldName}] {fullFieldType} {(bNotNull ? "NOT " : "")}NULL" +
               (isIdentity ? " IDENTITY (1, 1);" : bNotNull ? " default " + GetDefaultValue(fullFieldType) : "");
    }

    public override string DropField(string dbTableName, string dropFieldName)
    {
        return $"ALTER TABLE {dbTableName} DROP COLUMN [{dropFieldName}]";
    }

    public override string RenameField(string dbTableName, string oldName, string newName)
    {
        return $"exec sp_rename '{dbTableName}.{oldName}', '{newName}'";
    }

    public override string SetColumnNullable(string dbTableName, string dbFieldName, string fullFieldType)
    {
        return $"ALTER TABLE {dbTableName} ALTER COLUMN [{dbFieldName}] {fullFieldType} NULL";
    }

    public override string SetColumnNotNull(string dbTableName, string dbFieldName, string fullFieldType)
    {
        return $"ALTER TABLE {dbTableName} ALTER COLUMN [{dbFieldName}] {fullFieldType} NOT NULL";
    }

    public override string SetNotNullColumnValue(string dbTableName, string dbFieldName, object value)
    {
        return $"UPDATE {dbTableName} SET [{dbFieldName}] = {value} WHERE [{dbFieldName}] IS NULL;";
    }

    public override string ChangeField(string dbTableName, string tempColumnName, string pFieldDbFieldName,
        string castFieldType)
    {
        return $"UPDATE {dbTableName} SET {tempColumnName}=CAST([{pFieldDbFieldName}] AS {castFieldType})";
    }

    public override string ReadIndexes(string filterEntityDbTableName)
    {
        var command =
            "SELECT s.name [SchemaName],t.name [TableName],i.name [IndexName],i.is_unique [IsUnique],i.[data_space_id] [IndexSpaceId],i.[type] [IndexType], i.[is_primary_key] [IsPK]," +
            "c.name [FieldName],ic.is_included_column [IsIncluded],ic.is_descending_key IsDescending, ic.partition_ordinal, ic.key_ordinal " +
            "FROM sys.index_columns ic " +
            "INNER JOIN sys.tables  t ON t.object_id=ic.object_id " + //AND t.name LIKE 'M%%[_]C%%' 
            "INNER JOIN sys.schemas s ON s.schema_id=t.schema_id " +
            "INNER JOIN sys.columns c ON c.column_id = ic.column_id AND c.object_id=t.object_id " +
            "INNER JOIN sys.indexes i ON i.index_id=ic.index_id AND i.object_id=t.object_id " +
            "WHERE t.type='U' ";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += $" AND t.name LIKE '{filterEntityDbTableName}'";
        command += " ORDER BY ic.index_column_id, ic.key_ordinal";
        return command;
    }

    public override bool IndexIsUnique(string sIsUnique)
    {
        return sIsUnique == "True";
    }

    public override bool IndexIsDescending(string strIsDescending)
    {
        return strIsDescending == "True";
    }

    public override bool IndexIsIncluded(string strIsIncluded)
    {
        return strIsIncluded == "True";
    }

    public override string CreateIndex(bool entityIndexClustered, bool entityIndexIsUnique, string dbIndexName,
        string dbTableName)
    {
        return
            $"CREATE {(entityIndexClustered ? "CLUSTERED " : "")}{(entityIndexIsUnique ? "UNIQUE " : "")} " +
            $"INDEX {dbIndexName} ON {dbTableName} (";
    }

    public override string RenameIndex(string dbTableName, string dbIndexName, string newName)
    {
        return $"exec sp_rename '{dbTableName}." + dbIndexName + "', '" + newName + "', N'INDEX'";
    }

    public override string DeleteIndex(string dbIndexName, string dbTableName)
    {
        return $"DROP INDEX [{dbIndexName}] ON {dbTableName} WITH ( ONLINE = OFF )";
    }

    public override string DropTrigger(string dbTriggerSchema, string dbTriggerName)
    {
        return $"DROP TRIGGER [{dbTriggerSchema}].[{dbTriggerName}]";
    }

    public override string GetViews()
    {
        return
            "SELECT DISTINCT TABLE_NAME Name, TABLE_SCHEMA SchemaName, VIEW_DEFINITION FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_CATALOG='" +
            DatabaseName + "' ORDER BY TABLE_NAME";
    }

    public override string RenameView(string dbViewName, string newViewName)
    {
        return $"exec sp_rename {dbViewName}, {newViewName}";
    }

    public override string GetForeignKeys(string filterEntityDbTableName)
    {
        var command = "SELECT " +
                      "a.CONSTRAINT_NAME FKName, " +
                      "a.DELETE_RULE DELETE_RULE, " +
                      "a.UPDATE_RULE UPDATE_RULE, " +
                      "c.TABLE_NAME ForeignTable, " +
                      "c.TABLE_SCHEMA ForeignTableSchema, " +
                      "c.COLUMN_NAME ForeignKeyColumn, " +
                      "d.TABLE_NAME BaseTable, " +
                      "d.TABLE_SCHEMA BaseTableSchema, " +
                      "g.COLUMN_NAME BaseKeyColumn " +
                      "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS a " +
                      "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE c " +
                      "ON a.CONSTRAINT_NAME = c.CONSTRAINT_NAME AND a.CONSTRAINT_CATALOG='" +
                      DatabaseName + "' " +
                      "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS d " +
                      "ON a.UNIQUE_CONSTRAINT_NAME = d.CONSTRAINT_NAME AND d.CONSTRAINT_TYPE = 'PRIMARY KEY' " +
                      "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE g " +
                      "ON d.CONSTRAINT_NAME = g.CONSTRAINT_NAME " +
                      "WHERE a.CONSTRAINT_CATALOG='" + DatabaseName +
                      "' AND d.CONSTRAINT_TYPE = 'PRIMARY KEY' ";
        if (!string.IsNullOrEmpty(filterEntityDbTableName))
            command += " AND c.TABLE_NAME LIKE '" + filterEntityDbTableName + "'";
        return command;
    }
}
