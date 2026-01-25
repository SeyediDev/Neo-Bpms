namespace Neo.Bpms.Domain.Features.Definitions.Entities;

public abstract partial class FormDefinition
{
    public void ShowHide(string userChangeFieldId, string condition, params string[] controlledParams)
    {
        ShowHide(userChangeFieldId, condition, UIRuleTask.eCalcLocation.Client, controlledParams);
    }

    public void ShowHideControl(string userChangeFieldId, string condition, string controlId)
    {
        var id = $"ShowHide_{controlId}";
        AddUIRule(id, id);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client, condition);
        SetTask_SetProperty_ToControl(controlId);
    }
    public void ShowHideControl(string userChangeFieldId, string condition, params string[] controls)
    {
        var id = $"ShowHide_{controls}";
        AddUIRule(id, id);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client, condition);
        foreach (var control in controls)
        {
            SetTask_SetProperty_ToControl(control);
        }
    }

    public void ServerShowHide(string userChangeFieldId, string condition, params string[] controlledParams)
    {
        ShowHide(userChangeFieldId, condition, UIRuleTask.eCalcLocation.Server, controlledParams);
    }

    private void ShowHide(string userChangeFieldId, string condition, UIRuleTask.eCalcLocation side,
        params string[] controlledParams)
    {
        var id = $"ShowHide_By_{condition}";
        if (AddUIRule(id, id) == null) return;
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        SetTask_SetProperty(eControlPropertyId.ShowHide, side, condition);
        foreach (var controlledParam in controlledParams)
        {
            SetTask_SetProperty_ToField(controlledParam);
        }
    }

    public void ReadOnlyOnPageLoad(string condition, params string[] controlledParams)
    {
        var id = $"ReadOnly_By_OnPageLoad";
        AddUIRule(id, id);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        SetTask_SetProperty(eControlPropertyId.ReadOnly, UIRuleTask.eCalcLocation.Server, condition);
        foreach (var controlledParam in controlledParams)
        {
            SetTask_SetProperty_ToField(controlledParam);
        }
    }

    public void ShowHideEmptyIndexSubTable(string tableId, string controlId)
    {
        AddUIRule(tableId, tableId);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            $"!(CEF.IsTableEmpty('{tableId}'))");
        SetTask_SetProperty_ToControl(controlId);
    }

    public void FilterFormula(string userChangeFieldId, string controlledParam, string filter)
    {
        var id = $"FilterFormula_On_{controlledParam}";
        if (AddUIRule(id, id) == null) return;
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        {
            SetTask_SetProperty(eControlPropertyId.FilterFormula, UIRuleTask.eCalcLocation.Server, filter);
            SetTask_SetProperty_ToField(controlledParam);
        }
    }
    
    public void FilterFormula(List<string> userChangeFieldIds, List<string> controlledParams, string filter)
    {
        var id = $"FilterFormula_On_{string.Join(',', controlledParams)}";
        if (AddUIRule(id, id) == null) return;
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        foreach (var userChangeFieldId in userChangeFieldIds)
        {
            if (!string.IsNullOrEmpty(userChangeFieldId))
                AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        }
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
        {
            SetTask_SetProperty(eControlPropertyId.FilterFormula, UIRuleTask.eCalcLocation.Server, filter);
            foreach (var controlledParam in controlledParams)
            {
                if (!string.IsNullOrEmpty(controlledParam))
                    SetTask_SetProperty_ToField(controlledParam);
            }
        }
    }

    public void FilterFormulaWithCondition(string userChangeFieldId, string controlledParam, string filter, string condition)
    {
        var id = $"FilterFormula_On_{controlledParam}";
        if (AddUIRule(id, id) == null) return;
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetProperty, UIRuleTask.eControlType.IfCondition, UIRuleTask.eCalcLocation.Server, condition);
        {
            SetTask_SetProperty(eControlPropertyId.FilterFormula, UIRuleTask.eCalcLocation.Server, filter);
            SetTask_SetProperty_ToField(controlledParam);
        }
    }
    public void FilterFormulaWithClientCondition(string userChangeFieldId, string controlledParam, string filter, string condition)
    {
        var id = $"FilterFormula_On_{controlledParam}";
        if (AddUIRule(id, id) == null) return;
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        if (!string.IsNullOrEmpty(userChangeFieldId))
            AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, userChangeFieldId);
        AddUIRuleIfTask(UIRuleTask.eTaskType.SetProperty, UIRuleTask.eCalcLocation.Client, condition);
        {
            SetTask_SetProperty(eControlPropertyId.FilterFormula, UIRuleTask.eCalcLocation.Server, filter);
            SetTask_SetProperty_ToField(controlledParam);
        }
    }

    public void FilterUserByUserGroupAndActiveUsers(string userGroupCode)
    {
        AddProperty(eControlPropertyId.FilterFormula, Func.Exists(
            "UserAndOrganization.SystemUser_UserGroup",
            "(IsNull(Disable,0)==0)" +
            "And (UserId==(SystemUser.Id))" +
            $"And ((Group.Code)='{userGroupCode}')" + // user group code
            "And (IsNull((Group.Disable),0)==0)" + //active user group
            "And (IsNull((SystemUser.Disable),0)==0)" //active users
        ));
    }

    public void FilterUserByUserGroupAllUsers(string userGroupCode)
    {
        AddProperty(eControlPropertyId.FilterFormula, Func.Exists(
            "UserAndOrganization.SystemUser_UserGroup",
            "(IsNull(Disable,0)==0)" +
            "And (UserId==(SystemUser.Id))" +
            $"And ((Group.Code)='{userGroupCode}')" + // user group code
            "And (IsNull((Group.Disable),0)==0)" //active user group
        ));
    }

    public void AddGroupControlUiRule(string fieldName, params string[] groupFields)
    {
        if (AddUIRule($"{fieldName}Control", $"{fieldName}Control") != null)
        {
            var condition = "";
            foreach (var groupField in groupFields)
            {
                AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, $"{fieldName}.{groupField}");
                if (!string.IsNullOrEmpty(condition))
                    condition += " And ";
                condition +=
                    $"(IsNullOrEmpty(q[{fieldName}.{groupField}]) Or ({groupField}Id In(q[{fieldName}.{groupField}])))";
            }

            AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            {
                SetTask_SetProperty(eControlPropertyId.FilterFormula, UIRuleTask.eCalcLocation.Server, condition);
                SetTask_SetProperty_ToField(fieldName);
            }
        }
    }

    public void SetContentOnPageLoad(string fieldId, string formula)
    {
        var id = $"SetContent_On_{fieldId}";
        AddUIRule(id, id);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        AddUIRuleTask(UIRuleTask.eTaskType.SetContent);
        {
            SetTask_SetContent(fieldId, UIRuleTask.eCalcLocation.Server, formula);
        }
    }

    public void SetContentOnChange(string onchangeFieldId, string fieldId, string formula)
    {
        var id = $"SetContent_On_{fieldId}";
        AddUIRule(id, id);
        AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
        AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, onchangeFieldId);
        AddUIRuleTask(UIRuleTask.eTaskType.SetContent);
        {
            SetTask_SetContent(fieldId, UIRuleTask.eCalcLocation.Server, formula);
        }
    }
}
