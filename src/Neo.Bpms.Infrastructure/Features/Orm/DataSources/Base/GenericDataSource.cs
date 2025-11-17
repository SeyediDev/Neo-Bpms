using Neo.Bpms.Domain.Expressions;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataFilterings;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.DataTables;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Base;

public abstract class DataSource<T>(
    DataSourceDefinition definition, Connection connection, AuditTrail auditTrail) 
    : DataSource(definition, connection, auditTrail)
{
    protected IDataTable<T> DataTable { get; set; }
    protected IDataFiltering<T> DataFiltering { get; set; }

    protected void DataTableCreator(IEnumerable<T> items)
    {
        var dataFilteringCreator = DataFilteringBuilderCreator();
        dataFilteringCreator.CreateDataFiltering(Filters, objectFilter, FilterValues);
        DataFiltering = dataFilteringCreator.GetDataFiltering();
        if (objectFilter != null)
            objectFilter = null;

        var dataTableCreator = new DataTableCreator<T>(new DataTableBuilder<T>());
        dataTableCreator.CreateDataTable(items, DataFiltering, OrderBys, StartIndex, TopRows);
        DataTable = dataTableCreator.GetDataTable();
        DataTable.OrderResult();
        DataTable.SetPaging();
    }

    protected virtual DataFilteringCreator<T> DataFilteringBuilderCreator()
    {
        return new DataFilteringCreator<T>(new DataFilteringBuilder<T>());
    }

    public override bool read(out ElasticObject obj)
    {
        obj = null;
        if (!ReadRecord())
            return false;
        if (OnlyRecordCount)
        {
            obj = new ElasticObject { ["recordCount"] = RecordsAffected };
            return true;
        }

        obj = new ElasticObject();
        obj.MergeObject(DataTable.CurrentRecord);
        return true;
    }

    public override bool read<TOut>(out TOut obj)
    {
        var type = typeof(TOut);
        var ctr = type.GetConstructor([]);
        obj = default;
        if (!ReadRecord())
            return false;
        obj = (TOut)ctr?.Invoke([]);
        if (OnlyRecordCount)
        {
            SetField(obj, "recordCount", RecordsAffected);
            return true;
        }
        if (DataTable.CurrentRecord is TOut record)
        {
            obj = record;
            return true;
        }
        ConvertType.Copy(obj, DataTable.CurrentRecord);
        return true;
    }

    protected override object GetValue(string key)
    {
        return DataTable.CurrentRecord == null ? null : 
            ReflectionField.GetValue(DataTable.CurrentRecord, key, out var fv) ? fv : null;
    }

    private bool ReadRecord()
    {
        return DataTable.GetRecord();
    }
}
