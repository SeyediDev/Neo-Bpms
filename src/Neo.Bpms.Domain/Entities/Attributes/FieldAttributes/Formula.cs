namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes;

/// <summary>
/// تعریف فیلد فرمولی
/// </summary>
public class Formula : FAttr_IsFormula
{
    /// <summary>
    /// تعریف فیلد فرمولی
    /// </summary>
    /// <param name="formula">فرمول</param>
    public Formula(string formula = null)
    {
        Formula = formula;
    }
}