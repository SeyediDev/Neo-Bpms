namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DbView(string schema, string name) : DbTable(schema, name)
{
    public string ViewDefinition { get; set; }
}