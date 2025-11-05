namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public abstract class BinaryExpressionNode : ExpressionNode
{
    protected BinaryExpressionNode(eNodeType type, ExpressionNode leftExpression, ExpressionNode rightExpression,
        int depth)
        : base(type)
    {
        LeftExpression = leftExpression;
        RightExpression = rightExpression;
        Depth = depth;
    }

    public ExpressionNode LeftExpression { get; private set; }
    public ExpressionNode RightExpression { get; private set; }

    public ExpressionNode AdjustByPriority()
    {
        if (RightExpression is not BinaryExpressionNode right) return this;
        if (right.Depth != Depth) return this;
        if (getPriority() >= right.getPriority()) return this;
        RightExpression = right.LeftExpression;
        right.LeftExpression = this;
        return right;

    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        LeftExpression = LeftExpression?.replace(localVariables);
        RightExpression = RightExpression?.replace(localVariables);
        return LeftExpression is ConstantExpressionNode && RightExpression is ConstantExpressionNode
            ? new ConstantExpressionNode(Eval(null, localVariables))
            : this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        LeftExpression = LeftExpression?.processAndReplace(func, param);
        RightExpression = RightExpression?.processAndReplace(func, param);
        return func(this, param);
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        LeftExpression?.FetchNodes<T>(dic);
        RightExpression?.FetchNodes<T>(dic);
    }
}
