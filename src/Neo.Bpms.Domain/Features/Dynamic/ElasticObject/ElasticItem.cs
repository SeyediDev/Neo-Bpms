using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;

namespace Neo.Bpms.Domain.Features.Dynamic;

public partial class ElasticObject
{
    public object this[string field]
    {
        get => GetObject(field);
        set => SetField(field, value);
    }

    public long Id => GetLong("Id");

    public bool GetField(string field, out object retVal)
    {
        if (field == null)
            throw new ArgumentNullException(nameof(field));
        if (_elasticProvider.HasAttribute(field))
        {
            retVal = _elasticProvider.Attribute(field).InternalValue;
            return true;
        }
        List<ElasticObject> obj = _elasticProvider.ElementList(field);
        if (obj != null)
        {
            retVal = obj;
            return true;
        }

        object internalValue = InternalValue;
        if (internalValue != null)
        {
            Type objType = internalValue.GetType();
            FieldInfo fi = objType.GetField(field);
            if (fi != null)
            {
                retVal = fi.GetValue(internalValue);
                return true;
            }
            else
            {
                PropertyInfo pi = objType.GetProperty(field);
                if (pi != null)
                {
                    retVal = pi.GetValue(internalValue);
                    return true;
                }
            }
        }
        retVal = null;
        return false;
    }

    public void SetNullFieldIsNotExists(string fieldId)
    {
        if (!HasAttribute(fieldId))
            SetField(fieldId, null);
    }
    public bool SetField(string field, object value)
    {
        AddAttribute(field, value);
        return true;
    }
    public void SetFieldIfNotExists(string field, object value)
    {
        if (!HasAttribute(field))
            AddAttribute(field, value);
    }
    public T GetEnum<T>(string fieldName, T defaultValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
        if (!GetField(fieldName, out object val) || val == null)
            return defaultValue;
        bool wasLong = long.TryParse(val.ToString(), out long lValue);
        foreach (object item in Enum.GetValues(typeof(T)))
        {
            if (wasLong)
            {
                if ((int)item == lValue)
                    return (T)item;
            }
            else
            {
                if (item.ToString() == val.ToString())
                    return (T)item;
            }
        }
        return defaultValue;
    }

    public T GetEnumText<T>(string fieldName, T defaultValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
        if (!GetField(fieldName, out object val)) return defaultValue;
        return !Enum.TryParse(val?.ToString(), out T @enum) ? GetEnum(fieldName, defaultValue) : @enum;
    }

    public int GetInteger(string fieldName, int defaultValue = 0)
    {
        return (int)GetLong(fieldName, defaultValue);
    }

