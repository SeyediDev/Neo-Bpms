using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Data.Base;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Engine.Entities;

public partial class QueryUtility
{
    internal static SubDataSource NewSubDataSource(EntityConnection entityOrmUtility,
        LocalParameters filterValues, SubQueryDefinition subQueryDefinition)
    {
        var sub = new SubDataSource
        {
            fieldMappings = [],
            joinType = subQueryDefinition.joinType,
            dataSource = subQueryDefinition.Query.DataSource
        };
        foreach (var fieldMapping in subQueryDefinition.fieldMappings)
        {
            var fm = entityOrmUtility.Entity.GetField(fieldMapping.MainTableFieldName);
            var fj = subQueryDefinition.Query.Entity.GetField(fieldMapping.JoinTableFieldName);
            if (fm == null || fj == null) continue;
            sub.fieldMappings.Add(new JoinDefinition.JoinFieldMapping(
                fm.DbFieldName, PreProcessFormula(entityOrmUtility, filterValues, fieldMapping.MainTableFormula),
                fj.DbFieldName, PreProcessFormula(subQueryDefinition.Query, filterValues, fieldMapping.JoinTableFormula)));
        }

        return sub;
    }

    internal void PreProcessQuery(LocalParameters filterValues)
    {
        var tableIndex = 0;
        PreProcessQuery(ref tableIndex, this, filterValues);
    }

    private static void PreProcessQuery(ref int tableIndex, QueryUtility queryDef, LocalParameters filterValues)
    {
        if (queryDef.Entity == null) return;
        queryDef.LocalParameters ??= [];
        queryDef.LocalParameters.Set(filterValues);
        queryDef.Provider.SetConnectionParams(queryDef.ConnectionValues, queryDef.LocalParameters);
        var dataSource = queryDef.DataSource;
        InitialDataSource(dataSource, tableIndex, queryDef, filterValues);
        PreProcessFields(dataSource, queryDef);
        PreProcessFormulaFields(dataSource, queryDef, filterValues);
        PreProcessGroupBy(dataSource, queryDef, filterValues);
        PreProcessOrderBy(dataSource, queryDef, filterValues);
        PreProcessFilters(dataSource, queryDef, filterValues);
        PreProcessHaving(dataSource, queryDef, filterValues);
        tableIndex = PreProcessJoins(tableIndex, queryDef, filterValues);
    }

    private static void InitialDataSource(DataSource dataSource, int tableIndex,
        QueryUtility queryDef, LocalParameters filterValues)
    {
        dataSource.Alias = new string((char)('A' + tableIndex % 32), 1 + tableIndex / 32);
        dataSource.FilterValues = [];
        dataSource.FilterValues.Set(filterValues);
        dataSource.Distinct = queryDef.bDistinct;
        dataSource.StartIndex = queryDef.startIndex;
        dataSource.TopRows = queryDef.topRows;
        dataSource.OnlyRecordCount = queryDef._onlyRecordCount;
        dataSource.Fields = [];
        dataSource.Outfields = [];
        dataSource.Formulas = [];
        dataSource.Aggregates = null;
        dataSource.OrderBys = null;
        dataSource.Filters = null;
        dataSource.HavingFilters = null;
        dataSource.SqlCommand = "";
    }

    private static void PreProcessFields(DataSource dataSource, QueryUtility queryDef)
    {
        if (queryDef.fields == null) return;
        foreach (var item in queryDef.fields.Values)
        {
            var ef = queryDef.Entity.GetField(item.fieldName);
            if (ef == null || ef.NotMap) continue;
            var col = new ColumnDefinition(ef)
            {
                fieldName = ef.DbFieldName,
                overFieldName = string.IsNullOrEmpty(item.overFieldName) ? ef.Id : item.overFieldName
            };
            if (dataSource.Fields.ContainsKey(col.fieldName))
                continue;
            dataSource.Fields.Add(col.fieldName, col);
            dataSource.AddOutField(col);
        }
    }

    private static void PreProcessFormulaFields(DataSource dataSource, QueryUtility queryDef,
        LocalParameters filterValues)
    {
        if (queryDef.formulaInfos == null) return;
        foreach (var item in queryDef.formulaInfos)
        {
            if (!dataSource.Outfields.ContainsKey(item.asFieldName))
            {
                var expFormula = PreProcessFormula(queryDef, filterValues, item.formula);
                dataSource.AddFormula(expFormula, item.asFieldName);
                var col = new ColumnDefinition(null/*todo*/)
                {
                    fieldName = item.asFieldName,
                    overFieldName = item.asFieldName,
                    formula_value = item.formula
                };
                dataSource.AddOutField(col);
            }
        }
    }

