using Neo.Bpms.Domain.Models.Base;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public LocalParameters LocalParameters { get; private set; }
    public List<FilterDefinition> filters { get; set; }
    public List<FilterDefinition> havingFilters { get; set; }
    public void SetLocalParameterValue(string key, object value)
    {
        LocalParameters ??= [];
        LocalParameters.AddOrUpdate(key, value);
    }

    public QueryUtility AddFilter(string filter, string orGroupId = null)
    {
        if (filter != null)
            addFilter(Parser.Parse(filter), orGroupId);
        return this;
    }

    public QueryUtility AddFilter(ExpressionNode expressionNode, string orGroupId = null)
    {
        addFilter(expressionNode, orGroupId);
        return this;
    }

    public QueryUtility Where(string filter, string orGroupId = null)
    {
        return AddFilter(filter, orGroupId);
    }

    public QueryUtility Where(ExpressionNode expFilter, string orGroupId = null)
    {
        addFilter(expFilter, orGroupId);
        return this;
    }

    public QueryUtility WhereIds(IEnumerable<string> ids)
    {
        return Where($"{Entity?.KeyFields?.FirstOrDefault()?.Id} In({string.Join(",", ids.Select(id => $"'{id}'"))})");
    }
    public QueryUtility WhereIds(IEnumerable<long> ids)
    {
        return Where($"{Entity?.KeyFields?.FirstOrDefault()?.Id} In({string.Join(",", ids)})");
    }
    public QueryUtility Having(string having, string orGroupId = null)
    {
        addHaving(Parser.Parse(having), orGroupId);
        return this;
    }

    public QueryUtility Having(ExpressionNode expFilter, string orGroupId = null)
    {
        addHaving(expFilter, orGroupId);
        return this;
    }

    public QueryUtility AddPkFilter(string ids)
    {
        var list = new List<string>();
        var keys = Entity.entityFields?.Values.Where(f => f.IncludeInPkv).ToList();
        if (keys?.Count == 1)
            list.Add(keys[0].Id + "=='" + ids + "'");
        else if (keys != null)
        {
            var sids = ids.Split('#');
            var iSids = 0;
            foreach (var item in keys)
            {
                list.Add("(" + item.Id + "=='" + sids[iSids] + "')");
                iSids++;
                if (iSids >= sids.Length)
                    break;
            }
        }
        return Where(string.Join(" And ", list));
    }
    public QueryUtility ActiveStates()
    {
        if (Entity.GetStateCollection()?.States == null) return this;
        var states =
            Entity.GetStateCollection()
                .States?.Values.Where(g => (g.category & (uint)EntityStateCategory.BackupNode) == 0)
                .Select(g => g.Id)
                .ToList();
        if (states?.Count > 0)
            Where("StateId In (" + string.Join(",", states) + ")");
        return this;
    }

    private void addFilter(ExpressionNode filterExpression, string orGroupId)
    {
        filters ??= [];
        filters.Add(new FilterDefinition
        {
            FilterExpression = filterExpression,
            OrGroupId = orGroupId
        });
    }

    private void addHaving(ExpressionNode havingExpression, string orGroupId)
    {
        havingFilters ??= [];
        havingFilters.Add(new FilterDefinition
        {
            FilterExpression = havingExpression,
            OrGroupId = orGroupId
        });
    }
}
