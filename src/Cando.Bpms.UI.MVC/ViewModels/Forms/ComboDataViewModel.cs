namespace Neo.Bpms.UI.MVC.ViewModels.Forms;

public class ComboDataViewModel
{
    private readonly ComboData _comboData;
    public ComboDataViewModel(ComboData comboData)
    {
        _comboData = comboData;
    }
    public bool HasMore => _comboData.HasMore;
    public List<FormDataRow> Rows => _comboData.Rows;
}
