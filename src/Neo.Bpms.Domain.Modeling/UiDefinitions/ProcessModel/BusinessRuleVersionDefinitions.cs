namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessModel;

public class BusinessRuleVersionDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("U");
        AdditionalForms();
    }
    protected override void CUDFormsSubTables(CUDForm form)
    {
        _ = form.AddSubTable(nameof(BusinessRuleLogics), nameof(BusinessRuleLogics.BusinessRuleVersion),
            "BusinessRuleVersion", "منطق های قوانین کسبو کار");
    }
}
