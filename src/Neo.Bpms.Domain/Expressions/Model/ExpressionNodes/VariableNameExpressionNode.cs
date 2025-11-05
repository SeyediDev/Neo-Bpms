namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class VariableNameExpressionNode : ExpressionNode
{
    public readonly string Name;
    public readonly object Reference;

    public VariableNameExpressionNode(string name, object reference = null)
        : base(eNodeType.Variable)
    {
        Name = name;
        Reference = reference;
    }

    public VariableNameExpressionNode(string name, eNodeType nodeType)
        : base(nodeType)
    {
        Name = name;
    }

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        if (localVariables != null && Name == "_lp")
            outValue = localVariables;
        else if (localVariables != null && localVariables.TryGetValue(Name, out object value))
            outValue = value;
        else
            outValue = GetPropertyValue(obj, Name);
        return eControlType.Normal;
    }

    internal override eControlType setVal(object obj, object value, EvalOptions evalOptions)
    {
        SetPropertyValue(obj, Name, value);
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        return localVariables != null && localVariables.TryGetValue(Name, out object value) ? new ConstantExpressionNode(value) : this;
    }

    public override ExpressionNode clone()
    {
        VariableNameExpressionNode exp = new(Name, Reference);
        return exp;
    }

    public override string toText()
    {
        return Name;
    }

    protected override string GetNodeKey()
    {
        return Name;
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
    }
}
