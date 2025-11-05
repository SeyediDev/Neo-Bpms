using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

public interface IStructureDefinition
{
    object structureRef { get; set; }
    Entity structure { get; }
    EntityField entityField { get; }
}
