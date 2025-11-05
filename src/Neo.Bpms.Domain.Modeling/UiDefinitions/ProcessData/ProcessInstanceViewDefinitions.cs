namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

public class ProcessInstanceViewDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }
}
