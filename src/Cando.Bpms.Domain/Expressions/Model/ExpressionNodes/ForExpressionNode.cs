namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ForExpressionNode(ExpressionNode list, ExpressionNode result) : BlockExpressionNode(eNodeType.ForExpression)
{
    private ExpressionNode _list = list;
    private ExpressionNode _result = result;

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        //todo is not correct
        foreach (KeyValuePair<string, object> variable in localVariables)
        {
            SetLocalVariable(variable.Key, variable.Value);
        }
        foreach (ExpressionNode expression in Expressions)
        {
            eControlType ctrl = expression.eval(obj, ref localVariables, evalOptions, out outValue);
            switch (ctrl)
            {
                case eControlType.Break:
                    return eControlType.Normal;
                case eControlType.Continue:
                    continue;
                case eControlType.Return:
                case eControlType.GoTo:
                case eControlType.Throw:
                case eControlType.RunTimeError:
                    return ctrl;
            }
        }
        outValue = null;
        return eControlType.Normal;
    }
    //public override ExpressionNode replace(LocalParameters localVariables)
    //{
    //	//todo

    //	return this;
    //}
    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        _list = _list?.processAndReplace(func, param);
        _result = _result?.processAndReplace(func, param);
        return base.processAndReplace(func, param);
    }
}
