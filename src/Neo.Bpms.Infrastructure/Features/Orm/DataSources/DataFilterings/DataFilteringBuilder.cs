namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

public class DataFilteringBuilder<T> : IDataFilteringBuilder<T>
{
    private readonly DataFiltering<T> _dataFiltering = new();

    public void SetRunTimeFilters(IEnumerable<FilterDefinition> filters, ExpressionNode objectFilter)
    {
        _dataFiltering.RunTimeFilters = filters?.Select(f => f.FilterExpression ?? Parser.ParseTree(f.Filter).Root).ToList();
        if (objectFilter != null)
        {
            _dataFiltering.RunTimeFilters ??= [];
            _dataFiltering.RunTimeFilters.Add(objectFilter);
        }
    }

    public void SetFilterValues(LocalParameters filterValues)
    {
        _dataFiltering.FilterValues = filterValues;
    }

    public DataFiltering<T> GetDataFiltering()
    {
        return _dataFiltering;
    }
}