    private static void PreProcessGroupBy(DataSource dataSource, QueryUtility queryDef, LocalParameters filterValues)
    {
        if (queryDef.GroupBys == null) return;
        foreach (var item in queryDef.GroupBys)
        {
            if (item.formula != null)
            {
                var formula = PreProcessFormula(queryDef, filterValues, item.formula);
                dataSource.AddGroupBy(item.Field, item.fieldName, formula, item.function, item.aggregateScope, item.overFieldName);
                if (item.function != eAggregationFunctions.GroupByItem)
                {
                    var col = new ColumnDefinition(null/*null*/) { fieldName = item.overFieldName, overFieldName = item.overFieldName };
                    if (!dataSource.Outfields.ContainsKey(col.overFieldName))
                        dataSource.AddOutField(col);
                }
            }
            else
            {
                var ef = queryDef.Entity.GetField(item.fieldName);
                if (ef == null && item.fieldName != "*")
                    continue;
                var dbFieldName = ef != null ? ef.DbFieldName : item.fieldName == "*" ? "*" : "";
                if (string.IsNullOrEmpty(item.overFieldName))
                {
                    if (item.function != eAggregationFunctions.InColumn &&
                         item.function != eAggregationFunctions.GroupByItem)
                        item.overFieldName = item.function + "_" + item.fieldName; //todo change _ to $
                    else
                        item.overFieldName = item.fieldName;
                }

                if (item.function != eAggregationFunctions.GroupByItem)
                {
                    var col = new ColumnDefinition(ef) { fieldName = item.overFieldName, overFieldName = item.overFieldName };
                    if (!dataSource.Outfields.ContainsKey(col.overFieldName))
                        dataSource.AddOutField(col);
                }

                if (!string.IsNullOrEmpty(dbFieldName))
                    dataSource.AddGroupBy(ef, dbFieldName, ef?.Formula?.FormulaBody, item.function, item.aggregateScope,
                        item.overFieldName);
                else if (ef != null && ef.NotMap && ef.Formula?.FormulaBody != null)
                    dataSource.AddGroupBy(ef, dbFieldName, ef.Formula?.FormulaBody, item.function, item.aggregateScope,
                        item.overFieldName);
            }
        }
    }

    private static void PreProcessOrderBy(DataSource dataSource, QueryUtility queryDef, LocalParameters filterValues)
    {
        if (queryDef.OrderBys == null) return;
        foreach (var item in queryDef.OrderBys)
        {
            var ef = queryDef.Entity.GetField(item.fieldName);
            if (ef == null)
            {
                var orderbyFormula = PreProcessFormula(queryDef, filterValues, Parser.Parse(item.fieldName), false);
                dataSource.AddOrderBy(ef, item.orderIndex, orderbyFormula, item.order, item.orderById);
                continue;
            }

            if (!ef.NotMap)
            {
                dataSource.AddOrderBy(ef, item.orderIndex, ef.DbFieldName, item.order, item.orderById);
                continue;
            }

            if (ef.Formula?.FormulaBody != null)
            {
                var ff = queryDef.formulaInfos?.FirstOrDefault(f => f.asFieldName == ef.Id);
                if (ff != null)
                    dataSource.AddOrderBy(ef, item.orderIndex, ef.Id, item.order, item.orderById);
                //else todo add formula as sort dataSource.addOrderBy(ef.DBFieldName, item.order);
                continue;
            }

            if (ef.AssociationEntity?.Maps != null)
            {
                var itemOrderSubIndex = item.orderIndex;
                if (!item.orderById && ef.AssociationEntity.Entity()?.DisplayStrings != null && !ef.AssociationEntity.Entity().DontSync)
                {
                    var ojn = queryDef.LeftOuterJoin(ef);
                    foreach (var bf in ef.AssociationEntity.Entity().DisplayStrings)
                    {
                        var bef = ef.AssociationEntity.Entity().GetField(bf.FieldId);
                        if (bef == null) continue;
                        //ojn.Query.OrderBy(bef.Id, item.order);
                        dataSource.AddOrderBy(null, itemOrderSubIndex++,
                            $"{EntityDbNameManager.GetTableDbFullName(ef.AssociationEntity.Entity())}.[{bef.DbFieldName}]",
                            item.order);
                        if (queryDef.GroupBys != null) //ToDo
                            ojn.Query.GroupBy(bef.Id, eAggregationFunctions.GroupByItem);
                    }

                    continue;
                }

                foreach (var map in ef.AssociationEntity.Maps)
                {
                    var mef = queryDef.Entity.GetField(map.SourceField);
                    if (mef == null) continue;
                    dataSource.AddOrderBy(mef, itemOrderSubIndex++, mef.DbFieldName, item.order, item.orderById);
                    if (queryDef.GroupBys != null) //ToDo
                        queryDef.GroupBy(mef.DbFieldName, eAggregationFunctions.GroupByItem);
                }
            }
        }
    }

