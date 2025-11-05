using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Expressions.Parsers;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class FormulaFieldViewModel
{
    public FormulaFieldViewModel()
    {

    }
    public FormulaFieldViewModel(EntityFieldFormula fieldFormula)
    {
        formulaText = fieldFormula.FormulaText;
        usedForAggregationOnly = fieldFormula.UsedForAggregationOnly;
    }

    public string formulaText { get; set; }

    public bool usedForAggregationOnly { get; set; }

    public EntityFieldFormula ToFormula()
    {
        return string.IsNullOrEmpty(formulaText)
            ? null
            : new EntityFieldFormula
            {
                FormulaText = formulaText,
                FormulaBody = Parser.Parse(formulaText), // todo 1. correct? 2. other formulas!
                UsedForAggregationOnly = usedForAggregationOnly
            };
    }
}
