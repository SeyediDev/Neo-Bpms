namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType operatorType, ExpressionNode leftExpression, ExpressionNode rightExpression, int depth) : BinaryExpressionNode(eNodeType.ArithmeticExpressions, leftExpression, rightExpression, depth)
{
    public enum eArithmeticOperatorType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulo,
        Reminder,
        Power,
        Exponentiation,
        //			Negation,
        BitAnd,
        BitOr,
        BitXOr,
        LeftShift,
        RightShift
    }
    public readonly eArithmeticOperatorType OperatorType = operatorType;

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        object lo, ro;
        if (LeftExpression == null)
            lo = 0;
        else
        {
            LeftExpression.eval(obj, ref localVariables, evalOptions, out lo);
            if (lo == null || lo.ToString() == "undefined")
                lo = 0;
        }
        if (RightExpression == null)
            ro = 0;
        else
        {
            RightExpression.eval(obj, ref localVariables, evalOptions, out ro);
            if (ro == null || ro.ToString() == "undefined")
                ro = 0;
        }
        switch (OperatorType)
        {
            case eArithmeticOperatorType.Addition:
                outValue = (dynamic)lo + (dynamic)ro;
                break;
            case eArithmeticOperatorType.Subtraction:
                outValue = (dynamic)lo - (dynamic)ro;
                break;
            case eArithmeticOperatorType.Multiplication:
                outValue = (dynamic)lo * (dynamic)ro;
                break;
            case eArithmeticOperatorType.Division:
                if ((dynamic)ro == 0)
                {
                    Logger.LogError($"devide by zero {lo} / 0");
                    return eControlType.RunTimeError;
                }
                outValue = (dynamic)lo / (dynamic)ro;
                break;
            case eArithmeticOperatorType.Modulo:
                if ((dynamic)ro == 0)
                {
                    Logger.LogError($"devide by zero mode({lo}, 0)");
                    return eControlType.RunTimeError;
                }
                outValue = (int)((dynamic)lo / (dynamic)ro);
                break;
            case eArithmeticOperatorType.Reminder:
                if ((dynamic)ro == 0)
                {
                    Logger.LogError($"devide by zero {lo} % 0");
                    return eControlType.RunTimeError;
                }
                outValue = (dynamic)lo % (dynamic)ro;
                break;
            case eArithmeticOperatorType.Power:
                {
                    double.TryParse(lo.ToString().Replace("/", "."), out double d1);
                    double.TryParse(ro.ToString().Replace("/", "."), out double d2);
                    outValue = Math.Pow(d1, d2);
                }
                break;
            case eArithmeticOperatorType.Exponentiation:
                {
                    double.TryParse(lo.ToString().Replace("/", "."), out double d1);
                    double.TryParse(ro.ToString().Replace("/", "."), out double d2);
                    outValue = d1 * Math.Exp(d2);
                }
                break;
            //case eArithmeticOperatorType.Negation:
            //	outValue = -(dynamic)lo;
            //	break;
            case eArithmeticOperatorType.BitAnd:
                outValue = (dynamic)lo & (dynamic)ro;
                break;
            case eArithmeticOperatorType.BitOr:
                outValue = (dynamic)lo | (dynamic)ro;
                break;
            case eArithmeticOperatorType.BitXOr:
                outValue = (dynamic)lo ^ (dynamic)ro;
                break;
            case eArithmeticOperatorType.LeftShift:
                outValue = (dynamic)lo << (dynamic)ro;
                break;
            case eArithmeticOperatorType.RightShift:
                outValue = (dynamic)lo >> (dynamic)ro;
                break;
        }
        return eControlType.Normal;
    }

    protected override int getPriority()
    {
        return OperatorType switch
        {
            eArithmeticOperatorType.Addition or eArithmeticOperatorType.Subtraction => 4,
            eArithmeticOperatorType.Multiplication or eArithmeticOperatorType.Division or eArithmeticOperatorType.Modulo or eArithmeticOperatorType.Reminder => 3,
            eArithmeticOperatorType.Power or eArithmeticOperatorType.Exponentiation => 9,
            //case eArithmeticOperatorType.Negation:
            //	break;
            eArithmeticOperatorType.BitAnd => 8,
            eArithmeticOperatorType.BitOr or eArithmeticOperatorType.BitXOr => 10,
            eArithmeticOperatorType.LeftShift or eArithmeticOperatorType.RightShift => 5,
            _ => 0,
        };
    }

    public override ExpressionNode clone()
    {
        return new ArithmeticExpressionNode(OperatorType, LeftExpression.clone(), RightExpression?.clone(), Depth);
    }
    public override string toText()
    {
        return LeftExpression.toText() + " " + OperatorType + " " + RightExpression.toText();
    }

    protected override string GetNodeKey()
    {
        return OperatorType.ToString();
    }
}
