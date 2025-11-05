namespace Neo.Bpms.Domain.Models.Cmmn;

public class EnumerationItem : BaseModelClass
{
    public EnumerationItem(Enumeration enumeration, int id, string name, EntityStateCategory stateCategory)
        : base(enumeration, "" + id, name)
    {
        StateCategory = stateCategory;
    }

    public EnumerationItem()
    {
    }

    public Enumeration Enumeration => Parent as Enumeration;

    public EntityStateCategory StateCategory { get; set; }

    public string GetNameWithCulture(string culture)
    {
        return culture == "en" ? string.IsNullOrEmpty(EnName) ? Name : EnName : Name;
    }
    public string GetDescription(string culture)
    {
        return culture == "en" ? string.IsNullOrEmpty(EnDescription) ? Description : EnDescription : Description;
    }
}
