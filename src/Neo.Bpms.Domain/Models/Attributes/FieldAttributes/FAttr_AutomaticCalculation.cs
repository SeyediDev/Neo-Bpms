namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;


/// <summary>
/// Automatic Calculation
/// 
/// A field can have auto calc attribute
/// </summary>
/// <remarks>
/// مشخص کردن مقدار فیلد بصورت فرمول
/// </remarks>
/// <param name="formula">فرمول</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FAttr_AutomaticCalculation(string formula) : Attribute
{

    /// <summary>
    /// Gets or sets a value indicating whether the field must be calced only if it is null.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the field must be calced only if it is null; otherwise, <c>false</c>.
    /// </value>
    public bool IfNull { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field must be recalced on any change.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the field must be recalced on any change; otherwise, <c>false</c>.
    /// </value>
    public bool RecalcOnAnyChange { get; set; }

    public string Formula { get; set; } = formula;
    public string Condition { get; set; } = null;
    public object Location { get; set; } = 0;
    public eGenerationType GenerationType { get; set; } = eGenerationType.IfNull;
}
