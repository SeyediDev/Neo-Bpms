namespace Neo.Bpms.Domain.Modeling.UiDefinitions.CmmnConfig;

public class BPMNEngineDefinitions : CRUDDefinition
{
    protected override void IndexFormOrderBy(FormDefinition form)
    {
        base.IndexFormOrderBy(form);
        form.AddOrderBy(nameof(BPMNEngine.ProcessName));
        form.AddOrderBy(nameof(BPMNEngine.MachineName));
    }
}
