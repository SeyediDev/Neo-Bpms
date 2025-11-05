using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

public interface IFieldAwareElement : IItemAwareElement
{
    string fieldId { get; }
    EntityField field { get; }
}
