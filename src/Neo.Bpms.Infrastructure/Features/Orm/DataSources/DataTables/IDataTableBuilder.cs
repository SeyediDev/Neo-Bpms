using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

public interface IDataTableBuilder<T>
{
    void SetList(List<T> items);
    void SetOrderBy(List<OrderByDefinition> orderBys);
    void SetPagination(int startIndex, int topRows);
    DataTable<T> GetDataTable();
}
