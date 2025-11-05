using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;
using Neo.Bpms.Domain.Models.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public static class EstablishReportQuery
{
    public static QueryUtility GetRecordCountQuery(ReportData result, Entity entity, IList<object> ids,
        ConfiguredReport config, LocalParameters lp)
    {
        QueryUtility qCount = new(entity, "EstablishReportQuery.1");
        try
        {
            JoinQueriesData joinQueries = new(config.UniqueId);
            if (config.Parent?.ParentConfiguredReport != null && ids != null)
                AddParentReportJoinsAndFilters(qCount, config, ids, config.Parent.ParentConfiguredReport, config.Parent);
            AddReportFiltersAndHavings(result, config, qCount, lp);
            SelectReportFields(qCount, result.structure.SelectedColumns,
                entity, config.ViewType, joinQueries);
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.1.1", e.ToString());
        }
        return qCount;
    }

    public static QueryUtility GetTotalQuery(ReportData result, Entity entity, IList<object> ids,
        ConfiguredReport config, ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        LocalParameters lp)
    {
        QueryUtility q = new(entity, "EstablishReportQuery.2");
        JoinQueriesData joinQueries = new(config.UniqueId);
        try
        {
            if (parentReportConfig != null && ids != null)
                AddParentReportJoinsAndFilters(q, config, ids, parentReportConfig, subReport);
            else if (config.Parent?.ParentConfiguredReport != null && ids != null)
                AddParentReportJoinsAndFilters(q, config, ids, config.Parent.ParentConfiguredReport, config.Parent);
            AddReportFiltersAndHavings(result, config, q, lp);
            SelectReportFields(q, result.structure.SelectedColumns, entity, config.ViewType, joinQueries, true);
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.1.0", e.ToString());
        }
        return q;
    }

    public static QueryUtility GetQuery(CancellationToken cancellationToken, ReportData result,
        ConfiguredReport config,
        ConfiguredReport parentReportConfig,
        ConfiguredReport.ConfiguredSubReport subReport,
        IList<object> ids,
        ref int recordsPerPage,
        ref int pageNumber, bool forPrint, LocalParameters lp, UiEntity entity,
        JoinQueriesData joinQueries, out ReferFields referFields)
    {
        QueryUtility qd = new(entity, "EstablishReportQuery.3")
        {
            CancellationToken = cancellationToken
        };
        try
        {
            referFields = SelectReportFields(qd, result.structure.SelectedColumns,
                entity, config.ViewType, joinQueries);
            AddReportFormats(qd, config);
            if (parentReportConfig != null && ids != null)
                AddParentReportJoinsAndFilters(qd, config, ids, parentReportConfig, subReport);
            else if (config.Parent?.ParentConfiguredReport != null && ids != null)
                AddParentReportJoinsAndFilters(qd, config, ids, config.Parent.ParentConfiguredReport, config.Parent);
            AddReportFiltersAndHavings(result, config, qd, lp);
            SetQueryPage(config, ref recordsPerPage, ref pageNumber, forPrint, qd);
            AddOrderBys(result, qd);
        }
        catch (Exception e)
        {
            result.Errors.AddError(e.Message, "Query", "13.1.2", e.ToString());
            referFields = null;
        }

        return qd;
    }

    private static void AddReportFormats(QueryUtility qd, ConfiguredReport config)
    {
        if (config.Formats == null) return;
        foreach (ConditionalFormatting conditionalFormatting in config.Formats)
        {
            string formula = $"IF(({conditionalFormatting.Filter});1;0)";
            if (config.ViewType == ReportViewType.List)
                qd.SelectFormulaField(conditionalFormatting.QueryName, formula);
            else
                qd.GroupByFormula(formula, eAggregationFunctions.Formula, eAggregateScope.All,
                    conditionalFormatting.QueryName);

        }
    }

    private static void AddReportFiltersAndHavings(
        ReportData result, ConfiguredReport config, QueryUtility qd, LocalParameters lp)
    {
        Report report = config.Report;
        LocalParameters el = FormDataRoutines.GetLocalParamValues(lp["user"], qd.Entity.model.Id, qd.Entity.Id, report.Id,
            result.structure.FilterValues);
        if (result.structure.FilterValues != null)
            lp.AddOrUpdate("q", result.structure.FilterValues);
        result.structure.FilterValues?.AddIfNot("user", lp["user"]);
        FormDataFilter.AddFilters(qd, report, result.structure.FilterValues, out _);

        if (!string.IsNullOrEmpty(config.WhereCondition))
            qd.Where(config.WhereCondition);
        if (config.ViewType != ReportViewType.List)
        {
            if (report.InputRecordsHavings != null)
            {
                foreach (ReportHaving having in report.InputRecordsHavings)
                {
                    if (having.filter == null) continue;
                    if (having.condition != null)
                    {
                        ExpressionNode condition = having.condition?.Root.clone();
                        if (!ExpressionNode.CheckIfTrue(condition.Eval(el, lp)))
                            continue;
                    }

                    ExpressionNode f = having.filter.Root.clone();
                    qd.Having(f);
                }
            }

            if (!string.IsNullOrEmpty(config.HavingCondition))
                qd.Having(config.HavingCondition);
        }
    }

    private static void AddParentReportJoinsAndFilters(QueryUtility qd,
        ConfiguredReport config, IList<object> ids, ConfiguredReport parentConfig,
        ConfiguredReport.ConfiguredSubReport subReport)
    {
        int j = 0;
        AddParentReportJoinsAndFilters(qd, config, ids, ref j, parentConfig, subReport);
    }

    private static void AddParentReportJoinsAndFilters(QueryUtility qd, ConfiguredReport config, IList<object> ids,
        ref int j, ConfiguredReport parentConfig, ConfiguredReport.ConfiguredSubReport subReport)
    {
        while (true)
        {
            //check if parent entity is not the same as entity, in this case,  1. the association relation is meaningful 2. the parent config view type is report list
            if (!Equals(parentConfig.Report.entity, qd.Entity))
            {
                EntityField relField = qd.Entity.GetField(subReport.AssociationName);
                if (relField?.AssociationEntity?.Maps != null)
                {
                    foreach (EntityRelationMap map in relField.AssociationEntity.Maps)
                    {
                        AddParentReportFilter(qd, ids, ref j, qd.Entity.GetField(map.SourceField));
                        if (j >= ids.Count) break;
                    }
                }
            }
            else if (parentConfig.ViewType != ReportViewType.List)
            {
                //parent and sub reports are both from the same entity and the group by fields can be used as the filter.
                if (parentConfig.Fields != null)
                {
                    foreach (ConfiguredReport.SelectedField field in parentConfig.Fields.Values.OrderBy(f => f.Order))
                    {
                        if (field.type != ConfiguredReport.eFieldSelectionType.asGroupBy) continue;
                        if (!SelectQueryField(qd, field.entityId, field.AssociationName, field.fieldId, out QueryUtility colQuery, out EntityField colField)) continue;
                        if (colField?.AssociationEntity != null)
                        {
                            if (colField.AssociationEntity.Maps != null)
                            {
                                foreach (EntityRelationMap map in colField.AssociationEntity.Maps) AddParentReportFilter(colQuery, ids, ref j, colField.Entity.GetField(map.SourceField));
                            }
                        }
                        else
                            AddParentReportFilter(colQuery, ids, ref j, colField);

                        if (j > ids.Count) break;
                    }
                }

                //parent and sub reports are both from the same entity and the primary key can be used as the filter.
                if (!string.IsNullOrEmpty(parentConfig.WhereCondition)) qd.Where(parentConfig.WhereCondition);
                if (config.ViewType != ReportViewType.List)
                {
                    if (!string.IsNullOrEmpty(parentConfig.HavingCondition)) qd.Having(parentConfig.HavingCondition);
                }
            }
            else
            {
                //parent and sub reports are both from the same entity and the primary key can be used as the filter.
                if (!string.IsNullOrEmpty(parentConfig.WhereCondition)) qd.Where(parentConfig.WhereCondition);
                foreach (EntityField key in parentConfig.Report.entity.KeyFields)
                {
                    AddParentReportFilter(qd, ids, ref j, key);
                    if (j > ids.Count) break;
                }
            }

            if (Equals(parentConfig.Report.entity, qd.Entity) && parentConfig.Report.Id == config.Report.Id)
            {
                if (parentConfig.Parent?.ParentConfiguredReport != null)
                {
                    ConfiguredReport parentConfig1 = parentConfig;
                    config = parentConfig;
                    parentConfig = parentConfig.Parent.ParentConfiguredReport;
                    subReport = parentConfig1.Parent;
                    continue;
                }
            }

            break;
        }
    }

    private static void AddParentReportFilter(QueryUtility qd, IList<object> ids, ref int j, EntityField field)
    {
        if (j >= ids.Count) return;
        object obj = ids[j];
        if (field.FieldType == TVariableTypes.BOOL)
        {
            bool v = !string.IsNullOrEmpty(obj?.ToString()) && ConvUtill.ToBoolean(obj);
            if (v)
                qd.AddFilter(field.Id + "==1");
            else qd.AddFilter("ISNULL(" + field.Id + ",0)==0");
        }

        if (field.FieldType == TVariableTypes.Date)
        {
            DateTime v = !string.IsNullOrEmpty(obj?.ToString()) ? Convert.ToDateTime(obj) : DateTime.MinValue;
            if (v != DateTime.MinValue)
            {
                if (v.Hour == 0 && v.Minute == 0 && v.Second == 0 && v.Millisecond == 0)
                    qd.AddFilter(field.Id + "=='" + obj + "'");
                else
                {
                    v = v.AddSeconds(1);
                    qd.AddFilter("(" + field.Id + ">='" + obj + "') And (" + field.Id + "<'" + v.Year + "-" + v.Month +
                                    "-" + v.Day +
                                    " " + v.Hour + ":" + v.Minute + ":" + v.Second + "')");
                }
            }
            else
                qd.AddFilter("ISNULL(" + field.Id + ",'')==''");
        }
        else if (obj is string s)
        {
            if (!string.IsNullOrEmpty(s))
                qd.AddFilter(field.Id + "=='" + obj + "'");
            else
                qd.AddFilter($"((ISNULL({field.Id},'')=='') OR (Concat({field.Id},' ')==' '))");
        }
        else if (obj != null)
            qd.AddFilter(field.Id + "==" + obj);
        else
            qd.AddFilter("ISNULL(" + field.Id + ",0)==0");

        j++;
    }

    private static ReferFields SelectReportFields(
        QueryUtility qd, IEnumerable<ColumnFieldDefinition> selectedColumns,
        Entity entity, ReportViewType viewType,
        JoinQueriesData joinQueries, bool isTotalQuery = false)
    {
        ReferFields referFields = [];
        if (!isTotalQuery && viewType == ReportViewType.List)
        {
            foreach (EntityField efld in entity.KeyFields)
                qd.SelectField(efld); //+ (string.IsNullOrEmpty(item.Alias) ? "" : " " + item.Alias)
        }

        foreach (ColumnFieldDefinition item in selectedColumns)
        {
            if (item.ColumnName == "*")
            {
                qd.GroupBy("*", eAggregationFunctions.Count, eAggregateScope.All, item.ColumnTypeName);
                continue;
            }

            //+ (string.IsNullOrEmpty(item.Alias) ? "" : " " + item.Alias)
            if (!SelectQueryField(qd, item.entityId, item.AssociationName, item.ColumnName,
                out QueryUtility colQuery, out EntityField colField))
            {
                if (item.aggrType == eAggregationFunctions.Formula &&
                    !string.IsNullOrEmpty(item.formula))
                {
                    if (viewType == ReportViewType.List)
                        qd.SelectFormulaField(item.ColumnTypeName, item.formula);
                    else
                        qd.GroupByFormula(item.formula, eAggregationFunctions.InColumn, eAggregateScope.All
                            , item.ColumnTypeName);
                }

                continue;
            }

            if (colField?.AssociationEntity != null)
            {
                if (viewType == ReportViewType.List ||
                    item.aggrType == eAggregationFunctions.GroupByItem ||
                    item.aggrType == eAggregationFunctions.InColumn)
                {
                    if (!referFields.ContainsKey(item.AssociationPrefix + colField.Id))
                        referFields.Add(item.AssociationPrefix + colField.Id, new ReferField
                        {
                            Field = colField,
                            AssociationPrefix = item.AssociationPrefix
                        });
                    if (!isTotalQuery && colField.AssociationEntity.Maps != null)
                    {
                        foreach (EntityRelationMap map in colField.AssociationEntity.Maps)
                        {
                            if (viewType == ReportViewType.List)
                                colQuery?.SelectField(map.SourceField, item.AssociationPrefix + map.SourceField);
                            else
                                colQuery?.GroupBy(map.SourceField, item.aggrType, item.AssociationPrefix + map.SourceField);
                        }
                    }

                    string displayFields = item.Property(eControlPropertyId.DisplayFields);
                    if (string.IsNullOrEmpty(displayFields))
                        displayFields = colField?.Property(EntityFieldPropertyId.DisplayFields);
                    joinQueries.TryAdd(colField.AssociationEntity, displayFields?.Split(",").ToList());
                }
            }
            else
            {
                if (isTotalQuery &&
                    (item.aggrType == eAggregationFunctions.GroupByItem ||
                    item.aggrType == eAggregationFunctions.InColumn &&
                    (!colField?.Formula?.UsedForAggregationOnly ?? true)))
                    continue;
                if (viewType == ReportViewType.List)
                    colQuery?.SelectField(item.ColumnName, item.ColumnTypeName);
                //+ (string.IsNullOrEmpty(item.Alias) ? "" : " " + item.Alias)
                else /* if(item.aggrType == eAggregationFunctions.GroupByItem|| 
					        item.aggrType == eAggregationFunctions.InColumn || 
					        item.aggrType == eAggregationFunctions.Formula)*/
                    colQuery?.GroupBy(item.ColumnName, item.aggrType, item.ColumnTypeName);
            }
        }

        return referFields;
    }

    private static bool SelectQueryField(QueryUtility qd,
        string colEntityId, string associationName, string columnName,
        out QueryUtility colQuery, out EntityField colField)
    {
        colQuery = null;
        Entity entity = qd.Entity;
        if (!string.IsNullOrEmpty(associationName))
        {
            string[] associationItems = associationName.Split('.');
            int ia = 0;
            foreach (string associationItem in associationItems)
            {
                EntityField associationField = entity.GetField(associationItem);
                if (associationField?.AssociationEntity != null)
                {
                    QueryUtility.SubQueryDefinition jq = qd.LeftOuterJoin(associationField.AssociationEntity.Entity(), null);
                    if (jq.fieldMappings.Count == 0)
                        jq.SetMapping(associationField);
                    if (ia == associationItems.Length - 1)
                    {
                        colQuery = jq.Query;
                        colField = associationField.AssociationEntity.Entity().GetField(columnName);
                        return true;
                    }

                    qd = jq.Query;
                    entity = qd.Entity;
                }

                ia++;
            }
        }

        if (entity.Id != colEntityId)
        {
            //for backward compability
            EntityField leftfld = GetLeftEntityField(colEntityId, entity);
            if (leftfld != null)
            {
                QueryUtility.SubQueryDefinition jq = qd.LeftOuterJoin(leftfld.AssociationEntity.Entity(), null);
                if (jq.fieldMappings.Count == 0)
                    jq.SetMapping(leftfld);
                colQuery = jq.Query;
                colField = leftfld.Entity.GetField(columnName);
                return true;
            }

            if (qd.subQuerys != null)
            {
                foreach (QueryUtility.SubQueryDefinition sub in qd.subQuerys.Values)
                {
                    if (SelectQueryField(sub.Query, colEntityId,
                        associationName, columnName, out colQuery, out colField))
                        return true;
                }
            }

            colField = null;
            return false; //SelectOuterField(qd, ref colQuery, out colField, colEntity, entity);
        }

        colQuery = qd;
        colField = entity.GetField(columnName);
        return colField != null;
    }

    private static EntityField GetLeftEntityField(string colEntityId, Entity entity)
    {
        EntityField leftfld =
            entity.entityFields.Values.FirstOrDefault(
                f => f.AssociationEntity?.Entity()?.Id == colEntityId &&
                        f.AssociationEntity?.Maps?.FirstOrDefault(map =>
                        {
                            EntityField src = entity.GetField(map.SourceField);
                            return !src?.NotMap ?? false;
                        }) != null);
        return leftfld;
    }

    private static void AddOrderBys(ReportData result, QueryUtility qd)
    {
        foreach (ReportOrderInfo item in result.structure.OrderInfos)
        {
            if (item.EntityId == result.structure.EntityId)
            {
                //					if(/*item.Alias!=null && */
                //						(result.structure.ReportViewType == eReportViewType.Chart ||
                //					    result.structure.ReportViewType == eReportViewType.GroupByList))
                //					{
                //						if(!result.structure.SelectedColumns.Any(s=>(s.FieldName?? s.Alias) == item.ColumnName|| s.ColumnName==item.ColumnName || s.ColumnOrderName == item.ColumnName))
                //							continue;
                //					}
                qd.OrderBy(item.ColumnName,
                    item.Descending ? SortType.Descending : SortType.Ascending,
                    item.orderById);
            }
            else
            {
                if (!SelectQueryField(qd, item.EntityId, item.AssociationName, item.ColumnName,
                    out QueryUtility colQuery, out _))
                    continue;
                colQuery.OrderBy(item.ColumnName,
                    item.Descending ? SortType.Descending : SortType.Ascending,
                    item.orderById);
                if (result.structure.ReportViewType == ReportViewType.Chart ||
                    result.structure.ReportViewType == ReportViewType.GroupByList)
                    colQuery.GroupBy(item.ColumnName, eAggregationFunctions.GroupByItem);
            }
        }

        if (result.structure.OrderInfos.Count == 0 && qd.topRows > 0 && result.structure.ReportViewType == ReportViewType.List)
        {
            EntityField key = qd.Entity.KeyFields?.FirstOrDefault();
            if (key != null)
                qd.OrderBy(key.Id);
            else if (!string.IsNullOrEmpty(qd.Entity.PartitionField))
                qd.OrderBy(qd.Entity.PartitionField);
        }
    }

    private static void SetQueryPage(ConfiguredReport config, ref int recordsPerPage, ref int pageNumber,
        bool forPrint, QueryUtility qd)
    {
        if (forPrint || config.ViewType == ReportViewType.Chart &&
            (config.ChartType == Report.ChartType.WorldMap ||
            config.ChartType == Report.ChartType.Treemap ||
            config.ChartType == Report.ChartType.IranMap))
        {
            recordsPerPage = 1000;
            pageNumber = 1;
        }
        else if (config.ViewType == ReportViewType.Chart)
        {
            recordsPerPage = 64;
            pageNumber = 1;
        }

        qd.topRows = recordsPerPage;
        qd.startIndex = recordsPerPage * (Math.Max(1, pageNumber) - 1);
    }
}
