namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessModel;

public class BusinessRuleLogicsDefinitions : CRUDDefinition
{
    protected override void AdditionalForms()
    {
        _ = DefineForm<SubIndex>();
        _ = DefineForm<SubCreate>();
        _ = DefineForm<SubEdit>();
        _ = DefineForm<SubDetail>();
        _ = DefineForm<SubDelete>();
    }
    private static void AddFormFields(FormDefinition form)
    {
        _ = form.AddField(nameof(BusinessRuleLogics.Condition), eControlTypeId.MultilineTextInput);
        _ = form.AddProperty(eControlPropertyId.DoubleWidth);
        _ = form.AddField(nameof(BusinessRuleLogics.Logic), eControlTypeId.MultilineTextInput);
        _ = form.AddProperty(eControlPropertyId.DoubleWidth);
        _ = form.AddField(nameof(BusinessRuleLogics.Type));
        _ = form.AddField(nameof(BusinessRuleLogics.ToField), eControlPropertyId.DoubleWidth);
        _ = form.AddField(nameof(BusinessRuleLogics.AssociationNamespace), eControlPropertyId.DoubleWidth);
        _ = form.AddField(nameof(BusinessRuleLogics.AssociationEntity), eControlPropertyId.DoubleWidth);
        _ = form.AddField(nameof(BusinessRuleLogics.AssociationField), eControlPropertyId.DoubleWidth);
    }
    protected override void UIRules(FormDefinition form)
    {
        form.ShowHide(nameof(BusinessRuleLogics.Type), $"(q[{nameof(BusinessRuleLogics.Type)}] == 3) || (q[{nameof(BusinessRuleLogics.Type)}] == 4)",
            nameof(BusinessRuleLogics.AssociationNamespace),
            nameof(BusinessRuleLogics.AssociationEntity), nameof(BusinessRuleLogics.AssociationField));
        form.FilterFormula(nameof(BusinessRuleLogics.AssociationNamespace), nameof(BusinessRuleLogics.AssociationEntity), $"q[{nameof(BusinessRuleLogics.AssociationNamespace)}] == MetaNamespaceId");
    }

    protected class SubIndex : SubIndexForm
    {
        public override string SubjectId => "BusinessRuleVersion";
        protected override void ViewModel()
        {
            _ = AddColumn(nameof(BusinessRuleLogics.Condition));
            _ = AddColumn(nameof(BusinessRuleLogics.Logic));
            _ = AddColumn(nameof(BusinessRuleLogics.ToField));
        }
    }
    protected class SubCreate : SubCreateForm
    {
        public override string SubjectId => "BusinessRuleVersion";
        protected override void ViewModel()
        {
            AddFormFields(this);
        }

    }
    protected class SubEdit : SubEditForm
    {
        public override string SubjectId => "BusinessRuleVersion";
        protected override void ViewModel()
        {
            AddFormFields(this);
        }
    }
    protected class SubDetail : SubDetailForm
    {
        public override string SubjectId => "BusinessRuleVersion";
        protected override void ViewModel()
        {
            AddFormFields(this);
        }
    }
    protected class SubDelete : SubDeleteForm
    {
        public override string SubjectId => "BusinessRuleVersion";
        protected override void ViewModel()
        {
            AddFormFields(this);
        }
    }
}
