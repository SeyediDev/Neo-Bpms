using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

public class FormLogicOperationDefinition
{
    public FormLogicOperationDefinition()
    {
        FormulaList = [];
    }

    public FormLogicOperationDefinition(UIRuleTask op,
        IFormLogicHelper formLogicHelper)
    {
        FormulaList = [];
        AddFormula(op);

        Condition = formLogicHelper.ConvertToClientCode(op.conditionStr);
        Formula = op.formulaStr;
        Loop_ConditionType = op.ControlType;
        Type = formLogicHelper.GetClientOperationType(op);
    }

    public void AddFormula(UIRuleTask op)
    {
        if (op.conditionCalcLocation == UIRuleTask.eCalcLocation.Server)
            AddFormula(op.conditionStr);
        if (op.calcFormulaLocation != UIRuleTask.eCalcLocation.Server)
            return;
        AddFormula(op.formulaStr);
        foreach (UIRuleTask.TaskParam taskParam in op.Params ?? Enumerable.Empty<UIRuleTask.TaskParam>())
            AddFormula(taskParam.query_operationFieldId);
        foreach (UIRuleTask.QueryFilter queryFilter in op.queryFilters ?? Enumerable.Empty<UIRuleTask.QueryFilter>())
            AddFormula(queryFilter.filter);
    }

    private void AddFormula(string formula)
    {
        if (!string.IsNullOrEmpty(formula))
            FormulaList.Add(formula);
    }

    public List<string> FormulaList { get; set; }

    public eOperationType Type { get; set; }
    public UIRuleTask.eControlType Loop_ConditionType { get; set; } = UIRuleTask.eControlType.NoCondition;
    public string TargetId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Attr { get; set; } = string.Empty;
    public eTargetArea TargetArea { get; set; } = eTargetArea.Input;
    public string Value { get; set; } = string.Empty;
    public List<FormDataRow> Rows { get; set; } = [];
    public string Condition { get; set; } = string.Empty;
    public string Formula { get; set; }
}
