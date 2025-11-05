namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class BlockExpressionNode : ExpressionNode
{
    protected List<ExpressionNode> Expressions;
    private LocalParameters _localVariables;

    protected override int getPriority()
    {
        return 0;
    }

    internal void SetLocalVariable(string name, object value)
    {
        _localVariables ??= []; //todo
        _localVariables.AddOrUpdate(name, value);
    }

    public void AddExpression(ExpressionNode expression)
    {
        Expressions.Add(expression);
    }

    public BlockExpressionNode()
        : base(eNodeType.Block)
    {
        Expressions = [];
    }

    public BlockExpressionNode(eNodeType nodeType)
        : base(nodeType)
    {
        Expressions = [];
    }
    //public BlockExpressionNode(IEnumerable<ExpressionNode>expressions, eNodeType nodeType)
    //	: base(nodeType)
    //{
    //	this.expressions = new List<ExpressionNode>(expressions);
    //}


    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        outValue = null;
        List<object> ov = null;
        foreach (KeyValuePair<string, object> variable in localVariables)
        {
            SetLocalVariable(variable.Key, variable.Value);
        }

        foreach (ExpressionNode expression in Expressions)
        {
            if (expression == null) continue;
            eControlType ctrl = expression.eval(obj, ref localVariables, evalOptions, out object outValue1);
            if (ctrl == eControlType.Return)
                outValue = outValue1;
            if (ctrl != eControlType.Normal)
                return ctrl;
            if (outValue1 != null)
            {
                if (outValue == null)
                {
                    ov = [];
                }

                ov?.Add(outValue1);
            }
        }

        if (ov != null)
            outValue = ov;
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        List<ExpressionNode> list = [.. Expressions.Select(item => item?.replace(localVariables))];
        Expressions = list;
        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        List<ExpressionNode> list = [];
        foreach (ExpressionNode item in Expressions)
        {
            list.Add(item?.processAndReplace(func, param));
        }

        Expressions = list;
        return func(this, param);
    }

    public override ExpressionNode clone()
    {
        List<ExpressionNode> expressions = null;
        if (Expressions != null)
        {
            expressions = [];
            foreach (ExpressionNode item in Expressions)
            {
                expressions.Add(item.clone());
            }
        }

        //LocalParameters _localVariables = new LocalParameters(user);
        return new BlockExpressionNode
        {
            Expressions = expressions,
            _localVariables = _localVariables
        };
    }

    public override string toText()
    {
        string s = "{\r\n";
        if (Expressions != null)
        {
            foreach (ExpressionNode expression in Expressions)
                s += "\t" + expression.toText() + ";\r\n";
        }

        s += "}\r\n";
        return s;
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        if (Expressions != null)
        {
            foreach (ExpressionNode expression in Expressions)
                expression?.FetchNodes<T>(dic);
        }
    }
}
