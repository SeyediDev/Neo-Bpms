namespace Neo.Bpms.Domain.Features.UiDefinitions.CmmnConfig;

public class TimerStateLogDefinitions : CRUDDefinition
{
    protected override void IndexFormOrderBy(FormDefinition form)
    {
        base.IndexFormOrderBy(form);
        form.AddOrderBy("Name");
        form.AddOrderBy("ProcessName");
        form.AddOrderBy("MachineName");
    }
}
