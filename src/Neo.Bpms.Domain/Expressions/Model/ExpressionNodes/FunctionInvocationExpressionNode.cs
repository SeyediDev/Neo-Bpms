namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class FunctionInvocationExpressionNode : ExpressionNode
{
    public List<ExpressionNode> PositionalParameters;
    private readonly List<object> _optionalDefaultValues = [];
    public string FunctionName;
    public FunctionInvocationExpressionNode(string functionName, List<ExpressionNode> positionalParameters = null)
    : base(eNodeType.FunctionInvocation)
    {
        PositionalParameters = positionalParameters;
        FunctionName = functionName;
    }

    public FunctionInvocationExpressionNode(string functionName, params ExpressionNode[] positionalParameters)
        : base(eNodeType.FunctionInvocation)
    {
        PositionalParameters = [.. positionalParameters];
        FunctionName = functionName;
    }

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        if (evalOptions.ExcludeFunctionExpressions)
        {
            Logger.LogError("functions is not allowed");
            return eControlType.RunTimeError;
        }
        if (FunctionName == null)
        {
            Logger.LogError("builtin function name is null");
            return eControlType.RunTimeError;
        }

        Type type = obj?.GetType();
        MemberInfo memberInfo = type?.GetMethod(FunctionName);
        int methodParametersCount = 0;
        if (memberInfo == null)
        {
            (Type type, MemberInfo memberInfo, int methodParametersCount) f = DependencyInjectionHolder.Instance.BuiltInFunctionFinder.FindMethod(FunctionName, PositionalParameters, _optionalDefaultValues);
            if (f.type != null)
                type = f.type;
            if (f.memberInfo != null)
            {
                memberInfo = f.memberInfo;
                methodParametersCount = f.methodParametersCount;
            }
            if (memberInfo == null)
            {
                Logger.LogError($"can not find builtin function {FunctionName}");
                return eControlType.RunTimeError;
            }

            obj = ControlBuiltInFunctionObject(obj, type);
            if (obj == null /*&& !memberInfo.isStatic*/)
            {
                Logger.LogError($"can not create builtin object for function {memberInfo.Name}");
                return eControlType.RunTimeError;
            }

            SetPropertyValue(obj, "localVariables", localVariables);
        }

        object[] args = GenerateFunctionArguments(obj, evalOptions, ref localVariables, methodParametersCount);
        try
        {
            outValue = type.InvokeMember(memberInfo.Name, BindingFlags.InvokeMethod, null, obj, args);
        }
        catch (Exception e)
        {
            Logger.LogError($"can not eval function {memberInfo.Name}({string.Join(",", args)})");
            Logger.LogError(e, e.Message);
        }

        return eControlType.Normal;
    }

    private static object ControlBuiltInFunctionObject(object obj, Type type)
    {
        if (obj != null && obj.GetType() != type)
            obj = null;
        return obj ?? CreateBuiltInFunctionObject(type);
    }

    private static object CreateBuiltInFunctionObject(Type type)
    {
        Type[] types = [];
        ConstructorInfo cons = type.GetConstructor(types);
        object[] parameters = [];
        object obj = cons?.Invoke(parameters);
        return obj;
    }

    private object[] GenerateFunctionArguments(object obj, EvalOptions evalOptions,
        ref LocalParameters localVariables,
        int methodParametersCount)
    {
        int paramCount = PositionalParameters?.Count ?? 0;
        object[] args = new object[Math.Max(methodParametersCount, paramCount)];
        int i = 0;
        if (PositionalParameters != null)
        {
            foreach (ExpressionNode param in PositionalParameters)
            {
                if (param == null)
                    args[i] = null;
                else
                {
                    param.eval(obj, ref localVariables, evalOptions, out args[i]);
                    while (args[i] is IExpressionValue)
                        args[i] = ((IExpressionValue)args[i]).InternalValue;
                }

                i++;
            }
        }

        if (i < args.Length - 1)
        {
            foreach (object optionalDefaultValue in _optionalDefaultValues) // todo foreach is not correct
            {
                args[i++] = optionalDefaultValue;
            }
        }

        return args;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (PositionalParameters == null) return this;
        List<ExpressionNode> list = [];
        foreach (ExpressionNode item in PositionalParameters)
        {
            if (item == null) continue;
            list.Add(item.replace(localVariables));
        }

        PositionalParameters = list;
        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        ExpressionNode result = func(this, param);
        if (result is not FunctionInvocationExpressionNode result1) return result;
        if (result1.PositionalParameters != null)
        {
            List<ExpressionNode> list = [];
            foreach (ExpressionNode item in result1.PositionalParameters)
                list.Add(item?.processAndReplace(func, param));
            result1.PositionalParameters = list;
        }

        result = func(result1, param);
        return result;
    }

    public override ExpressionNode clone()
    {
        List<ExpressionNode> posParams = null;
        if (PositionalParameters != null)
        {
            posParams = [];
            foreach (ExpressionNode item in PositionalParameters)
            {
                posParams.Add(item?.clone());
            }
        }

        return new FunctionInvocationExpressionNode(FunctionName, posParams);
    }

    public override string toText()
    {
        string txt = FunctionName + "(";
        if (PositionalParameters != null)
        {
            bool bFirst = true;
            foreach (ExpressionNode item in PositionalParameters)
            {
                if (!bFirst) txt += ",";
                txt += item.toText();
                bFirst = false;
            }
        }

        txt += ")";
        return txt;
    }

    protected override string GetNodeKey()
    {
        return FunctionName;
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        if (PositionalParameters != null)
        {
            foreach (ExpressionNode item in PositionalParameters)
            {
                item.FetchNodes<T>(dic);
            }
        }
    }
}
