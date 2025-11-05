namespace Neo.Bpms.Domain.Models.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field SubTable attribute
/// 
/// This Attribute is for fields of view models for Edit, Details, Delete forms to add tables to the form.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_SubTable(string EntityId, string Association = null) : Attribute
{
    /// <summary>
    /// Gets or sets the subject identifier. if you set subject id to null, it means that the subject id is equal  to the name of field.
    /// </summary>
    /// <value>
    /// The subject identifier.
    /// </value>
    public string IndexFormSubjectId { get; set; }
    public string EntityId { get; set; } = EntityId;
    /// <summary>
    /// Gets or sets the association name. if not set, the first association to this entity will be selected.
    /// </summary>
    /// <value>
    /// The association.
    /// </value>
    public string Association { get; set; } = Association;
}
