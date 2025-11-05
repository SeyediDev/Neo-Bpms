namespace Neo.Bpms.Domain.Models.Cmmn;

public class ViewSetting
{
    public string Query { get; set; }

    public bool IsDbQuery { get; set; }

    /*public string entityName;
    public bool bDistinct;
    public Dictionary<string, ColumnDefinition> fields = new Dictionary<string, ColumnDefinition>();
    public List<FilterDefinition> filters;
    public List<FilterDefinition> havingFilters;
    public List<OrderByDefinition> orderbys;
    public List<AggregateDefinition> groupbys;
    public List<FormulaDefinition> formulaInfos;
    public Dictionary<string, SubJoin> subs;

    public class SubJoin : JoinDefinition
    {
        public ViewSetting viewSetting;
    }*/
}