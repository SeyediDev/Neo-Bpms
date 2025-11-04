namespace Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;

public class ListExpressionNode : ExpressionNode
{
    public List<ExpressionNode> Items { get; private set; }

    public ListExpressionNode(IEnumerable<ExpressionNode> items, int depth = 0) :
        base(eNodeType.List)
    {
        Items = [.. items];
        Depth = depth;
    }

    public ListExpressionNode(ExpressionNode left, ExpressionNode right, int depth = 0) :
        base(eNodeType.List)
    {
        Items = [left, right];
        Depth = depth;
    }

    public override eControlType eval(object obj, ref LocalParameters localVariables, EvalOptions evalOptions, out object outValue)
    {
        List<object> list = [];
        if (Items != null)
        {
            foreach (ExpressionNode item in Items)
            {
                object o;
                if (item != null)
                    item.eval(obj, ref localVariables, evalOptions, out o);
                else
                    o = null;
                list.Add(o);
            }
        }
        outValue = list;
        return eControlType.Normal;
    }

    public override ExpressionNode replace(LocalParameters localVariables)
    {
        if (Items != null)
        {
            List<ExpressionNode> list = [];
            foreach (ExpressionNode item in Items)
            {
                if (Items == null) continue;
                list.Add(item.replace(localVariables));
            }
            Items = list;
        }
        return this;
    }

    public override ExpressionNode processAndReplace(Func<ExpressionNode, object, ExpressionNode> func,
        object param)
    {
        if (Items != null)
        {
            List<ExpressionNode> list = [];
            foreach (ExpressionNode item in Items)
            {
                if (Items == null) continue;
                list.Add(item.processAndReplace(func, param));
            }
            Items = list;
        }
        return func(this, param);
    }

    public ExpressionNode Flatten()
    {
        bool changed;
        do
        {
            changed = false;
            foreach (ExpressionNode item in Items)
            {
                if (item is not ListExpressionNode node) continue;
                Items.Remove(node);
                Items.AddRange(node.Items);
                changed = true;
                break;
            }
        } while (changed);
        return this;
    }

    public override ExpressionNode clone()
    {
        List<ExpressionNode> items = [];
        foreach (ExpressionNode item in Items)
        {
            if (item != null) //todo
                items.Add(item.clone());
        }
        return new ListExpressionNode(items, Depth);
    }

    public override string toText()
    {
        string txt = "(";
        if (Items != null)
        {
            bool bFirst = true;
            foreach (ExpressionNode item in Items)
            {
                if (!bFirst) txt += ",";
                txt += item.toText();
                bFirst = false;
            }
        }
        txt += ")";
        return txt;
    }

    protected override int getPriority()
    {
        return 15;
    }

    protected override void FetchNodesInChildren<T>(Dictionary<string, List<ExpressionNode>> dic)
    {
        if (Items != null)
        {
            foreach (ExpressionNode item in Items)
            {
                item.FetchNodes<T>(dic);
            }
        }
    }
}
