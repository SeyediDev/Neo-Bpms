using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;

namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Controller;

public class ControllerViewModel
{
    public ControllerViewModel()
    {

    }
    public ControllerViewModel(UIRule uiRule)
    {
        id = uiRule.Id;
        name = uiRule.Name;
        events = uiRule.Events?.Select(e => new ControllerEventViewModel(e)) ??
            [];
        tasks = uiRule.Operations?.Select(o => new ControllerTaskViewModel(o)) ??
            [];
    }
    public string id { get; set; }
    public string name { get; set; }
    public IEnumerable<ControllerEventViewModel> events { get; set; }
    public IEnumerable<ControllerTaskViewModel> tasks { get; set; }

    public UIRule ToUiRule()
    {//todo
        UIRule uiRule = new(null, id, name);
        uiRule.Events = events?.Select(e => e.ToControllerEvent()).ToList();
        uiRule.Operations = tasks?.Select(t => t.ToOperation()).ToList();
        return uiRule;
    }
}