using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Controller;

public class ControllerTaskTypeSpecificPropertiesViewModel
{
    public eControlPropertyId? propertyType { get; set; }
    public string calcLocation { get; set; }
    public string formula { get; set; }
    public string target { get; set; }
    public IEnumerable<string> targets { get; set; }
    public int localParamId { get; set; }
}