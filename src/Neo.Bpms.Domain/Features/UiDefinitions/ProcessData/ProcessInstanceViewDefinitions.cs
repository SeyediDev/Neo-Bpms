namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessData;

public class ProcessInstanceViewDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }
}
