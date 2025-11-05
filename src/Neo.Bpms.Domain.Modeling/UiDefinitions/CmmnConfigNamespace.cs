namespace Neo.Bpms.Domain.Modeling.UiDefinitions;

public class CmmnConfigNamespace : ModelDefinition
{
    protected override bool Identify()
    {
        return DefineModel(nameof(BpmsSchema.CmmnConfig), "مدیریت سامانه", nameof(BpmsSchema.CmmnConfig), nameof(DomainProvider.Domain));
    }

    protected override void Entities()
    {
        DefineEntity<ScheduledReportLog>();

        DefineEntity<TreeConfig>();
        DefineEntity<TreeNodeType>();
        DefineEnumeration<TreeNodeTypeId>(true);

        DefineEntity<TimerState>();
        DefineEntity<TimerStateLog>();

        DefineEntity<BPMNEngine>();
    }

    protected override void Enumerations()
    {

    }
}
