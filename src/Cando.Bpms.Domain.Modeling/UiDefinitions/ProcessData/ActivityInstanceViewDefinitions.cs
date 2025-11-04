namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

public class ActivityInstanceViewDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }
}
