namespace Neo.Bpms.Domain.Features.Definitions.Entities;
/// <summary>
/// Base class to define forms and their details. All form definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract partial class FormDefinition : BaseModelingDefinition, IUIRuleDefinition
{
    public virtual List<string>? Roles { get; }
    protected UiEntity entity;

    public Form form;
    protected EntityDefinition entityDefinition;

    /// <summary>
    /// Defines all definitions.
    /// </summary>
    /// <param name="uiEntity">The entity.</param>
    /// <param name="definition"></param>
    /// <returns></returns>
    public Form DefineForm(UiEntity uiEntity, EntityDefinition definition)
    {
        entityDefinition = definition;
        entity = uiEntity;
        form = Identify();
        currentBaseElement = form;
        return form == null ? null : DefineAll(form);
    }

    /// <summary>
    /// Defines all definitions.
    /// </summary>
    /// <returns></returns>
    private Form DefineAll(Form uiForm)
    {
        form = uiForm;
        form.Roles = Roles;
        DefineFilters();
        Filters();
        DefineViewModel();
        ViewModel();
        DefineUIRules();
        UIRules();
        DefineDataOperations();
        DataOperations();
        FormOperation();
        return form;
    }

    protected abstract Form Identify();

    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="enName">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="formType">Type of the form.</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    /// <returns></returns>
    protected Form DefineForm(string enName, string name, Form.eFormType formType, string formSubjectId = null)
    {
        if (formType == Form.eFormType.VirtualDelete && !entity.IsStateBase && !ReflectionTools.IsInBaseInterface<ISoftDelete>(entity.EntityType))
            formType = Form.eFormType.Delete;
        if (formType == Form.eFormType.Delete && (entity.IsStateBase|| ReflectionTools.IsInBaseInterface<ISoftDelete>(entity.EntityType)))
            formType = Form.eFormType.VirtualDelete;
        form = new Form(entity, GetType().Name, name, enName, formType, formSubjectId)
        {
            //ManageObjectType = managedObjectTypeId
        };
        currentBaseElement = form;
        if (formType == Form.eFormType.ActiveProcessInstance
            || formType == Form.eFormType.ProcessCreate
            || formType == Form.eFormType.ProcessCreateOnExistingRecord
            || formType == Form.eFormType.WorkItem
            || formType == Form.eFormType.ProcessCreate
        )
        {
            AddControl(eControlTypeId.MultilineTextInput, RenderingForm.WorkDescription, "توضیحات کار");
            AddProperties(eControlPropertyId.DoubleWidth);
        }

        form.SetCommands(BeforSaveCommand, AfterSaveCommand);
        form.SpecificViewPage = ViewPage;
        return form;
    }

    public virtual Type BeforSaveCommand { get; }
    public virtual Type AfterSaveCommand { get; }

    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="formType">Type of the form.</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    /// <returns></returns>
    protected Form DefineForm(string name, Form.eFormType formType, string formSubjectId = null)
    {
        return DefineForm(AcquireFormEnName(formType), name, formType, formSubjectId);
    }

    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="formType">Type of the form.</param>
    /// <returns></returns>
    protected Form DefineForm(string name, Form.eFormType formType)
    {
        return DefineForm(AcquireFormEnName(formType), name, formType, null);
    }

    public virtual string Name => null;
    public virtual string EnName => null;
    public virtual string ViewPage => null;
    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="formType">Type of the form.</param>
    /// <param name="formSubjectId">The form subject identifier.</param>
    /// <returns></returns>
    protected Form DefineForm(Form.eFormType formType, string formSubjectId = null)
    {
        return DefineForm(AcquireFormEnName(formType), AcquireName(formType), formType, formSubjectId);
    }

    private string AcquireName(Form.eFormType formType)
    {
        if (!string.IsNullOrEmpty(Name))
            return Name;
        var name = "";
        switch (formType)
        {
            case Form.eFormType.Index:
                name = "فهرست";
                break;
            case Form.eFormType.Create:
                name = "ثبت";
                break;
            case Form.eFormType.Edit:
            case Form.eFormType.SpecificURLForRecord:
            case Form.eFormType.SpecificURL:
                name = "اصلاح";
                break;
            case Form.eFormType.Delete:
            case Form.eFormType.VirtualDelete:
                name = "حذف";
                break;
            case Form.eFormType.Detail:
                name = "مشاهده جزئیات";
                break;
            case Form.eFormType.BulkEdit:
                name = "اصلاح گروهی";
                break;
            case Form.eFormType.ProcessCreate:
            case Form.eFormType.ProcessCreateOnExistingRecord:
                name = "ایجاد فرآیند";
                break;
            case Form.eFormType.WorkItem:
            case Form.eFormType.ActiveProcessInstance:
            case Form.eFormType.ProcessInstance:
                name = "فرم فرآیند";
                break;
            case Form.eFormType.Report:
                name = "گزارش";
                break;
        }

        name += " " + entity.Name;
        return name;
    }

    private string AcquireFormEnName(Form.eFormType formType)
    {
        if (!string.IsNullOrEmpty(EnName))
            return EnName;
        var entityName = entity.EnName.GetDisplayableName() ?? entity.Id.GetDisplayableName();
        var formName = GetType().GetDisplayableName();
        if (formName.StartsWith(entityName))
            return formName;
        switch (formType)
        {
            case Form.eFormType.Index:
                return $"{entityName}s";
            case Form.eFormType.Create:
                return $"Create {entityName}";
            case Form.eFormType.Edit:
            case Form.eFormType.SpecificURLForRecord:
            case Form.eFormType.SpecificURL:
                return $"Edit {entityName}";
            case Form.eFormType.Delete:
            case Form.eFormType.VirtualDelete:
                return $"Delete {entityName}";
            case Form.eFormType.Detail:
                return $"{entityName} Details";
            case Form.eFormType.BulkEdit:
                return $"{entityName}s Bulk Edit";
            case Form.eFormType.ProcessCreate:
            case Form.eFormType.ProcessCreateOnExistingRecord:
                return $"Start Process for {entityName}";
            case Form.eFormType.WorkItem:
            case Form.eFormType.ActiveProcessInstance:
            case Form.eFormType.ProcessInstance:
                return $"Process Form for {entityName}";
            case Form.eFormType.Report:
                return $"{entityName} Report";
        }
        return $"{entityName} {formName}";
    }

    #region output state

    /// <summary>
    /// Sets the output state identifier.
    /// </summary>
    /// <param name="stateId">The state identifier.</param>
    /// <returns></returns>
    public bool SetOutputStateId(object stateId)
    {
        return _setOutputStateId(Convert.ToInt32(stateId));
    }

    public void SetOutputStateId(int state)
    {
        _setOutputStateId(state);
    }

    private bool _setOutputStateId(int state)
    {
        if (form?.DataOperation == null) return false;
        if (form.entity?.GetStateCollection()?.States.ContainsKey("" + state) ?? false)
            form.DataOperation.outputStateId = state;
        //else
        //    throw new Exception("Invalid StateId:" + stateId);
        return true;
    }

    #endregion output state

    /// <summary>
    /// Set Input Record Id Formula.
    /// </summary>
    /// <param name="formula">The formula.</param>
    protected void SetInputRecordIdFormula(string formula)
    {
        form.InputRecordIdFormula = formula;
    }

    /// <summary>
    /// Sets the specific URL.
    /// </summary>
    /// <param name="specificUrl">The specific URL.</param>
    protected void SetSpecificUrl(string specificUrl)
    {
        form.SpecificUrl = specificUrl;
    }

    /// <summary>
    /// Adds Order By
    /// </summary>
    /// <param name="fieldId">The field identifier</param>
    /// <param name="sortType">The sort type</param>
    /// <param name="byId">The by identifier</param>
    public void AddOrderBy(string fieldId, SortType sortType = SortType.Ascending, bool byId = false)
    {
        form.AddOrderBy(fieldId, sortType, byId);
    }

    protected void SetServiceInterfaceProtocol(ServiceInterfaceProtocol p)
    {
        ((Form)form).ServiceInterfaceProtocol = p;
    }
    protected void GetServiceOperation<T>() where T : IServiceOperation, new()
    {
        ((Form)form).GetServiceOperation = typeof(T).Name;
    }
    protected void ApplyServiceOperation<T>() where T : IServiceOperation, new()
    {
        form.ApplyServiceOperation = typeof(T).Name;
    }
    public void ApplyServiceOperation<T>(FormOperationType formOperationType,
        ServiceInterfaceProtocol serviceInterfaceProtocol)
        where T : IServiceOperation, new()
    {
        form.ServiceInterfaceProtocol = serviceInterfaceProtocol;
        form.ApplyServiceOperation = typeof(T).Name;
        form.ApplyFormOperationType = formOperationType;
    }
    protected void AddPassingParameters(string name)
    {
        ((Form)form).AddPassingParameters(name);
    }
    protected void AllowAnonymous()
    {
        form.AllowAnonymous = true;
    }
    protected void SetIndexFormSortType(FormSortType sortType)
    {
        form.SortType = sortType;
    }
}
