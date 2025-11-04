namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

public interface IDataFiltering<T>
{
    List<ExpressionNode> RunTimeFilters { get; set; }
    LocalParameters FilterValues { get; set; }
    bool IsValid(T record);
}