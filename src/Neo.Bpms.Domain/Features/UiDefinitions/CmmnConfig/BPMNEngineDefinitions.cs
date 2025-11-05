using Neo.Bpms.Domain.Entities.CmmnConfig;

namespace Neo.Bpms.Domain.Features.UiDefinitions.CmmnConfig;

public class BPMNEngineDefinitions : CRUDDefinition
{
    protected override void IndexFormOrderBy(FormDefinition form)
    {
        base.IndexFormOrderBy(form);
        form.AddOrderBy(nameof(BPMNEngine.ProcessName));
        form.AddOrderBy(nameof(BPMNEngine.MachineName));
    }
}
