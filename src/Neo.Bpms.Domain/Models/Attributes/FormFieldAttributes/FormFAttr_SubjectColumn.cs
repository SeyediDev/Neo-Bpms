namespace Neo.Bpms.Domain.Models.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field Subject Column attribute
/// 
/// This Attribute is for fields of view models for Index forms and determines that the field can be used for index columns and it has link to subjective edit and details forms.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_SubjectColumn : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormFAttr_SubjectColumn"/> class.
    /// </summary>
    public FormFAttr_SubjectColumn()
    {
        HasDetails = true;
        HasEdit = true;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the column has edit icon.
    /// </summary>
    /// <value>
    ///   <c>true</c> if has edit icon; otherwise, <c>false</c>.
    /// </value>
    public bool HasEdit { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the column has details icon.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the column has details icon; otherwise, <c>false</c>.
    /// </value>
    public bool HasDetails { get; set; }
    /// <summary>
    /// Gets or sets the subject identifier. if you set subject id to null, it means that the subject id is equal  to the name of field.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    public string FormSubjectId { get; set; }
}
