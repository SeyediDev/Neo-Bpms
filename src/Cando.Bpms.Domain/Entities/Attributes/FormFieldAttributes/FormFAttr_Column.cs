namespace Neo.Bpms.Domain.Entities.Attributes.FormFieldAttributes;

/// <summary>
/// Form Field Column attribute
/// 
/// This Attribute is for fields of view models for Index forms and determines that the field can be used for index columns.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FormFAttr_Column : Attribute
{
}
