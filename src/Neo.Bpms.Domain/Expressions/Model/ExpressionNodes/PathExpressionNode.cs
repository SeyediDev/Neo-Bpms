namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class PathExpressionNode(ExpressionNode expression, ExpressionNode rightExpression) : ExpressionNode(eNodeType.PathExpression)
{
    public ExpressionNode Expression = expression;
    public ExpressionNode RightExpression = rightExpression;
    public object Reference;

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        if (evalOptions.ExcludePathExpressions)
        {
            Logger.LogError($"path is not allowed");
            return eControlType.RunTimeError;
        }
        if (Expression == null || RightExpression == null)
        {
            Logger.LogError($"path is incorrect {toText()}");
            return eControlType.RunTimeError;
        }

        Expression.eval(obj, ref localVariables, evalOptions, out object o);
        return RightExpression.eval(o, ref localVariables, evalOptions, out outValue);
    }

    internal override eControlType setVal(object obj, object value, EvalOptions evalOptions)
    {
        if (Expression == null || RightExpression == null)
        {
            Logger.LogError($"path is incorrect {toText()}");
            return eControlType.RunTimeError;
        }

        LocalParameters localVariables = []; //todo
        Expression.eval(obj, ref localVariables, evalOptions, out object o);
        return RightExpression.setVal(o, value, evalOptions);
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (Expression == null || RightExpression == null) return this;
        Expression = Expression.replace(localVariables);
        return Expression is ConstantExpressionNode leftVar
            ? new ConstantExpressionNode(RightExpression.Eval(leftVar.Value, localVariables))
            : (ExpressionNode)this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        ExpressionNode result = func(this, param);
        if (result is not PathExpressionNode result1 || result1.Reference != null) return result;
        if (result1.Expression != null)
            result1.Expression = result1.Expression.processAndReplace(func, param);
        //if (result1.rightExpression != null)
        //	result1.rightExpression = result1.rightExpression.processAndReplace(func, param);

        return result;
    }

    public override ExpressionNode clone()
    {
        PathExpressionNode exp = new(Expression.clone(), RightExpression.clone()) { Reference = Reference };
        return exp;
    }

    public override string toText()
    {
        return $"{Expression.toText()}.{RightExpression.toText()}";
    }

    protected override string GetNodeKey()
    {
        return $"{Expression.toText()}.{RightExpression.toText()}";
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        Expression?.FetchNodes<T>(dic);
        RightExpression?.FetchNodes<T>(dic);
    }
}
