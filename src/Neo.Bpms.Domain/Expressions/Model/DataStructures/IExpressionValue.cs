namespace Neo.Bpms.Domain.Expressions.Model.DataStructures;

public interface IExpressionValue
{
    object InternalValue { get; set; }
    bool GetField(string field, out object retVal);
    bool SetField(string field, object value);
}