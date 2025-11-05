namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Entities;

/// <summary>
/// Base class to define data operations and their details. All data operation definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class DataOperationDefinitions : BaseModelingDefinition
{
    protected DataOperation DefineDataOperation(string name, DataOperation.eOperationType type)
    {
        dataOperation = new DataOperation(Entity, GetType().Name, name, type);
        Entity.addDataOperation(dataOperation);
        currentBaseElement = dataOperation;
        return dataOperation;
    }
    protected abstract void Identify();
    public DataOperation dataOperation;
    public Entity Entity;
    public DataOperation DefineAll(Entity entity)
    {
        Entity = entity;
        Identify();

        _ = DefineAutoCalculations();
        _ = DefineValidations();
        _ = DefineTriggers();
        _ = DefineDataEvents();
        return dataOperation;
    }

    /// <summary>
    /// Sets the data operation flag.
    /// </summary>
    /// <param name="flags">The flags.</param>
    protected void SetFlag(DataOperation.eFlags flags)
    {
        dataOperation?.SetFlag(flags);
    }

    #region Auto Calculations
    protected AutoCalc currentAutoCalc;
    /// <summary>
    /// Adds the automatic calculate item for data operation.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected AutoCalc AddAutoCalc(string fieldId, string formula, string condition = null)
    {
        if (dataOperation == null)
        {
            return null;
        }

        ExpressionTree formulaEx = Parser.ParseTree(formula),
            conditionEx = condition == null ? null : Parser.ParseTree(condition);
        return AddAutoCalc(fieldId, formulaEx, conditionEx);
    }
    /// <summary>
    /// Adds the automatic calculate item for data operation.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected AutoCalc AddAutoCalc(string fieldId, ExpressionTree formula, ExpressionTree condition = null)
    {
        if (dataOperation == null)
        {
            return null;
        }

        AutoCalc ac = new()
        {
            FieldId = fieldId,
            Formula = formula,
            Condition = condition,
            RecalcOnAnyChange = true
        };
        dataOperation.InitAutoCalcs();
        dataOperation.AutoCalcs.AddAutoCalc(ac);
        currentAutoCalc = ac;
        return ac;
    }
    #endregion Auto Calculations
    #region validation
    protected Validation currentValidation;
    /// <summary>
    /// Adds the validation item for data operation.
    /// </summary>
    /// <param name="fieldId">The scope identifier.</param>
    /// <param name="statement">The statement.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Validation AddValidation(string fieldId, ExpressionTree statement, string errorText, string enErrorText, ExpressionTree condition = null)
    {
        if (dataOperation == null)
        {
            return null;
        }

        ExceptionInformation exp = new(ExceptionArea.DataEngine, "Validation" + (dataOperation.Validations.Count + 1))
        {
            ErrorText = errorText,
            EnErrorText = enErrorText
        };
        Validation validation = new() { Condition = condition, Exception = exp, FieldId = fieldId, Statement = statement };
        dataOperation.AddValidation(validation);
        currentValidation = validation;
        //			currentBaseElement = validation;
        return validation;
    }
    /// <summary>
    /// Adds the validation item for data operation.
    /// </summary>
    /// <param name="fieldId">The scope identifier.</param>
    /// <param name="statement">The statement.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Validation AddValidation(string fieldId, string statement, string errorText, string enErrorText, string condition = null)
    {
        return dataOperation == null
            ? null
            : AddValidation(fieldId, Parser.ParseTree(statement), errorText, enErrorText, condition == null ? null : Parser.ParseTree(condition));
    }
    #endregion validation
    #region trigger
    protected Trigger currentTrigger;
    /// <summary>
    /// Adds the trigger item for data operation.
    /// </summary>
    /// <param name="triggeredOperation">The triggered operation.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Trigger AddTrigger(DataOperation triggeredOperation, ExpressionTree condition = null)//, QueryDefinition query=null
    {
        if (dataOperation == null)
        {
            return null;
        }

        Trigger trigger = new() { condition = condition, triggeredOperation = triggeredOperation };//query=query,  
        dataOperation.addTrigger(trigger);
        currentTrigger = trigger;
        //			currentBaseElement = trigger;
        return trigger;
    }
    /// <summary>
    /// Adds the trigger item for data operation.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected Trigger AddTrigger(string modelId, string entityId, string operationId, string condition = null)//, QueryDefinition query = null
    {
        if (dataOperation == null)
        {
            return null;
        }

        Entity entity = ProjectDefinition.Project.GetEntity(modelId, entityId);
        DataOperation dop = entity?.getDataOperation(operationId);
        if (dop == null)
        {
            return null;
        }

        return AddTrigger(dop, condition == null ? null : Parser.ParseTree(condition));//, query
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
    public abstract bool DefineAutoCalculations();
    public abstract bool DefineValidations();
    public abstract bool DefineTriggers();
    public abstract bool DefineDataEvents();
}
