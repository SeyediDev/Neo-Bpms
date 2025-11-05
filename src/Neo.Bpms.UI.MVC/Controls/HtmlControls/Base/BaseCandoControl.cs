namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Base;

public abstract class BaseNeoControl
{
    protected CommonProperties CommonProperties { get; set; }
    protected InputFieldDefinition Field { get; set; }
    protected ControlsRendererData ControlsRendererData { get; set; }
    protected string ControlsClassString => ControlsRendererData.ControlsClassString;
    protected CommonFormStructure Structure => ControlsRendererData.Structure;
    protected bool IsInReportPage => ControlsRendererData.Structure.FormType==Form.eFormType.Report;

    protected BaseNeoControl(InputFieldDefinition field, ControlsRendererData controlsRendererData)
    {
        Field = field;
        ControlsRendererData = controlsRendererData;
        CommonProperties = new CommonProperties(field.GetProperties());
        if (controlsRendererData.Options.IsReadOnly ||
            controlsRendererData.IsBulk && !CommonProperties.IsRequired)
        {
            CommonProperties.IsReadOnly = true;
        }
    }
}
