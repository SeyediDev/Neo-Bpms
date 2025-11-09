namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public List<OrderByDefinition> OrderBys { get; set; }
    public List<OrderByDefinition> BaseOrderBys { get; set; }

    public QueryUtility OrderBy(string fieldId, SortType order = SortType.Ascending, bool orderById = false)
    {
        var field = Entity.GetField(fieldId);
        if (field != null)
        {
            AddOrderBy(field, order, orderById);
        }
        else
        {
            var associationIds = fieldId.Split('.');
            var queryUtility = this;
            var hasSub = associationIds.Length > 1 && fieldId.IndexOf("(", StringComparison.Ordinal) < 0;
            if (hasSub)
            {
                for (var ia = 0; ia < associationIds.Length; ia++)
                {
                    field = queryUtility?.Entity?.GetField(associationIds[ia]);
                    if (ia == associationIds.Length - 1) break;
                    var associationEntity = field?.AssociationEntity?.Entity();
                    if (associationEntity == null) break;
                    var jq = queryUtility.LeftOuterJoin(associationEntity, null);
                    jq?.SetMapping(field);
                    queryUtility = jq?.Query;
                }
            }

            if (field != null && queryUtility != null)
                queryUtility.AddOrderBy(field, order, orderById);
            else if (hasSub && associationIds[0] == queryUtility?.Entity?.Id)
            {
                var fId = fieldId[(associationIds[0].Length + 1)..];
                addOrderBy(new OrderByDefinition(Entity.GetField(fId))
                { fieldName = fId, order = order, orderById = orderById });
            }
            else
                addOrderBy(new OrderByDefinition(null) { fieldName = fieldId, order = order, orderById = orderById });
        }

        return this;
    }

    public void AddKeyAsOrderIfNotAnyOrderBy()
    {
        if (BaseQuery.BaseOrderBys?.Count > 0)
            return;
        AddKeyAsOrderBy();
    }

    public void AddKeyAsOrderBy(SortType sortType = SortType.Ascending)
    {
        foreach (var keyField in Entity.KeyFields)
            AddOrderBy(keyField, sortType, true);
    }

    private void AddOrderBy(EntityField field, SortType order, bool orderById)
    {
        if (field.NotMap)
        {
            if (field.Formula?.FormulaBody != null)
                addOrderBy(new OrderByDefinition(field) { fieldName = field.Formula?.FormulaText, order = order, orderById = orderById });
            else if (field.AssociationEntity?.Maps != null)
            {
                foreach (var map in field.AssociationEntity.Maps)
                    OrderBy(map.SourceField, order, orderById);
            }
        }
        else if (field.AssociationEntity == null)
            addOrderBy(new OrderByDefinition(field) { fieldName = field.Id, order = order, orderById = orderById });
    }

    private void addOrderBy(OrderByDefinition orderby)
    {
        var baseQuery = BaseQuery;
        baseQuery.BaseOrderBys ??= [];
        if (baseQuery.BaseOrderBys.FirstOrDefault(g =>
                (g.fieldName ?? "") == (orderby.fieldName ?? "") &&
                g.formula?.toText() == orderby.formula?.toText()) != null)
            return;
        orderby.orderIndex = baseQuery.BaseOrderBys.Count * 100;
        baseQuery.BaseOrderBys.Add(orderby);
        OrderBys ??= [];
        OrderBys.Add(orderby);
    }
}
