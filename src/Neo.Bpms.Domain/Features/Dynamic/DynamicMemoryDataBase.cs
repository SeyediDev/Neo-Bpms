using System.Dynamic;
//using System.Linq;

namespace Neo.Bpms.Domain.Features.Dynamic;

public class DynamicMemoryDataBase : DynamicObject
{
    public Dictionary<string, object> RecordSets = [];

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        return AddRecordSet(binder.Name, value);
    }
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        bool b = RecordSets.TryGetValue(binder.Name, out result);
        return b;
    }
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
        result = null;
        if (indexes.Length < 1) return false;
        object collection = null;
        if (indexes[0] is string)
        {
            RecordSets.TryGetValue((string)indexes[0], out result);
        }
        else if (indexes[0] is Func<string, bool>)
        {
            Func<string, bool> filter = indexes[0] as Func<string, bool>;
            foreach (KeyValuePair<string, object> item in RecordSets)
            {
                if (filter(item.Key))
                {
                    collection = item.Value;
                    break;
                }
            }
        }
        else if (indexes[0] is Func<object, bool>)
        {
            Func<object, bool> filter = indexes[0] as Func<object, bool>;
            foreach (object item in RecordSets.Values)
            {
                if (filter(item))
                {
                    collection = item;
                    break;
                }
            }
        }
        else if (indexes[0] is Func<string>)
        {
            Func<string> filter = indexes[0] as Func<string>;
            string s = filter();
            RecordSets.TryGetValue(s, out collection);
        }
        if (collection == null)
            return false;
        if (indexes.Length > 1)
        {
            ElasticObject rs = GetRecord(collection, indexes[1]);
            if (rs == null) return false;
            if (indexes.Length > 2)

                result = GetFieldValue(rs, indexes[2].ToString());
            else
                result = rs;
        }
        else
            result = collection;
        return true;
    }

    internal bool AddRecordSet(string s, object collection)
    {
        bool b = RecordSets.ContainsKey(s);
        //rs.Type = collection.GetType();
        if (!b)
            RecordSets.Add(s, collection);
        else
            RecordSets[s] = collection;
        return b;
    }
    public IEnumerable<ElasticObject> GetDataSetAsEnumerable(string domain)
    {
        bool b = RecordSets.TryGetValue(domain, out object o);
        return !b ? null : GetDataSetAsEnumerable(o);
    }

    private IEnumerable<ElasticObject> GetDataSetAsEnumerable(object o)
    {
        SortedList<object, ElasticObject> rs = o as SortedList<object, ElasticObject>;
        if (rs != null)
            return rs.Values;
        Dictionary<object, ElasticObject> rs1 = o as Dictionary<object, ElasticObject>;
        if (rs1 != null)
            return rs1.Values;
        SortedDictionary<object, ElasticObject> rs2 = o as SortedDictionary<object, ElasticObject>;
        if (rs2 != null)
            return rs2.Values;
        IEnumerable<ElasticObject> rs3 = o as IEnumerable<ElasticObject>;
        return rs3 != null ? rs3 : null;
    }
    public object GetFieldValue(ElasticObject record, string fieldName)
    {
        if (!record.GetField(fieldName, out object result)) result = null;
        return result;
    }
    public bool SetFieldValue(ElasticObject record, string fieldName, object value)
    {
        return record.SetField(fieldName, value);
    }
    /*public bool ChangeState(ElasticObject record, string fieldName, EFieldState newState)
		{
			return record.ChangeState(fieldName, newState);
		}*/
    public bool AddRecord(string entityName, object key, ElasticObject r)
    {
        if (!RecordSets.ContainsKey(entityName)) return false;
        object o = RecordSets[entityName];
        SortedList<object, ElasticObject> rs = o as SortedList<object, ElasticObject>;
        if (rs != null)
            rs.Add(key, r);
        else
        {
            Dictionary<object, ElasticObject> rs1 = o as Dictionary<object, ElasticObject>;
            if (rs1 != null)
                rs1.Add(key, r);
            else
            {
                SortedDictionary<object, ElasticObject> rs2 = o as SortedDictionary<object, ElasticObject>;
                if (rs2 != null)
                    rs2.Add(key, r);
                else
                    return false;
            }
        }
        return true;
    }
    public bool AddRecord(string entityName, ElasticObject r)
    {
        if (!RecordSets.ContainsKey(entityName)) return false;
        object o = RecordSets[entityName];
        List<ElasticObject> rs = o as List<ElasticObject>;
        if (rs != null)
            rs.Add(r);
        else
        {
            LinkedList<ElasticObject> rs1 = o as LinkedList<ElasticObject>;
            if (rs1 != null)
                rs1.AddLast(r);
            else
            {
                HashSet<ElasticObject> rs2 = o as HashSet<ElasticObject>;
                if (rs2 != null)
                    rs2.Add(r);
                else
                {
                    SortedSet<ElasticObject> rs3 = o as SortedSet<ElasticObject>;
                    if (rs3 != null)
                        rs3.Add(r);
                    else
                    {
                        Queue<ElasticObject> rs4 = o as Queue<ElasticObject>;
                        if (rs4 != null)
                            rs4.Enqueue(r);
                        else
                        {
                            Stack<ElasticObject> rs5 = o as Stack<ElasticObject>;
                            if (rs5 != null)
                                rs5.Push(r);
                            else
                                return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //public Record CreateRecord(string entityName, Type[] paramTypes, object[] paramValues)
    //{
    //	if (!RecordSets.ContainsKey(entityName)) return false;

    //	var r = new Record();
    //	r.CallConstructor(paramTypes, paramValues);
    //	foreach (var dynamicFieldInfo in entity.FieldInfos)
    //	{
    //		r.Id = dynamicFieldInfo.Value.Id;
    //		if (dynamicFieldInfo.Value.HasDefaultValue)
    //		{
    //			r.SetField(dynamicFieldInfo.Key, dynamicFieldInfo.Value.DefaultValue);
    //		}
    //	}

    //	return r;
    //}
    public ElasticObject GetRecord(string entityName, object key)
    {
        object o = RecordSets[entityName];
        return GetRecord(o, key);
    }
    private ElasticObject GetRecord(object collection, object key)
    {
        ElasticObject retVal = null;
        if (collection == null) return null;
        Dictionary<object, ElasticObject> rs = collection as Dictionary<object, ElasticObject>;
        if (rs != null)
            rs.TryGetValue(key, out retVal);
        else
        {
            SortedList<object, ElasticObject> rs1 = collection as SortedList<object, ElasticObject>;
            if (rs1 != null)
                rs1.TryGetValue(key, out retVal);
            else
            {

                SortedDictionary<object, ElasticObject> rs2 = collection as SortedDictionary<object, ElasticObject>;
                if (rs2 != null)
                    rs2.TryGetValue(key, out retVal);

            }
        }
        return retVal;

    }
}
/*
		public class DynamicDataBase
		{
			public Dictionary<string, ExpandoObject> RecordSets = new Dictionary<string, ExpandoObject>();
		}
	 */
