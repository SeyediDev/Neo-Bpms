using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Infrastructure.Features.Orm.DDL;

public sealed class SqlServerDDLManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters)
    : SqlDDLManager(provider, configuration, connectionParameters)
{
    protected override bool IsProviderSupportIdentity()
    {
        return true;
    }

    protected override void NormalizeForeignKey(ForeignKeyItem dbForeignKey)
    {
    }
    protected override string CreateForeignKeyCommand(DbTable dbTable, string fkName, string fkFields, DbTable pkTable, string pkFields,
        ForeignKeyRule ruleOnDelete, ForeignKeyRule ruleOnUpdate)
    {
        return base.CreateForeignKeyCommand(dbTable, fkName, fkFields, pkTable, pkFields, ruleOnDelete, ruleOnUpdate)
                 + " ON UPDATE " + AddRuleDdl(ruleOnUpdate);
    }
    protected override bool FieldIsIdentity(ElasticObject item)
    {
        return item.GetLong("IsIdentity") == 1;
    }
    protected override string AddIndexOptions()
    {
        return "WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ";
    }
    protected override bool CreateIndexCommand(Entity entity, EntityIndex entityIndex, ref string ddl, ref string fields)
    {
        var bFirst = true;

        foreach (var entityIndexField in entityIndex.Fields)
        {
            if (entityIndexField.IsIncluded) continue;
            var eField = entity.GetField(entityIndexField.FieldName);
            if (eField == null)
            {
                AddLog("10.4.1.3", Log.Warning,
                    $"Field {entityIndexField.FieldName} not defined in index {entityIndex.Name} on entity {entity.Id}");
                continue;
            }

            if (eField.AssociationEntity == null)
            {
                ddl += (bFirst ? "" : ",") + $"[{eField.DbFieldName}]" + (entityIndexField.IsDescending ? " Desc" : "");
                fields += (bFirst ? "" : ",") + $"[{eField.DbFieldName}]";
                bFirst = false;
            }
            else
            {
                foreach (var map in eField.AssociationEntity.Maps)
                {
                    var ef = entity.GetField(map.SourceField);
                    if (ef == null) continue; //error in definition
                    ddl += (bFirst ? "" : ",") + $"[{ef.DbFieldName}]" + (entityIndexField.IsDescending ? " Desc" : "");
                    fields += (bFirst ? "" : ",") + $"[{ef.DbFieldName}]";
                    bFirst = false;
                }
            }
        }

        if (bFirst) return true;
        var firstInclude = true;
        foreach (var eiParam in entityIndex.Fields)
        {
            if (!eiParam.IsIncluded) continue;
            var eField = entity.GetField(eiParam.FieldName);
            if (eField == null) continue; //error in definition
            if (eField.AssociationEntity == null)
            {
                ddl += firstInclude ? ") Include (" : ",";
                ddl += eField.DbFieldName + (eiParam.IsDescending ? " Desc" : "");
                firstInclude = false;
            }
            else
            {
                foreach (var map in eField.AssociationEntity.Maps)
                {
                    var ef = entity.GetField(map.SourceField);
                    if (ef == null) continue; //error in definition
                    ddl += firstInclude ? ") Include (" : ",";
                    ddl += ef.DbFieldName + (eiParam.IsDescending ? " Desc" : "");
                    firstInclude = false;
                }
            }
        }
        return false;
    }
}
