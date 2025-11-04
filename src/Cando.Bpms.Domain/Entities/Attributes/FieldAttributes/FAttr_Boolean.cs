namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes;

/// <summary>
/// تعریف عنوان برای حالت های مختلف فیلد از نوع بولین
/// </summary>
/// <remarks>
/// تعریف عنوان برای حالت های مختلف فیلد از نوع بولین
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class FAttr_Boolean(string trueTitle, string falseTitle, string allTitle, string nullTitle = null) : Attribute
{
    public readonly string TrueTitle = trueTitle;
    public readonly string FalseTitle = falseTitle;
    public readonly string AllTitle = allTitle;
    public readonly string NullTitle = nullTitle ?? falseTitle;
}
