namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class IndexExpressionNode(ExpressionNode leftExpression, ExpressionNode indexExpression) : ExpressionNode(eNodeType.FilterExpression)
{
    private ExpressionNode _leftExpression = leftExpression;
    public ExpressionNode IndexExpression { get; private set; } = indexExpression;

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        object io;
        if (IndexExpression == null)
            return eControlType.Normal;
        if (_leftExpression == null)
            io = obj;
        else
            _leftExpression.eval(obj, ref localVariables, evalOptions, out io);
        if (io == null || io is string s && s == "undefined")
        {
            outValue = io;
            Logger.LogError($"can not index {toText()}");
            return eControlType.RunTimeError;
        }

        if (IndexExpression is VariableNameExpressionNode node)
        {
            outValue = ((dynamic)io)[node.Name];
        }
        else
        {
            IndexExpression.eval(io, ref localVariables, evalOptions, out object o);
            switch (o)
            {
                case null:
                case string s1 when s1 == "{null}":
                    outValue = null;
                    break;
                case string s1 when s1 == "undefined":
                    {
                        IndexExpression.eval(obj, ref localVariables, evalOptions, out o);
                        if (o == null || o is string s2 && s2 == "undefined")
                            outValue = o;
                        else
                            outValue = ((dynamic)io)[o];
                        break;
                    }

                default:
                    outValue = ((dynamic)io)[o];
                    break;
            }
        }

        return eControlType.Normal;
    }

    internal override eControlType setVal(object obj, object value, EvalOptions evalOptions)
    {
        object io;
        LocalParameters localVariables = []; //todo
        if (IndexExpression == null) return eControlType.Normal;
        if (_leftExpression == null)
            io = obj;
        else
            _leftExpression.eval(obj, ref localVariables, evalOptions, out io);
        if (io == null || io is string s && s == "undefined")
        {
            Logger.LogError($"can not index {toText()}");
            return eControlType.RunTimeError;
        }

        if (IndexExpression is VariableNameExpressionNode node)
        {
            SetPropertyValue(io, node.Name, value);
        }
        else
        {
            return IndexExpression.setVal(io, value, evalOptions);
        }

        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (_leftExpression == null || IndexExpression == null) return this;
        _leftExpression = _leftExpression.replace(localVariables);
        return _leftExpression is ConstantExpressionNode leftVar
            ? new ConstantExpressionNode(IndexExpression.Eval(leftVar.Value, localVariables))
            : (ExpressionNode)this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        _leftExpression = _leftExpression?.processAndReplace(func, param);
        IndexExpression = IndexExpression?.processAndReplace(func, param);
        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        return new IndexExpressionNode(_leftExpression.clone(), IndexExpression.clone());
    }

    public override string toText()
    {
        return _leftExpression.toText() + "[" + IndexExpression.toText() + "]";
    }

    protected override string GetNodeKey()
    {
        return _leftExpression.toText() + "[" + IndexExpression.toText() + "]";
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        _leftExpression?.FetchNodes<T>(dic);
        IndexExpression?.FetchNodes<T>(dic);
    }
}
