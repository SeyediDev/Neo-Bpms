using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Forms;

public interface IUIRuleDefinition
{
    #region controller
    /// <summary>
    /// Adds the Controller event.
    /// </summary>
    /// <param name="ev">The ev.</param>
    /// <returns></returns>
    UIRuleEvent AddUIRuleEvent(UIRuleEvent ev);

    /// <summary>
    /// Adds the Controller event.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="RepeatScopeId">The repeat scope identifier.</param>
    /// <returns></returns>
    UIRuleEvent AddUIRuleEvent(UIRuleEvent.eEventType type, string fieldId = null, string RepeatScopeId = null);//, TControllerEventKeyType KeyType		

    //		protected UIRuleEvent bool AddCtrlrKeyEvent(UIRuleEvent.enumEventType Type, UIRuleEvent.TControllerEventKeyType KeyType, int ParamId) { }
    //		bool AddCtrlrRepeatKeyEvent(UIRuleEvent.enumEventType Type, UIRuleEvent.TControllerEventKeyType KeyType, int ParamId, int RepeatScopeId) { }
    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleTask(UIRuleTask operation);

    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <param name="controlType">Type of the control.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType, UIRuleTask.eControlType controlType,
        UIRuleTask.eCalcLocation calcLocation, string ConditionStr); //TControllerTaskRunLocation RunLocation, 

    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType);

    /// <summary>
    /// Adds the Controller conditional task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <param name="ConditionCalcLocation">The condition calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleIfTask(UIRuleTask.eTaskType taskType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr);
    /// <summary>
    /// Adds the Controller while loop task.
    /// </summary>
    /// <param name="ActivityType">Type of the activity.</param>
    /// <param name="ConditionCalcLocation">The condition calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr);
    /// <summary>
    /// Adds the Controller do while loop task.
    /// </summary>
    /// <param name="ActivityType">Type of the activity.</param>
    /// <param name="ConditionCalcLocation">The condition calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    UIRuleTask AddUIRuleDoWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr);
    /// <summary>
    /// Sets the Controller task detail: set local parameter.
    /// </summary>
    /// <param name="localParamId">The local parameter identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <returns></returns>
    bool SetTask_SetLocalParam(eLocalParameterIds localParamId, UIRuleTask.eCalcLocation calcLocation, string formulaStr); //, int FormulaId
    /// <summary>
    /// Sets the Controller task detail: set content.
    /// </summary>
    /// <param name="assignToFieldId">The assign to field identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <returns></returns>
    bool SetTask_SetContent(string assignToFieldId, UIRuleTask.eCalcLocation calcLocation, string formulaStr);//, int FormulaId

    /// <summary>
    /// Sets the Controller task detail: set control part property.
    /// </summary>
    /// <param name="controlPropertyId">The control property identifier.</param>
    /// <param name="target">The target.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="FormulaStr">The formula string.</param>
    /// <returns></returns>
    bool SetTask_SetControlPartProperty(eControlPropertyId controlPropertyId, eControlPropertyTarget target, UIRuleTask.eCalcLocation calcLocation, string FormulaStr);

    /// <summary>
    /// Sets the Controller task detail: set property.
    /// </summary>
    /// <param name="controlPropertyId">The control property identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="FormulaStr">The formula string.</param>
    /// <returns></returns>
    bool SetTask_SetProperty(eControlPropertyId controlPropertyId, UIRuleTask.eCalcLocation calcLocation, string FormulaStr);//, int FormulaId
    /// <summary>
    /// Sets the Controller task detail: set property's destination field.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <returns></returns>
    bool SetTask_SetProperty_ToField(string fieldId);
    /// <summary>
    /// Sets the Controller task detail: set property's destination control.
    /// </summary>
    /// <param name="controlId">The control identifier.</param>
    /// <returns></returns>
    bool SetTask_SetProperty_ToControl(string controlId);
    /// <summary>
    /// Sets Controller task detail: validation.
    /// </summary>
    /// <param name="validationType">Type of the validation.</param>
    /// <param name="formulaCalcLocation">The formula calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <param name="exception">The exception.</param>
    /// <returns></returns>
    bool SetTask_Validation(UIRuleTask.eValidationType validationType, UIRuleTask.eCalcLocation formulaCalcLocation,
        string formulaStr, ExceptionInformation exception);
    /// <summary>
    /// Sets the Controller task detail: one row query.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    bool SetTask_OneRowQuery(string modelId, string entityId);
    /// <summary>
    /// Sets the Controller task detail: one row query field destination.
    /// </summary>
    /// <param name="queryFieldId">The query field identifier.</param>
    /// <param name="viewModelFieldId">The view model field identifier.</param>
    /// <returns></returns>
    bool SetTask_OneRowQuery_ToField(string queryFieldId, string viewModelFieldId);
    /// <summary>
    /// Sets the Controller task detail: one row query local parameter destination.
    /// </summary>
    /// <param name="queryFieldId">The query field identifier.</param>
    /// <param name="localParameterId">The local parameter identifier.</param>
    /// <returns></returns>
    bool SetTask_OneRowQuery_ToLocalParameter(string queryFieldId, eLocalParameterIds localParameterId);
    /// <summary>
    /// Sets the Controller task detail: one row query filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    bool SetTask_OneRowQuery_Filter(string filter);
    /// <summary>
    /// Sets the Controller task detail: Call the specific Data Operation
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <returns></returns>
    bool SetTask_CallDataOperation(string modelId, string operationId);
    /// <summary>
    /// Sets the Controller task detail: Parameter of Call the specific Data Operation 
    /// </summary>
    /// <param name="operationFieldId">The operation field identifier.</param>
    /// <param name="viewModelFieldId">The view model field identifier.</param>
    /// <returns></returns>
    bool SetTask_CallDataOperation_Param(string operationFieldId, string viewModelFieldId);
    /// <summary>
    /// Sets the Controller task detail: call controller.
    /// </summary>
    /// <param name="controllerId">The controller identifier.</param>
    /// <returns></returns>
    bool SetTask_CallController_ControllerId(string controllerId);
    /// <summary>
    /// Sets the Controller task detail: open form.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="formId">The form identifier.</param>
    /// <returns></returns>
    bool SetTask_OpenForm(string modelId, string formId);

    /// <summary>
    /// Sets the Controller task detail: open report.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="reportId">The report identifier.</param>
    /// <returns></returns>
    bool SetTask_OpenReport(string modelId, string reportId);
    #endregion controller
}
public class UIRuleDefinition : IUIRuleDefinition
{
    #region controller
    public UIRule CurrentUIRule { get; set; } = null;
    public UIRuleEvent currentEvent;
    /// <summary>
    /// Adds the Controller event.
    /// </summary>
    /// <param name="ev">The ev.</param>
    /// <returns></returns>
    public UIRuleEvent AddUIRuleEvent(UIRuleEvent ev)
    {
        currentEvent = ev;
        //			currentBaseElement = ev;
        CurrentUIRule.Events.Add(ev);
        return ev;
    }
    /// <summary>
    /// Adds the Controller event.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="RepeatScopeId">The repeat scope identifier.</param>
    /// <returns></returns>
    public UIRuleEvent AddUIRuleEvent(UIRuleEvent.eEventType type, string fieldId = null, string RepeatScopeId = null)//, TControllerEventKeyType KeyType
    {
        var ev = new UIRuleEvent(type, fieldId, RepeatScopeId);
        return AddUIRuleEvent(ev);
    }

