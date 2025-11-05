using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;

namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    private string GenerateGroupDeleteScript()
    {
        var filter = GetFilterSql();
        var id = $"'{filter.Replace("'", "''")}'";
        return GenerateDeleteScript(id, "", DataSrcDefinition.Entity);
    }

    private string GenerateDeleteScript(string keyValue, string keyFilterValues, Entity entity)
    {
        var commands = new NeoStringBuilder();
        GenerateDeleteScript(keyValue, keyFilterValues, entity, commands);
        return commands.ToString();
    }

    private void GenerateDeleteScript(string keyValue, string keyFilterValues, Entity entity, NeoStringBuilder commands)
    {
        var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
        var command = $"DELETE{(TopRows > 0 ? $" TOP({TopRows})" : "")} {dbTableName} " +
                      GenerateDmlCommandWhereClause(keyFilterValues, entity);
        GenerateParentDeleteScript(entity, keyFilterValues, commands);
    }

    private void GenerateParentDeleteScript(Entity entity, string keyFilterValues,
        NeoStringBuilder commandString)
    {
        var parents = Parents(entity)?.ToList();
        if (parents == null || parents.Count == 0) return;

        new DatabaseDataReader(this, entity).FetchParentIds().ReadData(keyFilterValues);

        foreach (var referenceField in parents)
        {
            var parentEntityRelationship = referenceField.Relationship as ParentEntity;
            if (parentEntityRelationship?.Maps?.FirstOrDefault() == null)
                continue;

            Fields.TryGetValue(referenceField.ParentAssociationField?.ParentEntity.Maps.FirstOrDefault()?.SourceField ?? "", out var relationMapField);

            GenerateDeleteScript(GetSqlValueField(false, relationMapField, entity).SqlValue,
                FetchParentEntityKeyFilterValue(referenceField),
                referenceField.Relationship.DestEntity, commandString);
        }
    }
}
