using Neo.Bpms.Domain.Models.Cmmn.Data.Base;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

public class DataTableCreator<T>(IDataTableBuilder<T> builder)
{
    public void CreateDataTable(IEnumerable<T> items, IDataFiltering<T> dataFiltering, List<OrderByDefinition> orderBys, int startIndex, int topRows)
    {
        builder.SetList(items?.Where(dataFiltering.IsValid).ToList());
        builder.SetOrderBy(orderBys);
        builder.SetPagination(startIndex, topRows);
    }

    public DataTable<T> GetDataTable()
    {
        return builder.GetDataTable();
    }
}
