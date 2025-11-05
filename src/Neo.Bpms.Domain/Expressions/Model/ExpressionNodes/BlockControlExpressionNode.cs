namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class BlockControlExpressionNode : ExpressionNode
{
    public enum eBlockControlType
    {
        Break,
        Return,
        Continue,
        Label,
        GoTo,
        Throw,
        Catch
    }

    private readonly eBlockControlType _controlType;
    private ExpressionNode _expression;

    protected override int getPriority()
    {
        return 0;
    }

    public BlockControlExpressionNode(eBlockControlType controlType)
        : base(eNodeType.BlockControl)
    {
        _controlType = controlType;
    }

    public BlockControlExpressionNode(eBlockControlType controlType, ExpressionNode expression)
        : base(eNodeType.BlockControl)
    {
        _controlType = controlType;
        _expression = expression;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        switch (_controlType)
        {
            case eBlockControlType.GoTo:
                if (_expression != null) _expression.eval(obj, ref localVariables, evalOptions, out outValue);
                return eControlType.GoTo;
            case eBlockControlType.Throw:
                if (_expression != null) _expression.eval(obj, ref localVariables, evalOptions, out outValue);
                return eControlType.Throw;
            case eBlockControlType.Return:
                if (_expression != null) _expression.eval(obj, ref localVariables, evalOptions, out outValue);
                return eControlType.Return;
            case eBlockControlType.Break:
                return eControlType.Break;
            case eBlockControlType.Continue:
                return eControlType.Continue;
            default:
                return eControlType.Normal;
        }
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        _expression = _expression?.replace(localVariables);
        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        _expression = _expression?.processAndReplace(func, param);
        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        return new BlockControlExpressionNode(_controlType, _expression.clone());
    }

    public override string toText()
    {
        //var txt = "{";
        //txt += this.expression.toText();
        //txt += "}";
        return _controlType + _expression.toText();
    }

    protected override string GetNodeKey()
    {
        return _controlType.ToString();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        _expression?.FetchNodes<T>(dic);
    }
}
