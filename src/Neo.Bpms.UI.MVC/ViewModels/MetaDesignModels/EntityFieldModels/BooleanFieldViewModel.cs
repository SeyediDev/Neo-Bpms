namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class BooleanFieldViewModel
{
    public BooleanFieldViewModel()
    {

    }

    public BooleanFieldViewModel(BooleanEntityField booleanEntityField)
    {
        trueTitle = booleanEntityField?.TrueTitle;
        falseTitle = booleanEntityField?.FalseTitle;
        allTitle = booleanEntityField?.AllTitle;
        nullTitle = booleanEntityField?.NullTitle;
    }

    public string trueTitle { get; set; }
    public string falseTitle { get; set; }
    public string allTitle { get; set; }
    public string nullTitle { get; set; }

    public BooleanEntityField ToBoolean()
    {
        return new BooleanEntityField
        {
            TrueTitle = trueTitle,
            FalseTitle = falseTitle,
            NullTitle = nullTitle,
            AllTitle = allTitle,
        };
    }
}
