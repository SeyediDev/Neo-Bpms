namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class DestEntityViewModel
{
    public string destNamespaceId { get; set; }
    public string destEntityId { get; set; }
}

public class BaseExtensionViewModel : DestEntityViewModel
{
    public string booleanFieldIdInParentThatPresentMe { get; set; }
}
