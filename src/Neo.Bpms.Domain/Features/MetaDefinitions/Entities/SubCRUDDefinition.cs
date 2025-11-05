namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;
public abstract class SubCRUDDefinition<TEntity> : SubCRUDDefinition
{
    public override Type DefinitionEntity => typeof(TEntity);
}
public abstract class SubCRUDDefinition : CRUDDefinition
{
    protected override void AdditionalForms()
    {
        base.AdditionalForms();
        DefineForm<SubIndex>();
        DefineForm<SubCreate>();
        DefineForm<SubEdit>();
        DefineForm<SubDetail>();
        DefineForm<SubDelete>();
    }
    public abstract void SubIndexViewModel(FormDefinition form);
    public abstract void SubViewModel(FormDefinition form);
    public virtual string SubjectId => "Sub";

    protected class SubIndex : SubIndexForm
    {
        protected SubCRUDDefinition SubDefinition => (SubCRUDDefinition)entityDefinition;
        public override string SubjectId => SubDefinition.SubjectId;
        protected override void ViewModel()
        {
            SubDefinition.SubIndexViewModel(this);
        }
    }
    protected class SubCreate : SubCreateForm
    {
        protected SubCRUDDefinition SubDefinition => (SubCRUDDefinition)entityDefinition;
        public override string SubjectId => SubDefinition.SubjectId;
        protected override void ViewModel()
        {
            SubDefinition.SubViewModel(this);
        }

    }
    protected class SubEdit : SubEditForm
    {
        protected SubCRUDDefinition SubDefinition => (SubCRUDDefinition)entityDefinition;
        public override string SubjectId => SubDefinition.SubjectId;
        protected override void ViewModel()
        {
            SubDefinition.SubViewModel(this);
        }
    }
    protected class SubDetail : SubDetailForm
    {
        protected SubCRUDDefinition SubDefinition => (SubCRUDDefinition)entityDefinition;
        public override string SubjectId => SubDefinition.SubjectId;
        protected override void ViewModel()
        {
            SubDefinition.SubViewModel(this);
        }
    }
    protected class SubDelete : SubDeleteForm
    {
        protected SubCRUDDefinition SubDefinition => (SubCRUDDefinition)entityDefinition;
        public override string SubjectId => SubDefinition.SubjectId;
        protected override void ViewModel()
        {
            SubDefinition.SubViewModel(this);
        }
    }
    protected override void UIRules(FormDefinition form)
    {
        base.UIRules(form);
        if (form.form.FormType != Form.eFormType.Create &&
            form.form.FormType != Form.eFormType.ProcessCreate)
        {
            Filters(form);
        }
    }

    protected override void ReportUiRules(ReportDefinition report)
    {
        base.ReportUiRules(report);
        Filters(report);
    }
    protected virtual void Filters(FormDefinition form)
    {
    }
}
