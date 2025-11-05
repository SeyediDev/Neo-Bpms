namespace Neo.Bpms.Domain.Models.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field IsDescriptionFor attribute
/// 
/// This Attribute is for fields of view models and determines that the field value is a description for othe fields and must be displayed as a tooltip or icon.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FormFAttr_IsDescriptionFor"/> class.
/// </remarks>
/// <param name="ForField">For field.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_IsDescriptionFor(string ForField) : Attribute
{
    public string ForField { get; set; } = ForField;
    /// <summary>
    /// Gets or sets a value indicating whether it is a description tooltip or describing icon.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the value determines a describing icon; otherwise(the value is a description tooltip), <c>false</c>.
    /// </value>
    public bool DisplayAsIcon { get; set; }
}
