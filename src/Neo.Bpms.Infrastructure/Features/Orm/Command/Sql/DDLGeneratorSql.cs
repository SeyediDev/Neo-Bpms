using Neo.Bpms.Domain.Models.Cmmn.Data.DDL;

namespace Neo.Bpms.Infrastructure.Features.Orm.Command.Sql;

public abstract class DDLGeneratorSql(string databaseName) : CommandGeneratorSql(databaseName), IDDLGenerator
{
    public abstract string GetForeignKeys(string specificEntity);
    public abstract string GetTablesFieldsDefaultConstraint(string filterEntityDbTableName);
    public abstract string GetDefaultValue(string fullFieldType);
    public abstract bool IsStringType(string sqlFieldType);
    public abstract string GetSqlFieldType(TVariableTypes fieldType);
    public abstract string GetFullFieldType(TVariableTypes fieldType, int maxLen);
    public abstract string GetFullFieldType(string fieldType, int maxLen);
    public abstract string GetFieldType(TVariableTypes fieldType);
    public abstract string DatabaseFiles();
    public abstract string ReadShemas();
    public abstract string GetRebuildIndexCommand(string dbIndexName, string dbTableName);

    public abstract string ModifyFileGroup(string dbName, string fileGroup);
    public abstract string AddFileGroup(string fileGroup);
    public abstract string AddFile(string fileDataName, string fileDataPhysicalName, string fileGroup);

    public abstract string GetDataSpaces();
    public abstract string GetPartitionFunctions();
    public abstract string GetPartitionFunctionsParameters();
    public abstract string GetPartitionFunctionsRanges();
    public abstract string GetPartitionSchemes();
    public abstract string GetTables(string filterEntityDbTableName);
    public abstract string ModifyTable(string dbTableName, string newTableName);
    public abstract string GetTablesFields(string filterEntityDbTableName);
    public abstract string AddField(string dbTableName, string fieldName, string fullFieldType, bool bNotNull, bool isIdentity);
    public abstract string DropField(string dbTableName, string dropFieldName);
    public abstract string RenameField(string dbTableName, string oldName, string newName);
    public abstract string SetColumnNullable(string dbTableName, string dbFieldName, string fullFieldType);
    public abstract string SetColumnNotNull(string dbTableName, string dbFieldName, string fullFieldType);
    public abstract string SetNotNullColumnValue(string dbTableName, string dbFieldName, object value);
    public abstract string ChangeField(string dbTableName, string tempColumnName, string pFieldDbFieldName, string castFieldType);
    public abstract string ReadIndexes(string filterEntityDbTableName);
    public abstract bool IndexIsUnique(string sIsUnique);
    public abstract bool IndexIsDescending(string strIsDescending);
    public abstract bool IndexIsIncluded(string strIsIncluded);
    public abstract string CreateIndex(bool entityIndexClustered, bool entityIndexIsUnique, string dbIndexName, string dbTableName);
    public abstract string RenameIndex(string dbTableName, string dbIndexName, string newName);
    public abstract string DeleteIndex(string dbIndexName, string dbTableName);
    public abstract string DropTrigger(string dbTriggerSchema, string dbTriggerName);
    public abstract string GetViews();
    public abstract string RenameView(string dbViewName, string newViewName);

    public bool IsStringType(TVariableTypes fieldType)
    {
        switch (fieldType)
        {
            case TVariableTypes.DateStr:
            case TVariableTypes.String:
            case TVariableTypes.Link:
                return true;
        }

        return false;
    }
}
