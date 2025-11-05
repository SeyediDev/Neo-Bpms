namespace Neo.Bpms.Domain.Expressions.FunctionImplementations;

public class ControlFunctionsImplementation : IFunctionImplementations
{
    public static bool IsNullOrEmpty(object expression)
    {
        while (true)
        {
            switch (expression)
            {
                case null:
                    return true;
                case string s:
                    return string.IsNullOrEmpty(s) || s == "undefined" || s == "{null}";
                case long _:
                    return ToInt(expression) == 0;
                case double _:
                    return Math.Abs(ToDouble(expression)) < .00000000001;
                case byte[] bytes:
                    return !bytes.Any();
                case IEnumerable<object> objects:
                    return !objects.Any();
                case IExpressionValue value:
                    expression = value.InternalValue;
                    continue;
                default:
                    return false;
            }
        }
    }

    public static object IsNull(object expression, object defaultValue)
    {
        return expression ?? defaultValue;
    }

    public static long ToInt(object v)
    {
        try
        {
            return Convert.ToInt64(v);
        }
        catch
        {
            return 0;
        }
    }

    public static double ToDouble(object v)
    {
        try
        {
            return Convert.ToDouble(v);
        }
        catch
        {
            return 0;
        }
    }

    public static double Plus(object i1, object i2) => ToDouble(i1) + ToDouble(i2);

    public static bool ToBoolean(object obj)
    {
        return ConvUtill.ToBoolean(obj);
    }
}