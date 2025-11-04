namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Entities;

public interface IMapCRUDDefinition
{
    string FirstParent { get; }
    string SecondParent { get; }
    void FirstColumns(FormDefinition form);
    void SecondColumns(FormDefinition form);
    void FirstViewModel(FormDefinition form);
    void SecondViewModel(FormDefinition form);
    void ExtraEditViewModel(FormDefinition form) { }
}
public abstract class MapCRUDDefinition : CRUDDefinition, IMapCRUDDefinition
{
    protected override void AdditionalForms()
    {
        base.AdditionalForms();
        DefineForm<FirstIndex>();
        DefineForm<FirstCreate>();
        DefineForm<FirstEdit>();
        DefineForm<FirstDetail>();
        DefineForm<FirstDelete>();

        DefineForm<SecondIndex>();
        DefineForm<SecondCreate>();
        DefineForm<SecondEdit>();
        DefineForm<SecondDetail>();
        DefineForm<SecondDelete>();
    }
    public abstract void FirstColumns(FormDefinition form);
    public abstract void SecondColumns(FormDefinition form);
    public virtual void ExtraEditViewModel(FormDefinition form) { }
    public abstract void FirstViewModel(FormDefinition form);
    public abstract void SecondViewModel(FormDefinition form);
    public abstract string FirstParent { get; }
    public abstract string SecondParent { get; }

    protected class FirstIndex : SubIndexForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).FirstParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).FirstColumns(this);
        }
    }
    protected class SecondIndex : SubIndexForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).SecondParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).SecondColumns(this);
        }
    }
    protected class FirstCreate : SubCreateForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).FirstParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).FirstViewModel(this);
        }
    }
    protected class SecondCreate : SubCreateForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).SecondParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).SecondViewModel(this);
        }
    }
    protected class FirstEdit : SubEditForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).FirstParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).FirstViewModel(this);
            ((IMapCRUDDefinition)entityDefinition).ExtraEditViewModel(this);
        }
    }
    protected class SecondEdit : SubEditForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).SecondParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).SecondViewModel(this);
            ((IMapCRUDDefinition)entityDefinition).ExtraEditViewModel(this);
        }
    }
    protected class FirstDetail : SubDetailForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).FirstParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).FirstViewModel(this);
        }
    }
    protected class SecondDetail : SubDetailForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).SecondParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).SecondViewModel(this);
        }
    }
    protected class FirstDelete : SubDeleteForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).FirstParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).FirstViewModel(this);
        }
    }
    protected class SecondDelete : SubDeleteForm
    {
        public override string SubjectId => ((IMapCRUDDefinition)entityDefinition).SecondParent;
        protected override void ViewModel()
        {
            ((IMapCRUDDefinition)entityDefinition).SecondViewModel(this);
        }
    }
}