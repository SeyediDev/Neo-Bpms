using System.Data.SqlTypes;
using Neo.Bpms.Domain.Expressions;

namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    #region read

    public override SqlXml GetSqlXml(int i)
    {
        return null;
    }


    public override bool read(out object[] values)
    {
        values = null;
        if (DataReader == null) return false;
        if (ReadFromDataReader())
        {
            values = new object[Outfields.Count];
            DataReader.GetValues(values);
            return true;
        }

        return false;
    }

    public override bool read(out ElasticObject obj)
    {
        obj = null;
        if (DataReader == null) return false;
        if (!ReadFromDataReader())
            return false;
        obj = new ElasticObject("sjvs");
        var values = ExtractValues();
        ReadFieldInfos(this, obj, values);
        return true;
    }

    public override bool read<T>(out T obj)
    {
        obj = default;
        if (DataReader == null) return false;
        if (!ReadFromDataReader())
            return false;
        var type = typeof(T);
        var ctr = type.GetConstructor(new Type[0]);
        obj = (T)ctr?.Invoke(new object[0]);
        var values = ExtractValues();
        ReadFieldInfos(obj, type, values);
        return true;
    }

    protected virtual bool ReadFromDataReader()
    {
        try
        {
            if ( /*Exceptions == null && connection.Exceptions == null &&*/
                DataReader != null && DataReader.Read())
                return true;
        }
        catch (Exception e)
        {
            AddException(logger, "1205", new DataLayerException(e));
        }

        return false;
    }

    protected override object GetValue(string key)
    {
        return DataReader.GetValue(DataReader.GetOrdinal(key));
    }

    private Dictionary<string, int> _valueNames;

    private object[] ExtractValues()
    {
        var values = new object[DataReader.FieldCount];
        DataReader.GetValues(values);
        if (_valueNames == null)
        {
            _valueNames = [];
            for (var i = 0; i < DataReader.FieldCount; i++)
            {
                var name = DataReader.GetName(i);
                if (!_valueNames.ContainsKey(name))
                    _valueNames.Add(DataReader.GetName(i), i);
            }
        }

        return values;
    }

    private void ReadFieldInfos(DataSource dataSource, ElasticObject obj,
        object[] values)
    {
        if (dataSource.OnlyRecordCount)
        {
            var value = DataReader.GetValue(0);
            if (value is not DBNull)
                obj.SetField("recordCount", value);
            return;
        }

        foreach (var item in dataSource.Outfields)
        {
            var valueIndex = _valueNames.GetItem(item.Key);
            var value = values[valueIndex];
            if (value is not DBNull && value as string != "NULL")
                obj.SetField(item.Key, value);
        }

        foreach (var item in dataSource.SubTables?.Values.Where(d => d.isJoin()) ?? [])
        {
            ReadFieldInfos(item.dataSource, obj, values);
        }
    }

    private void ReadFieldInfos<T>(T obj, Type type, object[] values)
        where T : new()
    {
        if (OnlyRecordCount)
        {
            var fi = type.GetField("recordCount");
            if (fi == null) return;
            var value = DataReader.GetValue(0);
            if (value is not DBNull)
                fi.SetValue(obj, value);
            return;
        }

        var parents = new Dictionary<string, object>();
        try
        {
            SetDataToObject(obj, type, values, parents);
        }
#pragma warning disable 168
        catch (Exception e)
#pragma warning restore 168
        {
            // ignored
        }

        try
        {
            SetParentsPkv(obj, type, values, parents);
        }
#pragma warning disable 168
        catch (Exception e)
#pragma warning restore 168
        {
            // ignored
        }
    }

    private void SetParentsPkv<T>(T obj, Type type, object[] values, Dictionary<string, object> parents)
        where T : new()
    {
        foreach (var parentField in DataSrcDefinition.Entity?.ParentEntities ?? [])
        {
            if (!parents.TryGetValue(parentField.Id, out var parentObject))
            {
                SetDataItemToObject(obj, type, values, parents, parentField.Id, null);
                if (!parents.TryGetValue(parentField.Id, out parentObject))
                    continue;
            }

            SetParentPkv(obj, type, parentField, parentObject);
        }
    }

    private void SetParentPkv<T>(T obj, Type type, EntityField parentField, object parentObject)
    {
        foreach (var map in parentField.AssociationEntity.Maps)
        {
            var existSource = ReflectionField.GetValue(obj, map.SourceField, out var sourceValue);
            var existDest = ReflectionField.GetValue(parentObject, map.DestField, out var destValue);
            if (existSource && existDest)
                continue;
            if (existDest)
                ReflectionField.SetValue(obj, ReflectionField.FetchMember(type, map.SourceField), destValue);
            else if (parentObject != null)
                ReflectionField.SetValue(parentObject,
                    ReflectionField.FetchMember(parentObject.GetType(), map.DestField), sourceValue);
        }
    }

    private void SetDataToObject<T>(T obj, Type type, object[] values, Dictionary<string, object> associations)
        where T : new()
    {
        foreach (var item in Outfields)
        {
            var valueIndex = _valueNames.GetItem(item.Key);
            var value = values[valueIndex];
            if (value is DBNull) continue;
            SetDataItemToObject(obj, type, values, associations, item.Key, value);
        }
    }

    private void SetDataItemToObject<T>(T obj, Type type, object[] values, Dictionary<string, object> associations,
        string item, object value)
        where T : new()
    {
        var keyIds = item.Split('.');
        var key = keyIds[0];
        var member = ReflectionField.FetchMember(type, key);
        if (member == null)
            return;
        if (ReflectionField.IsClass(member))
        {
            var memberType = ReflectionField.FetchMemberType(member);
            if (!associations.TryGetValue(key, out var subObj))
            {
                subObj = ReflectionField.CreateObject(memberType);
                associations.Add(key, subObj);
                SetDataToObject(subObj, memberType, values, associations);
                ConvertType.SetValue(obj, key, subObj);
            }

            SetDataItemToObject(subObj, memberType, values, associations, item, value);
        }
        else
            ConvertType.SetValue(obj, key, value);
    }

    #endregion

    #region find

    public override bool find<T>(ExpressionNode f, out T newRecord)
    //where T: new()
    {
        if (f != null)
            AddFilter(f, FilterValues, "");
        if (openForRead())
            return read(out newRecord);
        newRecord = default;
        return false;
    }

    public override ElasticObject find(ExpressionNode f)
    {
        if (f != null)
            AddFilter(f, FilterValues, "");
        if (!openForRead()) return null;
        return read(out ElasticObject elasticObject) ? elasticObject : null;
    }

    #endregion
}
