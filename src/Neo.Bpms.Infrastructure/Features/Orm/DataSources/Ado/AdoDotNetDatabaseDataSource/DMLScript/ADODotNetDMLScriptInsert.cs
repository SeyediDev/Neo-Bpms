using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement insert script functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    private string GenerateInsertScript(bool giveOutput, Entity entity)
    {
        var commands = new CandoStringBuilder();
        GenerateInsertScriptInOneTable(null, giveOutput, entity, commands);
        return commands.ToString();
    }

    private void GenerateInsertScriptInOneTable(EntityField parentEntityField,
        bool giveOutput, Entity entity, CandoStringBuilder commands)
    {
        GenerateInsertParentScript(giveOutput, entity, commands);

        var selectedFields = Fields.Values
            .Where(f => FieldIsForParent(parentEntityField, entity, f))
            .ToDictionary(f => f.FieldId);
        var commandStr = new CandoStringBuilder();
        var generateAuditRecord = entity.Auditable == AuditableVersion.V1;
        var insertValues = selectedFields.Select(fld => GetSqlValueField(true, fld.Value, DataSrcDefinition.Entity)).ToList();
        var values = insertValues.Where(f => f.SqlValue != "null").ToList();
        if (values.Count == 0 && insertValues.Count != 0)
            values.Add(insertValues.FirstOrDefault());
        if (values.Count == 0)
            throw new Exception($"Please set at-least one column in insert table {entity.Id}");
        var keyes = entity.KeyFields;
        if (parentEntityField != null)
            keyes = parentEntityField.ParentEntity.Maps.Select(map => DataSrcDefinition.Entity.GetField(map?.SourceField));
        var keyesIds = string.Join(",", keyes.Select(k => k.Id));
        var keyesIdsTypes = string.Join(",", keyes.Select(k => $"{k.Id} {GetFieldType(k?.CSharpType ?? typeof(long))}"));

        var memoryTableName = GetUniqueSqlVariableName("Table");
        if (generateAuditRecord || parentEntityField != null)
            commandStr += $"DECLARE {memoryTableName} TABLE({keyesIdsTypes});\r\n";
        var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
        commandStr += $@"INSERT INTO {dbTableName} 
	([{string.Join("],[", values.Select(v => v.FieldId))}]) ";
        if ((keyes?.Count() ?? 0) != 0 && (giveOutput || generateAuditRecord))
        {
            var keyNames = entity.KeyFields != null
                ? string.Join(",",
                    entity.KeyFields.Select(
                        keyField => "inserted." + keyField.DbFieldName +
                                        (Equals(keyField.Id, keyField.DbFieldName)
                                            ? ""
                                            : $" as {(parentEntityField == null ? $"[{keyField.Id}]" : $"{GetInnerInjectionKey()}@{parentEntityField.Id}")}"
                                        )))
                : "";
            commandStr += $"\r\nOUTPUT {keyNames}";
            if (generateAuditRecord || parentEntityField != null)
                commandStr += $" INTO {memoryTableName}({keyesIds})";
        }

        commandStr += $"\r\nVALUES ({string.Join(",", values.Select(v => v.SqlValue))});";
        if ((keyes?.Count() ?? 0) != 0)
        {
            if (parentEntityField != null)
            {
                var parentKeyType = GetFieldType(parentEntityField.CSharpType);
                commandStr +=
                    $"\r\nDECLARE @{parentEntityField.Id} {parentKeyType};" +
                    $"\r\nSELECT @{parentEntityField.Id} = {keyes?.FirstOrDefault()?.Id} FROM {memoryTableName}\r\n";
            }

            if (giveOutput && generateAuditRecord)
                commandStr += $"\r\nSELECT * FROM {memoryTableName} ";
        }

        commands.Append(commandStr.ToString());
    }

    private static bool FieldIsForParent(EntityField parentEntityField, Entity entity, ColumnDefinition f)
    {
        if (parentEntityField == null)
        {
            if (f.Field == null)
                return true;
            if (!f.Field.IsForParent &&
                        !f.Field.CheckFlag(EntityFieldFlags.DerivedEntityBooleanField))
                return true;
        }
        else if (f.Field != null)
        {
            if (f.Field.IsForParentField(parentEntityField, entity))
                return true;
            if (f.Field.CheckFlag(EntityFieldFlags.DerivedEntityBooleanField) && Equals(f.Field.Entity, entity))
                return true;
        }

        return false;
    }

    private void GenerateInsertParentScript(
        bool giveOutput, Entity entity, CandoStringBuilder commands)
    {
        foreach (var referenceField in Parents(entity))
        {
            var parentEntityRelationship = referenceField.Relationship as ParentEntity;
            if (parentEntityRelationship == null)
                continue;
            var booleanFieldId = parentEntityRelationship.BooleanFieldIdInParentThatPresentMe ?? "";
            Fields.Remove(booleanFieldId);

            var baseBoolField = referenceField.Relationship.DestEntity.GetField(booleanFieldId);
            Fields.Add(booleanFieldId, new ColumnDefinition(baseBoolField, true));

            if (referenceField.ParentAssociationField == null)
            {

            }
            GenerateInsertScriptInOneTable(referenceField.ParentAssociationField, giveOutput,
                referenceField.Relationship.DestEntity, commands);
            foreach (var maps in parentEntityRelationship.Maps)
            {
                Fields.Remove(maps.SourceField);
                Fields.Add(maps.SourceField,
                    new ColumnDefinition(entity.GetField(maps.SourceField), $"@{referenceField.ParentAssociationField.Id}"));
            }
        }
    }

    private static string GenerateInsertScriptFromQuery(DataSource queryDataSource, Entity entity)
    {
        var insertFields = new List<string>();
        var beGenerateGroupBy = CheckMustBeGenerateGroupBy(queryDataSource);
        FetchAggregateAndFields(beGenerateGroupBy, queryDataSource, insertFields);
        var commandString = new CandoStringBuilder();
        var dbTableName = GetTableDbName(entity, queryDataSource.DataSrcDefinition.connection.DatabaseName);
        commandString += $@"INSERT INTO {dbTableName} ([";
        if (insertFields.Count > 0)
            commandString += string.Join("],[", insertFields);
        commandString += "])\r\n" + queryDataSource.GenerateQuery();
        return commandString.ToString();
    }

    private static string GetUniqueSqlVariableName(string name)
    {
        var guid = Guid.NewGuid().ToString("N");
        return $"@sjvs{name}{guid}";
    }
}
