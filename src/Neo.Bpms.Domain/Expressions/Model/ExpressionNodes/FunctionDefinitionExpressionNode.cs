namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class FunctionDefinitionExpressionNode(string funcName, IEnumerable<string> parameterNames, ExpressionNode body) : ExpressionNode(eNodeType.FunctionDefinition)
{
    private readonly List<string> _parameterNames = [.. parameterNames];
    public readonly string FunctionName = funcName;
    private ExpressionNode _body = body;

    //public void setParamValue(int paramIndex, object value)
    //{
    //	string paramName = (paramIndex < parameterNames.Count) ? parameterNames[paramIndex] : null;
    //	if (paramName != null)
    //		setLocalVariable(paramName, value);
    //}
    //public void setParamValue(string name, object value)
    //{
    //	string paramName = parameterNames.FirstOrDefault(s => s == name);
    //	if (paramName != null)
    //		setLocalVariable(paramName, value);
    //}
    protected override int getPriority() { return 0; }
    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        localVariables.AddOrUpdate(FunctionName, this);
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        return this;
    }
    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func, object param)
    {
        _body = _body?.processAndReplace(func, param);
        return func(this, param);
    }
    public override ExpressionNode clone()
    {
        /*List<string> paramNames = null;
			if (this.parameterNames != null)
			{
				paramNames = new List<string>();
				foreach (var item in this.parameterNames )
				{
					paramNames.Add(item);
				}
			}*/
        return new FunctionDefinitionExpressionNode(FunctionName, _parameterNames, _body.clone());
    }
    public override string toText()
    {
        string txt = FunctionName + "(";
        if (_parameterNames != null)
        {
            bool bFirst = true;
            foreach (string item in _parameterNames)
            {
                if (!bFirst) txt += ",";
                txt += item;
                bFirst = false;
            }
        }
        txt += ")";
        txt += "\n{\n";
        txt += _body.toText();
        txt += "\n}";
        return txt;
    }
    protected override string GetNodeKey()
    {
        return FunctionName;
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        _body?.FetchNodes<T>(dic);
    }
}
