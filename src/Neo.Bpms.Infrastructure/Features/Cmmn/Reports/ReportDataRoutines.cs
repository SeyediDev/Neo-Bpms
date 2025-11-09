using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReportDataRoutines(ReportStructRoutines reportStructRoutines, SubReportData subReportData)
{
    public async Task<ReportData> GetReportData(ConfiguredReport config,
        bool loadData, ElasticObject filterValues, int pageNo, string sortFields,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        IList<object> ids, string culture, bool forPrint, IdentityUser user,
        int recordsPerPage = 20,
        bool giveQueryTexts = false, CancellationToken cancellationToken = default)
    {
        (ReportStructure structure, int configRecordsPerPage) = await reportStructRoutines.GetReportStructure(config, sortFields, culture, user);

        NormalizeFormValues(filterValues, config.Report.entity);

        ReportData reportData = await PrivateGetReportData(config, structure, loadData, filterValues, pageNo,
            parentReportConfig, subReport, ids, culture, forPrint, user,
            configRecordsPerPage <= 0 ? recordsPerPage : configRecordsPerPage,
            giveQueryTexts, cancellationToken);

        // loading combo data only if needed (used in parent filter)
        if (config.Parent?.ParentConfiguredReport == null)
        {
            LocalParameters lp = GetLocalParameters(user);
            FormComboData.SetCombosData(config.Report, reportData.structure, culture, null, lp);
            if (filterValues != null)
            {
                ComboDataRoutines.SetComboDataSelectedId(reportData.structure, filterValues, true, config.Report.entity);
            }
        }

        return reportData;
    }

    private async Task<ReportData> PrivateGetReportData(ConfiguredReport config, ReportStructure structure,
        bool loadData, ElasticObject filterValues, int pageNo,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        IList<object> ids, string culture, bool forPrint, IdentityUser user,
        int recordsPerPage, bool giveQueryTexts,
        CancellationToken cancellationToken)
    {
        ReportData reportData = new(config, structure, filterValues, giveQueryTexts)
        {
            recordsPerPage = recordsPerPage
        };
        if (loadData)
        {
            LocalParameters lp = GetLocalParameters(user);
            await GetReportRecords(reportData, config, parentReportConfig, subReport, ids,
                reportData.recordsPerPage, pageNo, culture, forPrint, lp, user,
                cancellationToken);
        }

        return reportData;
    }
    public static LocalParameters GetLocalParameters(IdentityUser user) =>
        new() { { "user", user }, { "userId", user?.Id } };

    internal async Task GetReportRecords(ReportData reportData, ConfiguredReport config,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        IList<object> ids, int recordsPerPage, int pageNumber,
        string culture, bool forPrint, LocalParameters lp, IdentityUser user,
        CancellationToken cancellationToken)
    {
        if (reportData.structure.SelectedColumns.Count == 0) return;
        UiEntity entity = config.Report.entity;

        if (reportData.structure.ReportViewType != ReportViewType.Chart)
        {
            if (ReportTotalRecord.GetRecordCount(cancellationToken, reportData, entity, ids, config, lp) == 0)
                return;
            ReportTotalRecord.GetTotalRecord(cancellationToken, reportData, entity,
                ids, config, parentReportConfig, subReport, lp);
        }

        _ = await RunQueryPage(cancellationToken,
            reportData, config, parentReportConfig, subReport, ids, 
            recordsPerPage, pageNumber, culture, forPrint, lp, user, entity);
    }

    public async Task<(int recordsPerPage, int pageNumber)>
        RunQueryPage(CancellationToken cancellationToken,
        ReportData result, ConfiguredReport config,
        ConfiguredReport parentReportConfig, 
        ConfiguredReport.ConfiguredSubReport subReport, IList<object> ids,
        int recordsPerPage, int pageNumber,
        string culture, bool forPrint, LocalParameters lp, IdentityUser user, UiEntity entity)
    {
        JoinQueriesData joinQueries = new(config.UniqueId);
        QueryUtility qd = EstablishReportQuery.GetQuery(cancellationToken,
            result, config, parentReportConfig, subReport, ids,
            ref recordsPerPage, ref pageNumber, forPrint, lp, entity, joinQueries, out ReferFields referFields);
        try
        {
            await RunQuery(qd, result, config, referFields, culture, forPrint, lp, user, joinQueries, cancellationToken);
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.2.0", e.ToString());
        }
        qd.ReleaseQuery();
        return (recordsPerPage, pageNumber);
    }

    private static void NormalizeFormValues(ElasticObject filterValues, UiEntity entity)
    {
        if (filterValues == null) return;
        List<KeyValuePair<string, ElasticObject>> attributes = filterValues.Attributes.ToList();
        foreach (KeyValuePair<string, ElasticObject> filterValue in attributes)
        {
            EntityField field = entity.GetField(filterValue.Key);
            if (field == null || filterValue.Value == null) continue;
            if (field.IsTimeSpan() && filterValue.Value.InternalValue is not TimeSpan)
            {
                string v = filterValue.Value.InternalValue?.ToString();
                filterValue.Value.InternalValue =
                    !string.IsNullOrEmpty(v) ? (TimeSpan?)ConvUtill.FetchTimeSpan(v) : null;
            }
        }
    }

    private async Task RunQuery(QueryUtility q, ReportData result,
        ConfiguredReport config, ReferFields referFields,
        string culture, bool forPrint, LocalParameters lp, IdentityUser user,
        JoinQueriesData joinQueries, CancellationToken cancellationToken)
    {
        if (!q.GetDocuments(result.structure.FilterValues, lp))
        {
            result.QueryInfo.AddByQueryUtility(q);
            return;
        }

        ConcurrentDictionary<string, BitmaskData> bitmaskData = new();
        var rows = GetRecords(q, result).ToList();
        await Task.WhenAll(rows.Select(
            async row => await RunQueryRecord(q, result, config, referFields, culture,
                forPrint, lp, user, joinQueries, row, bitmaskData, cancellationToken)));
        result.QueryInfo.AddByQueryUtility(q);
        q.ReleaseQuery();
        if (result.Rows.Count <= 0)
            return;
        if (q.CancellationToken.IsCancellationRequested)
            return;
        FetchJoinQuery.FetchJoinQueriesData(culture, lp, joinQueries, result.QueryInfo);
    }

    private static IEnumerable<ReportRowInfo> GetRecords(QueryUtility q, ReportData result)
    {
        foreach (ElasticObject record in q.GetRecords())
        {
            ReportRowInfo row = new() { Data = record /*, SubReport=result.SubReportId*/};
            result.Rows.Add(row);
            yield return row;
        }
    }

    private Task RunQueryRecord(QueryUtility q, ReportData result, ConfiguredReport config,
        ReferFields referFields, string culture, bool forPrint, LocalParameters lp, IdentityUser user,
        JoinQueriesData joinQueries, ReportRowInfo row, 
        ConcurrentDictionary<string, BitmaskData> bitmaskData, CancellationToken cancellationToken)
    {
        SetReferFieldsPerRecord(q, referFields, joinQueries, row.Data, culture);
        SetRecordIds(q, result, config, culture, row.Data, bitmaskData);
        FormDataRoutines.SetEnumValues(culture, row.Data, q.Entity);
        return subReportData.RunSubQueries(this, culture, result, config, row, forPrint, lp, user, cancellationToken);
    }

    private static void SetReferFieldsPerRecord(QueryUtility q, ReferFields referFields,
        JoinQueriesData joinQueries, ElasticObject r, string culture)
    {
        if (referFields?.Values != null) {
            foreach (ReferField referField in referFields?.Values)
            {
                EntityField rField = referField.Field;
                if (rField.AssociationEntity?.Entity()?.IsEntityState != false)
                {
                    long stateId = r.GetLong("StateId");
                    EntityState entityState = q.Entity.GetState(stateId.ToString());
                    r.SetField(referField.Id,
                        (culture == "en" ? entityState?.enName : entityState?.Name)
                                  ?? stateId.ToString());
                    continue;
                }

                if (rField.AssociationEntity?.Entity()?.DisplayStrings == null) continue;
                joinQueries.TryGetValue(rField.AssociationEntity, out JoinQueryData joinQueryData);
                if (joinQueryData == null) continue;
                string fkIds = "";
                foreach (EntityRelationMap fkField in rField.AssociationEntity.Maps)
                {
                    r.GetField(referField.AssociationPrefix + fkField.SourceField, out object fk);
                    fkIds += (string.IsNullOrEmpty(fkIds) ? "" : "#") + fk;
                }

                if (string.IsNullOrEmpty(fkIds) || fkIds == "null")
                    continue;
                joinQueryData.Add(fkIds, referField.Id, r);
            }
        }
    }

    private static void SetRecordIds(QueryUtility q, ReportData result, ConfiguredReport config, string culture,
        ElasticObject r, ConcurrentDictionary<string, BitmaskData> bitmaskData)
    {
        object obj1;
        List<object> idArray = [];
        if (config.ViewType == ReportViewType.List)
            idArray.AddRange(from key in config.Report.entity.KeyFields
                             select r.GetField(key.Id, out obj1) ? obj1 : null);
        else
        {
            foreach (ColumnFieldDefinition field in result.structure.SelectedColumns)
            {
                if (field.aggrType != eAggregationFunctions.GroupByItem)
                    continue;
                Entity entity1 = q.Entity.Id == field.entityId
                    ? q.Entity
                    : ProjectDefinition.Project.GetEntityByEntityId(field.entityId);
                if (entity1 == null) continue;
                EntityField fe = entity1.GetField(field.ColumnName);
                if (fe?.AssociationEntity?.Maps != null)
                    idArray.AddRange(
                        fe.AssociationEntity.Maps.Select(
                            map => r.GetField(field.AssociationPrefix + map.SourceField, out obj1) ? obj1 : null));
                else
                    idArray.Add(r.GetField(field.ColumnTypeName, out obj1) ? obj1 : null);
            }
        }

        SetBitmaskValues(q, result, culture, bitmaskData, r);
        r.SetField("Ids", string.Join(",", idArray));
    }

    private static void SetBitmaskValues(QueryUtility q, ReportData result, string culture,
        ConcurrentDictionary<string, BitmaskData> bitmaskData, ElasticObject r)
    {
        foreach (ColumnFieldDefinition field in result.structure.SelectedColumns)
        {
            Entity entity1 = q.Entity.Id == field.entityId
                ? q.Entity
                : ProjectDefinition.Project.GetEntityByEntityId(field.entityId);
            EntityField fe = entity1?.GetField(field.ColumnName);
            if (fe?.FieldType != TVariableTypes.Association || fe.AssociationEntity?.Maps == null ||
                 fe.AssociationEntity.Maps.Count <= 0)
                continue;
            if (!fe.AssociationEntity.CheckFlag(EntityFieldFlags.IsBitMask))
                continue;
            object value = null;
            string fieldName = "";
            foreach (EntityRelationMap map in fe.AssociationEntity.Maps)
            {
                r.GetField(map.SourceField, out value);
                fieldName = map.SourceField;
                break;
            }

            if (value == null) continue;
            string key = fe.Entity.Id + "_" + fe.Id;
            if (!bitmaskData.TryGetValue(key, out BitmaskData bitmask))
            {
                bitmask = new BitmaskData();
                QueryUtility bitmaskQ = new(fe.AssociationEntity.Entity(), "ReportDataRoutines.1");
                bitmaskQ.AddPkFields().AddBasicFields(culture);
                if (bitmaskQ.GetDocuments())
                {
                    ElasticObject r2;
                    while ((r2 = bitmaskQ.GetRecord()) != null)
                        bitmask.Add(bitmaskQ.GetIdsData(r2), bitmaskQ.GetBasicFieldsData(r2, culture));
                }

                bitmaskQ.ReleaseQuery();
                bitmaskData.TryAdd(key, bitmask);
            }

            int v = Convert.ToInt32(value);
            string newValue = "";
            for (int i = 0; i < 64; i++)
            {
                int id = 1 << i;
                if ((id & v) == 0) continue;
                string bId = i.ToString();
                if (!bitmask.ContainsKey(bId)) continue;
                if (!string.IsNullOrEmpty(newValue)) newValue += " - ";
                newValue += bitmask[bId];
            }

            r[fieldName] = newValue;
            r[fe.Id] = newValue;
        }
    }

    private class BitmaskData : Dictionary<string, string>
    {
    }
}
