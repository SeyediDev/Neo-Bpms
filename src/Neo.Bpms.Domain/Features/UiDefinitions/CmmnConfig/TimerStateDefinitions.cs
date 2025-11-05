namespace Neo.Bpms.Domain.Features.UiDefinitions.CmmnConfig;

public class TimerStateDefinitions : CRUDDefinition
{
    protected override void IndexFormOrderBy(FormDefinition form)
    {
        form.AddOrderBy("ProcessName");
        form.AddOrderBy("Name");
        base.IndexFormOrderBy(form);
    }
}
