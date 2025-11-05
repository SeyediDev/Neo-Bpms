namespace Neo.Bpms.UI.MVC.ViewModels.Forms;

public class ComboInitValuesBindingModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string FormId { get; set; }
    public PageAddress.PageTypeEnum PageType { get; set; }
    public string FieldId { get; set; }
    public string InitValues { get; set; }
    public IList<QField> Qs { get; set; }
    public string Filter { get; set; }
}
