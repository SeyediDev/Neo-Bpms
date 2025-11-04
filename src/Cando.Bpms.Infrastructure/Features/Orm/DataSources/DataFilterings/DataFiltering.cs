namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

public class DataFiltering<T> : IDataFiltering<T>
{
    public List<ExpressionNode> RunTimeFilters { get; set; }
    public LocalParameters FilterValues { get; set; }

    public bool IsValid(T record)
    {
        var filtered = RunTimeFilters?.Any(filter =>
                           CheckFilterExpression(filter, record))
                       ?? false;
        return !filtered;
    }

    protected ExpressionNode ReplaceInstanceData(ExpressionNode exp, object inRecord)
    {
        if (exp is not VariableNameExpressionNode vExp)
            return exp;
        object value;
        bool find;
        if (inRecord is ElasticObject eRecord)
            find = eRecord.GetField(vExp.Name, out value);
        else
        {
            if (inRecord is not T record)
                return exp;
            find = ReflectionField.GetValue(record, vExp.Name, out value);
        }

        if (!find && FilterValues != null)
            FilterValues.TryGetValue(vExp.Name, out value);
        return new ConstantExpressionNode(value);
    }

    protected virtual object EvalExpression(ExpressionNode exp, T record)
    {
        exp = exp.clone();
        exp = exp?.processAndReplace(ReplaceInstanceData, record);
        object value;
        try
        {
            var lc = new LocalParameters();
            lc.MergeObject(record);
            value = exp?.Eval(record, lc);
        }
        catch
        {
            value = exp?.toText();
        }

        return value;
    }

    private bool CheckFilterExpression(ExpressionNode exp, T record)
    {
        return !ExpressionNode.CheckIfTrue(EvalExpression(exp, record));
    }
}
