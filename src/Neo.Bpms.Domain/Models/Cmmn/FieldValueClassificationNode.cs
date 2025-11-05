namespace Neo.Bpms.Domain.Models.Cmmn;

public class FieldValueClassificationNode
{
    public FieldValueClassificationNode(object MinValue, object MaxValue)
    {
        this.MinValue = MinValue;
        this.MaxValue = MaxValue;
        Values = null;
    }
    public FieldValueClassificationNode(IEnumerable<object> Values)
    {
        MinValue = null;
        MaxValue = null;
        this.Values = Values;
    }
    public object MinValue, MaxValue;
    public IEnumerable<object> Values;
    public void addSubNode(FieldValueClassificationNode subNode)
    {
        SubNodes ??= [];
        SubNodes.Add(subNode);
    }
    public List<FieldValueClassificationNode> SubNodes;
}