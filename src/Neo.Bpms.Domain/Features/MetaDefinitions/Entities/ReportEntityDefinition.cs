namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public abstract class ReportEntityDefinition : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("RL");
        AdditionalForms();
    }
}
