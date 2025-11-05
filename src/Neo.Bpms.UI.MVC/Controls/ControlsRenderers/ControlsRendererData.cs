namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public record ControlsRendererData(
    CommonFormStructure Structure, 
    ElasticObject Record , 
    RendererOptions Options , 
    IdentityUser User,
    IUrlHelper Url)
{
    public bool IsBulk => Structure.IsBulk || Options.IsBulk;
    public IncludeNeeds IncludeNeedingControls { get; } = new IncludeNeeds();

    internal string ModelAssetsRoot => "/Content/ModelAssets";
    internal string ControlsClassString { get; } = "neo-control ";
    internal static readonly HtmlEncoder Encoder = HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic);
    public void AddIncludeNeed(PluginInclude plugin)
    {
        IncludeNeedingControls.AddPlugin(plugin);
    }

    internal void AddIncludeNeed(IBusinessDefinedControl control)
    {
        IncludeNeedingControls.AddBusinessDefinedControl(control);
    }
}
