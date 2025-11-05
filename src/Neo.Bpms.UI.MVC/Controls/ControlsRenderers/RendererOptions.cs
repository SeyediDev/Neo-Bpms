namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public class RendererOptions
{
    public bool IsReadOnly { get; set; } = false;
    public bool IsFilter { get; set; } = false;
    public bool IsDesignMode { get; set; } = false;
    public bool IsInToolBox { get; set; } = false;
    public string CalendarType { get; set; } = ProjectDefinition.Project.DefaultCalendar;
    public bool IsBulk { get; set; } = false;
    public bool IsIframe { get; set; } = false;
    public bool ShowFormLinkIconForChoices { get; set; } = true;
}
