namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ConstantExpressionNode(object value) : ExpressionNode(eNodeType.LiteralConstant)
{
    //public enum eLiteralType
    //{
    //	Bool,
    //	String,
    //	Number,
    //	Null,
    //	Undefined
    //}
    //		eLiteralType literalType;
    public object Value = value;

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = Value;
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        return this;
    }

    public override ExpressionNode clone()
    {
        return new ConstantExpressionNode(Value);
    }

    public override string toText()
    {
        return Value?.ToString() ?? "null";
    }

    protected override string GetNodeKey()
    {
        return Value?.ToString();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
    }
}
