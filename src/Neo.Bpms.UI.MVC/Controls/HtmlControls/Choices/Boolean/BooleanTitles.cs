using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices.Boolean;

public class BooleanTitles(InputFieldDefinition field)
{
    public InputFieldDefinition Field { get; set; } = field;

    public string TrueProperty => Field.PropertyValue(eControlPropertyId.TrueTitle);

    public string FalseProperty => Field.PropertyValue(eControlPropertyId.FalseTitle);

    public string AllProperty => Field.PropertyValue(eControlPropertyId.AllTitle);

    public string NullProperty => Field.PropertyValue(eControlPropertyId.NullTitle);
    public string True => TrueProperty ?? ViewTexts.TrueTitle;

    public string False => Field.PropertyValue(eControlPropertyId.FalseTitle) ?? ViewTexts.FalseTitle;

    public string All => Field.PropertyValue(eControlPropertyId.AllTitle) ?? ViewTexts.AllTitle;

    public string Null => Field.PropertyValue(eControlPropertyId.NullTitle);
}
