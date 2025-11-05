using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Controller;

public class ControllerTaskViewModel
{
    public ControllerTaskViewModel()
    {

    }
    public ControllerTaskViewModel(UIRuleTask task)
    {
        type = task.TaskType;
        calcLocation = task.conditionCalcLocation.ToString();
        condition = task.conditionStr;
        typeSpecificProperties = new ControllerTaskTypeSpecificPropertiesViewModel
        {
            propertyType = task.SpecificAttribute,
            calcLocation = task.calcFormulaLocation.ToString(),
            formula = task.formulaStr,
            target = task.assignToFieldId,
            targets = task.Params?.Select(p => p.field_controlId),
            localParamId = task.localParamId
        };
    }
    public UIRuleTask.eTaskType? type { get; set; }
    public string calcLocation { get; set; }
    public string condition { get; set; }
    public ControllerTaskTypeSpecificPropertiesViewModel typeSpecificProperties { get; set; }

    public UIRuleTask ToOperation()
    {
        return new UIRuleTask
        {
            TaskType = type ?? UIRuleTask.eTaskType.SetProperty,//todo!!
            conditionCalcLocation = calcLocation.Equals("Server") ? UIRuleTask.eCalcLocation.Server : UIRuleTask.eCalcLocation.Client,
            conditionStr = condition,
            ControlType = string.IsNullOrWhiteSpace(condition) ? UIRuleTask.eControlType.NoCondition : UIRuleTask.eControlType.IfCondition,
            SpecificAttribute = typeSpecificProperties?.propertyType ?? eControlPropertyId.None,
            calcFormulaLocation = (typeSpecificProperties?.calcLocation?.Equals("Server") ?? false) ? UIRuleTask.eCalcLocation.Server : UIRuleTask.eCalcLocation.Client,
            formulaStr = typeSpecificProperties?.formula,
            assignToFieldId = typeSpecificProperties?.target,
            Params = typeSpecificProperties?.targets?.Select(t => new UIRuleTask.TaskParam(t)).ToList(),
            localParamId = typeSpecificProperties?.localParamId ?? 0
        };
    }
}