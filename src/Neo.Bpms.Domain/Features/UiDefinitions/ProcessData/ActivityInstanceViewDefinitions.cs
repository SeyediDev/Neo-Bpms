namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessData;

public class ActivityInstanceViewDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }
}
