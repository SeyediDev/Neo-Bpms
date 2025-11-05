namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

/// <summary>
/// 
/// </summary>
public interface IItemAwareElement
{
    string Name { get; set; }
    ItemDefinition itemSubjectRef { get; set; }
    DataState dataState { get; set; }
    bool CheckItem(string itemId, string itemName);
}
