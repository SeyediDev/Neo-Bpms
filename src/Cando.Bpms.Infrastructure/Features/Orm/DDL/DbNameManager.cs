using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Infrastructure.Features.Orm.DDL;

public static class DbNameManager
{
    public static string GetDbIndexName(EntityIndex index)
    {
        return "IX_" + EntityDbNameManager.ToPascalCase(!string.IsNullOrEmpty(index.Id) ? index.Id : index.EnName,
            false);
    }

    public static string GetDbForeignKeyIndexName(Association association)
    {
        return "IX_" + EntityDbNameManager.ToPascalCase(
            !string.IsNullOrEmpty(association.EnName)
                ? association.EnName + "Id"
                : association.Maps[0].SourceField,
            false);
    }

    public static string GetDbForeignKeyName(Entity entity, Association association)
    {
        return string.IsNullOrEmpty(association.DbConstraintNameMap)
            ? "FK_" + GetDbSchemaName(entity) + "." + EntityDbNameManager.GetDbTableName(entity) +
              "_" + GetDbSchemaName(association.Entity()) + "." +
              EntityDbNameManager.GetDbTableName(association.Entity()) +
              "_" + EntityDbNameManager.ToPascalCase(association.EnName, false) + "Id"
            : association.DbConstraintNameMap;
    }

    public static string GetDbSchemaName(Entity entity)
    {
        var schema = EntityDbNameManager.ToPascalCase(entity?.Schema ?? "dbo", false);
        if (schema == "Dbo")
            schema = "dbo";
        return schema;
    }

    public static string GetDbTableCollation(Entity entity)
    {
        var collation = entity.Collation;
        if (string.IsNullOrEmpty(collation))
            collation = ProjectDefinition.Project.DefaultCollation;
        return !string.IsNullOrEmpty(collation) ? collation : "Persian_100_CI_AI";
    }
}
