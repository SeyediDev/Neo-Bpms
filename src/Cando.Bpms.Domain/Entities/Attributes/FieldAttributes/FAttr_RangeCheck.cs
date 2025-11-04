namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes;

/// <summary>
/// Range Check
/// 
/// This attribute is only designed for attributes(fields) of entities.
/// Range Check is a field validaton attribute
/// 
/// </summary>
public class FAttr_RangeCheck : FieldValidation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FAttr_RangeCheck"/> class.
    /// </summary>
    /// <param name="Min">The minimum.</param>
    /// <param name="Max">The maximum.</param>
    public FAttr_RangeCheck(object Min, object Max)
    {
        MinValue = Min;
        MaxValue = Max;
        MinCheckIsClosed = true;
        MaxCheckIsClosed = true;
    }
    public FAttr_RangeCheck()
    {
    }
    public bool MinCheckIsClosed { get; set; }
    public bool MaxCheckIsClosed { get; set; }
    public object MinValue { get; set; }
    public object MaxValue { get; set; }
}
