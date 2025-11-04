using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Base;

public abstract class BaseFieldDefinition(EntityField field)
{
    public EntityField Field { get; set; } = field;
    public string fieldName { get; set; } = field?.DbFieldName;
}

public interface IContainsFormula
{
    EntityField Field { get; set; }
    string fieldName { get; set; }
    ExpressionNode formula { get; set; }
}


public class FieldDefinition(EntityField field) : BaseFieldDefinition(field)
{
    public string overFieldName = field?.Id;
    public string FieldId => overFieldName ?? fieldName;
}

public class ColumnDefinition(EntityField field, object formulaOrValue = null) : FieldDefinition(field)
{
    public object formula_value = formulaOrValue;
}

public class OrderByDefinition(EntityField field) : BaseFieldDefinition(field), IContainsFormula
{
    public ExpressionNode formula { get; set; }
    public SortType order { get; set; }
    public bool orderById { get; set; }
    public int orderIndex { get; set; }
}

public class AggregateDefinition(EntityField field) : BaseFieldDefinition(field), IContainsFormula
{
    public ExpressionNode formula { get; set; }
    public eAggregationFunctions function { get; set; }
    public eAggregateScope aggregateScope { get; set; }
    public string overFieldName { get; set; }

    public bool isField => function != eAggregationFunctions.InColumn;
}

public class FormulaDefinition
{
    public string asFieldName;
    public ExpressionNode formula;
}

public class FilterDefinition
{
    public string Filter { get; set; }
    public string OrGroupId { get; set; }
    public ExpressionNode FilterExpression { get; set; }
}