namespace Neo.Bpms.Domain.Entities.Cmmn;

[View(
    @"SELECT DISTINCT TABLE_NAME Name, TABLE_SCHEMA SchemaName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'",
    true)]
public class DatabaseTables
{
    public string Name;
    public string SchemaName;
}
