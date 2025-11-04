namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public partial class ControlsRenderer
{
    internal static IDictionary<string, IBusinessDefinedControl>
        BusinessDefinedControls
    { get; } = new Dictionary<string, IBusinessDefinedControl>();

    public static void RegisterControl(IBusinessDefinedControl control)
    {
        BusinessDefinedControls.Add(control.Id, control);
    }
}
