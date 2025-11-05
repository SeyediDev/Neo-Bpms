namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ExpressionNodeValue : IComparable<ExpressionNodeValue>
{
    public ExpressionNodeValue(object originalValue)
    {
        if (originalValue is IExpressionValue value)
            originalValue = value.InternalValue;
        OriginalValue = originalValue;
        ConvertedValue = OriginalValue;

        switch (originalValue)
        {
            case null:
                ValueType = ValueTypes.Null;
                return;
            case DateTime _:
                ValueType = ValueTypes.DateTime;
                return;
            case string s:
                {
                    if (bool.TryParse(s, out bool b))
                    {
                        ConvertedValue = b;
                        ValueType = ValueTypes.Boolean;
                        return;
                    }

                    if (double.TryParse(s.Replace("/", "."), out double d))
                    {
                        ConvertedValue = d;
                        ValueType = ValueTypes.Number;
                        return;
                    }

                    if (s.Equals(string.Empty) ||
                        s.Equals("undefined"))
                    {
                        ValueType = ValueTypes.Null;
                        ConvertedValue = null;
                        return;
                    }

                    ValueType = ValueTypes.String;
                    return;
                }

            case bool _:
                ValueType = ValueTypes.Boolean;
                return;
        }

        if (originalValue is double || originalValue is int || originalValue is long || originalValue.GetType().IsEnum)
        {
            ConvertedValue = Convert.ToDouble(originalValue);
            ValueType = ValueTypes.Number;
            return;
        }

        ValueType = ValueTypes.Other;
    }

    public object OriginalValue { get; }
    public ValueTypes ValueType { get; }
    public object ConvertedValue { get; }

    public enum ValueTypes
    {
        Null,
        Number,
        Boolean,
        String,
        DateTime,
        Other // todo Always ArgumentOutOfRangeException exception without logging? 
    }


    protected bool Equals(ExpressionNodeValue other)
    {
        return ValueType == other.ValueType
            ? ValueType == ValueTypes.Null ? true : ConvertedValue.Equals(other.ConvertedValue)
            : ValueType switch
        {
            ValueTypes.Null => other.ValueType switch
            {
                ValueTypes.Number => Math.Abs((double)other.ConvertedValue) < .00000000001,
                ValueTypes.Boolean => (bool)other.ConvertedValue == false,
                ValueTypes.String or ValueTypes.DateTime => false,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.Number => other.ValueType switch
            {
                ValueTypes.Null => Math.Abs((double)ConvertedValue) < .00000000001,
                ValueTypes.Boolean => Math.Abs((double)ConvertedValue) < .00000000001 == !(bool)other.ConvertedValue,
                ValueTypes.String or ValueTypes.DateTime => false,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.Boolean => other.ValueType switch
            {
                ValueTypes.Null => (bool)ConvertedValue == false,
                ValueTypes.Number => Math.Abs((double)other.ConvertedValue) < .000001 == !(bool)ConvertedValue,
                ValueTypes.String or ValueTypes.DateTime => false,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.String => other.ValueType switch
            {
                ValueTypes.Null or ValueTypes.DateTime or ValueTypes.Number or ValueTypes.Boolean => false,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.DateTime => other.ValueType switch
            {
                ValueTypes.Null or ValueTypes.Number or ValueTypes.Boolean or ValueTypes.String => false,
                _ => throw new ArgumentOutOfRangeException(),
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(null, obj)
            ? false
            : ReferenceEquals(this, obj) ? true : obj.GetType() == typeof(ExpressionNodeValue) && Equals((ExpressionNodeValue)obj);
    }

    public override int GetHashCode()
    {
        return ConvertedValue != null ? ConvertedValue.GetHashCode() : 0;
    }

    public static bool operator ==(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return !Equals(left, right);
    }

    public int CompareTo(ExpressionNodeValue other)
    {
        return ReferenceEquals(this, other)
            ? 0
            : ReferenceEquals(null, other)
            ? 1
            : Equals(other)
            ? 0
            : ValueType switch
        {
            ValueTypes.Null => other.ValueType switch
            {
                ValueTypes.Null => 0,
                ValueTypes.Number or ValueTypes.Boolean or ValueTypes.String or ValueTypes.DateTime => -1,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.Number => other.ValueType switch
            {
                ValueTypes.Null => 1,
                ValueTypes.Number => ((double)ConvertedValue).CompareTo((double)other.ConvertedValue),
                ValueTypes.Boolean => ((double)ConvertedValue).CompareTo(ConvertBoolToNumber(other.ConvertedValue)),
                ValueTypes.String => string.Compare(ConvertedValue.ToString(), other.ConvertedValue.ToString(),
                                                StringComparison.Ordinal),
                ValueTypes.DateTime => 0,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.Boolean => other.ValueType switch
            {
                ValueTypes.Null => 1,
                ValueTypes.Number => ConvertBoolToNumber(ConvertedValue).CompareTo((double)other.ConvertedValue),
                ValueTypes.Boolean => ((bool)ConvertedValue).CompareTo((bool)other.ConvertedValue),
                ValueTypes.String => -1,
                ValueTypes.DateTime => 0,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.String => other.ValueType switch
            {
                ValueTypes.Null => 1,
                ValueTypes.Number => string.Compare(ConvertedValue.ToString(), other.ConvertedValue.ToString(),
                                                StringComparison.Ordinal),
                ValueTypes.Boolean => 1,
                ValueTypes.String => string.Compare(ConvertedValue.ToString(), other.ConvertedValue.ToString(),
                                                StringComparison.Ordinal),
                ValueTypes.DateTime => 0,
                _ => throw new ArgumentOutOfRangeException(),
            },
            ValueTypes.DateTime => other.ValueType switch
            {
                ValueTypes.Null or ValueTypes.Number or ValueTypes.Boolean or ValueTypes.String => 0,
                ValueTypes.DateTime => DateTime.Compare((DateTime)ConvertedValue, (DateTime)other.ConvertedValue),
                _ => throw new ArgumentOutOfRangeException(),
            },
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private static double ConvertBoolToNumber(object booleanValue)
    {
        return (bool)booleanValue ? 1 : 0;
    }

    public static bool operator <(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(ExpressionNodeValue left, ExpressionNodeValue right)
    {
        return left.CompareTo(right) >= 0;
    }
}
