namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

public class DataStore(BpmnDefinitions bpmnDefinitions, string id, string name,
    ItemDefinition itemSubjectRef, bool isUnlimited, int capacity) : RootElement(bpmnDefinitions, id, name), IItemAwareElement
{
    //		public string name { get; set; }

    /// <summary>
    /// default is true
    /// </summary>
    // ReSharper disable once MemberInitializerValueIgnored
    public bool isUnlimited = isUnlimited;

    public int capacity = capacity;

    public ItemDefinition itemSubjectRef { get; set; } = itemSubjectRef;
    public DataState dataState { get; set; }
    public bool CheckItem(string itemId, string itemName)
    {
        return ItemAwareElement.CheckItem(this, itemId, itemName);
    }

    public string NamespaceId => itemSubjectRef?.structure?.NamespaceId;
    public string EntityId => itemSubjectRef?.structure?.Id;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not DataStore newItem)
        {
            return;
        }

        Name = newItem.Name;
        isUnlimited = newItem.isUnlimited;
        capacity = newItem.capacity;
        dataState = newItem.dataState;
        itemSubjectRef = newItem.itemSubjectRef;
    }
}
