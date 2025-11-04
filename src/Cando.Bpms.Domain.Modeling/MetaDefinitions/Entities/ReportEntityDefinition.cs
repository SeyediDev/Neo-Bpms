namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Entities;

public abstract class ReportEntityDefinition : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("RL");
        AdditionalForms();
    }
}
