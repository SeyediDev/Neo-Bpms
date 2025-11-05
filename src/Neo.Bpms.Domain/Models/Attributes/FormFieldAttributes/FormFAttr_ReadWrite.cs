namespace Neo.Bpms.Domain.Models.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field Read,Write,Mandatory attribute
/// 
/// This Attribute is for fields of view models and modifies the default behavior of the field (read only/ write only/mandatory)
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_ReadWrite : Attribute
{
    public bool ReadOnly { get; set; }
    public bool WriteOnly { get; set; }
    public bool Mandatory { get; set; }
    /// <summary>
    /// Gets or sets the error rext for mandatory fields if set to empty.
    /// </summary>
    /// <value>
    /// The error rext.
    /// </value>
    public string ErrorRext { get; set; }
    /// <summary>
    /// Gets or sets the english error rext for mandatory fields if set to empty.
    /// </summary>
    /// <value>
    /// The english error text.
    /// </value>
    public string EnErrorText { get; set; }
}
