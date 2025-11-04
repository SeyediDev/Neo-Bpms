namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    public string GetIdsData(ElasticObject r)
    {
        var ids = "";
        foreach (var item in Entity.entityFields.Values)
        {
            if (!item.IncludeInPkv) continue;
            if (!string.IsNullOrEmpty(ids)) ids += ",";
            object v;
            if (r.GetField(item.Id, out v))
                ids += v.ToString();
        }
        return ids;
    }
    public string GetBasicFieldsData(ElasticObject record, string culture)
    {
        var ids = "";
        if (Entity.DisplayStrings == null) return ids;
        foreach (var item in Entity.DisplayStrings)
        {
            if (!item.CheckCulture(culture))
                continue;
            if (!string.IsNullOrEmpty(item.OtherFieldThatIgnoreMe))
            {
                if (record.GetField(item.OtherFieldThatIgnoreMe, out var obj1) && obj1 != null)
                    continue;
            }
            if (!string.IsNullOrEmpty(ids)) ids += "-";
            if (record.GetField(item.FieldId, out var v))
                ids += v.ToString();
        }
        return ids;
    }
}
