using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

public interface IDataTable<T>
{
    List<T> Items { get; set; }
    List<OrderByDefinition> OrderBys { get; set; }
    int StartIndex { get; set; }
    int TopRows { get; set; }
    void OrderResult();
    void SetPaging();
    T CurrentRecord { get; }
    bool GetRecord();
}
