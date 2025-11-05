namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType logicalExpressionType, ExpressionNode leftExpression,
    ExpressionNode rightExpression, int depth) : BinaryExpressionNode(eNodeType.LogicalExpressions, leftExpression, rightExpression, depth)
{
    public enum eLogicalExpressionType
    {
        And,
        Or,
        XOr,

        //			Not,
        InstanceOf
    }

    public readonly eLogicalExpressionType LogicalExpressionType = logicalExpressionType;

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        object lo = null, ro = null;
        LeftExpression?.eval(obj, ref localVariables, evalOptions, out lo);
        switch (LogicalExpressionType)
        {
            case eLogicalExpressionType.And:
                if (!CheckIfTrue(lo))
                {
                    outValue = false;
                    break;
                }

                RightExpression?.eval(obj, ref localVariables, evalOptions, out ro);
                outValue = CheckIfTrue(ro);
                break;
            case eLogicalExpressionType.Or:
                if (CheckIfTrue(lo))
                {
                    outValue = true;
                    break;
                }

                RightExpression?.eval(obj, ref localVariables, evalOptions, out ro);
                outValue = CheckIfTrue(ro);
                break;
            case eLogicalExpressionType.XOr:
                RightExpression?.eval(obj, ref localVariables, evalOptions, out ro);
                outValue = CheckIfTrue(lo) ^ CheckIfTrue(ro);
                break;
            //case eLogicalExpressionType.Not:
            //	outValue = !checkIfTrue(lo);
            //	break;
            case eLogicalExpressionType.InstanceOf:
                RightExpression?.eval(obj, ref localVariables, evalOptions, out ro);
                outValue = ro switch
                {
                    Dictionary<object, object> objects => lo != null && objects.ContainsKey(lo),
                    List<object> list => list.FirstOrDefault(r => (dynamic)lo == (dynamic)r),
                    _ => (dynamic)lo == (dynamic)ro,
                };
                break;
        }

        return eControlType.Normal;
    }

    protected override int getPriority()
    {
        return LogicalExpressionType switch
        {
            eLogicalExpressionType.And => 11,
            eLogicalExpressionType.Or => 12,
            eLogicalExpressionType.XOr => 12,//???
                                             //case eLogicalExpressionType.Not:
                                             //	break;
            eLogicalExpressionType.InstanceOf => 1,
            _ => 0,
        };
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        ExpressionNode result = base.replace(localVariables);
        if (result is not LogicalExpressionNode logical) return result;
        if (logical.LeftExpression is ConstantExpressionNode)
        {
            bool b = CheckIfTrue(logical.LeftExpression.Eval(null, localVariables));
            return SingleReplace(b, logical.RightExpression);
        }

        if (logical.RightExpression is not ConstantExpressionNode) return result;
        {
            bool b = CheckIfTrue(logical.RightExpression.Eval(null, localVariables));
            return SingleReplace(b, logical.LeftExpression);
        }

    }

    private ExpressionNode SingleReplace(bool b, ExpressionNode expressionNode)
    {
        switch (LogicalExpressionType)
        {
            case eLogicalExpressionType.And:
                if (!b) return new ConstantExpressionNode(false);
                return expressionNode;
            case eLogicalExpressionType.Or:
                if (b) return new ConstantExpressionNode(true);
                return expressionNode;
            default:
                return this;
        }
    }

    public override ExpressionNode clone()
    {
        return new LogicalExpressionNode(LogicalExpressionType, LeftExpression.clone(), RightExpression.clone(),
            Depth);
    }

    public override string toText()
    {
        return LeftExpression?.toText() + " " + LogicalExpressionType + " " + RightExpression?.toText();
    }

    protected override string GetNodeKey()
    {
        return LogicalExpressionType.ToString();
    }
}
