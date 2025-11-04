namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public abstract class ExpressionNode(ExpressionNode.eNodeType nodeType)
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

    public enum eNodeType
    {
        Block,
        FunctionDefinition,
        ForExpression,
        IfExpression,
        Switch,

        BlockControl,

        LiteralConstant,
        Variable,

        Convert,
        Unary,
        UnaryTest,
        Interval,
        ArithmeticExpressions,
        ComparisionExpression,
        LogicalExpressions,
        QuantifiedExpression,
        AssignmentOperator,

        PathExpression,
        FilterExpression,

        FunctionInvocation,
        //FunctionNamedParameter,
        //FunctionPositionalParameter,

        List,

        Dictionary
        //			List,
        //			Context,
        //			ContextEntry,
    }

    public enum eControlType
    {
        Normal,
        Break,
        Return,
        GoTo,
        Throw,
        Continue,
        RunTimeError
    }

    public readonly eNodeType NodeType = nodeType;
    protected int Depth;

    protected abstract int getPriority();
    public abstract eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue);
    public abstract ExpressionNode replace(LocalParameters localVariables);

    public virtual ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        return func(this, param);
    }

    public object Eval(object obj, LocalParameters paramValues, EvalOptions evalOptions = null)
    {
        evalOptions ??= EvalOptions.Default;
        LocalParameters localParams = paramValues;
        eval(obj, ref localParams, evalOptions, out object o);
        return o;
    }

    public static object GetPropertyValue(object obj, string name)
    {
        Type type = obj?.GetType() ?? typeof(object);
        if (type.IsClass)
        {
            string name1 = name.Replace("[", "").Replace("]", "");
            if (name == "this" || type.Name == name1)
                return obj;
            if (obj is IExpressionValue el)
            {
                if (el.GetField(name, out object outVal))
                    return outVal;
            }

            if (ReflectionField.GetValue(obj, type, name1, out object value))
                return value;
        }

        if (type.IsEnum)
        {
            string[] names = type.GetEnumNames();
            Array values = type.GetEnumValues();
            int i = 0;
            foreach (object v in values)
            {
                if (names[i] == name)
                {
                    return v;
                }

                i++;
            }
        }

        return "undefined";
    }


    public static void SetPropertyValue(object obj, string name, object value)
    {
        try
        {
            Type type = obj.GetType();
            if (!type.IsClass) return;
            ConvertType.SetValue(obj, name, value);
        }
        catch //(Exception e)
        {
            // ignored
        }
    }

    public static bool CheckIfTrue(object obj)
    {
        if (obj == null) return false;
        Type type = obj.GetType();
        return type.IsClass ? true : ToBoolean(obj);
    }

    public static bool ToBoolean(object value)
    {
        try
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return false;
            return value.ToString() == "1" || value.ToString().ToLower() == "true"
                ? true
                : value.ToString() == "2" || value.ToString().ToLower() == "false" ? false : value is int i ? i != 0 : Convert.ToBoolean(value);
        }
        catch (Exception e)
        {
            Logger.LogCritical(e, value?.ToString() ?? "null");
            return false;
        }
    }

    public abstract ExpressionNode clone();
    public abstract string toText();

    internal virtual eControlType setVal(object obj, object value, EvalOptions evalOptions)
    {
        return eControlType.Normal;
    }

    public void FetchNodes<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        if (GetType() == typeof(T))
        {
            string key = GetNodeKey() ?? GetType().Name;
            if (!dic.ContainsKey(key))
                dic.Add(key, []);
            dic[key].Add(this);
        }
        else
            FetchNodesInChildren<T>(dic);
    }

    public Dictionary<string, List<ExpressionNode>> FetchNodes<T>()
    {
        Dictionary<string, List<ExpressionNode>> dic = [];
        FetchNodes<T>(dic);
        return dic;
    }

    protected virtual string GetNodeKey()
    {
        return GetType().Name;
    }

    protected abstract void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic);
}

public class EvalOptions
{
    public bool ExcludeFunctionExpressions { get; set; }
    public bool ExcludePathExpressions { get; set; }
    public static EvalOptions Default = new()
    {
        ExcludePathExpressions = false,
        ExcludeFunctionExpressions = false
    };
}
