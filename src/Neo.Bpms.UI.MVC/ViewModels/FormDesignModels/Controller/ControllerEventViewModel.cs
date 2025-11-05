namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Controller;

public class ControllerEventViewModel
{
    public ControllerEventViewModel()
    {

    }
    public ControllerEventViewModel(UIRuleEvent @event)
    {
        type = @event.type;
        controlId = @event.exporterFieldId;
    }

    public UIRuleEvent.eEventType? type { get; set; }
    public string controlId { get; set; }

    public UIRuleEvent ToControllerEvent()
    {
        return new UIRuleEvent
        {
            exporterFieldId = controlId,
            type = type ?? UIRuleEvent.eEventType.invalid
        };
    }
}