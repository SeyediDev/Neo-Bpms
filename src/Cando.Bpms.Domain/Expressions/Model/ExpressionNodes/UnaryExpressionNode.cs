namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType unaryOperationType, ExpressionNode expression) : ExpressionNode(eNodeType.Unary)
{
    public enum eUnaryOperationType
    {
        Negation,
        Decrement,
        Increment,
        BitNot,
        Not
    }
    public readonly eUnaryOperationType UnaryOperationType = unaryOperationType;
    public ExpressionNode Expression { get; private set; } = expression;
    protected override int getPriority() { return 1; }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        if (Expression == null) return eControlType.Normal;
        Expression.eval(obj, ref localVariables, evalOptions, out object o);
        switch (UnaryOperationType)
        {
            case eUnaryOperationType.Negation:
                outValue = -(dynamic)o;
                break;
            case eUnaryOperationType.Decrement:
                if (localVariables.ContainsKey(o.GetType().Name))
                    localVariables[o.GetType().Name] = (dynamic)o - 1;
                else
                    SetPropertyValue(obj, o.GetType().Name, (dynamic)o - 1);
                break;
            case eUnaryOperationType.Increment:
                if (localVariables.ContainsKey(o.GetType().Name))
                    localVariables[o.GetType().Name] = (dynamic)o + 1;
                else
                    SetPropertyValue(obj, o.GetType().Name, (dynamic)o + 1);
                break;
            case eUnaryOperationType.BitNot:
                outValue = ~(dynamic)o;
                break;
            case eUnaryOperationType.Not:
                outValue = o == null ? false : !(dynamic)o;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return eControlType.Normal;
    }
    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (Expression != null)
        {
            Expression = Expression.replace(localVariables);
            if (Expression is ConstantExpressionNode value)
            {
                return new ConstantExpressionNode(Eval(value, localVariables));
            }
        }
        else
            return new ConstantExpressionNode(null);
        return this;
    }
    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        Expression = Expression?.processAndReplace(func, param);
        return func(this, param);
    }
    public override ExpressionNode clone()
    {
        return new UnaryExpressionNode(UnaryOperationType, Expression?.clone());
    }
    public override string toText()
    {
        return UnaryOperationType + Expression.toText();
    }
    protected override string GetNodeKey()
    {
        return UnaryOperationType.ToString();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        Expression?.FetchNodes<T>(dic);
    }
}
