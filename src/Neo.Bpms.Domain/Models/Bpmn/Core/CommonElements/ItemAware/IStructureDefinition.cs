namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

public interface IStructureDefinition
{
    object structureRef { get; set; }
    Entity structure { get; }
    EntityField entityField { get; }
}
