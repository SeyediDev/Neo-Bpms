namespace Neo.Bpms.Domain.Enums;

public enum DataEngineException : int
{
    None,
    DefaultOperationFailed,
    DefaultQueryFailed,
    DefaultConnectionFailed,
    CanNotReadAggregatedQueryWithInMemoryFilter,
    Conformance,
    UniqueIndex,
}
