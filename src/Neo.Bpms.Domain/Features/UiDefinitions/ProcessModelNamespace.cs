namespace Neo.Bpms.Domain.Features.UiDefinitions;

public class ProcessModelNamespace : ModelDefinition
{
    protected override bool Identify()
    {
        return DefineModel("Process Model", "مدل فرآیند", nameof(BpmsSchema.ProcessModel), nameof(DomainProvider.Domain));
    }
    protected override void Entities()
    {
        _ = DefineEntity<BPMNProcess>();
        _ = DefineEntity<BPMNProcessVersion>();
        _ = DefineEntity<BPMNInterface>();
        _ = DefineEntity<BPMNOperation>();
        _ = DefineEntity<BPMNFlowNode>();
        _ = DefineEntity<BPMNFlowNodeType>();
        DefineEnumeration<FlowNodeTypeId>(true, "نوع المان فرآیند", "BPMNFlowNodeType");
        _ = DefineEntity<BPMNEventType>();
        DefineEnumeration<Event.eEventType>(true, "نوع‌فراخوانی رویداد", "BPMNEventType");

        _ = DefineEntity<BusinessRule>();
        _ = DefineEntity<BusinessRuleLogics>();
        _ = DefineEntity<BusinessRuleLogicType>();
        DefineEnumeration<BusinessRuleLogicTypeId>(true);
        _ = DefineEntity<BusinessRuleVersion>();
        _ = DefineEntity<BusinessRuleType>();
        DefineEnumeration<BusinessRuleTypeId>(true);
    }
}
