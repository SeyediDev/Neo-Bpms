namespace Neo.Bpms.UI.MVC.ViewModels.Forms;

public class LcBindingModel
{
    public string Field { get; set; }
    public string Value { get; set; }
    public LCField ToLcField() => new() { Field = Field, Value = Value };
}
