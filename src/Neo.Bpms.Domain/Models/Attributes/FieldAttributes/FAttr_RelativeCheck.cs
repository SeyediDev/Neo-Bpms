namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

/// <summary>
/// Relative Check, 
/// 
/// This attribute is only designed for attributes(fields) of entities.
/// Range Check is a field validaton attribute
/// 
/// This attribute is used for relative check validation.
/// In relative check validation, one attribute(field) can be check in comparriosion to the other attribute(field) of an entity.
/// </summary>
public class FAttr_RelativeCheck : FieldValidation
{
    public enum ComparisionType
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqual,
        GreaterThan,
        GreaterEqual,
        //	NotGreaterThan,
        //	NotLessThan,
        //	In,
        //	InstanceOf,
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="FAttr_RelativeCheck"/> class.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="comparision">The comparision.</param>
    public FAttr_RelativeCheck(string fieldName, ComparisionType comparision)
    {
        this.comparision = comparision;
        this.fieldName = fieldName;
    }
    public FAttr_RelativeCheck()
    {
    }
    public ComparisionType comparision { get; set; }
    string fieldName { get; set; }
    public bool isValid(object fieldValue, object relativeFieldValue)
    {
        switch (comparision)
        {
            case ComparisionType.Equal:
                return (dynamic)fieldValue == relativeFieldValue;
            case ComparisionType.NotEqual:
                return (dynamic)fieldValue != relativeFieldValue;
            case ComparisionType.LessThan:
                return (dynamic)fieldValue < relativeFieldValue;
            case ComparisionType.LessEqual:
                return (dynamic)fieldValue <= relativeFieldValue;
            case ComparisionType.GreaterThan:
                return (dynamic)fieldValue > relativeFieldValue;
            case ComparisionType.GreaterEqual:
                return (dynamic)fieldValue >= relativeFieldValue;
            default:
                break;
        }
        return true;
    }
}
