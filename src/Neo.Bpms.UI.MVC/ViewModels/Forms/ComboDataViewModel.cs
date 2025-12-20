namespace Neo.Bpms.UI.MVC.ViewModels.Forms;

public class ComboDataViewModel(ComboData comboData)
{
    private readonly ComboData _comboData = comboData;

    public bool HasMore => _comboData.HasMore;
    public List<FormDataRow> Rows => _comboData.Rows;
}
