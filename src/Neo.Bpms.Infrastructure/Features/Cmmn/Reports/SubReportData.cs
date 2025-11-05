using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class SubReportData(ReportStructRoutines reportStructRoutines)
{
    public async Task RunSubQueries(ReportDataRoutines reportDataRoutines,
        string culture, ReportData result,
        ConfiguredReport config, ReportRowInfo row, bool forPrint,
        LocalParameters lp, IdentityUser user, CancellationToken cancellationToken)
    {
        if (config.SubReports == null || config.SubReports.IsEmpty)
            return;
        List<object> idArray = [];
        if (config.ViewType == ReportViewType.List)
        {
            //for association and same entity sub reports from detail list, primary key ids are always the relating key values
            foreach (EntityField fe in config.Report.entity.KeyFields)
            {
                object obj;
                if (fe.AssociationEntity?.Maps != null)
                    idArray.AddRange(
                        fe.AssociationEntity.Maps.Select(map => row.Data.GetField(map.SourceField, out obj) ? obj : null));
                else
                    idArray.Add(row.Data.GetField(fe.Id, out obj) ? obj.ToString() : null);
            }
        }
        foreach (ConfiguredReport.ConfiguredSubReport subReport in config.SubReports.Values)
        {
            if (!Equals(subReport.ConfiguredReport.Report.entity, config.Report.entity))
            {
                if (config.ViewType != ReportViewType.List ||
                    subReport.AssociationName == null)
                    continue;
            }
            else
            {
                if (config.ViewType != ReportViewType.List)
                {
                    foreach (ColumnFieldDefinition field in result.structure.SelectedColumns)
                    {
                        if (field.aggrType != eAggregationFunctions.GroupByItem) continue;
                        object obj;
                        Entity entity1 = config.Report.entity.Id == field.entityId
                            ? config.Report.entity
                            : ProjectDefinition.Project.GetEntityByEntityId(field.entityId);
                        if (entity1 == null) continue;
                        EntityField fe = entity1.GetField(field.ColumnName);
                        if (fe?.AssociationEntity?.Maps != null)
                            idArray.AddRange(
                                fe.AssociationEntity.Maps.Select(
                                    map => row.Data.GetField(field.AssociationPrefix + map.SourceField, out obj) ? obj : null));
                        else
                            idArray.Add(row.Data.GetField(field.ColumnTypeName, out obj) ? obj.ToString() : null);
                    }
                }
            }
            if (subReport.Type == Report.SubReportType.DrillDown)
                continue;
            ReportData subQuery = await RunSubQuery(reportDataRoutines,
                idArray, config, subReport, culture, forPrint, lp, user, result.structure.FilterValues, cancellationToken);
            if (subQuery == null)
                continue;
            row.SubReports ??= [];
            row.SubReports.Add(subQuery);
        }
    }

    private async Task<ReportData> RunSubQuery(
        ReportDataRoutines reportDataRoutines,
        IList<object> parentIds,
        ConfiguredReport parentConfig, ConfiguredReport.ConfiguredSubReport subReport,
        string culture, bool forPrint, LocalParameters lp, IdentityUser user, ElasticObject parentFilterValues, CancellationToken cancellationToken)
    {
        (ReportStructure structure, int subConfigRecordsPerPage) = await reportStructRoutines.GetReportStructure(
            subReport.ConfiguredReport, null, culture, user);
        if (subConfigRecordsPerPage <= 100)
            subConfigRecordsPerPage = 100;
        ReportData subQuery = new(parentConfig, structure, parentFilterValues)
        {
            Config = subReport.ConfiguredReport,
            recordsPerPage = subConfigRecordsPerPage
        };
        await reportDataRoutines.GetReportRecords(subQuery, subReport.ConfiguredReport,
            parentConfig, subReport,
            parentIds, subConfigRecordsPerPage, 1, culture, forPrint, lp, user, cancellationToken);
        return subQuery;
    }
}