    //		public UIRuleEvent bool AddCtrlrKeyEvent(UIRuleEvent.enumEventType Type, UIRuleEvent.TControllerEventKeyType KeyType, int ParamId) { }
    //		bool AddCtrlrRepeatKeyEvent(UIRuleEvent.enumEventType Type, UIRuleEvent.TControllerEventKeyType KeyType, int ParamId, int RepeatScopeId) { }
    UIRuleTask currentOperation;
    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleTask(UIRuleTask operation)
    {
        currentOperation = operation;
        //			currentBaseElement = operation;
        CurrentUIRule.Operations.Add(operation);
        return operation;

    }
    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <param name="controlType">Type of the control.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType, UIRuleTask.eControlType controlType,
        UIRuleTask.eCalcLocation calcLocation, string ConditionStr) //TControllerTaskRunLocation RunLocation, 
    {
        return AddUIRuleTask(new UIRuleTask()
        {
            TaskType = taskType,
            ControlType = controlType,
            conditionCalcLocation = calcLocation,
            conditionStr = ConditionStr
        });
    }
    /// <summary>
    /// Adds the Controller task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType)
    {
        return AddUIRuleTask(taskType, UIRuleTask.eControlType.NoCondition, UIRuleTask.eCalcLocation.Client, null);
    }
    /// <summary>
    /// Adds the Controller conditional task.
    /// </summary>
    /// <param name="taskType">Type of the task.</param>
    /// <param name="ConditionCalcLocation">The condition calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleIfTask(UIRuleTask.eTaskType taskType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr)
    {
        return AddUIRuleTask(taskType, UIRuleTask.eControlType.IfCondition, ConditionCalcLocation, ConditionStr);
    }
    /// <summary>
    /// Adds the Controller while loop task.
    /// </summary>
    /// <param name="ActivityType">Type of the activity.</param>
    /// <param name="ConditionCalcLocation">The condition calculate location.</param>
    /// <param name="ConditionStr">The condition string.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr)
    {
        return AddUIRuleTask(ActivityType, UIRuleTask.eControlType.WhileDoCondition, ConditionCalcLocation, ConditionStr);
    }
    /// <summary>
    /// Adds the Controller do while loop task.
    /// </summary>
    /// <param name="ActivityType">Type of the activity.</param>
    /// <param name="conditionCalcLocation">The condition calculate location.</param>
    /// <param name="conditionStr">The condition string.</param>
    /// <returns></returns>
    public UIRuleTask AddUIRuleDoWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation conditionCalcLocation, string conditionStr)
    {
        return AddUIRuleTask(ActivityType, UIRuleTask.eControlType.DoWhileCondition, conditionCalcLocation, conditionStr);
    }
    /// <summary>
    /// Sets the Controller task detail: set local parameter.
    /// </summary>
    /// <param name="localParamId">The local parameter identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <returns></returns>
    public bool SetTask_SetLocalParam(eLocalParameterIds localParamId, UIRuleTask.eCalcLocation calcLocation, string formulaStr) //, int FormulaId
    {
        if (currentOperation == null) return false;
        currentOperation.localParamId = (int)localParamId;
        currentOperation.calcFormulaLocation = calcLocation;
        currentOperation.formulaStr = formulaStr;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: set content.
    /// </summary>
    /// <param name="assignToFieldId">The assign to field identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <returns></returns>
    public bool SetTask_SetContent(string assignToFieldId, UIRuleTask.eCalcLocation calcLocation, string formulaStr)//, int FormulaId
    {
        if (currentOperation == null) return false;
        //currentOperation.TargetParamType = ModelNamespace.isHeaderField(assignToFieldId)?1:2;
        //if (ModelNamespace.isHeaderField(assignToFieldId))
        //	currentOperation.TargetHeadParamId = (int)assignToFieldId;
        //else
        currentOperation.assignToFieldId = assignToFieldId;
        currentOperation.calcFormulaLocation = calcLocation;
        currentOperation.formulaStr = formulaStr;
        return true;
    }

    /// <summary>
    /// Sets the Controller task detail: set control part property.
    /// </summary>
    /// <param name="controlPropertyId">The control property identifier.</param>
    /// <param name="target">The target.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="FormulaStr">The formula string.</param>
    /// <returns></returns>
    public bool SetTask_SetControlPartProperty(eControlPropertyId controlPropertyId, eControlPropertyTarget target, UIRuleTask.eCalcLocation calcLocation, string FormulaStr)
    {
        if (currentOperation == null) return false;
        currentOperation.SpecificAttribute = controlPropertyId;
        currentOperation.calcFormulaLocation = calcLocation;
        currentOperation.formulaStr = FormulaStr;
        currentOperation.ControlPart = target;
        return true;
    }//, int FormulaId
    /// <summary>
    /// Sets the Controller task detail: set property.
    /// </summary>
    /// <param name="controlPropertyId">The control property identifier.</param>
    /// <param name="calcLocation">The calculate location.</param>
    /// <param name="FormulaStr">The formula string.</param>
    /// <returns></returns>
    public bool SetTask_SetProperty(eControlPropertyId controlPropertyId, UIRuleTask.eCalcLocation calcLocation, string FormulaStr)//, int FormulaId
    {
        if (currentOperation == null) throw new Exception("use SetTask_SetProperty without addCtrlrTask");
        if (!string.IsNullOrEmpty(currentOperation.conditionStr) && controlPropertyId == eControlPropertyId.FilterFormula)
        {
            //throw new Exception("use SetTask_SetProperty with addCtrlrIfTask");
        }
        currentOperation.SpecificAttribute = controlPropertyId;
        currentOperation.calcFormulaLocation = calcLocation;
        currentOperation.formulaStr = FormulaStr;
        currentOperation.ControlPart = eControlPropertyTarget.Content;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: set property's destination field.
    /// </summary>
    /// <param name="fieldId">The field identifier.</param>
    /// <returns></returns>
    public bool SetTask_SetProperty_ToField(string fieldId)
    {
        if (currentOperation == null) throw new Exception("use SetTask_SetProperty_ToField without addCtrlrTask");
        currentOperation.addParameter(new UIRuleTask.TaskParam(fieldId));
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: set property's destination control.
    /// </summary>
    /// <param name="controlId">The control identifier.</param>
    /// <returns></returns>
    public bool SetTask_SetProperty_ToControl(string controlId)
    {
        if (currentOperation == null) throw new Exception("use SetTask_SetProperty_ToControl without addCtrlrTask");
        currentOperation.addParameter(new UIRuleTask.TaskParam(controlId));
        return true;
    }
    /// <summary>
    /// Sets Controller task detail: validation.
    /// </summary>
    /// <param name="validationType">Type of the validation.</param>
    /// <param name="formulaCalcLocation">The formula calculate location.</param>
    /// <param name="formulaStr">The formula string.</param>
    /// <param name="exception">The exception.</param>
    /// <returns></returns>
    public bool SetTask_Validation(UIRuleTask.eValidationType validationType, UIRuleTask.eCalcLocation formulaCalcLocation,
        string formulaStr, ExceptionInformation exception)
    {
        if (currentOperation == null) throw new Exception("use SetCtrlrTask_Validation without addCtrlrTask");
        currentOperation.validationType = validationType;
        currentOperation.calcFormulaLocation = formulaCalcLocation;
        currentOperation.formulaStr = formulaStr;
        currentOperation.exceptionInfo = exception;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: one row query.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    public bool SetTask_OneRowQuery(string modelId, string entityId)
    {
        if (currentOperation == null) return false;
        currentOperation.modelId = modelId;
        currentOperation.entityId = entityId;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: one row query field destination.
    /// </summary>
    /// <param name="queryFieldId">The query field identifier.</param>
    /// <param name="viewModelFieldId">The view model field identifier.</param>
    /// <returns></returns>
    public bool SetTask_OneRowQuery_ToField(string queryFieldId, string viewModelFieldId)
    {
        if (currentOperation == null) return false;
        currentOperation.addParameter(new UIRuleTask.TaskParam(queryFieldId, viewModelFieldId));
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: one row query local parameter destination.
    /// </summary>
    /// <param name="queryFieldId">The query field identifier.</param>
    /// <param name="localParameterId">The local parameter identifier.</param>
    /// <returns></returns>
    public bool SetTask_OneRowQuery_ToLocalParameter(string queryFieldId, eLocalParameterIds localParameterId)
    {
        if (currentOperation == null) return false;
        currentOperation.addParameter(new UIRuleTask.TaskParam(queryFieldId, localParameterId));
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: one row query filter.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns></returns>
    public bool SetTask_OneRowQuery_Filter(string filter)
    {
        if (currentOperation == null) return false;
        currentOperation.addQueryFilter(new UIRuleTask.QueryFilter(filter));
        return true;
    }

    /// <summary>
    /// Sets the Controller task detail: Call the specific Data Operation
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="operationId">The operation identifier.</param>
    /// <returns></returns>
    public bool SetTask_CallDataOperation(string modelId, string operationId)
    {
        if (currentOperation == null) return false;
        currentOperation.modelId = modelId;
        currentOperation.operationId = operationId;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: Parameter of Call the specific Data Operation 
    /// </summary>
    /// <param name="operationFieldId">The operation field identifier.</param>
    /// <param name="viewModelFieldId">The view model field identifier.</param>
    /// <returns></returns>
    public bool SetTask_CallDataOperation_Param(string operationFieldId, string viewModelFieldId)
    {
        if (currentOperation == null) return false;
        currentOperation.addParameter(new UIRuleTask.TaskParam(operationFieldId, viewModelFieldId));
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: call controller.
    /// </summary>
    /// <param name="controllerId">The controller identifier.</param>
    /// <returns></returns>
    public bool SetTask_CallController_ControllerId(string controllerId)
    {
        if (currentOperation == null) return false;
        currentOperation.controlerId = controllerId;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: open form.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="formId">The form identifier.</param>
    /// <returns></returns>
    public bool SetTask_OpenForm(string modelId, string formId)
    {
        if (currentOperation == null) return false;
        currentOperation.modelId = modelId;
        currentOperation.form_rerportId = formId;
        return true;
    }
    /// <summary>
    /// Sets the Controller task detail: open report.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="reportId">The report identifier.</param>
    /// <returns></returns>
    public bool SetTask_OpenReport(string modelId, string reportId)
    {
        if (currentOperation == null) return false;
        currentOperation.modelId = modelId;
        currentOperation.form_rerportId = reportId;
        return true;
    }
    #endregion controller
}
