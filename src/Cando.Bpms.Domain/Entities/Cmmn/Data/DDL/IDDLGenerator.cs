using Neo.Bpms.Domain.Entities.Cmmn.Data.Command;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
public interface IDDLGenerator : ICommandGenerator
{
    string GetForeignKeys(string specificEntity);
    string GetTablesFieldsDefaultConstraint(string filterEntityDbTableName);
    string GetDefaultValue(string fullFieldType);
    bool IsStringType(string sqlFieldType);
    string GetSqlFieldType(TVariableTypes fieldType);
    string GetFullFieldType(TVariableTypes fieldType, int maxLen);
    string GetFullFieldType(string fieldType, int maxLen);
    string GetFieldType(TVariableTypes fieldType);
    bool IsStringType(TVariableTypes fieldType);
    string DatabaseFiles();
    string ReadShemas();
    string GetRebuildIndexCommand(string dbIndexName, string dbTableName);
    string ModifyFileGroup(string dbName, string fileGroup);
    string AddFileGroup(string fileGroup);
    string AddFile(string fileDataName, string fileDataPhysicalName, string fileGroup);
    string GetDataSpaces();
    string GetPartitionFunctions();
    string GetPartitionFunctionsParameters();
    string GetPartitionFunctionsRanges();
    string GetPartitionSchemes();
    string GetTables(string filterEntityDbTableName);
    string ModifyTable(string dbTableName, string newTableName);
    string GetTablesFields(string filterEntityDbTableName);
    string AddField(string dbTableName, string fieldName, string fullFieldType, bool bNotNull, bool isIdentity);
    string DropField(string dbTableName, string dropFieldName);
    string RenameField(string dbTableName, string oldName, string newName);
    string SetColumnNullable(string dbTableName, string dbFieldName, string fullFieldType);
    string SetColumnNotNull(string dbTableName, string dbFieldName, string fullFieldType);
    public string SetNotNullColumnValue(string dbTableName, string dbFieldName, object value);
    string ChangeField(string dbTableName, string tempColumnName, string pFieldDbFieldName, string castFieldType);
    string ReadIndexes(string filterEntityDbTableName);
    bool IndexIsUnique(string sIsUnique);
    bool IndexIsDescending(string strIsDescending);
    bool IndexIsIncluded(string strIsIncluded);
    string CreateIndex(bool entityIndexClustered, bool entityIndexIsUnique, string dbIndexName, string dbTableName);
    string RenameIndex(string dbTableName, string dbIndexName, string newName);
    string DeleteIndex(string dbIndexName, string dbTableName);
    string DropTrigger(string dbTriggerSchema, string dbTriggerName);
    string GetViews();
    string RenameView(string dbViewName, string newViewName);

}
