using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

public class DataFilteringCreator<T>(IDataFilteringBuilder<T> builder)
{
    public void CreateDataFiltering(IEnumerable<FilterDefinition> filters, ExpressionNode objectFilter, LocalParameters filterValues)
    {
        builder.SetRunTimeFilters(filters, objectFilter);
        builder.SetFilterValues(filterValues);
    }

    public DataFiltering<T> GetDataFiltering()
    {
        return builder.GetDataFiltering();
    }
}
