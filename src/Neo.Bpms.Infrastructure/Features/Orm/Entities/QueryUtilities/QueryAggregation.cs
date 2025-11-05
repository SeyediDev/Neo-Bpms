using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public QueryUtility Max(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        if (overFieldId == null && aggScope == eAggregateScope.All)
            overFieldId = fieldId;
        return GroupBy(fieldId, eAggregationFunctions.Max, aggScope, overFieldId);
    }

    public QueryUtility Min(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        if (overFieldId == null && aggScope == eAggregateScope.All)
            overFieldId = fieldId;
        return GroupBy(fieldId, eAggregationFunctions.Min, aggScope, overFieldId);
    }

    public QueryUtility Count(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        if (overFieldId == null && aggScope == eAggregateScope.All)
            overFieldId = fieldId;
        return GroupBy(fieldId, eAggregationFunctions.Count, aggScope, overFieldId);
    }

    public QueryUtility Sum(string fieldId, string overFieldId = null, eAggregateScope aggScope = eAggregateScope.All)
    {
        if (overFieldId == null && aggScope == eAggregateScope.All)
            overFieldId = fieldId;
        return GroupBy(fieldId, eAggregationFunctions.Sum, aggScope, overFieldId);
    }

    public QueryUtility SumFormula(string formula, string overFieldId, eAggregateScope aggScope = eAggregateScope.All)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = Parser.Parse(formula),
            function = eAggregationFunctions.Sum,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }

    public QueryUtility MinFormula(string formula, string overFieldId, eAggregateScope aggScope = eAggregateScope.All)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = Parser.Parse(formula),
            function = eAggregationFunctions.Min,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }

    public QueryUtility MaxFormula(string formula, string overFieldId, eAggregateScope aggScope = eAggregateScope.All)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = Parser.Parse(formula),
            function = eAggregationFunctions.Max,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }

    public QueryUtility CountFormula(string formula, string overFieldId, eAggregateScope aggScope = eAggregateScope.All)
    {
        addGroupBy(new AggregateDefinition(Entity.GetField(overFieldId))
        {
            formula = Parser.Parse(formula),
            function = eAggregationFunctions.Count,
            aggregateScope = aggScope,
            overFieldName = overFieldId
        });
        return this;
    }
}
