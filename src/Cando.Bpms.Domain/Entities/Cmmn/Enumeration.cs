namespace Neo.Bpms.Domain.Entities.Cmmn;

public class Enumeration : BaseModelClass
{
    public Enumeration(ModelNamespace model, string name, string enName)
        : base(model, enName, name)
    {
    }

    public Enumeration(string namespaceId, string entityId)
        : base(null, entityId, null)
    {
        _namespaceId = namespaceId;
    }

    public Enumeration()
    {
    }

    private string _namespaceId;

    public string NamespaceId
    {
        get { return !string.IsNullOrEmpty(_namespaceId) ? _namespaceId : Parent?.Id; }
        set { _namespaceId = value; }
    }
    public string EntityId => Id;
    public bool DeleteExtraItems { get; set; }


    public Dictionary<string, EnumerationItem> items = [];

    public EnumerationItem addItem(EnumerationItem item)
    {
        if (items.ContainsKey(item.Id)) return null;
        items.TryAdd(item.Id, item);
        return item;
    }

    public EnumerationItem addItem(int id, string itemName)
    {
        EnumerationItem item = new(this, id, itemName, EntityStateCategory.ActiveNode);
        return addItem(item);
    }

    public EnumerationItem getItem(int id)
    {
        return getItem("" + id);
    }

    public EnumerationItem getItem(string id)
    {
        items.TryGetValue(id, out EnumerationItem result);
        return result;
    }
}
