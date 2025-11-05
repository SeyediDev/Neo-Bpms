namespace Neo.Bpms.Domain.Models.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field UIBlock attribute
/// 
/// This Attribute is for fields of view models and determines that the field will be replaced with another form.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_UIBlock : Attribute
{
    public string FormId { get; set; }
}
