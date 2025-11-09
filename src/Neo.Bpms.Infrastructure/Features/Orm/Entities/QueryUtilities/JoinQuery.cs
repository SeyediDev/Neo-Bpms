namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public Dictionary<string, SubQueryDefinition> subQuerys;
    public QueryUtility? ParentQuery => Parent as QueryUtility;
    public QueryUtility BaseQuery
    {
        get
        {
            var baseQuery = this;
            while (baseQuery.ParentQuery != null)
                baseQuery = baseQuery.ParentQuery;
            return baseQuery;
        }
    }

    public QueryUtility Include(string association)
    {
        var field = Entity?.GetField(association);
        if (field == null) return null;

        var query = this;
        foreach (var associationId in association.Split('.'))
        {
            field = query.Entity?.GetField(associationId);
            if (field?.AssociationEntity?.Entity() == null) return null;
            query = query.LeftOuterJoin(field)?.Query;
            if (query == null) break;
        }
        return query;
    }

    public QueryUtility IncludeFields(string association, params string[] fieldList)
    {
        var join = Include(association);
        foreach (var item in fieldList)
            join.SelectField(item, association + "." + item);
        return this;
    }

    public QueryUtility Join(string association)
    {
        var field = Entity?.GetField(association);
        if (field == null) return null;

        var query = this;
        foreach (var associationId in association.Split('.'))
        {
            field = query.Entity?.GetField(associationId);
            if (field?.AssociationEntity?.Entity() == null) return null;
            query = query.Join(field)?.Query;
            if (query == null) break;
        }
        return query;
    }
    public SubQueryDefinition Join(EntityField field)
    {
        var q = addOrGetSubTable(eJoinType.InnerJoin, field?.AssociationEntity?.Entity().model.Id, (field?.AssociationEntity).Entity().Id, null);
        q.SetMapping(field);
        return q;
    }

    public SubQueryDefinition LeftOuterJoin(EntityField field)
    {
        var q = addOrGetSubTable(eJoinType.LeftOuterJoin, field?.AssociationEntity?.Entity().model.Id, (field?.AssociationEntity).Entity().Id,
            null);
        q.SetMapping(field);
        return q;
    }

    public SubQueryDefinition Union(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.Union, namespaceId, entityId, mappings);
    }

    public SubQueryDefinition Intersect(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.Intersect, namespaceId, entityId, mappings);
    }

    //Include and Shape:
    public SubQueryDefinition ShapeLists(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.ShapeLists, namespaceId, entityId, mappings);
    }

    public SubQueryDefinition ShapePerItemByItem(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.ShapePerItemByItem, namespaceId, entityId, mappings);
    }

    public QueryUtility GroupShapeLists()
    {
        return addOrGetSubTable(eJoinType.GroupShapeLists, Entity.model.Id, Entity.Id,
            new JoinDefinition.JoinFieldMapping("Id", null, "Id", null)).Query;
    }

    public QueryUtility GroupShapeItemByItem()
    {
        return addOrGetSubTable(eJoinType.GroupShapeItemByItem, Entity.model.Id, Entity.Id,
            new JoinDefinition.JoinFieldMapping("Id", null, "Id", null)).Query;
    }

    public SubQueryDefinition Join(string namespaceId, string entityId,
        string joinTableFieldName, string mainTableFieldName)
    {
        return addOrGetSubTable(eJoinType.InnerJoin, namespaceId, entityId,
            new JoinDefinition.JoinFieldMapping(mainTableFieldName, joinTableFieldName));
    }
    public SubQueryDefinition Join<TJoinEntity>(
        string joinTableFieldName, string mainTableFieldName)
        where TJoinEntity : new()
    {
        var joinEntity = ProjectDefinition.Project.GetEntity<TJoinEntity>();
        return Join(joinEntity.NamespaceId, joinEntity.Id,
            joinTableFieldName, mainTableFieldName);
    }

    public SubQueryDefinition Join(string namespaceId, string entityId, params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.InnerJoin, namespaceId, entityId, mappings);
    }

    public SubQueryDefinition Join(Entity joinEntity, params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.InnerJoin, joinEntity, mappings);
    }

    public SubQueryDefinition LeftOuterJoin(Entity joinEntity, params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.LeftOuterJoin, joinEntity, mappings);
    }

    public SubQueryDefinition LeftOuterJoin(string namespaceId, string entityId, string joinTableFieldName,
        string mainTableFieldName)
    {
        return addOrGetSubTable(eJoinType.LeftOuterJoin, namespaceId, entityId,
            new JoinDefinition.JoinFieldMapping(mainTableFieldName, joinTableFieldName));
    }

    public SubQueryDefinition LeftOuterJoin<T>(string joinTableFieldName, string mainTableFieldName)
        where T : new()
    {
        return addOrGetSubTable(eJoinType.LeftOuterJoin, ProjectDefinition.Project.GetEntity<T>(),
            new JoinDefinition.JoinFieldMapping(mainTableFieldName, joinTableFieldName));
    }

    public SubQueryDefinition LeftOuterJoin(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.LeftOuterJoin, namespaceId, entityId, mappings);
    }

    public SubQueryDefinition RightOuterJoin(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.RightOuterJoin, namespaceId, entityId, mappings);
    }

    //public QueryDefinition Exists,
    public SubQueryDefinition GroupJoin(string namespaceId, string entityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(eJoinType.GroupJoin, namespaceId, entityId, mappings);
    }

    public static string GetSubTableKey(eJoinType joinType, string subNamespaceId, string subEntityId)
    {
        var key = subNamespaceId + "." + subEntityId;
        key += "_" + (int)joinType;
        return key;
    }

    public SubQueryDefinition GetSubQueryJoin(string namespaceId, string entityId, eJoinType eJoinType)
    {
        if (subQuerys == null) return null;
        var key = GetSubTableKey(eJoinType, namespaceId, entityId) ?? "";
        SubQueryDefinition result;
        if (subQuerys.TryGetValue(key, out result))
            return result;
        foreach (var sub in subQuerys.Values)
        {
            var q = sub.Query.GetSubQueryJoin(namespaceId, entityId, eJoinType);
            if (q != null)
                return q;
        }
        return null;
    }

    private SubQueryDefinition addOrGetSubTable(eJoinType joinType, Entity joinEntity,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        return addOrGetSubTable(joinType, joinEntity.model.Id, joinEntity.Id, mappings);
    }

    private SubQueryDefinition addOrGetSubTable(eJoinType joinType, string subNamespaceId, string subEntityId,
        params JoinDefinition.JoinFieldMapping[] mappings)
    {
        if (subNamespaceId.StartsWith("Shared") && subEntityId.StartsWith("EntityStateName")) return null;
        var key = GetSubTableKey(joinType, subNamespaceId, subEntityId);
        SubQueryDefinition std;
        subQuerys ??= [];
        subQuerys.TryGetValue(key, out std);
        if (std == null)
        {
            std = new SubQueryDefinition
            {
                joinType = joinType,
                Query = new QueryUtility(ProjectDefinition.Project.GetEntity(subNamespaceId, subEntityId),
                        subNamespaceId + "." + subEntityId, "JoinQuery.1")
                { Parent = this },
            };
            subQuerys.Add(key, std);
        }
        if (mappings != null)
        {
            foreach (var mapping in mappings)
            {
                std.fieldMappings.Add(mapping);
            }
        }
        return std;
    }
}
