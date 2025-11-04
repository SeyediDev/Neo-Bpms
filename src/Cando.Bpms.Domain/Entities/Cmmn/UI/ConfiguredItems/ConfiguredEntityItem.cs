namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

public abstract class ConfiguredEntityItem : ConfiguredItem
{
    public ConfiguredEntityItem() { }
    public ConfiguredEntityItem(long id, string name)
    {
        Id = id;
        Name = name;
    }
    public string ConfigId { get; set; }
    public EntityItem EntityItem { get; set; }
}
