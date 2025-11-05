namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public abstract class Sub2CRUDDefinition<TEntity> : SubCRUDDefinition<TEntity>
{
    protected override void AdditionalForms()
    {
        base.AdditionalForms();
        DefineForm<Sub2Index>();
        DefineForm<Sub2Create>();
        DefineForm<Sub2Edit>();
        DefineForm<Sub2Detail>();
        DefineForm<Sub2Delete>();
    }
    public abstract void Sub2IndexViewModel(FormDefinition form);
    public abstract void Sub2ViewModel(FormDefinition form);
    public virtual string SubjectId2 => "Sub";

    protected class Sub2Index : SubIndexForm
    {
        protected Sub2CRUDDefinition<TEntity> Sub2Definition => (Sub2CRUDDefinition<TEntity>)entityDefinition;
        public override string SubjectId => Sub2Definition.SubjectId2;
        protected override void ViewModel()
        {
            Sub2Definition.Sub2IndexViewModel(this);
        }
    }
    protected class Sub2Create : SubCreateForm
    {
        protected Sub2CRUDDefinition<TEntity> Sub2Definition => (Sub2CRUDDefinition<TEntity>)entityDefinition;
        public override string SubjectId => Sub2Definition.SubjectId2;
        protected override void ViewModel()
        {
            Sub2Definition.Sub2ViewModel(this);
        }

    }
    protected class Sub2Edit : SubEditForm
    {
        protected Sub2CRUDDefinition<TEntity> Sub2Definition => (Sub2CRUDDefinition<TEntity>)entityDefinition;
        public override string SubjectId => Sub2Definition.SubjectId2;
        protected override void ViewModel()
        {
            Sub2Definition.Sub2ViewModel(this);
        }
    }
    protected class Sub2Detail : SubDetailForm
    {
        protected Sub2CRUDDefinition<TEntity> Sub2Definition => (Sub2CRUDDefinition<TEntity>)entityDefinition;
        public override string SubjectId => Sub2Definition.SubjectId2;
        protected override void ViewModel()
        {
            Sub2Definition.Sub2ViewModel(this);
        }
    }
    protected class Sub2Delete : SubDeleteForm
    {
        protected Sub2CRUDDefinition<TEntity> Sub2Definition => (Sub2CRUDDefinition<TEntity>)entityDefinition;
        public override string SubjectId => Sub2Definition.SubjectId2;
        protected override void ViewModel()
        {
            Sub2Definition.Sub2ViewModel(this);
        }
    }
}
