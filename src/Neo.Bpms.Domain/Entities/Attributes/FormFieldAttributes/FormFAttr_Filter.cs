namespace Neo.Bpms.Domain.Entities.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field Filter attribute
/// 
/// This Attribute is for fields of view models for Index forms and determines that the field can be used to filter records.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_Filter : Attribute
{
}
