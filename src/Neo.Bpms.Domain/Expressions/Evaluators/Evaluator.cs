namespace Neo.Bpms.Domain.Expressions.Evaluators;

public class Evaluator
{
    private LocalParameters paramValues;
    public void setParam(string paramName, object value)
    {
        paramValues ??= [];//todo
        _ = paramValues.AddOrUpdate(paramName, value);
    }
    public void setParamIndex(string paramName, int index, object value)
    {
        paramValues ??= [];//todo
        if (!paramValues.ContainsKey(paramName))
        {
            paramValues[paramName] = new Dictionary<int, object>();
        }

        if (paramValues[paramName] is not Dictionary<int, object> collection)
        {
            return;
        }

        if (collection.ContainsKey(index))
        {
            collection[index] = value;
        }
        else
        {
            collection.Add(index, value);
        }
    }

}
