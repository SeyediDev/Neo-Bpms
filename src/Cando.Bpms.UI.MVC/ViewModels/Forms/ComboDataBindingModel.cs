namespace Neo.Bpms.UI.MVC.ViewModels.Forms;

public class ComboDataBindingModel
{
    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public PageAddress.PageTypeEnum PageType { get; set; }
    public string FormId { get; set; }
    public string FieldId { get; set; }
    public bool IsMandatory { get; set; }
    public string Expression { get; set; }
    public string Filter { get; set; }
    public IList<QField> Qs { get; set; }
    public IList<LcBindingModel> LCs { get; set; }
    public int? Count { get; set; }
    public int Page { get; set; }
}
