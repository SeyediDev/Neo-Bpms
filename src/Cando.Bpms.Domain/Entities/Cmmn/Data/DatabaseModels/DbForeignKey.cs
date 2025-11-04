namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DbForeignKey
{
    public string Name;
    public ForeignKeyRule DeleteRule { get; set; }
    public ForeignKeyRule UpdateRule;
    public DbTable BaceTable;
    public DbTable ForeignTable;
    public List<DbColumn> Columns;

    public DbColumn AddColumn(string foreignKeyColumn, string baseKeyColumn)
    {
        Columns ??= [];
        DbColumn column = new()
        {
            ForeignKeyColumn = foreignKeyColumn,
            BaseKeyColumn = baseKeyColumn
        };
        Columns.Add(column);
        return column;
    }

    public static ForeignKeyRule GetForeignKeyRule(string rule)
    {
        rule = rule.ToUpper();
        return rule switch
        {
            "CASCADE" => ForeignKeyRule.Cascade,
            "SET NULL" => ForeignKeyRule.SetNull,
            "SET DEFAULT" => ForeignKeyRule.SetDefault,
            _ => ForeignKeyRule.NoAction,
        };
    }

    public class DbColumn
    {
        public string ForeignKeyColumn;
        public string BaseKeyColumn;
    }
}

public enum ForeignKeyRule
{
    NoAction = 1,
    Cascade = 2,
    SetNull = 3,
    SetDefault = 4
}

public enum FieldRelationType
{
    NoAction = 1,
    Set = 2,
    SetDefault = 3
}