    private static void PreProcessFilters(DataSource dataSource, QueryUtility queryDef,
        LocalParameters filterValues)
    {
        foreach (var item in queryDef.filters ?? Enumerable.Empty<FilterDefinition>())
        {
            if (ExpressionIsTrue(queryDef, item, filterValues, out ExpressionNode expFilter))
                continue;
            dataSource.AddFilter(expFilter, filterValues, item.OrGroupId);
        }
    }

    private static void PreProcessHaving(DataSource dataSource, QueryUtility queryDef,
        LocalParameters filterValues)
    {
        foreach (var item in queryDef.havingFilters ?? Enumerable.Empty<FilterDefinition>())
        {
            if (ExpressionIsTrue(queryDef, item, filterValues, out ExpressionNode expFilter))
                continue;
            dataSource.AddHaving(expFilter, filterValues, item.OrGroupId);
        }
    }

    private static bool ExpressionIsTrue(QueryUtility queryDef, FilterDefinition item, LocalParameters filterValues,
        out ExpressionNode expFilter)
    {
        expFilter = item.FilterExpression.clone();
        expFilter = expFilter.replace(filterValues);
        if (ExpressionIsTrue(expFilter))
            return true;
        expFilter = PreProcessFormula(queryDef, filterValues, expFilter, false);
        if (ExpressionIsTrue(expFilter))
            return true;
        return false;
    }

    private static bool ExpressionIsTrue(ExpressionNode expFilter)
    {
        var val = (expFilter as ConstantExpressionNode)?.Value;
        return val is bool b && b;
    }

    private static int PreProcessJoins(int tableIndex, QueryUtility queryDef,
        LocalParameters filterValues)
    {
        if (queryDef.subQuerys == null) return tableIndex;
        queryDef.DataSource.SubTables = [];
        for (var i = 0; i < queryDef.subQuerys.Values.Count; i++)
        {
            var subQueryDefinition = queryDef.subQuerys.Values.ToArray()[i];
            foreach (var fieldMapping in subQueryDefinition.fieldMappings)
            {
                var fm = queryDef.Entity.GetField(fieldMapping.MainTableFieldName);
                var fj = subQueryDefinition.Query.Entity.GetField(fieldMapping.JoinTableFieldName);
                if (fm == null || fj == null) continue;
                PreProcessFormula(queryDef, filterValues, fieldMapping.MainTableFormula);
                PreProcessFormula(subQueryDefinition.Query, filterValues, fieldMapping.JoinTableFormula);
            }
        }

        for (var i = 0; i < queryDef.subQuerys.Values.Count; i++)
        {
            var subQueryDefinition = queryDef.subQuerys.Values.ToArray()[i];
            var sub = NewSubDataSource(queryDef, filterValues, subQueryDefinition);
            tableIndex++;
            PreProcessQuery(ref tableIndex, subQueryDefinition.Query, filterValues);
            queryDef.DataSource.SubTables.Add(subQueryDefinition.Key, sub);
        }
        return tableIndex;
    }

    private static ExpressionNode PreProcessFormula(EntityConnection expressionReference,
        LocalParameters filterValues, ExpressionNode formulaExp, bool clone = true)
    {
        if (formulaExp == null) return null;
        if (clone) formulaExp = formulaExp.clone();
        formulaExp = formulaExp.replace(filterValues);
        formulaExp = formulaExp.processAndReplace(EntityConversions.ConvertFormulaFields, expressionReference);
        formulaExp = formulaExp.processAndReplace(EntityConversions.ConvertNames, expressionReference);
        formulaExp = formulaExp.replace(filterValues);
        return formulaExp;
    }
}
