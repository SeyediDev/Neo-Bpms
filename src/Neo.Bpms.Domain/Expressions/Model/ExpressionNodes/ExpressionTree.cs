namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ExpressionTree
{
    public string ExpressionString;
    [XmlIgnore]
    public ExpressionNode Root;

    public ExpressionNode.eControlType SetVal(object obj, object value)
    {
        return Root.setVal(obj, value, EvalOptions.Default);
    }
}
