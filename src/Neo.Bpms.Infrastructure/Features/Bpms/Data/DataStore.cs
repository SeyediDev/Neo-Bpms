using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    internal static List<ElasticObject> ReadDataStoreList(DataStoreReference dataStoreReference, ProcessInstance pi,
        IList<string> fields)
    {
        if (dataStoreReference?.dataStore?.itemSubjectRef?.structureRef == null)
        {
            return null;
        }

        DataStore dataStore = dataStoreReference.dataStore;
        QueryUtility q = new(dataStore.NamespaceId, dataStore.EntityId);
        if (q.Entity == null)
        {
            return null;
        }

        LocalParameters localParameters = [];
        ElasticObject qParams = new();
        foreach (DataStoreReference.FilterMapping filterMapping in dataStoreReference.FilterMappings ??
                                      Enumerable.Empty<DataStoreReference.FilterMapping>())
        {
            EntityField field = q.Entity.GetField(filterMapping.DataStoreField);
            if (field == null)
            {
                pi.LogError(
                    $"can not find DataStoreField:{filterMapping.DataStoreField} in DataStore:{dataStore.Id} in entity {q.Entity.Id}. process:{pi.Process.Id}, pi:{pi.Id}");
                continue;
            }

            _ = pi.GetData(filterMapping.ProcessProperty, out object value);
            string qFieldId = $"q{field.Id}";
            string filter = GenerateFilter(field, true, qFieldId, value?.ToString() ?? filterMapping.ProcessProperty);
            qParams[qFieldId] = value;
            _ = q.AddFilter(filter);
        }

        ExpressionNode filterExp = dataStoreReference.FilterExpression?.Expression?.Root?.clone();
        if (filterExp != null)
        {
            Dictionary<string, List<ExpressionNode>> fetchNodes = filterExp.FetchNodes<IndexExpressionNode>();
            List<string> paramList = fetchNodes.Where(itm => itm.Key.StartsWith("q["))
                .Select(nodes =>
                {
                    VariableNameExpressionNode l = (VariableNameExpressionNode)nodes.Value.OfType<IndexExpressionNode>().FirstOrDefault().IndexExpression;
                    return l?.Name;
                }).ToList();
            foreach (string paramId in paramList)
            {
                _ = pi.GetData(paramId, out object value);
                qParams[paramId] = value;
            }

            _ = q.AddFilter(filterExp);
        }

        _ = localParameters.AddOrUpdate("q", qParams);
        foreach (string fld in fields ?? Enumerable.Empty<string>())
        {
            _ = q.SelectField(fld);
        }
        /*foreach (var fld in dataStore.itemSubjectRef.structureRef.entityFields)
           if (!fld.Value.NotMap)
           {
               q.SelectField(fld.Key); //, fld.Value
           }*/
        List<ElasticObject> list = q.ToList(localParameters);
        q.ReleaseQuery();
        pi.LogTrace($"DataStore List pi:{pi.Id}, Query: {q.CommandTxt}");
        return list;
    }

    private static string GenerateFilter(
        EntityField field, bool writeQ, string qFieldId, string value)
    {
        if (field.IsForeignParam())
        {
            if (!value.Contains(","))
            {
                return field.Id + "=='" + value + "'";
            }

            return value.StartsWith("{") ? field.Id + " In(List(" + value + "))" : field.Id + " In(List({" + value + "}))";
        }

        if (field.IsTextParam())
        {
            return $"ISNULL({field.Id},'')=={(writeQ ? "q[" + qFieldId + "]" : "'" + value + "'")}";
        }

        if (field.IsBoolParam())
        {
            return $"ISNULL({field.Id},0)=={(value == "1" || value.ToLower() == "true" ? "1" : "0")}";
        }

        if (field.IsDoubleParam())
        {
            return $"ISNULL({field.Id},0)=={(writeQ ? "q[" + qFieldId + "]" : value)}";
        }

        if (field.IsLongParam())
        {
            return $"ISNULL({field.Id},0)=={(writeQ ? "q[" + qFieldId + "]" : value)}";
        }

        if (field.IsTimeSpan())
        {
            return $"ISNULL({field.Id},0)=={(writeQ ? "q[" + qFieldId + "]" : value)}";
        }

        return field.IsDateTime()
            ? $"({field.Id}>={(writeQ ? "q[" + qFieldId + "]" : value)})" +
                   $" and ({field.Id}<AddDays({(writeQ ? "q[" + qFieldId + "]" : value)},1))"
            : $"{field.Id}=={(writeQ ? "q[" + qFieldId + "]" : value)}";
    }

    internal static ElasticObject ReadDataStore(DataStoreReference dataStoreReference,
        ProcessInstance pi, IList<string> fields)
    {
        if (dataStoreReference?.dataStore == null)
        {
            return null;
        }

        DataStore dataStore = dataStoreReference.dataStore;
        ElasticObject el = null;

        QueryUtility q = new(dataStore.NamespaceId, dataStore.EntityId);
        if (q.Entity == null)
        {
            return null;
        }

        LocalParameters lc = new(pi?.AuditTrail?.User);
        foreach (DataStoreReference.FilterMapping filterMapping in dataStoreReference.FilterMappings ??
                                      Enumerable.Empty<DataStoreReference.FilterMapping>())
        {
            _ = q.AddFilter($"{filterMapping.DataStoreField}==__{filterMapping.ProcessProperty}");
            _ = lc.AddOrUpdate("__" + filterMapping.ProcessProperty,
                pi.GetData(filterMapping.ProcessProperty, out object value)
                    ? value
                    : filterMapping.ProcessProperty);
        }

        foreach (string fld in fields ?? Enumerable.Empty<string>())
        {
            _ = q.SelectField(fld);
        }

        /*foreach (var fld in dataStore.itemSubjectRef.structureRef.entityFields)
		{
			q.SelectField(fld.Key);
		}*/
        if (q.GetDocuments(lc))
        {
            el = q.GetRecord();
        }

        q.ReleaseQuery();
        pi.LogTrace($"DataStore pi:{pi.Id}, Query: {q.CommandTxt}");
        return el;
    }

    internal static bool UpsertDataStore(DataStoreReference dataStoreReference,
        ElasticObject record, FlowNodeInstance ai, LocalParameters lp)
    {
        DataStore dataStore = dataStoreReference?.dataStore;
        if (dataStore?.itemSubjectRef?.structure == null)
        {
            return false;
        }

        if (dataStore.itemSubjectRef.structure.NotMapped)
        {
            return true;
        }

        Entity entity = dataStore.itemSubjectRef.structure;
        ApplyUtility au = new(entity, ai.AuditTrail, lp);
        if (au.Entity == null)
        {
            return false;
        }

        IEnumerable<string> fieldIds = entity.entityFields.Select(fld => fld.Key);
        foreach (string fieldId in fieldIds)
        {
            if (record.GetField(fieldId, out object fValue))
            {
                _ = au.AddField(fieldId, fValue);
            }
        }

        if (!au.Save(record))
        {
            ai.LogTrace($"can not save Upsert Data Store. command : {au.CommandTxt}");
            return false;
        }

        EntityField pkField = entity.KeyFields?.FirstOrDefault();
        if (!string.IsNullOrEmpty(dataStoreReference.SaveOnPrimaryKeyProperty) && pkField != null)
        {
            _ = ai.SetData(dataStoreReference.SaveOnPrimaryKeyProperty, record[pkField.Id]);
        }

        return true;
    }

    public static void SaveAudit(AuditTrail auditTrail)
    {
    }
}
