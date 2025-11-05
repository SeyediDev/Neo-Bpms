namespace Neo.Bpms.Domain.Expressions.Model;

public interface IFunctionImplementations
{
}

public interface IBuiltInFunctionFinder
{
    void AddAssembly(Assembly asm);
    (Type type, MemberInfo memberInfo, int methodParametersCount) FindMethod(string functionName,
        List<ExpressionNode> positionalParameters, List<object> optionalDefaultValues);
}

public class BuiltInFunctionFinder : IBuiltInFunctionFinder
{
    private readonly List<Type> _builtInFunctions = [];

    public (Type type, MemberInfo memberInfo, int methodParametersCount) FindMethod(string functionName,
        List<ExpressionNode> positionalParameters, List<object> optionalDefaultValues)
    {
        int methodParametersCount = 0;
        Type type = null!;
        MemberInfo memberInfo = null!;
        foreach (Type builtInFunction in _builtInFunctions ?? Enumerable.Empty<Type>())
        {
            if (builtInFunction == null)
            {
                continue;
            }

            type = builtInFunction;
            MethodInfo method = GetMethod(type, functionName) ?? GetMethod(type, "feel_" + functionName);
            methodParametersCount = method?.GetParameters().Length ?? 0;
            memberInfo = method!;
            if (memberInfo != null)
            {
                break;
            }
        }
        return (type, memberInfo, methodParametersCount);

        MethodInfo GetMethod(Type type, string methodName)
        {
            int positionalParametersCount = positionalParameters?.Count ?? 0;
            foreach (MethodInfo method in type?.GetMethods().Where(m => m.Name.Equals(methodName)) ??
                                   [])
            {
                ParameterInfo[] parameters = method.GetParameters();
                bool hasParams = parameters.Any(p => p.IsDefined(typeof(ParamArrayAttribute), false));
                int requiredParameters =
                    parameters.Count(p => !p.IsOptional && !p.IsDefined(typeof(ParamArrayAttribute), false));
                List<ParameterInfo> optionalParameters =
                    [.. parameters.Where(p => p.IsOptional && !p.IsDefined(typeof(ParamArrayAttribute), false))];
                if (requiredParameters == positionalParametersCount ||
                    (positionalParametersCount - optionalParameters.Count > requiredParameters && hasParams) ||
                    (positionalParametersCount - requiredParameters == optionalParameters.Count && !hasParams)
                )
                {
                    foreach (ParameterInfo optional in optionalParameters)
                    {
                        optionalDefaultValues.Add(optional.DefaultValue);
                    }

                    return method;
                }

                if (parameters.Length > (positionalParameters?.Count ?? 0))
                {
                    bool found = true;
                    for (int i = positionalParameters?.Count ?? 0; i < parameters.Length; i++)
                    {
                        if (parameters[i].IsDefined(typeof(ParamArrayAttribute), false))
                        {
                            return method;
                        }

                        if (!parameters[i].IsOptional)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        return method;
                    }
                }
            }

            return null;
        }
    }

    public void AddAssembly(Assembly asm)
    {
        IEnumerable<Type> types = asm.GetLoadableTypes().Where(t => !t.IsAbstract);
        foreach (Type type in types.Where(ReflectionTools.IsInBaseInterface<IFunctionImplementations>))
        {
            _builtInFunctions.Add(type);
        }
    }
}
