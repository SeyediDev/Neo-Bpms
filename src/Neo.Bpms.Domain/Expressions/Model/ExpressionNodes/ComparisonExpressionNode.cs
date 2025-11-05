using System.Collections;

namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType comparisonType, ExpressionNode leftExpression,
    ExpressionNode rightExpression, int depth) : BinaryExpressionNode(eNodeType.ComparisionExpression, leftExpression, rightExpression, depth)
{
    public enum eComparisonType
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqual,
        GreaterThan,
        GreaterEqual,
        NotGreaterThan,
        NotLessThan,
        In,
        NotIn,
        InstanceOf
    }

    public readonly eComparisonType ComparisonType = comparisonType;

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;

        ExpressionNodeValue lo = FetchExpressionValue(obj, LeftExpression, evalOptions, ref localVariables);
        ExpressionNodeValue ro = FetchExpressionValue(obj, RightExpression, evalOptions, ref localVariables);
        try
        {
            switch (ComparisonType)
            {
                case eComparisonType.Equal:
                    outValue = lo == ro;
                    break;
                case eComparisonType.NotEqual:
                    outValue = lo != ro;
                    break;
                case eComparisonType.LessThan:
                    outValue = lo < ro;
                    break;
                case eComparisonType.NotLessThan:
                    outValue = !(lo < ro);
                    break;
                case eComparisonType.LessEqual:
                    outValue = lo <= ro;
                    break;
                case eComparisonType.GreaterThan:
                    outValue = lo > ro;
                    break;
                case eComparisonType.NotGreaterThan:
                    outValue = !(lo > ro);
                    break;
                case eComparisonType.GreaterEqual:
                    outValue = lo >= ro;
                    break;
                case eComparisonType.In:
                    if (ro?.OriginalValue is IList)
                        outValue = ((IList)ro.OriginalValue).Contains((dynamic)lo.ConvertedValue);
                    else if (ro?.ConvertedValue is string)
                        outValue = ((string)ro.ConvertedValue).Split(',').Contains(lo.ConvertedValue.ToString());
                    else
                        outValue = ro?.OriginalValue.ToString().Contains(lo.ToString()) ?? false;
                    break;
            }
        }
        catch
        {
            outValue = false;
        }

        return eControlType.Normal;
    }

    protected override int getPriority()
    {
        return ComparisonType switch
        {
            eComparisonType.Equal or eComparisonType.NotEqual => 7,
            eComparisonType.LessThan or eComparisonType.LessEqual or eComparisonType.GreaterThan or eComparisonType.GreaterEqual or eComparisonType.NotLessThan or eComparisonType.NotGreaterThan => 6,
            _ => 0,
        };
    }

    public override ExpressionNode clone()
    {
        return new ComparisonExpressionNode(ComparisonType, LeftExpression?.clone(), RightExpression?.clone(), Depth);
    }

    public override string toText()
    {
        return LeftExpression?.toText() + " " + ComparisonType + " " + RightExpression?.toText();
    }

    protected override string GetNodeKey()
    {
        return ComparisonType.ToString();
    }

    private static ExpressionNodeValue FetchExpressionValue(object obj, ExpressionNode expression,
        EvalOptions evalOptions,
        ref LocalParameters localVariables)
    {
        object o;
        if (expression == null)
            o = null;
        else
            expression.eval(obj, ref localVariables, evalOptions, out o);
        return new ExpressionNodeValue(o);
    }
}
