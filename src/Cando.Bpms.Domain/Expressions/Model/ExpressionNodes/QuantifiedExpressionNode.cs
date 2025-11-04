namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class QuantifiedExpressionNode : ExpressionNode
{
    public QuantifiedExpressionNode()
        : base(eNodeType.QuantifiedExpression)
    {
    }

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        //todo:....
        outValue = null;
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        return this;
    }

    public override ExpressionNode clone()
    {
        return new QuantifiedExpressionNode();
    }

    public override string toText()
    {
        return NodeType.ToString();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
    }
}
