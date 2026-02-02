namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public class DefaultCRUDDefinition : CRUDDefinition { }
public abstract class CRUDDefinition<TEntity> : CRUDDefinition
{
    public override Type DefinitionEntity => typeof(TEntity);
}
public abstract class CRUDDefinition : EntityDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("CRUDL");
        AdditionalForms();
    }

    protected virtual void AdditionalForms()
    {
    }

    /// <summary>
    /// Defines Create - Read - Update - Delete forms
    /// </summary>
    /// <param name="template"></param>
    protected void DefineCRUDForms(string template)
    {
        if (template.Contains('C'))
        {
            _ = DefineCRUDForm<CreateForm>();
        }

        if (template.Contains('U'))
        {
            _ = DefineCRUDForm<EditForm>();
        }

        if (template.Contains('D'))
        {
            _ = DefineCRUDForm<DeleteForm>();
        }

        if (template.Contains('R'))
        {
            _ = DefineCRUDForm<DetailForm>();
        }

        if (template.Contains('L'))
        {
            _ = DefineCRUDForm<IndexForm>();
        }
    }

    protected bool DefineCRUDForm<T>() where T : FormDefinition
    {
        return DefineForm<T>();
    }

    protected virtual void CUDFormsViewModel()
    {
        form.AddAllFields(FormField.Type.Field, false, false);
    }

    protected virtual void CUDFormsSubTables()
    {
    }

    protected virtual void CreateFormViewModel()
    {
        CUDFormsViewModel();
    }
    protected virtual void CreateFormOperation()
    {
    }

    protected virtual void EditFormViewModel()
    {
        CUDFormsViewModel();
    }

    protected virtual void CreateFormSubTables()
    {
        CUDFormsSubTables();
    }

    protected virtual void EditFormSubTables()
    {
        CUDFormsSubTables();
    }

    protected virtual void IndexFormSubTables()
    {
        CUDFormsSubTables();
    }
    
    /// <summary>
    /// Defines UI Rules
    /// </summary>
    /// <param name="form">form</param>
    protected virtual void UIRules(FormDefinition form)
    {
    }
    
    protected virtual void UIRules()
    {
        UIRules(form);
    }

    protected virtual void CreateFormUIRules(FormDefinition form)
    {
    }

    protected virtual void CreateFormUIRules()
    {
        UIRules();
        CreateFormUIRules(form);
    }

    protected virtual void EditFormUIRules(FormDefinition form)
    {
    }
    protected virtual void EditFormUIRules()
    {
        UIRules();
        EditFormUIRules(form);
    }
    protected virtual void DeleteFormUIRules(FormDefinition form)
    {
    }
    protected virtual void DeleteFormUIRules()
    {
        UIRules();
        DeleteFormUIRules(form);
    }
    protected virtual void IndexFormUIRules(FormDefinition form)
    {
    }
    protected virtual void IndexFormUIRules()
    {
        UIRules();
        IndexFormUIRules(form); ;
    }

    protected virtual void CUDFormsDataOperations()
    {
        if (!Entity.IsStateBase)
        {
            return;
        }

        if (form.form.outputStateId > 0)
        {
            return;
        }

        EntityStateCategory category = form.form.FormType == Form.eFormType.Create
            ? EntityStateCategory.ActiveNode
            : form.form.FormType == Form.eFormType.VirtualDelete
                ? EntityStateCategory.BackupNode
                : EntityStateCategory.None;
        if (category != EntityStateCategory.None)
        {
            EntityState state = Entity.GetStateCollection()?.States.Values
                .FirstOrDefault(s => (s.category & (uint)category) != 0);
            if (state != null)
            {
                _ = form.SetOutputStateId(state.Id);
            }
        }
    }

    protected virtual void IndexFormFilters(FormDefinition form)
    {
        form.AddAllFields(FormField.Type.FilterField, false, true);
    }
    protected virtual void IndexFormFilters()
    {
        IndexFormFilters(form);
    }

    protected virtual void IndexFormViewModel(FormDefinition form)
    {
        form.AddAllFields(FormField.Type.ColumnField, false, true);
    }
    protected virtual void IndexFormViewModel()
    {
        IndexFormViewModel(form);
    }

    protected virtual void IndexFormOrderBy(FormDefinition form)
    {
    }
    protected virtual void IndexFormOrderBy()
    {
        IndexFormOrderBy(form);
    }

    protected virtual void ReportFilters(ReportDefinition report)
    {
        report.AddAllFields(FormField.Type.FilterField, true, true);
    }

    protected virtual void ReportDataSources(ReportDefinition report)
    {
        report.AddAllFields(FormField.Type.ColumnField, true, true);
    }

    protected virtual void ReportGroupBy(ReportDefinition report)
    {
        report.AddAllAssociationToGroupBy();
    }

    protected virtual void PossibleSubReports(ReportDefinition report)
    {
    }
    protected virtual void ReportUiRules(ReportDefinition report)
    {
    }

    public abstract class CUDForm : FormDefinition
    {
        protected CRUDDefinition CrudDefinition => (CRUDDefinition)entityDefinition;
        public override List<string>? Roles => entityDefinition.Roles;
        public virtual string SubjectId => null;
        /// <summary>
        /// Defines Form View Model
        /// </summary>
        protected override void ViewModel()
        {
            CrudDefinition.CUDFormsViewModel();
            CrudDefinition.CUDFormsSubTables();
        }

        /// <summary>
        /// Defines Form Data Operations
        /// </summary>
        protected override void DataOperations()
        {
            CrudDefinition.CUDFormsDataOperations();
        }

        /// <summary>
        /// Defines Form UI Rules
        /// </summary>
        protected override void UIRules()
        {
            CrudDefinition.UIRules();
        }
    }

    public class CommandForm : CreateForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.CommandForm, SubjectId);
        }
    }
    public class CreateSubjectForm<TForm> : CreateForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
    }
    public class CreateForm : CUDForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Create, SubjectId);
        }

        protected override void FormOperation()
        {
            CrudDefinition.CreateFormOperation();
        }

        protected override void ViewModel()
        {
            CrudDefinition.CreateFormViewModel();
            CrudDefinition.CreateFormSubTables();
        }

        protected override void UIRules()
        {
            CrudDefinition.CreateFormUIRules();
        }
    }

    public class SubjectEditForm2<TForm> : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
    }
    
    public class SubjectEditForm<TForm>(string name) : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
        public override string Name => name;
    }

    public class EditForm : CUDForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Edit, SubjectId);
        }

        protected override void ViewModel()
        {
            CrudDefinition.EditFormViewModel();
            CrudDefinition.EditFormSubTables();
        }

        protected override void UIRules()
        {
            CrudDefinition.EditFormUIRules();
        }
    }
    
    public class SpecificURLForm : CUDForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.SpecificURL, SubjectId);
        }

        protected override void ViewModel()
        {
            CrudDefinition.EditFormViewModel();
            CrudDefinition.EditFormSubTables();
        }

        protected override void UIRules()
        {
            CrudDefinition.EditFormUIRules();
        }
    }

    public class ProcessCreateForm : CreateForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.ProcessCreate, SubjectId);
        }
    }
    
    public class InProcessForm : CreateForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.WorkItem, SubjectId);
        }
    }

    public class SubjectDetailForm<TForm> : DetailForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
    }
    public class DetailForm : CUDForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Detail, SubjectId);
        }
    }

    public class SubjectDeleteForm<TForm> : DeleteForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
    }
    public class DeleteForm : CUDForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.VirtualDelete, SubjectId);
        }
        protected override void UIRules()
        {
            CrudDefinition.DeleteFormUIRules();
        }
    }
    protected virtual string IndexName { get; set; }

    public class SubjectIndexForm<TForm> : IndexForm, ISubjectFormDefinition
    {
        public override string SubjectId => typeof(TForm).Name;
    }
    public class IndexForm : CUDForm
    {
        protected override Form Identify()
        {
            return string.IsNullOrEmpty(CrudDefinition.IndexName)
                ? DefineForm(Form.eFormType.Index, SubjectId)
                : DefineForm(CrudDefinition.IndexName, CrudDefinition.IndexName, Form.eFormType.Index, SubjectId);
        }

        protected override void Filters()
        {
            CrudDefinition.IndexFormFilters();
        }

        protected override void ViewModel()
        {
            CrudDefinition.IndexFormViewModel();
            CrudDefinition.IndexFormOrderBy();
        }
        protected override void UIRules()
        {
            CrudDefinition.IndexFormUIRules();
        }
    }
    #region SubForm
    protected abstract class SubIndexForm : IndexForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Index, SubjectId);
        }

        protected override void Filters()
        {
        }

        protected override void ViewModel()
        {
            AddAllFields(FormField.Type.ColumnField, false, true);
        }
    }

    protected class SubCreateForm : CreateForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Create, SubjectId);
        }

        protected override void ViewModel()
        {
            AddAllFields(FormField.Type.Field, false, true);
        }
    }

    protected class SubEditForm : EditForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Edit, SubjectId);
        }

        protected override void ViewModel()
        {
            AddAllFields(FormField.Type.Field, false, true);
        }
    }

    protected class SubDetailForm : DetailForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.Detail, SubjectId);
        }
    }

    protected class SubDeleteForm : DeleteForm
    {
        protected override Form Identify()
        {
            return DefineForm(Form.eFormType.VirtualDelete, SubjectId);
        }
    }
    #endregion
    #region Report
    public partial class PublicReport : ReportDefinition
    {
        protected CRUDDefinition CrudDefinition => (CRUDDefinition)entityDefinition;
        public override string EnName => $"{entity.Name} Report";
        public override string Name => "گزارش " + entity.Name;

        protected override void Filters()
        {
            CrudDefinition.ReportFilters(this);
        }

        protected override void DataSources()
        {
            CrudDefinition.ReportDataSources(this);
        }

        protected override void UIRules()
        {
            CrudDefinition.ReportUiRules(this);
        }
        protected override void DefineGroupBy()
        {
            CrudDefinition.ReportGroupBy(this);
        }

        protected override void PossibleSubReports()
        {
            CrudDefinition.PossibleSubReports(this);
        }
    }
    #endregion
    #region Dashboard
    protected virtual void DashboardUiRules(DashboardDefinition dashboard)
    {
    }
    public class PublicDashboard : DashboardDefinition
    {
        public override string Name => "داشبورد";
        public override string EnName => "Dashboard";
        protected override Form Identify()
        {
            return DefineDashboard(Name, EnName);
        }
        //protected CRUDDefinition CrudDefinition => (CRUDDefinition)entityDefinition;

        protected override void UIRules()
        {
            //CrudDefinition.DashboardUiRules(this);
        }/*
        protected override void Filters()
        {
            AddSpecialFilterField(nameof(ActivityInstanceRecord.Process));
            AddProperty(eControlPropertyId.Required);
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ProcessVersion));
            AddSpecialFilterField(nameof(ActivityInstanceRecord.FromDate));
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ToDate));
        }
*/
        protected override void DataSources()
        {
            base.DataSources();
            _ = AddReport(entity.NamespaceId, entity.Id, nameof(PublicReport));
        }
    }
    #endregion
}
