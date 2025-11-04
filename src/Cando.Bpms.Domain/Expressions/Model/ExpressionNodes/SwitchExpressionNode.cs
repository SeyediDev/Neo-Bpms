namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class SwitchExpressionNode(ExpressionNode valueExpression, IEnumerable<SwitchCaseExpressionNode> cases,
    ExpressionNode defaultExpression) : ExpressionNode(eNodeType.Switch)
{
    public readonly List<SwitchCaseExpressionNode> Cases = [.. cases];
    private ExpressionNode _valueExpression = valueExpression;
    public readonly ExpressionNode DefaultExpression = defaultExpression;

    protected override int getPriority()
    {
        return 0;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        object value;
        outValue = null;
        if (_valueExpression == null)
            value = null;
        else
            _valueExpression.eval(obj, ref localVariables, evalOptions, out value);
        foreach (SwitchCaseExpressionNode caseItem in Cases)
        {
            object caseValue;
            if (caseItem.CaseExpression == null)
                caseValue = null;
            else
                caseItem.CaseExpression.eval(obj, ref localVariables, evalOptions, out caseValue);
            if ((dynamic)value == (dynamic)caseValue)
            {
                if (caseItem.BodyExpression == null)
                {
                    outValue = null;
                    return eControlType.Normal;
                }

                eControlType ctrl = caseItem.BodyExpression.eval(obj, ref localVariables, evalOptions, out outValue);
                if (ctrl == eControlType.Return || ctrl == eControlType.RunTimeError || ctrl == eControlType.Throw ||
                    ctrl == eControlType.GoTo)
                    return ctrl;
                if (ctrl != eControlType.Normal)
                    break;
            }
        }

        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        _valueExpression = _valueExpression?.replace(localVariables);
        if (_valueExpression is ConstantExpressionNode value)
        {
            foreach (SwitchCaseExpressionNode caseItem in Cases)
            {
                if (caseItem.CaseExpression != null)
                    caseItem.CaseExpression = caseItem.CaseExpression.replace(localVariables);
                if (caseItem.BodyExpression != null)
                    caseItem.BodyExpression = caseItem.BodyExpression.replace(localVariables);
                if (caseItem.CaseExpression is ConstantExpressionNode caseValue && caseValue.Value == value.Value)
                    return caseItem.BodyExpression;
            }
        }
        else
        {
            foreach (SwitchCaseExpressionNode caseItem in Cases)
            {
                if (caseItem.CaseExpression != null)
                    caseItem.CaseExpression = caseItem.CaseExpression.replace(localVariables);
                if (caseItem.BodyExpression != null)
                    caseItem.BodyExpression = caseItem.BodyExpression.replace(localVariables);
            }
        }

        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        _valueExpression = _valueExpression?.processAndReplace(func, param);
        foreach (SwitchCaseExpressionNode caseItem in Cases)
        {
            if (caseItem.CaseExpression != null)
                caseItem.CaseExpression = caseItem.CaseExpression.processAndReplace(func, param);
            if (caseItem.BodyExpression != null)
                caseItem.BodyExpression = caseItem.BodyExpression.processAndReplace(func, param);
        }

        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        List<SwitchCaseExpressionNode> cs = [];
        foreach (SwitchCaseExpressionNode item in Cases)
        {
            cs.Add(item.Clone());
        }

        return new SwitchExpressionNode(_valueExpression.clone(), cs, DefaultExpression.clone());
    }

    public override string toText()
    {
        string txt = "switch(" + _valueExpression?.toText() + "," + DefaultExpression.toText();
        foreach (SwitchCaseExpressionNode item in Cases)
            txt += "," + item.CaseExpression.toText() + "," + item.BodyExpression.toText();
        return txt;
    }

    protected override string GetNodeKey()
    {
        return _valueExpression?.toText();
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        _valueExpression?.FetchNodes<T>(dic);
        DefaultExpression?.FetchNodes<T>(dic);
        foreach (SwitchCaseExpressionNode item in Cases)
        {
            item.CaseExpression?.FetchNodes<T>(dic);
            item.BodyExpression?.FetchNodes<T>(dic);
        }
    }
}

public class SwitchCaseExpressionNode(ExpressionNode caseExpression,
    ExpressionNode bodyExpression) //: ExpressionNode
{
    public ExpressionNode CaseExpression { get; internal set; } = caseExpression;
    public ExpressionNode BodyExpression { get; internal set; } = bodyExpression;

    internal SwitchCaseExpressionNode Clone()
    {
        return new SwitchCaseExpressionNode(CaseExpression.clone(), BodyExpression.clone());
    }
}