    public long GetLong(string fieldName, long defaultValue = 0)
    {
        if (!GetField(fieldName, out object obj) || obj == null)
            return defaultValue;
        return obj is int ? (int)obj : obj is long ? (long)obj : !long.TryParse(obj.ToString(), out long i) ? defaultValue : i;
    }
    public long? GetNullableLong(string fieldName)
    {
        if (!GetField(fieldName, out object obj) || obj == null)
            return null;
        return obj is int ? (int)obj : obj is long ? (long)obj : !long.TryParse(obj.ToString(), out long i) ? null : i;
    }
    public bool GetBool(string fieldName, bool defaultValue = false)
    {
        if (!GetField(fieldName, out object obj) || obj == null)
            return defaultValue;
        if (obj is bool)
            return (bool)obj;
        if (obj is long)
            return (long)obj != 0;
        if (obj is int)
            return (int)obj != 0;
        if (obj.ToString() == "1") return true;
        if (obj.ToString() == "2") return false;//todo
        return !bool.TryParse(obj.ToString(), out bool b) ? defaultValue : b;
    }
    public double GetDouble(string fieldName)
    {
        return !GetField(fieldName, out object obj) || obj == null
            ? 0
            : obj is double || obj is float
            ? (double)obj
            : !double.TryParse(obj.ToString().Replace("/", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ? 0 : d;
    }
    public string GetString(string fieldName)
    {
        return !GetField(fieldName, out object obj) || obj == null ? null : obj is string ? obj as string : obj.ToString();
    }
    public DateTime GetDateTime(string fieldName)
    {
        if (!GetField(fieldName, out object obj) || obj == null)
            return DateTime.MinValue;
        if (obj is DateTime)
            return (DateTime)obj;
        return DateTime.TryParse(obj.ToString(), out DateTime dateTime) ? dateTime : DateTime.MinValue;
    }
    public DateTime? GetNullableDateTime(string fieldName)
    {
        return !GetField(fieldName, out object obj) || obj == null
            ? null
            : obj is DateTime time ? time : DateTime.TryParse(obj.ToString(), out DateTime dateTime) ? (DateTime?)dateTime : null;
    }
    public TimeSpan GetTimeSpan(string fieldName)
    {
        if (!GetField(fieldName, out object obj) || obj == null)
            return TimeSpan.MinValue;
        if (obj is TimeSpan)
            return (TimeSpan)obj;
        if (obj is long || obj is double || obj is int)
            return TimeSpan.FromTicks((long)obj);
        return TimeSpan.TryParse(obj.ToString(), out TimeSpan timeSpan) ? timeSpan : TimeSpan.MinValue;
    }

    public ElasticObject GetElastic(string fieldName)
    {
        return !GetField(fieldName, out object obj) || obj == null ? null : obj as ElasticObject;
    }

    public object GetObject(string fieldName)
    {
        GetField(fieldName, out object obj);
        return obj;
    }

    public int? GetIntDynamic(string fieldName)
    {
        return !GetField(fieldName, out dynamic obj) || obj == null ? null : (int?)obj;
    }
    public bool? GetBoolDynamic(string fieldName)
    {
        return !GetField(fieldName, out dynamic obj) || obj == null ? null : (bool?)obj;
    }
    public double? GetDoubleDynamic(string fieldName)
    {
        return !GetField(fieldName, out dynamic obj) || obj == null ? null : (double?)obj;
    }
    public string GetStringDynamic(string fieldName)
    {
        return !GetField(fieldName, out dynamic obj) || obj == null ? null : (string)obj;
    }
    public ElasticObject GetElasticDynamic(string fieldName)
    {
        return !GetField(fieldName, out dynamic obj) || obj == null ? null : (ElasticObject)obj;
    }
    public ElasticObject Merge(ElasticObject values)
    {
        IEnumerable<KeyValuePair<string, ElasticObject>> attributes = values?.Attributes?.ToList() ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>();
        foreach (KeyValuePair<string, ElasticObject> value in attributes)
        {
            if (value.Key == null) continue;
            SetField(value.Key, value.Value);
        }
        return this;
    }
    public ElasticObject MergeValues(ElasticObject values)
    {
        IEnumerable<KeyValuePair<string, ElasticObject>> attributes = values?.Attributes?.ToList() ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>();
        foreach (KeyValuePair<string, ElasticObject> attribute in attributes)
        {
            if (attribute.Key == null) continue;
            SetField(attribute.Key, attribute.Value?.InternalValue);
        }
        return this;
    }
    public ElasticObject MergeNotExists(ElasticObject values)
    {
        IEnumerable<KeyValuePair<string, ElasticObject>> attributes = values?.Attributes?.ToList() ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>();
        foreach (KeyValuePair<string, ElasticObject> value in attributes)
        {
            if (value.Key == null) continue;
            SetFieldIfNotExists(value.Key, value.Value?.InternalValue);
        }
        return this;
    }
    public ElasticObject Merge(LocalParameters values)
    {
        if (values == null) return this;
        foreach (var value in values)
        {
            if (value.Key == null) continue;
            SetField(value.Key, value.Value);
        }
        return this;
    }
    public ElasticObject DeepMerge(LocalParameters values)
    {
        if (values == null) return this;
        foreach (var value in values)
        {
            if (value.Key == null) continue;
            if (value.Value is IEnumerable<LocalParameters> listValues)
            {
                List<ElasticObject> val = [.. listValues.Select(listValue => listValue.DeepToElastic())];
                SetField(value.Key, val);
                continue;
            }
            SetField(value.Key, value.Value);
        }
        return this;
    }
    public LocalParameters ToLocalParameters()
    {
        var lp = new LocalParameters();
        if (Attributes == null) return lp;
        foreach (KeyValuePair<string, ElasticObject> item in Attributes)
            lp.AddOrUpdate(item.Key, item.Value);
        return lp;
    }
    public LocalParameters DeepToLocalParameters()
    {
        var lp = new LocalParameters();
        if (Attributes == null) return lp;
        foreach (KeyValuePair<string, ElasticObject> item in Attributes)
        {
            if (item.Value.ObjectType == typeof(List<ElasticObject>))
            {
                List<ElasticObject> list = (List<ElasticObject>)item.Value.InternalValue;
                List<LocalParameters> listLp = [];
                foreach (ElasticObject l in list)
                {
                    var listItem = new LocalParameters();
                    if (l.Attributes != null)
                    {
                        foreach (KeyValuePair<string, ElasticObject> listItemAttribute in l.Attributes)
                        {
                            listItem.AddOrUpdate(listItemAttribute.Key, listItemAttribute.Value.ToString());
                        }
                    }
                    listLp.Add(listItem);
                }
                lp.AddOrUpdate(item.Key, listLp);
            }
            else
                lp.AddOrUpdate(item.Key, item.Value);
        }
        return lp;
    }

    public void AddIfNot(ElasticObject paramValues)
    {
        if (paramValues == null) return;
        IEnumerable<KeyValuePair<string, ElasticObject>> attributes = paramValues.Attributes?.ToList() ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>();
        foreach (KeyValuePair<string, ElasticObject> item in attributes)
            AddIfNot(item.Key, item.Value);
    }
    public void AddIfNot(string key, object value)
    {
        if (!GetField(key, out object v))
            SetField(key, value);
    }

    public ElasticObject Clone(bool preserveNulls = true)
    {
        ElasticObject ret = new("", InternalValue);
        IEnumerable<KeyValuePair<string, ElasticObject>> attributes = Attributes?.ToList() ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>();
        foreach (KeyValuePair<string, ElasticObject> item in attributes)
        {
            if (!preserveNulls)
            {
                ElasticObject elValue = item.Value;
                if (elValue == null || (!elValue.Attributes.Any() && string.IsNullOrEmpty(elValue.InternalValue?.ToString())))
                    continue;
            }
            ret.SetField(item.Key, item.Value?.InternalValue);
        }

        //foreach (var item in ElementCollections?.Values ?? Enumerable.Empty<List<ElasticObject>>())
        //{
        //	if (!preserveNulls)
        //	{
        //		var elValue = item.Value;
        //		if (elValue == null || (!elValue.Attributes.Any() && string.IsNullOrEmpty(elValue.InternalValue?.ToString())))
        //			continue;
        //	}
        //	ret.AddElement(item);
        //}
        return ret;
    }
}
