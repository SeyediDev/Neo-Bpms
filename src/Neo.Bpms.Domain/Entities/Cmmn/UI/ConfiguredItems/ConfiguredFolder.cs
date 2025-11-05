namespace Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems;

public class ConfiguredFolder : ConfiguredEntityItem
{
    public ConfiguredFolder() { }
    public ConfiguredFolder(long id, string name):base(id, name) { }
    
    public bool IsForConfig { get; set; }
}
