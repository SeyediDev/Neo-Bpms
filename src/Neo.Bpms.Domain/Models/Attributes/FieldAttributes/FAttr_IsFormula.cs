namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

/// <summary>
/// Is Formula Attribute
/// 
/// This attribute is for formula fields
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FAttr_IsFormula : Attribute
{
    /// <summary>
    /// تعریف فیلد فرمولی
    /// </summary>
    public FAttr_IsFormula()
    {

    }
    /// <summary>
    /// تعریف فیلد فرمولی
    /// </summary>
    /// <param name="formula">فرمول</param>
    public FAttr_IsFormula(string formula)
    {
        Formula = formula;
    }
    public string FormulaId { get; set; }
    public string Formula { get; set; }
    public bool UsedForAggregationOnly { get; set; }
}
