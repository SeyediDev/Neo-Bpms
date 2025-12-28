using System.Security.AccessControl;

namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

/// <summary>
/// تعریف فیلد فرمولی
/// </summary>
public class FormulaAttribute : FAttr_IsFormulaAttribute
{
    /// <summary>
    /// تعریف فیلد فرمولی
    /// </summary>
    /// <param name="formula">فرمول</param>
    public FormulaAttribute(string formula)
    {
        Formula = formula;
    }
    public FormulaAttribute(string format, params object []objects)
    {
        Formula = string.Format(format, objects);
    }
}
