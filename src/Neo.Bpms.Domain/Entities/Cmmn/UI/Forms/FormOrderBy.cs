namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

public class FormOrderBy : BaseModelClass
{
    public string FieldId { get; set; }
    public SortType SortType { get; set; }
    public bool ById { get; set; }
}