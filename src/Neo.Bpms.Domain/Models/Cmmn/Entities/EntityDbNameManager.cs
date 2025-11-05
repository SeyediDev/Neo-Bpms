using Neo.Common.Utility;

namespace Neo.Bpms.Domain.Models.Cmmn.Entities;

public static class EntityDbNameManager
{
    public static string GetSchema(Entity entity)
    {
        string schema = ToPascalCase(entity?.Schema ?? "dbo", false);
        if (schema == "Dbo")
            schema = "dbo";
        return schema;
    }
    public static string GetTableDbFullName(Entity entity)
    {
        return $"[{GetSchema(entity)}].[{GetDbTableName(entity)}]";
    }
    public static string GetTableDbDeletedFullName(Entity entity)
    {
        return $"[{GetSchema(entity)}].[del_{GetDbTableName(entity)}]";
    }
    public static string GetTableDbDeletedName(Entity entity)
    {
        return $"del_{GetDbTableName(entity)}";
    }
    public static string GetTableOldDbFullName(Entity entity)
    {
        return $"[{GetSchema(entity)}].[{entity.OldDbTableNameMap}]";
    }
    public static string GetDbTableName(Entity entity)
    {
        if (entity == null)
            return "undefined";
        FetchDbTableNameInEntityOrBase(entity, out string dbTableName);
        return dbTableName;
    }

    private static void FetchDbTableNameInEntityOrBase(Entity entity, out string dbTableName)
    {
        while (true)
        {
            if (entity.BaseExtension != null)
            {
                entity = entity.BaseExtension.DestEntity;
                continue;
            }
            if (string.IsNullOrEmpty(entity.DbTableNameMap))
            {
                entity.DbTableNameMap = ToPascalCase(PluralEntityId(entity), false);
            }
            dbTableName = entity.DbTableNameMap;
            break;
        }

        static string PluralEntityId(Entity entity)
        {
            return entity.Id.EndsWith("Map") ? entity.Id : new PluralNoun().Plural(entity.Id);
        }
    }

    public static string ToPascalCase(string name, bool lower)
    {
        return name.ToPascalCase(lower);
    }
}
