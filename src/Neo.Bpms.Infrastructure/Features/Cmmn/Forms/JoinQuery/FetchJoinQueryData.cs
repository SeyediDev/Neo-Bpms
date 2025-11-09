using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.JoinQuery;

public class FetchJoinQueryData
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public string Culture { get; set; }
    public LocalParameters LocalParameters { get; set; }
    public JoinQueryData JoinQueryData { get; set; }
    public QueryUtility Query { get; set; }

    public void Fetch(QueryInfo queryInfo)
    {
        if (JoinQueryData.Association.Entity().IsEntityState || JoinQueryData.Association.Entity().UseEnumData)
            return;
        Query = new QueryUtility(JoinQueryData.Association.Entity(), "FetchJoinQueryData.1");
        int i = 0;
        if (JoinQueryData.Association.Maps != null)
            foreach (EntityRelationMap map in JoinQueryData.Association.Maps)
            {
                Query.SelectField(map.DestField);
                i++;
            }

        if (JoinQueryData.DisplayFields == null || JoinQueryData.DisplayFields.Count == 0)
        {
            foreach (BasicField bf in Query.Entity.DisplayStrings)
            {
                if (!bf.CheckCulture(Culture))
                    continue;
                if (!string.IsNullOrEmpty(bf.OtherFieldThatIgnoreMe))
                    Query.SelectField(bf.OtherFieldThatIgnoreMe);
                Query.SelectField(bf.FieldId);
            }
        }
        else
        {
            foreach (string bf in JoinQueryData.DisplayFields)
            {
                //TODO if (!bf.CheckCulture(Culture))
                //    continue;
                Query.SelectField(bf);
            }
        }
        string filter = i > 1 ? "concat(" : "";
        string pkFieldIds = JoinQueryData.Association.Maps?
            .Aggregate("",
                (current, map) => current + (string.IsNullOrEmpty(current) ? "" : ",\"#\",") + map.DestField);
        filter += pkFieldIds;
        if (i > 1) filter += ")";
        string listId = JoinQueryData.Refers.Aggregate("",
            (current, item) =>
                current +
                (string.IsNullOrEmpty(current) ? "" : ",") +
                (string.IsNullOrEmpty(item.Key) ? "" : "'" + item.Key + "'")
        );
        filter += " In (" + listId + ")";
        if (filter == "Id In ()")
            filter = "1=0";
        if (filter != "1=0")
        {
            Query.AddFilter(filter);
            Query.topRows = JoinQueryData.Refers.Count;
            Query.GetDocuments(LocalParameters);
            queryInfo?.AddByQueryUtility(Query);
        }
    }

    public void Set()
    {
        if (JoinQueryData.Association.Entity().IsEntityState)
        {
            if (JoinQueryData.Association.SourceEntity?.GetStateCollection()?.States != null)
            {
                foreach (EntityState state in JoinQueryData.Association.SourceEntity.GetStateCollection().States.Values)
                {
                    ElasticObject record = new();
                    record.SetField("Id", state.Id);
                    record.SetField("Name", state.Name);
                    record.SetField("EnName", state.enName);
                    SetRecord(JoinQueryData.Association.Entity(), record);
                }
            }
        }
        else if (JoinQueryData.Association.Entity().UseEnumData)
        {
            if (JoinQueryData.Association.Entity()?.GetStateCollection()?.States != null)
            {
                foreach (EntityState state in JoinQueryData.Association.Entity().GetStateCollection().States.Values)
                {
                    ElasticObject record = new();
                    record.SetField("Id", state.Id);
                    record.SetField("Name", state.Name);
                    record.SetField("EnName", state.enName);
                    SetRecord(JoinQueryData.Association.Entity(), record);
                }
            }
        }
        else
        {
            foreach (ElasticObject record in Query.GetRecords())
            {
                SetRecord(Query.Entity, record);
            }

            Query.ReleaseQuery();
        }
    }

    private void SetRecord(Entity entity, IExpressionValue record)
    {
        string ids = "";
        if (JoinQueryData.Association.Entity().IsEntityState)
        {
            record.GetField("Id", out object obj);
            ids += (string.IsNullOrEmpty(ids) ? "" : "#") + obj;
        }
        else if (JoinQueryData.Association.Maps != null)
        {
            foreach (EntityRelationMap map in JoinQueryData.Association.Maps)
            {
                record.GetField(map.DestField, out object obj);
                ids += (string.IsNullOrEmpty(ids) ? "" : "#") + obj;
            }
        }

        JoinQueryData.Refers.TryGetValue(ids, out ReferList refers);
        if (refers == null) return;
        bool first = true;
        string displayValue = "";
        if (JoinQueryData.Association.Entity().IsEntityState)
        {
            record.GetField("Name", out object obj);
            displayValue += obj?.ToString() ?? "";
        }
        else
        {
            if (JoinQueryData.DisplayFields == null || JoinQueryData.DisplayFields.Count == 0)
            {
                foreach (BasicField bf in entity.DisplayStrings)
                {
                    if (!bf.CheckCulture(Culture))
                        continue;
                    EntityField bff = entity.GetField(bf.FieldId);
                    if (bff == null) continue;
                    if (bff.AssociationEntity != null) continue;
                    if (!string.IsNullOrEmpty(bf.OtherFieldThatIgnoreMe))
                    {
                        if (record.GetField(bf.OtherFieldThatIgnoreMe, out object obj1) || obj1 == null)
                            continue;
                    }

                    record.GetField(bf.FieldId, out object obj);
                    if (!first)
                        displayValue += " ";
                    displayValue += obj?.ToString() ?? "";
                    first = false;
                    JoinQueryData.SafeDo(() =>
                        {
                            foreach (ReferData refer in refers)
                            {
                                if (refer == null) // bug
                                {
                                    Logger.LogCritical("Refer is null");
                                    continue;
                                }

                                refer.Data.SetField(refer.Id + "." + bf.FieldId, obj);
                            }
                        }
                    );
                }
            }
            else
            {
                foreach (string bf in JoinQueryData.DisplayFields)
                {
                    //TODO if (!bf.CheckCulture(Culture))
                    //    continue;
                    EntityField bff = entity.GetField(bf);
                    if (bff == null) continue;
                    if (bff.AssociationEntity != null) continue;
                    record.GetField(bf, out object obj);
                    if (!first)
                        displayValue += " ";
                    displayValue += obj?.ToString() ?? "";
                    first = false;
                    JoinQueryData.SafeDo(() =>
                    {
                        foreach (ReferData refer in refers)
                        {
                            if (refer == null) // bug
                            {
                                Logger.LogCritical("Refer is null");
                                continue;
                            }

                            refer.Data.SetField(refer.Id + "." + bf, obj);
                        }
                    }
                    );
                }
            }
        }

        JoinQueryData.SafeDo(() =>
        {
            foreach (ReferData refer in refers)
            {
                refer?.Data.SetField(refer.Id, displayValue);
            }
        });
    }
}
