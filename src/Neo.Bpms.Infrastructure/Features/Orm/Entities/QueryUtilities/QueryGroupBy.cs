namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public List<AggregateDefinition> GroupBys;
    public QueryUtility GroupBy(string fieldId, string overName = null)
    {
        AddGroupBy(fieldId, eAggregationFunctions.GroupByItem, eAggregateScope.All, overName);
        return this;
    }

    public QueryUtility GroupByField(string fieldId, string overName = null)
    {
        AddGroupBy(fieldId, eAggregationFunctions.InColumn, eAggregateScope.All, overName);
        return this;
    }

    public QueryUtility GroupBy(string fieldId, bool inColumn, string overName = null)
    {
        if (inColumn)
            AddGroupBy(fieldId, eAggregationFunctions.InColumn, eAggregateScope.All, overName);
        AddGroupBy(fieldId, eAggregationFunctions.GroupByItem, eAggregateScope.All, overName);
        return this;
    }
    public QueryUtility GroupByFormula(string formula, bool inColumn, string overName = null)
    {
        if (inColumn)
            GroupByFormula(formula, eAggregationFunctions.InColumn, eAggregateScope.All, overName);
        GroupByFormula(formula, eAggregationFunctions.GroupByItem, eAggregateScope.All, overName);
        return this;
    }

    public QueryUtility GroupBy(string fieldId, eAggregationFunctions func, string overName = null)
    {
        AddGroupBy(fieldId, func, eAggregateScope.All, overName);
        return this;
    }

    public QueryUtility GroupBy(string fieldId, eAggregationFunctions func, eAggregateScope aggScope)
    {
        AddGroupBy(fieldId, func, aggScope, null);
        return this;
    }

    public QueryUtility GroupBy(string fieldId, eAggregationFunctions func, eAggregateScope aggScope, string overFieldId)
    {
        AddGroupBy(fieldId, func, aggScope, overFieldId);
        return this;
    }

    public QueryUtility GroupBy(ExpressionNode formula, eAggregationFunctions func, eAggregateScope aggScope,
        string overFieldId)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = formula,
            function = func,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }

    public QueryUtility GroupByFormula(string formula, eAggregationFunctions func, eAggregateScope aggScope,
        string overFieldId)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = Parser.Parse(formula),
            function = func,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }

    public QueryUtility GroupByItem(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        return GroupBy(fieldId, eAggregationFunctions.GroupByItem, aggScope, overFieldId);
    }

    public QueryUtility GroupByColumn(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        return GroupBy(fieldId, eAggregationFunctions.InColumn, aggScope, overFieldId);
    }

    private void AddGroupBy(string fieldId, eAggregationFunctions func, eAggregateScope aggScope, string overFieldId)
    {
        var field = Entity.GetField(fieldId);
        if (field != null)
        {
            if (field.NotMap)
            {
                if (field.Formula?.FormulaBody != null)
                {
                    if (!field.Formula.UsedForAggregationOnly && (func == eAggregationFunctions.Formula || func == eAggregationFunctions.InColumn))
                    {
                        addGroupBy(new AggregateDefinition(field)
                        {
                            formula = field.Formula?.FormulaBody,
                            function = eAggregationFunctions.GroupByItem,
                            aggregateScope = aggScope,
                            overFieldName = string.IsNullOrEmpty(overFieldId) ? field.Id : overFieldId
                        });
                    }
                    addGroupBy(new AggregateDefinition(field)
                    {
                        formula = field.Formula?.FormulaBody,
                        function = func,
                        aggregateScope = aggScope,
                        overFieldName = string.IsNullOrEmpty(overFieldId) ? field.Id : overFieldId
                    });
                }
                else if (field.AssociationEntity?.Maps != null)
                {
                    foreach (var map in field.AssociationEntity.Maps)
                        addGroupBy(new AggregateDefinition(field)
                        {
                            fieldName = map.SourceField,
                            function = func,
                            aggregateScope = aggScope,
                            overFieldName = overFieldId
                        });
                }
            }
            else if (field.AssociationEntity == null)
                addGroupBy(new AggregateDefinition(field)
                {
                    fieldName = field.Id,
                    function = func,
                    aggregateScope = aggScope,
                    overFieldName = overFieldId
                });
        }
        else
            addGroupBy(new AggregateDefinition(field)
            {
                fieldName = fieldId,
                function = func,
                aggregateScope = aggScope,
                overFieldName = overFieldId
            });
    }

    private void addGroupBy(AggregateDefinition groupby)
    {
        GroupBys ??= [];
        if (GroupBys.FirstOrDefault(g =>
                (g.fieldName ?? "") == (groupby.fieldName ?? "") &&
                g.formula?.toText() == groupby.formula?.toText() &&
                g.overFieldName == groupby.overFieldName &&
                g.function == groupby.function &&
                g.aggregateScope == groupby.aggregateScope &&
                g.isField == groupby.isField) != null)
            return;
        GroupBys.Add(groupby);
    }
}
