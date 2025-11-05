using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

internal class ExpressionInInstance
{
    internal ExpressionInInstance(ProcessInstance pi, FlowNodeInstance ai,
        LocalParameters inputParams, BpmnExpression conditionExpression)
        : this(pi, ai, inputParams, (ExpressionTree)null)
    {
        if (conditionExpression is FormalExpression exp)
        {
            _expression = exp.Expression;
        }
    }
    internal ExpressionInInstance(ProcessInstance pi, FlowNodeInstance ai,
        LocalParameters inputParams, ExpressionTree expression)
    {
        _pi = pi;
        _ai = ai;
        _inputParams = inputParams;
        _expression = expression;
    }

    private readonly ProcessInstance _pi;
    private readonly FlowNodeInstance _ai;
    private readonly LocalParameters _inputParams;
    private readonly ExpressionTree _expression;

    internal bool CheckCondition(string traceName)
    {
        return _expression == null || ExpressionNode.CheckIfTrue(EvalExpression($"Check {traceName}"));
    }
    internal object EvalExpression(string traceName)
    {
        ExpressionNode exp = _expression?.Root?.clone();
        object value = Eval(ref exp);
        if (!string.IsNullOrEmpty(traceName))
        {
            ((ScopeInstance)_ai ?? _pi).LogTrace($"{(_ai != null ? _ai.FlowNode.GetType() + "." + _ai.FlowNode.Id + "." : "")}{traceName}\n\r{_expression?.ExpressionString} \n\r=> {exp?.toText()} \n\r=> {value}");
        }

        return value;
    }

    private object Eval(ref ExpressionNode exp)
    {
        exp = SetExpressionPropertyValues(exp);
        object value;
        try
        {
            LocalParameters lc = new(_pi?.AuditTrail?.User);
            if (_inputParams != null)
            {
                _ = lc.Set(_inputParams);
            }

            value = exp?.Eval(_pi?.Data, lc);
        }
        catch
        {
            value = exp?.toText();
        }

        return value;
    }

    private ExpressionNode SetExpressionPropertyValues(ExpressionNode exp)
    {
        if (_ai != null)
        {
            exp = exp?.processAndReplace(ReplaceInstanceData, _ai);
        }

        if (_pi != null)
        {
            exp = exp?.processAndReplace(ReplaceInstanceData, _pi);
        }

        return exp;
    }

    public static ExpressionNode ReplaceInstanceData(ExpressionNode exp, object instance)
    {
        if (exp is not VariableNameExpressionNode)
        {
            return exp;
        }

        VariableNameExpressionNode vExp = (VariableNameExpressionNode)exp;
        return instance switch
        {
            ActivityInstance ai when ai.GetData(vExp.Name, out object obj) => new ConstantExpressionNode(obj),
            ProcessInstance pi when pi.GetData(vExp.Name, out object obj) => new ConstantExpressionNode(obj),
            _ => exp,
        };
    }
}
