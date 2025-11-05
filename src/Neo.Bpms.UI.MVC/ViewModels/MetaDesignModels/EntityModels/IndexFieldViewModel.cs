using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;

public class IndexFieldViewModel
{
    public IndexFieldViewModel()
    {

    }

    public IndexFieldViewModel(IndexField indexField)
    {
        fieldCode = indexField.FieldName;
        isAsc = !indexField.IsDescending; //todo client
        isInclude = indexField.IsIncluded;
    }
    public string fieldCode { get; set; }
    public bool isAsc { get; set; } //todo client
    public bool isInclude { get; set; }
    public IndexField ToIndexField()
    {
        return new IndexField
        {
            FieldName = fieldCode,
            IsDescending = !isAsc,
            IsIncluded = isInclude
        };
    }
}
