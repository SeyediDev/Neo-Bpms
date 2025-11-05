using System.Text.Json.Serialization;
using Neo.Bpms.Domain.Entities.Base.Audit;
using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Model.UI.Forms;

/// <summary>
/// Form definition
/// </summary>
public partial class Form : BaseModelClass, IEntityPage, IEntityItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Form"/> class.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName"></param>
    /// <param name="dataOperation">The data operation.</param>
    /// <param name="formType">Type of the form.</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    protected Form(Entity entity, string id, string name, string enName, DataOperation dataOperation,
        eFormType formType, string formSubjectId = null)
        : base(entity, id, name)
    {
        DataOperation = dataOperation;
        FormType = formType;
        FormSubjectId = formSubjectId;
        EnName = enName;
        dataOperation.type = formType switch
        {
            eFormType.Create or eFormType.ProcessCreate => DataOperation.eOperationType.Constructor,
            eFormType.Delete => DataOperation.eOperationType.Destructor,
            _ => DataOperation.eOperationType.Update,
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Form"/> class.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName"></param>
    /// <param name="formType">Type of the form.</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    public Form(Entity entity, string id, string name, string enName, eFormType formType,
        string formSubjectId = null) //, 
        : base(entity, id, name)
    {
        FormSubjectId = formSubjectId;
        FormType = formType;
        SetFormDateOperation();
        formFields = [];
        EnName = enName;
    }

    public Type BeforSaveCommand { get; private set; }
    public Type AfterSaveCommand { get; private set; }
    public void SetCommands(Type beforSaveCommand, Type afterSaveCommand)
    {
        BeforSaveCommand = beforSaveCommand;
        AfterSaveCommand = afterSaveCommand;
    }

    public void SetFormDateOperation()
    {
        if (FormType != eFormType.Detail && FormType != eFormType.Index)
        {
            if (DataOperation == null)
            {
                DataOperation.eOperationType type = FormType switch
                {
                    eFormType.Create or eFormType.ProcessCreate => DataOperation.eOperationType.Constructor,
                    eFormType.Delete => DataOperation.eOperationType.Destructor,
                    _ => DataOperation.eOperationType.Update,
                };
                DataOperation = new DataOperation(entity, Id, Name, type);
                entity.addDataOperation(DataOperation);
            }
            else
            {
                DataOperation.Clear();
            }
        }
        else
            DataOperation = null;
    }

    protected Form()
    {
        formFields = [];
    }

    [JsonIgnore]
    public string UniqueId => $"{NamespaceId}.{EntityId}.{Id}";
    public UiEntity entity => Parent as UiEntity;
    public UiEntity Entity => Parent as UiEntity;
    public string SpecificUrl { get; set; }
    public string SpecificViewPage { get; set; }
    public List<string> PassingParameters { get; set; }
    public DataOperation DataOperation { get; set; }

    public bool DontForceActiveStatesFilter { get; set; }
    public string InputRecordIdFormula { get; set; }
    public bool isReadOnly = false;
    public string NamespaceId => entity.NamespaceId;

    public List<IncludeEntity> Includes { get; set; }
    public List<FormOrderBy> OrderBys { get; set; }
    public List<FormField> formFields { get; set; } = [];
    public List<UIRule> UiRules { get; private set; } = [];
    public eFormType FormType { get; set; }
    public string FormSubjectId { get; set; }

    [XmlIgnore]
    public int outputStateId
    {
        get { return DataOperation?.outputStateId ?? 0; }
        set { DataOperation.outputStateId = value; }
    }

    public string EntityId => entity.Id;

    public string PagePackId => $"{FormType:G}_{NamespaceId}_{EntityId}_{Id}";

    public string IndexPagePackId
    {
        get
        {
            return FormType == eFormType.Report || FormType == eFormType.Dashboard ||
                 FormType == eFormType.Index
                ? PagePackId
                : entity.getForm(null, eFormType.Index, FormSubjectId)?.PagePackId
                     ?? PagePackId;
        }
    }

    [XmlIgnore] public long DbId { get; set; }

    /// <summary>
    /// The input records filters: Filter definition for record selection (specially for index forms)
    /// </summary>
    public List<EntityFormFilter> InputRecordsFilters { get; set; }

    public ServiceInterfaceProtocol ServiceInterfaceProtocol { get; set; }
    public string GetServiceOperation { get; set; }
    public FormOperationType? ApplyFormOperationType { get; set; }
    public string ApplyServiceOperation { get; set; }
    public bool AllowAnonymous { get; set; }
    public FormSortType SortType { get; set; }

    /// <summary>
    /// add ui rule logic
    /// </summary>
    /// <param name="uiRule">UI Rule</param>
    /// <returns></returns>
    public void AddUIRule(UIRule uiRule)
    {
        UiRules ??= [];
        UiRules.Add(uiRule);
    }

    public void AddUIRuleList(List<UIRule> uiRuleList)
    {
        UiRules ??= [];
        UiRules.AddRange(uiRuleList);
    }
    /// <summary>
    /// add Form Field
    /// </summary>
    /// <param name="formField">form Field</param>
    /// <returns></returns>
    public void AddFormField(FormField formField)
    {
        formFields ??= [];
        formFields.Add(formField);
    }

    /// <summary>
    /// Add Include Entity
    /// left join associated entity
    /// </summary>
    /// <param name="associationId">associationId</param>
    /// <returns></returns>
    public IncludeEntity AddIncludeEntity(string associationId)
    {
        Includes ??= [];
        var inc = new IncludeEntity
        {
            AssociationId = associationId
        };
        Includes.Add(inc);
        return inc;
    }

    /// <summary>
    /// add UIBlock
    /// </summary>
    /// <param name="uiBlock">ui Block</param>
    /// <param name="parentControlId">parent Control</param>
    /// <returns></returns>
    public void AddUiBlock(Form uiBlock, string parentControlId)
    {
        foreach (var item in uiBlock.formFields)
        {
            var ff = new FormField(item);
            ff.ParentControlId ??= parentControlId;
            AddFormField(ff);
        }

        foreach (var item in uiBlock.UiRules)
        {
            AddUIRule(new UIRule(item));
        }
    }

    /// <summary>
    /// All Fields Are Read Only
    /// </summary>
    /// <returns></returns>
    public bool AllFieldsAreReadOnly()
    {
        return FormType == eFormType.Delete || FormType == eFormType.VirtualDelete ||
                 FormType == eFormType.Detail || FormType == eFormType.Index;
    }

    public List<string> GetWriteFields()
    {
        List<string> fields = [];
        if (!AllFieldsAreReadOnly())
        {
            if (formFields != null)
            {
                fields.AddRange(from ff in formFields where !ff.CheckProperty(eControlPropertyId.ReadOnly) select ff.Id);
            }

            if (DataOperation?.AutoCalcs != null)
            {
                fields.AddRange(DataOperation.AutoCalcs.Calculations.Select(ac => ac.FieldId));
            }

            if (outputStateId > 0)
                fields.Add("StateId");
        }

        return fields;
    }

    public void AddOrderBy(string fieldId, SortType sortType, bool byId)
    {
        OrderBys ??= [];
        OrderBys.Add(new FormOrderBy
        {
            FieldId = fieldId,
            SortType = sortType,
            ById = byId
        });
    }

    public static bool IsBulk(eFormType formType)
    {
        return formType == eFormType.BulkEdit /*||
			formType == eFormType.ProcessCreateOnExistingRecord*/;
    }

    public EntityField GetEntityField(string fieldId)
    {
        string[] fieldIds = fieldId.Split('.');
        return entity.GetField(fieldIds[0]);
    }

    public TriggerTypeId TriggerTypeId(bool isApply)
    {
        return FormType switch
        {
            eFormType.Create => Entities.Base.Audit.TriggerTypeId.CreateForm,
            eFormType.Index or eFormType.Edit or eFormType.BulkEdit => Entities.Base.Audit.TriggerTypeId.EditForm,
            eFormType.Delete or eFormType.VirtualDelete => Entities.Base.Audit.TriggerTypeId.DeleteForm,
            eFormType.ProcessCreate or eFormType.ProcessCreateOnExistingRecord => isApply
                                    ? Entities.Base.Audit.TriggerTypeId.CreateAndCompleteTaskByUser
                                    : Entities.Base.Audit.TriggerTypeId.CreateInstanceByUser,
            eFormType.WorkItem or eFormType.ActiveProcessInstance or eFormType.ProcessInstance => isApply ? Entities.Base.Audit.TriggerTypeId.CompleteTaskByUser : Entities.Base.Audit.TriggerTypeId.EditTaskByUser,
            _ => Entities.Base.Audit.TriggerTypeId.EditForm,
        };
    }

    public bool HasTemplateFile { get; set; }

    public List<string> Roles { get; set; } = ["office"];
    public void AddRole(string roleId)
    {
        Roles ??= [];
        if (!Roles.Contains(roleId))
        { 
            Roles.Add(roleId); 
        }
    }
    
    public bool CheckRole(string roleId )
    {
        return Roles?.Contains(roleId)??false;
    }
    
    public bool CheckAnyRole(List<string> roleIds)
    {
        return roleIds.Any(Roles.Contains);
    }

    public void AddPassingParameters(string name)
    {
        PassingParameters ??= [];
        PassingParameters.Add(name);
    }
    /// <summary>
    /// Add Input Records Filter
    /// </summary>
    /// <param name="filter">filter</param>
    /// <returns></returns>
    public void AddInputRecordsFilter(EntityFormFilter filter)
    {
        InputRecordsFilters ??= [];
        InputRecordsFilters.Add(filter);
    }

    public virtual Form Clone()
    {
        List<Form> clonedForms = [.. entity.getForms().Where(f => f.Id.StartsWith($"{Id}_Copy"))];
        string copyId = Id + "_Copy";
        if (clonedForms.Count != 0)
            copyId = $"{copyId}{clonedForms.Count + 1}";

        var cloneForm = new Form(entity, copyId,
            "کپی " + Name, "Copy of " + EnName, FormType, FormSubjectId);
        cloneForm.AddUIRuleList(UiRules);
        cloneForm.formFields = [.. formFields];
        return cloneForm;
    }

    public string GetSpecificViewPage()
    {
        if (string.IsNullOrEmpty(SpecificViewPage))
            return null;
        return $"~/Views/{SpecificViewPage}.cshtml";
    }
}

public enum FormOperationType
{
    Apply,
    AfterApply
}

public enum FormSortType
{
    PassToProvider = 0,
    DoNotSort = 1,
    InMemory = 2

}
