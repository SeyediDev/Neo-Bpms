namespace Neo.Bpms.Domain.Entities.Cmmn.Fields;

public class EntityFieldFormula
{
    public string FormulaMethodId;
    [XmlIgnore]
    public ExpressionNode FormulaBody { get; set; }
    public string FormulaText { get; set; }
    public bool UsedForAggregationOnly { get; set; }
}
