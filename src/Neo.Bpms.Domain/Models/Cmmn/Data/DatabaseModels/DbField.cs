namespace Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;

public class DbField
{
    public string Name;
    public string Type;
    public int Len;
    public bool NotNull;
    public bool Deleted;

    public DefaultConstraint Default;

    public class DefaultConstraint
    {
        public string Name;
        public string Value;
    }

    public string Collation { get; set; } = "Persian_100_CI_AI";
    public bool IsIdentity { get; set; }

    public static bool IsForeignKey(EntityField pEntityFieldDef)
    {
        return pEntityFieldDef.IsForeignParam();
    }
}