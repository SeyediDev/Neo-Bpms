namespace Neo.Bpms.Domain.Entities.Cmmn;

public enum SortType
{
    None,
    Ascending,
    Descending
}
public enum eAggregationFunctions
{
    GroupByItem,

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

    InColumn,
    Formula, //???

    //odprmagg_ReCalcFormula = 7,
    //odprmagg_CumulativeSum = 8,
    //odprmagg_Trend = 9,
    //odprmagg_Mode = 10,
    //odprmagg_ModeCount = 11,
}

public enum eAggregateScope
{
    All,
    Distinct
}