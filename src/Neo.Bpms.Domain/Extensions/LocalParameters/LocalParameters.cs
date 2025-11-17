using System.Text;

namespace Neo.Bpms.Domain.Utility;

public partial class LocalParameters : ConcurrentDictionary<string, object>
{
    public LocalParameters()
    {
    }
    public LocalParameters(object user)
    {
        if (user != null)
            AddOrUpdate("user", user);
    }

    public LocalParameters(IEnumerable<KeyValuePair<string, object>> source)
    {
        AddOrUpdate(source);
    }

    public long Id
    {
        get => GetLong("Id") ?? 0;
        set => this["Id"] = value;
    }
    public object AuditUser
    {
        get => GetLong("user") ?? 0;
        set => this["user"] = value;
    }


    public LocalParameters AddOrUpdate(IEnumerable<KeyValuePair<string, object>> source)
    {
        if (source == null) return this;
        foreach (KeyValuePair<string, object> item in source)
            AddOrUpdate(item.Key, item.Value);
        return this;
    }

    public LocalParameters AddOrUpdate<T>(IEnumerable<KeyValuePair<string, T>> source)
    {
        if (source == null) return this;
        foreach (KeyValuePair<string, T> item in source)
            AddOrUpdate(item.Key, item.Value);
        return this;
    }

    public LocalParameters AddOrUpdate(string key, object value)
    {
        if (!ContainsKey(key))
            TryAdd(key, value);
        else
            base[key] = value;
        return this;
    }
    public LocalParameters AddIfNotExistsAndNotNull(string key, object value)
    {
        switch (value)
        {
            case string _ when string.IsNullOrEmpty(value.ToString()):
            case long l when l == 0:
                value = null;
                break;
        }
        if (value != null)
            AddIfNotExists(key, value);
        return this;
    }
    public LocalParameters AddIfNotExists(string key, object value)
    {
        if (!ContainsKey(key))
            TryAdd(key, value);
        return this;
    }
    public LocalParameters Add(string key, object value)
    {
        return AddOrUpdate(key, value);
    }

    public object Get(string key)
    {
        TryGetValue(key, out object value);
        return value;
    }

    public string GetString(string key)
    {
        return Get(key)?.ToString();
    }
    public long? GetLong(string key)
    {
        object v = Get(key);
        return string.IsNullOrEmpty(v?.ToString()) ? null : long.TryParse(v.ToString(), out long l) ? l : null;
    }
    public double? GetDouble(string key)
    {
        object v = Get(key);
        return string.IsNullOrEmpty(v?.ToString()) ? null : Convert.ToDouble(v);
    }

    public LocalParameters Set(ConcurrentDictionary<string, object> dictionary)
    {
        if (dictionary == null) return this;
        foreach (KeyValuePair<string, object> item in dictionary)
            AddOrUpdate(item.Key, item.Value);
        return this;
    }
    public LocalParameters Set(Dictionary<string, object> dictionary)
    {
        if (dictionary == null) return this;
        foreach (KeyValuePair<string, object> item in dictionary)
            AddOrUpdate(item.Key, item.Value);
        return this;
    }

    public LocalParameters Set(LocalParameters lp)
    {
        if (lp == null) return this;
        foreach (KeyValuePair<string, object> item in lp)
            AddOrUpdate(item.Key, item.Value);
        return this;
    }

    public override string ToString()
    {
        StringBuilder s = new();
        s.Append("{");
        bool first = true;
        foreach (KeyValuePair<string, object> item in this)
        {
            if (!first)
                s.Append(",");
            s.Append("\"" + item.Key + "\":");
            Dictionary<string, object> valueAsDic = item.Value as Dictionary<string, object>;
            List<object> valueAsList = item.Value as List<object>;
            if (valueAsDic != null)
            {
                s.Append("{");
                bool firstItem = true;
                foreach (KeyValuePair<string, object> itemValue in valueAsDic)
                {
                    if (!firstItem)
                        s.Append(",");
                    s.Append("\"" + itemValue.Key + "\":");
                    s.Append("\"" + itemValue.Value + "\"");
                    firstItem = false;
                }
                s.Append("}");
            }
            else if (valueAsList != null)
            {
                s.Append("[");
                bool firstItem = true;
                foreach (object itemValue in valueAsList)
                {
                    if (!firstItem)
                        s.Append(",");
                    s.Append(itemValue);
                    firstItem = false;
                }
                s.Append("]");
            }
            else
                s.Append("\"" + item.Value + "\"");
            first = false;
        }
        s.Append("}");
        return s.ToString();
    }

    public bool ContainsValid(string keyId)
    {
        return ContainsKey(keyId) && GetString(keyId) != "undefined";
    }

    public LocalParameters Clone()
    {
        LocalParameters newLc = [];
        newLc.Set(this);
        return newLc;
    }
}
