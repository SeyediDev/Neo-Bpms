namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

public interface IFieldAwareElement : IItemAwareElement
{
    string fieldId { get; }
    EntityField field { get; }
}
