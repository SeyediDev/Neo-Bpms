using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    #region constructor
    public QueryUtility(Entity entity, string name)
        : this(entity, "", name)
    {
    }

    public QueryUtility(string namespaceId, string entityId, string name = null)
        : this(ProjectDefinition.Project.GetEntity(namespaceId, entityId),
            entityId, name)
    {
    }

    private QueryUtility(Entity entity, string entityId, string name)
        : base(entity, null, entityId ?? name)
    {
    }

    #endregion

    public bool bDistinct;

    public QueryUtility SetDistinct(bool distinct)
    {
        bDistinct = distinct;
        return this;
    }

    public int startIndex;
    public int topRows;

    public QueryUtility SetPage(int pageNo, int rowCount)
    {
        startIndex = Math.Max((pageNo - 1) * rowCount, 0);
        topRows = rowCount;
        return this;
    }

    public bool InsertFromQuery(DataSource applyDataSource)
    {
        return applyDataSource.InsertFromQuery(DataSource);
    }
}
