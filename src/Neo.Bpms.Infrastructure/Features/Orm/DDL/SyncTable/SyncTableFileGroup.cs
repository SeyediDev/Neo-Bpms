global using Neo.Bpms.Infrastructure.Features.Orm.DDL;
using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;
public abstract partial class DDLManager
{
    private void SyncTableFileGroup(Entity entity, DbTable dbTable)
    {
        if (dbTable.FileGroup != entity.FileGroup && !entity.DerivedEntitiesExtendedMe)
            ChangeTableFileGroup(entity, dbTable);
    }

    private void ChangeTableFileGroup(Entity entity, DbTable dbTable)
    {
        var pkIndex = dbTable.PkIndex;
        var pkConstraintName = pkIndex?.Name ?? GetPkConstraintName(entity, dbTable);
        var indexType = pkIndex?.Type ?? DbIndex.IndexType.NonClustered;
        //if (indexType == DbIndex.IndexType.Clustered)
        //	foreach (var item in MappedEntityFields(entity))
        //		if (item.fieldType == TVariableTypes.varString && item.MaxLen <= 0)
        //			indexType = DbIndex.IndexType.NonClustered;
        var clusteredType = $"{(indexType == DbIndex.IndexType.NonClustered ? "NON" : "")}CLUSTERED";
        var keys = FetchKeys(entity);
        if (string.IsNullOrEmpty(keys))
        {
            AddLog("44.3.0.1", Log.Error, $"Entity {entity.Id} {entity.Name} do not have any primary key");
            return;
        }
        if (pkIndex == null)
        {
            var ddl =
                $"ALTER TABLE {dbTable.FullName} ADD PRIMARY KEY {clusteredType} ({keys}) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [{entity.FileGroup}]";
            AddToCommandList(entity, ddl, false);
            DoSqlCommand(entity, ddl, "", "44.3.2.1");
        }
        else
        {
            var ddl =
                $"CREATE UNIQUE {clusteredType} INDEX [{pkConstraintName}] ON [{DbNameManager.GetDbSchemaName(entity)}].[{dbTable.Name}]({keys}) WITH(DROP_EXISTING = ON, ONLINE = OFF) ON [{entity.FileGroup}]";
            AddToCommandList(entity, ddl, false);
            if (!DoSqlCommand(entity, ddl, "", "44.3.0"))
            {
                ddl =
                    $"CREATE UNIQUE {clusteredType} INDEX [{pkConstraintName}] ON [{DbNameManager.GetDbSchemaName(entity)}].[{dbTable.Name}]({keys}) WITH(DROP_EXISTING = ON, ONLINE = ON) ON [{entity.FileGroup}]";
                AddToCommandList(entity, ddl, false);
                DoSqlCommand(entity, ddl, "", "44.3.1");
            }
        }

        dbTable.FileGroup = entity.FileGroup;
    }
}
