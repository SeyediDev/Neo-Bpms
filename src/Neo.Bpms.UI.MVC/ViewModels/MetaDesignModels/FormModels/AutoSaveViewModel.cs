namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.FormModels;

public class AutoSaveViewModel
{
    public AutoSaveViewModel()
    {

    }

    public AutoSaveViewModel(AutoCalc autoCalc)
    {
        fieldId = autoCalc.FieldId;
        formula = autoCalc.Formula?.ExpressionString;
        condition = autoCalc.Condition?.ExpressionString;
    }
    public string fieldId { get; set; }
    public string formula { get; set; }
    public string condition { get; set; }

    public AutoCalc ToAutoCalc()
    {
        return new AutoCalc
        {
            FieldId = fieldId,
            Condition = Parser.ParseTree(condition),
            Formula = Parser.ParseTree(formula)
        };
    }
}
