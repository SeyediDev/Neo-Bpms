namespace Neo.Bpms.UI.MVC.Controls;

public class IncludeNeeds
{
    public ISet<PluginInclude> PluginIncludes { get; private set; }
    public ISet<IBusinessDefinedControl> BusinessDefinedControls { get; private set; }

    public void AddPlugin(PluginInclude plugin)
    {
        PluginIncludes ??= new HashSet<PluginInclude>();
        PluginIncludes.Add(plugin);
    }
    public void AddBusinessDefinedControl(IBusinessDefinedControl control)
    {
        BusinessDefinedControls ??= new HashSet<IBusinessDefinedControl>();
        BusinessDefinedControls.Add(control);
    }
}