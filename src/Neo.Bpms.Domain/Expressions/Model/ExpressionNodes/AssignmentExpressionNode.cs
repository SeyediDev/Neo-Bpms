namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator assignmentOperator, ExpressionNode leftExpression,
    ExpressionNode rightExpression, int depth) : BinaryExpressionNode(eNodeType.AssignmentOperator, leftExpression, rightExpression, depth)
{
    public enum eAssignmentOperator
    {
        SimpleAssignment,
        AddAssignment,
        SubtractAssignment,
        MultiplicationAssignment,
        DivisionAssignment,
        PowerAssignment,
        ModuloAssignment,
        AndAssignment,
        OrAssignment,
        XOrAssignment,
        BitAndAssignment,
        BitOrAssignment,
        RightShiftAssignment,
        LeftShiftAssignment,
        PostDecrementAssignment,
        PostIncrementAssignment,
        PreDecrementAssignment,
        PreIncrementAssignment
    }

    //		string name;
    private readonly eAssignmentOperator _assignmentOperator = assignmentOperator;

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        object leftValue = null;
        LeftExpression?.eval(obj, ref localVariables, evalOptions, out leftValue);
        object rightValue = null;
        RightExpression?.eval(obj, ref localVariables, evalOptions, out rightValue);

        switch (_assignmentOperator)
        {
            case eAssignmentOperator.SimpleAssignment:
                outValue = rightValue;
                break;
            case eAssignmentOperator.AddAssignment:
                outValue = GetArithmeticValue(leftValue) + GetArithmeticValue(rightValue);
                break;
            case eAssignmentOperator.SubtractAssignment:
                outValue = GetArithmeticValue(leftValue) - GetArithmeticValue(rightValue);
                break;
            case eAssignmentOperator.MultiplicationAssignment:
                outValue = GetArithmeticValue(leftValue) * GetArithmeticValue(rightValue);
                break;
            case eAssignmentOperator.DivisionAssignment:
                if (GetArithmeticValue(rightValue) == 0)
                    return eControlType.RunTimeError;
                outValue = GetArithmeticValue(leftValue) / GetArithmeticValue(rightValue);
                break;
            case eAssignmentOperator.PowerAssignment:
                double.TryParse(leftValue?.ToString() ?? "0", out double d1);
                double.TryParse(rightValue?.ToString().Replace("/", ".") ?? "0", out double d2);
                outValue = Math.Pow(d1, d2);
                break;
            case eAssignmentOperator.ModuloAssignment:
                if ((dynamic)rightValue == 0)
                    return eControlType.RunTimeError;
                outValue = (dynamic)leftValue % (dynamic)rightValue;
                break;
            case eAssignmentOperator.AndAssignment:
                outValue = (dynamic)leftValue && (dynamic)rightValue;
                break;
            case eAssignmentOperator.OrAssignment:
                outValue = (dynamic)leftValue || (dynamic)rightValue;
                break;
            case eAssignmentOperator.XOrAssignment:
                outValue = (dynamic)leftValue ^ (dynamic)rightValue;
                break;
            case eAssignmentOperator.BitAndAssignment:
                outValue = (dynamic)leftValue & (dynamic)rightValue;
                break;
            case eAssignmentOperator.BitOrAssignment:
                outValue = (dynamic)leftValue | (dynamic)rightValue;
                break;
            case eAssignmentOperator.RightShiftAssignment:
                outValue = (dynamic)leftValue >> (dynamic)rightValue;
                break;
            case eAssignmentOperator.LeftShiftAssignment:
                outValue = (dynamic)leftValue << (dynamic)rightValue;
                break;
                //case eAssignmentOperator.PostDecrementAssignment:
                //	break;
                //case eAssignmentOperator.PostIncrementAssignment:
                //	break;
                //case eAssignmentOperator.PreDecrementAssignment:
                //	break;
                //case eAssignmentOperator.PreIncrementAssignment:
                //	break;
        }
        //		setPropertyValue(obj, name, outValue);

        if (LeftExpression is VariableNameExpressionNode node)
        {
            string name = node.Name;
            localVariables.AddOrUpdate(name, outValue);
        }

        return eControlType.Normal;
    }

    private static decimal GetArithmeticValue(object value)
    {
        return value == null ? 0 : Convert.ToDecimal(value);
    }

    protected override int getPriority()
    {
        return 14;
    }

    public override ExpressionNode clone()
    {
        return new AssignmentExpressionNode(_assignmentOperator, LeftExpression?.clone(), RightExpression?.clone(),
            Depth);
    }

    public override string toText()
    {
        return LeftExpression.toText() + _assignmentOperator + RightExpression.toText();
    }

    protected override string GetNodeKey()
    {
        return _assignmentOperator.ToString();
    }
}
