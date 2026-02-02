namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessModel;

public class BusinessRuleVersionDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("U");
        AdditionalForms();
    }
    protected override void CUDFormsSubTables()
    {
        _ = form.AddSubTable(nameof(BusinessRuleLogics), nameof(BusinessRuleLogics.BusinessRuleVersion),
            "BusinessRuleVersion", "منطق های قوانین کسبو کار");
    }
}
