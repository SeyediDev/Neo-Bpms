namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

public class DataTableBuilder<T> : IDataTableBuilder<T>
{
    private readonly DataTable<T> _dataTable = new();

    public void SetList(List<T> items)
    {
        _dataTable.Items = items;
    }

    public void SetOrderBy(List<OrderByDefinition> orderBys)
    {
        _dataTable.OrderBys = orderBys;
    }

    public void SetPagination(int startIndex, int topRows)
    {
        _dataTable.StartIndex = startIndex;
        _dataTable.TopRows = topRows;
    }

    public DataTable<T> GetDataTable()
    {
        return _dataTable;
    }
}
