namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public interface IEntityDefinition
{
}
/// <summary>
/// Base class to define entities and their details. All entity definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class EntityDefinition : BaseModelingDefinition, IEntityDefinition
{
    public virtual List<string>? Roles { get; }
    public ModelNamespace Model { get; set; }
    public UiEntity Entity { get; set; }
    public virtual Type DefinitionEntity { get; }

    /// <summary>
    /// Icon class for the entity (e.g., "fa fa-users", "flaticon bpms-flaticon-campaign")
    /// Override this property in derived UIDefinitions to set the entity's icon.
    /// </summary>
    public virtual string? Icon { get; }

    /// <summary>
    /// Defines the field structure.
    /// </summary>
    /// <returns></returns>
    public UiEntity DefineFieldStructure()
    {
        if (Entity == null)
        {
            return null;
        }
        Entity.Defined = true;
        _order = 0;

        // Set the entity icon from the definition
        if (!string.IsNullOrEmpty(Icon))
        {
            Entity.Icon = Icon;
        }
        foreach (DocumentControlDefinitions documentControl in ExtractSubsInstances<DocumentControlDefinitions>())
        {
            var field = AddField(documentControl.GetType().Name, documentControl.Name,
                documentControl.GetType().Name, TVariableTypes.File, documentControl.GetType(), EntityFieldFlags.NotMap);
            if (field != null)
            {
                field.CSharpType = documentControl.GetType();
                if (documentControl.Multiple)
                {
                    AddFieldProperty(EntityFieldPropertyId.Multiple, true);
                }
                if (documentControl.UseFileServer)
                {
                    AddFieldProperty(EntityFieldPropertyId.UseFileServer, true);
                }
                if (documentControl.ShowDocumentInPage)
                {
                    AddFieldProperty(EntityFieldPropertyId.ShowDocumentInPage, true);
                }
                if (documentControl.DocumentType is not null)
                {
                    AddFieldProperty(EntityFieldPropertyId.DocumentType, documentControl.DocumentType);
                }
            }
        }
        return Entity;
    }

    /// <summary>
    /// Define data operations and methods.
    /// </summary>
    /// <returns></returns>
    public void DefineOperations()
    {
        foreach (DataOperationDefinitions dataOperationDefinitions in ExtractSubsInstances<DataOperationDefinitions>())
        {
            _ = dataOperationDefinitions.DefineAll(Entity);
        }

        foreach (EntityServiceDefinition serviceDefinition in ExtractSubsInstances<EntityServiceDefinition>())
        {
            serviceDefinition.DefineAll(Entity);
        }
    }

    /// <summary>
    /// Define UI.
    /// </summary>
    /// <returns></returns>
    public void DefineUI()
    {
        RefineEntity();
        DefineSubjectsForms();
        Forms();
        foreach (ReportDefinition report in ExtractSubsInstances<ReportDefinition>())
        {
            _ = report.DefineReport(Entity, this);
        }
        foreach (DashboardDefinition dashboard in ExtractSubsInstances<DashboardDefinition>())
        {
            _ = dashboard.DefineDashboard(Entity, this);
        }
    }
    protected virtual void RefineEntity()
    {
    }

    protected EntityField? RenameField(string fieldName, string name, string enName)
    {
        var field = Entity.GetField(fieldName);
        if (field != null)
        {
            field.Name = name;
            field.EnName = enName;
        }

        return field;
    }

    #region IdentifyReport functions

    /// <summary>
    /// Sets the entity database table map.
    /// </summary>
    /// <param name="dbTableName">Name of the database table.</param>
    protected void SetEntityDBTableMap(string dbTableName)
    {
        if (Entity != null)
        {
            Entity.DbTableNameMap = dbTableName;
        }
    }

    #endregion IdentifyReport functions

    #region entity fields

    protected EntityField currentField;

    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="enName">Name of the en.</param>
    /// <param name="name">The name.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="flags">The flags.</param>
    /// <returns></returns>
    protected EntityField AddField(string enName, string name, TVariableTypes fieldType, Type cSharpType,
        EntityFieldFlags flags = EntityFieldFlags.None)
    {
        string id = enName.Replace(" ", "");
        return AddField(id, name, enName, fieldType, cSharpType, flags);
    }

    /// <summary>
    /// Adds the field.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName">Name of the en.</param>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="flags">The flags.</param>
    /// <returns></returns>
    protected EntityField AddField(string id, string name, string enName, TVariableTypes fieldType, Type cSharpType,
        EntityFieldFlags flags = EntityFieldFlags.None)
    {
        currentField = new EntityField(Entity, id, name, enName, cSharpType ?? EntityField.GetFieldType(fieldType), fieldType,
            flags)
        {
            Order = _order++
        };
        currentBaseElement = currentField;
        _ = Entity.AddField(currentField);
        return currentField;
    }

    protected void AddFieldProperty(EntityFieldPropertyId entityFieldPropertyId, object value)
    {
        if (currentField == null) return;
        currentField.AddProperty(entityFieldPropertyId, value);
    }

    /// <summary>
    /// Sets the field formula identifier.
    /// </summary>
    /// <param name="formulaId">The formula identifier.</param>
    /// <returns></returns>
    protected bool SetFieldFormulaId(string formulaId)
    {
        if (currentField == null)
        {
            return false;
        }

        currentField.Formula = new EntityFieldFormula
        {
            FormulaMethodId = formulaId
        };
        return true;
    }

    /// <summary>
    /// Sets the field formula.
    /// </summary>
    /// <param name="formula">The formula.</param>
    /// <returns></returns>
    protected bool SetFieldFormula(string formula)
    {
        if (currentField == null)
        {
            return false;
        }

        currentField.Formula = new EntityFieldFormula
        {
            FormulaText = formula,
            FormulaBody = Parser.Parse(formula)
        };
        return true;
    }

    /// <summary>
    /// Sets the field default value.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    protected bool SetFieldDefaultValue(object defaultValue)
    {
        if (currentField == null)
        {
            return false;
        }

        currentField.DefaultValue = defaultValue;
        return true;
    }

    //	bool AddEntityInheritanceTable( int TableId, unsigned int Flags/*TEntityFieldFlags*/ ) {}
    /// <summary>
    /// Adds the field conformance.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="access">The access.</param>
    /// <param name="level">The level.</param>
    /// <param name="conformanceFunc">The conformance function.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <returns></returns>
    protected bool AddFieldConformance(string id, string name, UserSecurityAccessFlags access, ConformanceLevel level,
        string conformanceFunc, string errorText, string enErrorText)
    {
        if (currentField == null)
        {
            return false;
        }

        ExpressionNode exp = conformanceFunc == null ? null : Parser.Parse(conformanceFunc);
        if (exp == null)
        {
            return false;
        }

        currentField.addConformance(new Conformance(Entity, currentField, id, name, access, level, null, exp,
            new ExceptionInformation(DataEngineException.Conformance, name)
            { EnErrorText = enErrorText, ErrorText = errorText }));
        return true;
    }

    #endregion entity fields

    #region form

    protected FormDefinition form { get; set; }

    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="sFormType">Type of the s form.</param>
    /// <returns></returns>
    protected bool DefineForm(string sFormType)
    {
        Type thisType = GetType();
        Type type = thisType.GetNestedType(sFormType);
        return type == null ? throw new Exception("This Form Does not Exist" + sFormType) : DefineForm(type);
    }

    /// <summary>
    /// Defines Form
    /// </summary>
    /// <typeparam name="T">form</typeparam>
    /// <returns></returns>
    protected bool DefineForm<T>() where T : FormDefinition
    {
        return DefineForm(typeof(T));
    }

    /// <summary>
    /// Defines the form.
    /// </summary>
    /// <param name="formType">Type of the form.</param>
    /// <returns></returns>
    protected bool DefineForm(Type formType)
    {
        if (formType.IsSubclassOf(typeof(FormDefinition)))
        {
            Type[] types = [];
            ConstructorInfo cons = formType.GetConstructor(types) ?? throw new Exception("This Form Does not Exist" + formType);
            object[] parameters = [];
            if (cons.Invoke(parameters) is not FormDefinition formDef)
            {
                throw new Exception("This Form Does not Exist" + formType);
            }
            form = formDef;
            var entityForm = formDef.DefineForm(Entity, this);
            entityForm.Roles ??= Roles;
            Entity.AddForm(entityForm);
        }

        return true;
    }

    #endregion

    #region Display String

    /// <summary>
    /// Adds the basic(display string field).
    /// </summary>
    /// <param name="fieldId">Name of the _ field.</param>
    /// <param name="eFlags">The _ flags.</param>
    /// <param name="otherFieldThatIgnoreMe"></param>
    /// <param name="culture"></param>
    /// <param name="withTitle"></param>
    /// <param name="ignoreIfNull"></param>
    /// <returns></returns>
    protected bool AddBasic(string fieldId, string otherFieldThatIgnoreMe, string culture, bool withTitle, bool ignoreIfNull)
    {
        EntityField fld = Entity.GetField(fieldId);
        return fld != null && Entity.AddBasicField(new BasicField
        { FieldId = fieldId, WithTitle = withTitle, IgnoreIfNull = ignoreIfNull, OtherFieldThatIgnoreMe = otherFieldThatIgnoreMe, Culture = culture });
    }

    #endregion Display String

    #region Index

    private EntityIndex currentIndex;

    /// <summary>
    /// Adds the index.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="EnName">Name of the en.</param>
    /// <param name="Name">The name.</param>
    /// <param name="clustered"></param>
    /// <returns></returns>
    protected bool AddIndex(string id, string EnName, string Name, bool clustered)
    {
        if (Entity == null)
        {
            return false;
        }

        currentIndex = new EntityIndex
        { Id = id, Name = Name, EnName = EnName, IsUnique = false, Clustered = clustered };
        //			currentBaseElement = currentIndex;
        Entity.indexes.Add(currentIndex);
        return true;
    }

    /// <summary>
    /// Adds the unique index.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="EnName">Name of the en.</param>
    /// <param name="Name">The name.</param>
    /// <param name="clustered"></param>
    /// <returns></returns>
    protected bool AddUniqueIndex(string id, string EnName, string Name, bool clustered)
    {
        if (Entity == null)
        {
            return false;
        }

        currentIndex = new EntityIndex { Id = id, Name = Name, EnName = EnName, IsUnique = true, Clustered = clustered };
        //currentBaseElement = currentIndex;
        Entity.indexes.Add(currentIndex);
        return true;
    }

    /// <summary>
    /// Adds the index parameter.
    /// </summary>
    /// <param name="FieldId">The field identifier.</param>
    /// <returns></returns>
    protected bool AddIndexParameter(string FieldId)
    {
        if (currentIndex == null)
        {
            return false;
        }

        currentIndex.Fields.Add(new IndexField { FieldName = FieldId });
        return true;
    }

    #endregion Index

    #region Auto Calculations

    protected AutoCalc currentAutoCalc;

    /// <summary>
    /// Adds the automatic calculate.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <param name="recalculateOnAnyChange"></param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected AutoCalc AddAutoCalc(string fieldId, string formula, bool recalculateOnAnyChange,
        string condition = null)
    {
        if (Entity == null)
        {
            return null;
        }

        ExpressionTree formulaEx = Parser.ParseTree(formula),
            conditionEx = condition == null ? null : Parser.ParseTree(condition);
        return AddAutoCalc(fieldId, formulaEx, recalculateOnAnyChange, conditionEx);
    }

    /// <summary>
    /// Adds the automatic calculate.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <param name="recalculateOnAnyChange"></param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected AutoCalc AddAutoCalc(string fieldId, ExpressionTree formula, bool recalculateOnAnyChange,
        ExpressionTree condition = null)
    {
        if (Entity == null)
        {
            return null;
        }

        AutoCalc ac = new()
        {
            FieldId = fieldId,
            Formula = formula,
            Condition = condition,
            RecalcOnAnyChange = recalculateOnAnyChange
        };
        Entity.InitAutoCalcs();
        Entity.AutoCalcs.AddAutoCalc(ac);
        currentAutoCalc = ac;
        return ac;
    }

    #endregion Auto Calculations

    #region validation

    protected Validation currentValidation;

    /// <summary>
    /// Adds the validation.
    /// </summary>
    /// <param name="ScopeId">The scope identifier.</param>
    /// <param name="statement">The statement.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    private Validation AddValidation(string ScopeId, ExpressionTree statement, string errorText, string enErrorText,
        ExpressionTree condition = null)
    {
        if (Entity == null)
        {
            return null;
        }

        ExceptionInformation exp = new(ExceptionArea.DataEngine,
                "Validation" + (Entity.Validations?.Count ?? 0 + 1))
        {
            ErrorText = errorText,
            EnErrorText = enErrorText,
            EnName = enErrorText
        };
        Validation validation = new() { Condition = condition, Exception = exp, FieldId = ScopeId, Statement = statement };
        Entity.addValidation(validation);
        currentValidation = validation;
        //			currentBaseElement = validation;
        return validation;
    }

    /// <summary>
    /// Adds the validation.
    /// </summary>
    /// <param name="ScopeId">The scope identifier.</param>
    /// <param name="statement">The statement.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public Validation AddValidation(string ScopeId, string statement, string errorText, string enErrorText, string condition = null)
    {
        return Entity == null
            ? null
            : AddValidation(ScopeId, Parser.ParseTree(statement), errorText, enErrorText,
            condition == null ? null : Parser.ParseTree(condition));
    }

    #endregion validation

    #region trigger

    protected Trigger currentTrigger;

    /// <summary>
    /// Adds the trigger.
    /// </summary>
    /// <param name="triggeredOperation">The triggered operation.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Trigger
        AddTrigger(DataOperation triggeredOperation, ExpressionTree condition = null) //, QueryDefinition query = null
    {
        if (Entity == null)
        {
            return null;
        }

        Trigger trigger = new() { condition = condition, triggeredOperation = triggeredOperation }; //, query = query
        Entity.addTrigger(trigger);
        currentTrigger = trigger;
        //			currentBaseElement = trigger;
        return trigger;
    }

    //todo: implementation, learning and usage of this and many other features
    /// <summary>
    /// Adds the trigger.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Trigger AddTrigger(string modelId, string entityId, string operationId,
            string condition = null) //, QueryDefinition query = null
    {
        if (Entity == null)
        {
            return null;
        }

        Entity triggerEntity = ProjectDefinition.Project.GetEntity(modelId, entityId);
        if (Entity == null)
        {
            return null;
        }

        DataOperation dop = triggerEntity.getDataOperation(operationId);
        if (dop == null)
        {
            return null;
        }

        return AddTrigger(dop, condition == null ? null : Parser.ParseTree(condition)); //, query
    }

    /// <summary>
    /// Adds the trigger parameter.
    /// </summary>
    /// <param name="trigFieldId">The trig field identifier.</param>
    /// <param name="sourceFieldId">The source field identifier.</param>
    /// <returns></returns>
    protected bool AddTriggerParameter(string trigFieldId, string sourceFieldId)
    {
        if (currentTrigger == null)
        {
            return false;
        }

        currentTrigger.addParameter(new Trigger.Parameter(trigFieldId, sourceFieldId));
        return true;
    }

    /// <summary>
    /// Adds the trigger parameter.
    /// </summary>
    /// <param name="trigFieldId">The trig field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <returns></returns>
    protected bool AddTriggerParameter(string trigFieldId, ExpressionTree formula)
    {
        if (currentTrigger == null)
        {
            return false;
        }

        currentTrigger.addParameter(new Trigger.Parameter(trigFieldId, formula));
        return true;
    }

    /// <summary>
    /// Adds the trigger parameter_ formula.
    /// </summary>
    /// <param name="trigFieldId">The trig field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <returns></returns>
    protected bool AddTriggerParameter_Formula(string trigFieldId, string formula)
    {
        if (currentTrigger == null)
        {
            return false;
        }

        currentTrigger.addParameter(new Trigger.Parameter(trigFieldId, Parser.ParseTree(formula)));
        return true;
    }

    #endregion trigger

    #region data event

    protected DataEvent currentDataEvent;

    /// <summary>
    /// Adds the data event.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    private DataEvent AddDataEvent(string id, string name, ExpressionNode condition) //, Entity structure
    {
        if (Entity == null)
        {
            return null;
        }

        DataEvent dataEvent = new(Entity, id, name, condition); //, structure
        Entity.addDataEvent(dataEvent);
        currentDataEvent = dataEvent;
        currentBaseElement = dataEvent;
        return dataEvent;
    }

    /// <summary>
    /// Adds the data event.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    protected DataEvent AddDataEvent(string id, string condition, string name) //, string modelId, string entityId
    {
        if (Entity == null)
        {
            return null;
        }
        //			var model = ModelNamespace.GetModel(modelId); if (model == null) return null;
        //			var eventEntity = Entity.GetEntity(modelId, entityId); if (entity == null) return null;
        return AddDataEvent(id, name, Parser.Parse(condition)); //, eventEntity
    }

    #endregion data event

    #region process fields

    /*//protected bool setWeightField(string fieldId)
		//{
		//	if (entity == null) return false;
		//	entity.weightField = fieldId;
		//	return true;
		//}
		protected bool SetPriorityField(string fieldId)
		{
			if (entity == null) return false;
//			entity.PriorityField = fieldId;
			return true;
		}*/
    //protected bool setConfidentialityField(string fieldId)
    //{
    //	if (entity == null) return false;
    //	entity.confidentialityFieldId = fieldId;
    //	return true;
    //}

    #endregion process fields

    #region conformance

    /// <summary>
    /// Adds the conformance.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="access">The access.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="level">The level.</param>
    /// <param name="conformanceFunc">The conformance function.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <returns></returns>
    protected bool AddConformance(string id, string name, UserSecurityAccessFlags access, string subject,
        ConformanceLevel level,
        string conformanceFunc, string errorText, string enErrorText)
    {
        if (Entity == null)
        {
            return false;
        }

        ExpressionNode exp = level != ConformanceLevel.Expression || conformanceFunc == null
            ? null
            : Parser.Parse(conformanceFunc);
        if (exp == null)
        {
            return false;
        }

        Entity.AddConformance(new Conformance(Entity, null, id, name, access, level, subject, exp,
            new ExceptionInformation(DataEngineException.Conformance, name)
            { EnErrorText = enErrorText, ErrorText = errorText }));
        return true;
    }

    #endregion conformance

    #region dependency

    #endregion dependency

    #region package

    protected Package currentPackage;
    protected PackageEntity currentPackageEntity;
    protected Package currentParentPackage;

    /// <summary>
    /// Adds the package.
    /// </summary>
    /// <param name="package">The package.</param>
    /// <returns></returns>
    protected Package AddPackage(Package package)
    {
        if (Entity == null)
        {
            return null;
        }

        currentPackage = package;
        currentBaseElement = package;
        currentPackageEntity = package.mainEntity;
        if (currentParentPackage != null)
        {
            currentParentPackage.addSubPackage(package);
        }
        else
        {
            Entity.addPackage(package);
        }

        return package;
    }

    /// <summary>
    /// Removes the field from package.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <returns></returns>
    protected bool RemoveField(string fieldId)
    {
        if (currentPackageEntity == null)
        {
            return false;
        }

        currentPackageEntity.removeField(fieldId);
        return true;
    }

    /// <summary>
    /// Adds the package.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    protected Package AddPackage(string id, string name)
    {
        if (Entity == null)
        {
            return null;
        }

        Package package = new(id, name);
        return AddPackage(package);
    }

    /// <summary>
    /// Adds the package entity.
    /// </summary>
    /// <param name="_entity">The entity.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    protected PackageEntity AddPackageEntity(Entity _entity, PackageEntity.eType type)
    {
        if (currentPackage == null)
        {
            return null;
        }

        currentPackageEntity = currentPackage.addEntity(_entity, type);
        return currentPackageEntity;
    }


    /// <summary>
    /// Starts the sub package.
    /// </summary>
    protected void StartSubPackage()
    {
        currentParentPackage = currentPackage;
        currentBaseElement = currentPackage;
    }

    /// <summary>
    /// Ends the sub package.
    /// </summary>
    protected void EndSubPackage()
    {
        currentParentPackage = currentParentPackage?.parentPackage;
    }

    #endregion package

    #region State

    protected EntityState currentState;
    private int _order;

    /// <summary>
    /// Adds the state.
    /// </summary>
    /// <param name="State">The state.</param>
    /// <returns></returns>
    protected EntityState AddState(EntityState State)
    {
        if (Entity == null)
        {
            return null;
        }

        currentState = State;
        currentBaseElement = State;
        return Entity.AddState(State);
    }

    /// <summary>
    /// Adds the state.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="enName">Name of the en.</param>
    /// <param name="category">The category.</param>
    /// <returns></returns>
    protected EntityState AddState(int id, string name, string enName, EntityStateCategory category)
    {
        if (Entity == null)
        {
            return null;
        }

        EntityState State = new(Entity, id, name, enName, category);
        return State;
    }

    //bool SetStateProgressPercent( short ProcessProgressPercent ) {}

    #endregion State

    /// <summary>
    /// Defines Forms
    /// </summary>
    /// <returns></returns>
    protected virtual void Forms()
    {
    }

    protected void DefineSubjectsForms()
    {
        foreach (FormDefinition formDef in ExtractSubsInstances<FormDefinition>())
        {
            if (formDef is ReportDefinition or null)
            {
                continue;
            }
            if (formDef is not ISubjectFormDefinition)
            {
                continue;
            }
            form = formDef;
            var entityForm = formDef.DefineForm(Entity, this);
            entityForm.Roles ??= Roles;
            Entity.AddForm(entityForm);
        }
    }

    public static string GetEntityName<TEntity>()
    {
        var name = typeof(TEntity).Name.Replace($"UiDefinitions", "");
        return name.Replace("Definitions", "");
    }

    public static string GetNamespaceName<TEntity>()
    {
        var typeEntity = typeof(TEntity);
        string reportNamespaceId =
            typeEntity.Namespace?[Math.Max(0, typeEntity.Namespace.LastIndexOf('.', typeEntity.Namespace.Length - 1) + 1)..];
        return reportNamespaceId;
    }
    #region Form
    protected bool IsCreateForm => form.form.FormType == Form.eFormType.Create;
    protected void AddColumns(params string[] fieldIdList)
        => form.AddColumns(fieldIdList);
    protected void AddField(string fieldName, eControlTypeId controlTypeId, params eControlPropertyId[] controlProperties)
        => form.AddField(fieldName, controlTypeId, controlProperties);
    protected void AddField(string fieldName, params eControlPropertyId[] controlProperties)
        => form.AddField(fieldName, controlProperties);
    protected void AddFields(params string[] fieldNames)
        => form.AddFields(fieldNames);
    protected void AddHiddenField(string fieldName)
        => form.AddHiddenField(fieldName);
    protected void AddSubjectColumn<TForm>(string? name = null)
        where TForm : FormDefinition, ISubjectFormDefinition, new()
        => form.AddSubjectColumn<TForm>(name);
    protected void AddSubTable<TTableEntity>(
        string tableAssociation, string labelName,
        ContainerControl containerControl = ContainerControl.MultiTab,
        string filter = null, int? recordCount = null,
        string tableIndexFormSubjectId = null, string association = null, bool editable = false,
        string enLabelName = null)
        where TTableEntity : IEntity
        => form.AddSubTable<TTableEntity>(tableAssociation, labelName, containerControl,
            filter, recordCount, tableIndexFormSubjectId, association, editable, enLabelName);
    protected void AddOrderBy(string fieldId, SortType sortType = SortType.Ascending, bool byId = false)
        => form.AddOrderBy(fieldId, sortType, byId);
    protected void AddOrderBy(params string[] fieldNames)
        => form.AddOrderBy(fieldNames);
    protected void AddGroup(string id, string name, string enName = null)
        => form.AddGroup(id, name, enName);
    protected void EndGroup()
        => form.EndGroup();
    protected void ShowHide(string userChangeFieldId, string condition, params string[] controlledParams)
        => form.ShowHide(userChangeFieldId, condition, controlledParams);
    protected void FilterFormula(string userChangeFieldId, string controlledParam, string filter)
        => form.FilterFormula(userChangeFieldId, controlledParam, filter);
    protected void SetFilterFormula(List<string> userChangeFieldIds, List<string> controlledParams, string filter)
        => form.SetFilterFormula(userChangeFieldIds, controlledParams, filter);
    #endregion
}

