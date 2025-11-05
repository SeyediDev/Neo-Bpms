using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

public interface IDataFilteringBuilder<T>
{
    void SetRunTimeFilters(IEnumerable<FilterDefinition> filters, ExpressionNode objectFilter);
    void SetFilterValues(LocalParameters filterValues);
    DataFiltering<T> GetDataFiltering();
}
