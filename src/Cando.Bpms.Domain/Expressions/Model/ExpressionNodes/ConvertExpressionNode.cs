namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ConvertExpressionNode : ExpressionNode
{
    public readonly Type Type;
    public ExpressionNode Expression { get; private set; }

    internal ConvertExpressionNode(Type type, ExpressionNode expression)
        : base(eNodeType.Convert)
    {
        Expression = expression;
        Type = type;
    }

    protected override int getPriority()
    {
        return 1;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        object o = null;
        Expression?.eval(obj, ref localVariables, evalOptions, out o);
        outValue = Convert(o);
        return eControlType.Normal;
    }

    internal override eControlType setVal(object obj, object value, EvalOptions evalOptions)
    {
        return Expression.setVal(obj, value, evalOptions);
    }

    private object Convert(object o)
    {
        if (Type == typeof(double))
            return o is double ? o : double.Parse(o.ToString());
        return Type == typeof(int)
            ? o is int ? o : int.Parse(o.ToString())
            : Type == typeof(bool) ? o is bool ? o : bool.Parse(o.ToString()) : Type == typeof(string) ? o is string ? o : o.ToString() : o;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (Expression == null) return this;
        Expression = Expression.replace(localVariables);
        ConstantExpressionNode constExp = Expression as ConstantExpressionNode;
        if (constExp == null) return this;
        constExp.Value = Convert(constExp.Value);
        return constExp;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        Expression = Expression?.processAndReplace(func, param);
        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        return new ConvertExpressionNode(Type, Expression.clone());
    }

    public override string toText()
    {
        return "(" + Type + ") " + Expression.toText();
    }

    protected override string GetNodeKey()
    {
        return Type.ToString();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        Expression?.FetchNodes<T>(dic);
    }
}
