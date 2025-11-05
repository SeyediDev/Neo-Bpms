// <summary>
// First Sample of Builder Design Pattern
// </summary>

using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

public class DataTable<T> : IDataTable<T>
{
    public List<T> Items { get; set; }
    public List<OrderByDefinition> OrderBys { get; set; }
    public int StartIndex { get; set; }
    public int TopRows { get; set; }
    public T CurrentRecord { get; private set; }

    public bool GetRecord()
    {
        CurrentRecord = Items.FirstOrDefault();
        if (CurrentRecord == null)
            return false;
        Items.Remove(CurrentRecord);
        return CurrentRecord != null;
    }


    public void OrderResult()
    {
        if (OrderBys?.Count > 0)
            Items = [.. Items.OrderBy(r => r)];
    }

    public void SetPaging()
    {
        if (StartIndex > 0)
            Items.RemoveRange(0, StartIndex);
        if (TopRows > 0 && Items.Count > TopRows)
            Items.RemoveRange(TopRows, Items.Count - TopRows);
    }
}
