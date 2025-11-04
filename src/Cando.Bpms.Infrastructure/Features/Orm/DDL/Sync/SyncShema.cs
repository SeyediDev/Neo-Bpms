using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

namespace Neo.Bpms.Engine.DDL;

abstract partial class DDLManager
{
    private void ReadShemas()
    {
        _model.Schemas = [];
        var sql = DDLGenerator.ReadShemas();
        var dt = Select(sql, "10.1.1.6");
        if (dt != null)
        {
            foreach (var item in dt)
            {
                var dbSchema = new DbSchema
                {
                    Name = item.GetString("Name"),
                    Owner = item.GetString("Owner"),
                    DefaultCharacterSetCatalog = item.GetString("DefaultCharacterSetCatalog"),
                    DefaultCharacterSetSchema = item.GetString("DefaultCharacterSetSchema"),
                    DefaultCharacterSetName = item.GetString("DefaultCharacterSetName")
                };
                _model.Schemas.Add(dbSchema.Name, dbSchema);
            }
        }
    }
}
