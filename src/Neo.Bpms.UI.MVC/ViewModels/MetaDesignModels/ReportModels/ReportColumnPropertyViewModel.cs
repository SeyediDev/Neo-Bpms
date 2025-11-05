using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class ReportColumnPropertyViewModel
{
    public ReportColumnPropertyViewModel()
    {
    }
    public ReportColumnPropertyViewModel(FormProperty formProperty)
    {
        id = (int)formProperty.PropertyId;
        value = formProperty.value?.ToString();
    }

    public int id { get; set; }
    public string value { get; set; }

    public FormProperty ToFieldPropery()
    {
        return new FormProperty
        {
            PropertyId = (eControlPropertyId)id,
            Value = value
        };
    }
}
