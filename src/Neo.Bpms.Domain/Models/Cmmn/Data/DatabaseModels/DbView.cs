namespace Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

public class DbView(string schema, string name) : DbTable(schema, name)
{
    public string ViewDefinition { get; set; }
}