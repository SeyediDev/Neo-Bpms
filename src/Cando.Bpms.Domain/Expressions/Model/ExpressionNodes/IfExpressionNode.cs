namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class IfExpressionNode : ExpressionNode
{
    public ExpressionNode IfExpression { get; private set; }
    public ExpressionNode ThenExpression { get; private set; }
    public ExpressionNode ElseExpression { get; private set; }

    public IfExpressionNode(ExpressionNode ifExpression, ExpressionNode thenExpression,
        ExpressionNode elseExpression)
        : base(eNodeType.IfExpression)
    {
        IfExpression = ifExpression;
        ThenExpression = thenExpression;
        ElseExpression = elseExpression;
    }

    public IfExpressionNode(ExpressionNode ifExpression, ExpressionNode thenExpression)
        : base(eNodeType.IfExpression)
    {
        IfExpression = ifExpression;
        ThenExpression = thenExpression;
        ElseExpression = null;
    }

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        object o1;
        eControlType ctrl;
        if (IfExpression == null)
            o1 = false;
        else
        {
            ctrl = IfExpression.eval(obj, ref localVariables, evalOptions, out o1);
            if (ctrl == eControlType.Throw || ctrl == eControlType.RunTimeError) return ctrl;
        }

        ctrl = CheckIfTrue(o1) ? ThenExpression.eval(obj, ref localVariables, evalOptions, out outValue) : ElseExpression.eval(obj, ref localVariables, evalOptions, out outValue);
        return ctrl;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        IfExpression = IfExpression?.replace(localVariables);
        if (IfExpression is ConstantExpressionNode ifConstExp)
        {
            return CheckIfTrue(ifConstExp.Value)
                ? ThenExpression == null
                    ? new ConstantExpressionNode(false)
                    : ThenExpression.replace(localVariables)
                : ElseExpression == null
                    ? new ConstantExpressionNode(false)
                    : ElseExpression.replace(localVariables);
        }

        ThenExpression = ThenExpression?.replace(localVariables);
        ElseExpression = ElseExpression?.replace(localVariables);
        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        IfExpression = IfExpression?.processAndReplace(func, param);
        ThenExpression = ThenExpression?.processAndReplace(func, param);
        ElseExpression = ElseExpression?.processAndReplace(func, param);
        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        return new IfExpressionNode(IfExpression.clone(), ThenExpression?.clone(),
            ElseExpression?.clone());
    }

    public override string toText()
    {
        return "IF(" + IfExpression.toText() + "," + (ThenExpression != null ? ThenExpression.toText() : "") + "," +
                (ElseExpression != null ? ElseExpression.toText() : "") + ")";
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        IfExpression?.FetchNodes<T>(dic);
        ThenExpression?.FetchNodes<T>(dic);
        ElseExpression?.FetchNodes<T>(dic);
    }
}
