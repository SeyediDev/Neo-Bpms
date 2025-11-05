namespace Neo.Bpms.Domain.Modeling.Definitions.Entities;

public abstract partial class FormDefinition
{
    protected virtual bool DefineDataOperations() { return true; }
    protected virtual void DataOperations() { }
    #region Auto Calculations

    /// <summary>
    /// Adds the automatic calculate item.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="formula">The formula.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public void AddAutoCalc(string fieldId, string formula, string condition = null)
    {
        if (form?.DataOperation == null) return;
        var formulaEx = Parser.ParseTree(formula);
        var conditionEx = condition == null ? null : Parser.ParseTree(condition);
        var ac = new AutoCalc
        {
            FieldId = fieldId,
            Formula = formulaEx,
            Condition = conditionEx,
            RecalcOnAnyChange = true
        };
        form.DataOperation.InitAutoCalcs();
        form.DataOperation.AutoCalcs.AddAutoCalc(ac);
    }
    #endregion Auto Calculations
    #region validation

    /// <summary>
    /// Adds the validation.
    /// </summary>
    /// <param name="_statement">The statement.</param>
    /// <param name="errorText">The error text.</param>
    /// <param name="enErrorText">The en error text.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="fieldId"></param>
    /// <returns></returns>
    public void AddValidation(string _statement, string errorText, string enErrorText, string condition, string fieldId)
    {
        if (entity == null) return;

        var statement = Parser.ParseTree(_statement);
        var conditionExp = condition == null ? null : Parser.ParseTree(condition);
        if (form?.DataOperation == null) return;
        var exp = new ExceptionInformation(ExceptionArea.DataEngine, "Validation" + ((form.DataOperation.Validations?.Count ?? 0) + 1))
        {
            ErrorText = errorText,
            EnErrorText = enErrorText
        };
        var validation = new Validation
        {
            Condition = conditionExp,
            Exception = exp,
            FieldId = fieldId,
            Statement = statement
        };
        form?.DataOperation.AddValidation(validation);
    }
    #endregion validation
    /*
		#region trigger
		private Trigger _currentTrigger;
		
		/// <summary>
		/// Adds the trigger.
		/// </summary>
		/// <param name="modelId">The model identifier.</param>
		/// <param name="entityId">The entity identifier.</param>
		/// <param name="condition">The condition.</param>
		/// <returns></returns>
		protected void AddTrigger<T>(string modelId, string entityId, string condition = null) where T : IDataOperationDefinitions
		{
			if (form?.dataOperation == null) return;
			var entity1 = ProjectDefinition.Project.GetEntity(modelId, entityId);
			var triggeredOperation = entity1?.getDataOperation(typeof(T).Name);
			if (triggeredOperation == null) return;
			var trigger = new Trigger
			{
				condition = condition == null ? null : Parser.ParseTree(condition),
				triggeredOperation = triggeredOperation
			};
			form.dataOperation.addTrigger(trigger);
			_currentTrigger = trigger;
		}

		/// <summary>
		/// Adds the trigger parameter.
		/// </summary>
		/// <param name="trigFieldId">The trig field identifier.</param>
		/// <param name="sourceFieldId">The source field identifier.</param>
		/// <returns></returns>
		protected void AddTriggerParameter(string trigFieldId, string sourceFieldId)
		{
			_currentTrigger?.addParameter(new Trigger.Parameter(trigFieldId, sourceFieldId));
		}
		/// <summary>
		/// Adds the trigger parameter.
		/// </summary>
		/// <param name="trigFieldId">The trig field identifier.</param>
		/// <param name="formula">The formula.</param>
		/// <returns></returns>
		protected bool AddTriggerParameter(string trigFieldId, ExpressionTree formula)
		{
			if (_currentTrigger == null) return false;
			_currentTrigger.addParameter(new Trigger.Parameter(trigFieldId, formula));
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
			if (_currentTrigger == null) return false;
			_currentTrigger.addParameter(new Trigger.Parameter(trigFieldId, Parser.ParseTree(formula)));
			return true;
		}
		#endregion trigger
		*/
}
