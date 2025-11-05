namespace Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

public class ConfiguredFilter : ConfiguredEntityItem
{
    public ConfiguredFilter() { }
    public ConfiguredFilter(long id, string name) : base(id, name) { }

    public List<ConfiguredFilterValue> Values { get; set; }
}
