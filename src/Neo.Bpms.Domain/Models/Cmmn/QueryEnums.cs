namespace Neo.Bpms.Domain.Models.Cmmn;

public enum SortType
{
    None,
    Ascending,
    Descending
}
public enum eAggregationFunctions
{
    Sum = 1,
    Avg = 2,
    Min,
    Max,
    First,
    Last,

    Count,
    StDev,
    StDevP,
    Var,
    VarP,

    CHECKSUM_AGG, //only in T-SQL//Returns the checksum of the values in a group. Null values are ignored

    //COUNT_BIG,//only in T-SQL return big int
    GROUPING,

    AggregationFormula,

    GroupByItem,
    InColumn,
}

public enum eAggregateScope
{
    All,
    Distinct
}
