using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Forms;


namespace Neo.Bpms.Domain.Modeling.Definitions.Entities;

public abstract partial class FormDefinition
{
    public virtual bool DefineUIRules()
    {
        return true;
    }

    protected virtual void UIRules()
    {
    }

    /// <summary>
    /// Adds the UI Rule.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="pagePart"></param>
    /// <returns></returns>
    public UIRule AddUIRule(string id, string name, PagePart pagePart = PagePart.Form)
    {
        var uiRule = new UIRule(form, id, name, pagePart);
        currentBaseElement = uiRule;
        uiRuleDefinition.CurrentUIRule = uiRule;
        form.AddUIRule(uiRule);
        return uiRule;
    }

    #region interface functions

    private readonly UIRuleDefinition uiRuleDefinition = new UIRuleDefinition();

    public UIRuleEvent AddUIRuleEvent(UIRuleEvent ev)
    {
        return uiRuleDefinition.AddUIRuleEvent(ev);
    }

    public UIRuleEvent AddUIRuleEvent(UIRuleEvent.eEventType type, string fieldId = null, string RepeatScopeId = null)
    {
        return uiRuleDefinition.AddUIRuleEvent(type, fieldId, RepeatScopeId);
    }

    public UIRuleTask AddUIRuleTask(UIRuleTask operation)
    {
        return uiRuleDefinition.AddUIRuleTask(operation);
    }

    public UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType, UIRuleTask.eControlType controlType,
        UIRuleTask.eCalcLocation calcLocation, string ConditionStr)
    {
        return uiRuleDefinition.AddUIRuleTask(taskType, controlType, calcLocation, ConditionStr);
    }

    public UIRuleTask AddUIRuleTask(UIRuleTask.eTaskType taskType)
    {
        return uiRuleDefinition.AddUIRuleTask(taskType);
    }

    public UIRuleTask AddUIRuleIfTask(UIRuleTask.eTaskType taskType, UIRuleTask.eCalcLocation ConditionCalcLocation,
        string ConditionStr)
    {
        return uiRuleDefinition.AddUIRuleIfTask(taskType, ConditionCalcLocation, ConditionStr);
    }

    public UIRuleTask AddUIRuleWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr)
    {
        return uiRuleDefinition.AddUIRuleWhileTask(ActivityType, ConditionCalcLocation, ConditionStr);
    }

    public UIRuleTask AddUIRuleDoWhileTask(UIRuleTask.eTaskType ActivityType,
        UIRuleTask.eCalcLocation ConditionCalcLocation, string ConditionStr)
    {
        return uiRuleDefinition.AddUIRuleDoWhileTask(ActivityType, ConditionCalcLocation, ConditionStr);
    }

    public bool SetTask_SetLocalParam(eLocalParameterIds localParamId, UIRuleTask.eCalcLocation calcLocation,
        string formulaStr)
    {
        return uiRuleDefinition.SetTask_SetLocalParam(localParamId, calcLocation, formulaStr);
    }

    public bool SetTask_SetContent(string assignToFieldId, UIRuleTask.eCalcLocation calcLocation, string formulaStr)
    {
        return uiRuleDefinition.SetTask_SetContent(assignToFieldId, calcLocation, formulaStr);
    }

    public bool SetTask_SetControlPartProperty(eControlPropertyId controlPropertyId, eControlPropertyTarget target,
        UIRuleTask.eCalcLocation calcLocation, string FormulaStr)
    {
        return uiRuleDefinition.SetTask_SetControlPartProperty(controlPropertyId, target, calcLocation, FormulaStr);
    }

    public bool SetTask_SetProperty(eControlPropertyId controlPropertyId, UIRuleTask.eCalcLocation calcLocation,
        string FormulaStr)
    {
        return uiRuleDefinition.SetTask_SetProperty(controlPropertyId, calcLocation, FormulaStr);
    }

    public bool SetTask_SetProperty_ToField(string fieldId)
    {
        return uiRuleDefinition.SetTask_SetProperty_ToField(fieldId);
    }

    public bool SetTask_SetProperty_ToControl(string controlId)
    {
        return uiRuleDefinition.SetTask_SetProperty_ToControl(controlId);
    }

    public bool SetTask_Validation(UIRuleTask.eValidationType validationType,
        UIRuleTask.eCalcLocation formulaCalcLocation, string formulaStr, ExceptionInformation exception)
    {
        return uiRuleDefinition.SetTask_Validation(validationType, formulaCalcLocation, formulaStr, exception);
    }

    public bool SetTask_OneRowQuery(string modelId, string entityId)
    {
        return uiRuleDefinition.SetTask_OneRowQuery(modelId, entityId);
    }

    public bool SetTask_OneRowQuery_ToField(string queryFieldId, string viewModelFieldId)
    {
        return uiRuleDefinition.SetTask_OneRowQuery_ToField(queryFieldId, viewModelFieldId);
    }

    public bool SetTask_OneRowQuery_ToLocalParameter(string queryFieldId, eLocalParameterIds localParameterId)
    {
        return uiRuleDefinition.SetTask_OneRowQuery_ToLocalParameter(queryFieldId, localParameterId);
    }

    public bool SetTask_OneRowQuery_Filter(string filter)
    {
        return uiRuleDefinition.SetTask_OneRowQuery_Filter(filter);
    }

    public bool SetTask_CallDataOperation(string modelId, string operationId)
    {
        return uiRuleDefinition.SetTask_CallDataOperation(modelId, operationId);
    }

    public bool SetTask_CallDataOperation_Param(string operationFieldId, string viewModelFieldId)
    {
        return uiRuleDefinition.SetTask_CallDataOperation_Param(operationFieldId, viewModelFieldId);
    }

    public bool SetTask_CallController_ControllerId(string controllerId)
    {
        return uiRuleDefinition.SetTask_CallController_ControllerId(controllerId);
    }

    public bool SetTask_OpenForm(string modelId, string formId)
    {
        return uiRuleDefinition.SetTask_OpenForm(modelId, formId);
    }

    public bool SetTask_OpenReport(string modelId, string reportId)
    {
        return uiRuleDefinition.SetTask_OpenReport(modelId, reportId);
    }

    #endregion interface functions
}
