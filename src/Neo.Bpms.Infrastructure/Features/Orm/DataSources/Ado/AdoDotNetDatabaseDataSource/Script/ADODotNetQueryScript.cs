namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    public override string Name => $"[{DataSrcDefinition.name}]";
    protected virtual string SetNoCountOn { get; } = "";

    private string GenerateQuery(Connection dbConnection, DataSource dataSource, bool bRoot)
    {
        ClearExtractedParentEntities(dataSource);
        NeoStringBuilder sqlOrderBy = new();
        NeoStringBuilder sqlGroupBy = new();
        bool beGenerateGroupBy = CheckMustBeGenerateGroupBy(dataSource);
        if (beGenerateGroupBy)
        {
            GenerateQuery_FetchGroupBy(dataSource, ref sqlGroupBy);
        }

        GenerateQuery_FetchOrderBys(dataSource, ref sqlOrderBy);

        NeoStringBuilder sql = new();
        GenerateQuery_WriteSelect(dataSource, ref sql);

        bool isFirstColumn = true;
        GenerateQuery_WriteColumns(dataSource, beGenerateGroupBy, ref sql, ref isFirstColumn, false);

        GenerateQuery_WriteTopRowColumn(dataSource, sqlOrderBy, ref sql, isFirstColumn);

        bool writeFrom = false;
        const bool writeJoinsInFrom = false;
        Dictionary<string, string> tables = [];
        GenerateQuery_WriteFrom(dataSource, ref sql, ref writeFrom, writeJoinsInFrom, ref tables);
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (!writeJoinsInFrom)
        {
            GenerateQuery_WriteJoins(dataSource, ref sql, ref tables);
        }

        GenerateQuery_WriteFilter(dataSource, ref sql, writeJoinsInFrom, "\nWHERE ");

        GenerateQuery_WriteGroupBy(dataSource, sqlGroupBy, beGenerateGroupBy, ref sql);

        GenerateQuery_Unions(dbConnection, dataSource, ref sql);

        if (!dataSource.OnlyRecordCount &&
            dataSource.HasPaging && sqlOrderBy.Length == 0)
        {
            GenerateQuery_FetchAdditionOrderBy(dataSource, ref sqlOrderBy, sqlGroupBy, beGenerateGroupBy);
        }

        GenerateQuery_WriteOrderBy(dataSource, bRoot, sqlOrderBy, ref sql);

        if (!dataSource.OnlyRecordCount && dataSource.HasPaging &&
            PagingVersion == PageingVersion.OffsetRows && sqlOrderBy.Length != 0)
        {
            sql += "\nOFFSET " + dataSource.StartIndex + " ROWS FETCH NEXT " + dataSource.TopRows + " ROWS ONLY";
        }

        if (dataSource.OnlyRecordCount && bRoot)
        {
            string s = sql.ToString();
            if (SetNoCountOn.Length > 0)
            {
                s = s[SetNoCountOn.Length..];
            }

            return $"{SetNoCountOn} SELECT Count(*) recordCount FROM (\n{s}\n) __SJVS";
        }

        return sql.ToString();
    }

    private static void ClearExtractedParentEntities(DataSource dataSource)
    {
        dataSource.ExtractedParentEntities = null;
        if (dataSource.SubTables == null)
        {
            return;
        }

        foreach (SubDataSource subTable in dataSource.SubTables.Values)
        {
            ClearExtractedParentEntities(subTable.dataSource);
        }
    }

    private static void GenerateQuery_WriteFilter(DataSource dataSource, ref NeoStringBuilder sql,
        bool writeJoinsInFrom, string prefix)
    {
        FilterItems filterItems = new();
        GenerateQuery_WriteFilters(dataSource, filterItems, writeJoinsInFrom);
        GenerateQuery_FilterItems(ref sql, filterItems, prefix);
    }

    private static void GenerateQuery_WriteGroupBy(DataSource dataSource, NeoStringBuilder sqlGroupBy,
        bool bMustGenerateGroupBy, ref NeoStringBuilder sql)
    {
        if (!bMustGenerateGroupBy)
        {
            return;
        }

        if (sqlGroupBy.Length > 0)
        {
            sql += "\nGROUP BY " + sqlGroupBy;
        }

        FilterItems havingItems = new();
        GenerateQuery_WriteGroupByHaving(dataSource, havingItems);
        GenerateQuery_FilterItems(ref sql, havingItems, "\n HAVING ");
    }

    private static void GenerateQuery_FilterItems(ref NeoStringBuilder sql, FilterItems havingItems, string prefix)
    {
        if (!havingItems.Any())
        {
            return;
        }

        sql += prefix;
        sql += string.Join(" And ", havingItems.Select(havingItem => "(" + (
            havingItem.Key == ""
                ? string.Join(" And ",
                    havingItem.Value.Select(filter =>
                        $"({filter})"))
                : string.Join(" OR ",
                    havingItem.Value.Select(filter =>
                        $"({filter})"))
        ) + ")"));
    }

    private void GenerateQuery_WriteSelect(DataSource dataSource,
        ref NeoStringBuilder sql)
    {
        if (!dataSource.OnlyRecordCount &&
            dataSource.HasPaging)
        {
            if (SupportTopRowsInQueryCommands())
            {
                sql += SetNoCountOn;
                if (PagingVersion == PageingVersion.TopRows)
                {
                    sql += "SELECT * FROM ( ";
                }

                sql += "SELECT ";
                if (dataSource.Distinct)
                {
                    sql += "DISTINCT ";
                }

                if (PagingVersion == PageingVersion.TopRows)
                {
                    if (dataSource.TopRows > 0)
                    {
                        sql += "TOP (" + (dataSource.StartIndex + dataSource.TopRows) + ") ";
                    }
                }
                return;
            }
        }

        if (sql.Length != 0)
        {
            return;
        }

        sql += $"{SetNoCountOn} SELECT ";
        if (dataSource.Distinct)
        {
            sql += "DISTINCT ";
        }

        if (!SupportTopRowsInQueryCommands())
        {
            return;
        }

        if (PagingVersion != PageingVersion.TopRows)
        {
            return;
        }

        if (dataSource.TopRows > 0)
        {
            sql += "TOP (" + dataSource.TopRows + ") ";
        }
        else
        {
            sql += "TOP 1000000 "; //todo
        }
    }

    protected virtual bool SupportTopRowsInQueryCommands()
    {
        return true;
    }

    private static void GenerateQuery_WriteTopRowColumn(DataSource dataSource,
        NeoStringBuilder sqlOrderBy, ref NeoStringBuilder sql, bool firstColumn)
    {
        if (dataSource.OnlyRecordCount || (dataSource.TopRows <= 0 && dataSource.StartIndex <= 0))
        {
            return;
        }

        if (PagingVersion != PageingVersion.TopRows)
        {
            return;
        }

        if (!firstColumn)
        {
            sql += ",\n";
        }

        sql += " ROW_NUMBER() OVER ( " + sqlOrderBy + ") as 'Row_Number' ";
    }

    private void GenerateQuery_WriteOrderBy(DataSource dataSource, bool bRoot,
        NeoStringBuilder sqlOrderBy, ref NeoStringBuilder sql)
    {
        if (dataSource.OnlyRecordCount || !bRoot)
        {
            return;
        }

        if (dataSource.HasPaging && sqlOrderBy.Length > 0)
        {
            if (SupportTopRowsInQueryCommands()
                && PagingVersion == PageingVersion.TopRows)
            {
                sql += "\n) as tbl WHERE tbl.Row_Number >= " + (dataSource.StartIndex + 1);
                if (dataSource.TopRows > 0)
                {
                    sql += " AND tbl.Row_Number <= " + (dataSource.StartIndex + dataSource.TopRows);
                }

                sql += "\nOrder By Row_Number";
            }
            else
            {
                sql += sqlOrderBy;
            }
        }
        else
        {
            sql += sqlOrderBy;
        }
    }

    private void GenerateQuery_Unions(Connection dbConnection, DataSource dataSource, ref NeoStringBuilder sql)
    {
        foreach (SubDataSource sub in dataSource.SubTables?.Values ?? Enumerable.Empty<SubDataSource>())
        {
            if (sub.joinType == eJoinType.Union)
            {
                sql += GenerateQuery(dbConnection, sub.dataSource, false);
            }
        }
    }

    private void GenerateQuery_FetchAdditionOrderBy(DataSource dataSource,
        ref NeoStringBuilder sqlOrderBy, NeoStringBuilder sqlGroupBy, bool bMustGenerateGroupBy)
    {
        GenerateOrderByOfGroupBy(dataSource, ref sqlOrderBy);
        if (sqlOrderBy.Length == 0 && bMustGenerateGroupBy && sqlGroupBy.Length != 0)
        {
            sqlOrderBy += "\nORDER BY " + sqlGroupBy;
        }

        if (sqlOrderBy.Length != 0)
        {
            return;
        }

        if (bMustGenerateGroupBy)
        {
            return;
        }

        if (DataSrcDefinition.Entity.KeyFields != null)
        {
            foreach (EntityField field in DataSrcDefinition.Entity.KeyFields)
            {
                if (sqlOrderBy.Length == 0)
                {
                    sqlOrderBy += "\nORDER BY ";
                }

                sqlOrderBy += SqlFieldName(dataSource, field, field.DbFieldName);
                break;
            }
        }
        else
        {
            EntityField field = DataSrcDefinition.Entity.MappedEntityFields.FirstOrDefault(f => f.CSharpType == typeof(DateTime));
            if (field == null)
            {
                return;
            }

            if (sqlOrderBy.Length == 0)
            {
                sqlOrderBy += "\nORDER BY ";
            }

            sqlOrderBy += SqlFieldName(dataSource, field, field.DbFieldName);
        }
    }

    private static void GenerateOrderByOfGroupBy(DataSource dataSource, ref NeoStringBuilder sqlOrderBy)
    {
        foreach (AggregateDefinition gItem in dataSource.Aggregates?.Where(g => g.function == eAggregationFunctions.GroupByItem) ??
                              [])
        {
            sqlOrderBy += sqlOrderBy.Length == 0 ? "\nORDER BY " : ",";
            sqlOrderBy += FetchFieldOrFormula(dataSource, gItem);
        }

        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            GenerateOrderByOfGroupBy(sub.dataSource, ref sqlOrderBy);
        }
    }

    public override void GenerateQuery_WriteJoins(DataSource dataSource, ref NeoStringBuilder sql,
        ref Dictionary<string, string> tables)//todo no need ref
    {
        GenerateQuery_WriteParentJoins(dataSource, sql, tables);
        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            string tableName = GetTableDbName(sub.dataSource.DataSrcDefinition);
            if (tables.TryAdd(tableName, tableName))
            {
                sql += "\n";
                switch (sub.joinType)
                {
                    case eJoinType.InnerJoin:
                        sql += "join";
                        break;
                    case eJoinType.LeftOuterJoin:
                        sql += "left join";
                        break;
                    case eJoinType.RightOuterJoin:
                        sql += "right join";
                        break;
                }

                sql += " " + tableName + " with(nolock)";
                if (sub.fieldMappings != null)
                {
                    bool writeOn = false;
                    foreach (JoinDefinition.JoinFieldMapping item in sub.fieldMappings)
                    {
                        sql += !writeOn ? " ON " : " And ";
                        sql += FetchMappingSql(dataSource, sub, item);
                        writeOn = true;
                    }
                }
            }

            GenerateQuery_WriteJoins(sub.dataSource, ref sql, ref tables);
        }
    }

    private void GenerateQuery_WriteParentJoins(DataSource dataSource, NeoStringBuilder sql,
        IDictionary<string, string> tables)
    {
        if (dataSource.ExtractedParentEntities == null)
        {
            return;
        }

        foreach (ExtractedParentEntity parentEntity in dataSource.ExtractedParentEntities.Values)
        {
            string key = parentEntity.TableName;
            if (tables.ContainsKey(key))
            {
                continue;
            }

            tables.Add(key, key);
            sql += $"\nJOIN {key}";
            bool writeOn = false;
            foreach (EntityRelationMap item in parentEntity.ParentEntityRelation.Maps)
            {
                sql += !writeOn ? " ON " : " And ";
                sql += FetchMappingSqlOfParent(dataSource, parentEntity, item);
                writeOn = true;
            }
        }
    }

    private static string FetchFieldOrFormula(DataSource dataSource, IContainsFormula gItem)
    {
        return FetchFieldOrFormula(dataSource, gItem.fieldName, gItem.formula, "0", gItem.Field);
    }

    private static string FetchFieldOrFormula(DataSource dataSource,
        string fieldId, ExpressionNode formulaExp, string errorInFormulaTxt = "'Error In Formula'",
        EntityField field = null)
    {
        string sql;
        if (formulaExp != null)
        {
            sql = ConvertExpressionToScript(dataSource, formulaExp, errorInFormulaTxt);
        }
        else
        {
            field ??= dataSource.DataSrcDefinition.Entity.GetFieldByDbName(fieldId);
            sql = SqlFieldName(dataSource, field, fieldId);
        }

        return sql;
    }

    private static string ConvertExpressionToScript(DataSource dataSource, ExpressionNode formulaExp,
        string errorInFormulaTxt)
    {
        string sql = dataSource.ConvertExpressionToScript(true, formulaExp,
                out string formula, out ExpressionNode outFormula, dataSource.FilterValues) &&
            outFormula == null
            ? $"({formula})"
            : $"({errorInFormulaTxt})";
        return sql;
    }

    private static void GenerateQuery_WriteGroupByHaving(DataSource dataSource, FilterItems havingItems)
    {
        FetchFilterItems(havingItems, dataSource.HavingFilters);
        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            GenerateQuery_WriteGroupByHaving(sub.dataSource, havingItems);
        }
    }

    private static void FetchFilterItems(FilterItems havingItems, IEnumerable<FilterDefinition> havingFilters)
    {
        foreach (FilterDefinition filterDefinition in havingFilters ?? [])
        {
            AddFilterItem(havingItems, filterDefinition.OrGroupId ?? "", filterDefinition.Filter);
        }
    }

    private static void AddFilterItem(FilterItems havingItems, string key, string filter)
    {
        if (!havingItems.TryGetValue(key, out List<string> havingItemList))
        {
            havingItemList = [];
            havingItems.Add(key, havingItemList);
        }

        havingItemList.Add(filter);
    }

    private static void GenerateQuery_FetchGroupBy(DataSource dataSource, ref NeoStringBuilder sqlGroupBy)
    {
        if (dataSource.Aggregates != null)
        {
            foreach (AggregateDefinition gItem in dataSource.Aggregates)
            {
                if (gItem.function != eAggregationFunctions.GroupByItem)
                {
                    continue;
                }

                sqlGroupBy += sqlGroupBy.Length == 0 ? "" : ",";
                sqlGroupBy += FetchFieldOrFormula(dataSource, gItem);
            }
        }

        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            GenerateQuery_FetchGroupBy(sub.dataSource, ref sqlGroupBy);
        }
    }

    private static bool CheckMustBeGenerateGroupBy(DataSource dataSource)
    {
        return dataSource.Aggregates != null && dataSource.Aggregates.Count != 0 || (dataSource.SubTables?.Values.Any(g => g.isJoin() && CheckMustBeGenerateGroupBy(g.dataSource)) ?? false);
    }

    private static void GenerateQuery_WriteColumns(DataSource dataSource,
        bool beGenerateGroupBy, ref NeoStringBuilder sql, ref bool firstColumn, bool bRoot)
    {
        if (dataSource.OnlyRecordCount && !dataSource.Distinct)
        {
            sql += "1 sjvs ";
            return;
        }

        if (beGenerateGroupBy)
        {
            GenerateQuery_WriteAggregates(dataSource, ref sql, ref firstColumn);
        }
        else
        {
            GenerateQuery_WriteFields(dataSource, ref sql, ref firstColumn);
            GenerateQuery_WriteFormulaColumns(dataSource, ref sql, ref firstColumn);
        }

        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            GenerateQuery_WriteColumns(sub.dataSource, beGenerateGroupBy, ref sql, ref firstColumn, false);
        }

        if (!firstColumn || !bRoot)
        {
            return;
        }

        sql += " " + dataSource.Name + ".* ";
        firstColumn = false;
    }

    private static void GenerateQuery_WriteAggregates(DataSource dataSource, ref NeoStringBuilder sql,
        ref bool firstColumn)
    {
        foreach (AggregateDefinition gItem in dataSource.Aggregates ?? Enumerable.Empty<AggregateDefinition>())
        {
            if (gItem.function == eAggregationFunctions.GroupByItem)
            {
                continue;
            }

            if (!firstColumn)
            {
                sql += ",\n";
            }

            switch (gItem.function)
            {
                case eAggregationFunctions.Sum:
                    sql += "SUM(";
                    break;
                case eAggregationFunctions.Avg:
                    sql += "AVG(";
                    break;
                case eAggregationFunctions.Min:
                    sql += "MIN(";
                    break;
                case eAggregationFunctions.Max:
                    sql += "MAX(";
                    break;
                case eAggregationFunctions.First:
                    sql += "MIN("; /*"FIRST("*/
                    break;
                case eAggregationFunctions.Last:
                    sql += "MAX("; /*"LAST("*/
                    break;
                case eAggregationFunctions.Count:
                    sql += "COUNT(";
                    break;
                case eAggregationFunctions.StDev:
                    sql += "STDEV(";
                    break;
                case eAggregationFunctions.StDevP:
                    sql += "STDEVP(";
                    break;
                case eAggregationFunctions.Var:
                    sql += "VAR(";
                    break;
                case eAggregationFunctions.VarP:
                    sql += "VARP(";
                    break;
                case eAggregationFunctions.CHECKSUM_AGG:
                    sql += "CHECKSUM_AGG(";
                    break;
                case eAggregationFunctions.GROUPING:
                    sql += "GROUPING(";
                    break;
                    //case eAggregationFunctions.InColumn://no function
            }

            if (gItem.aggregateScope == eAggregateScope.Distinct)
            {
                sql += "Distinct ";
            }

            sql += FetchFieldOrFormula(dataSource, gItem);
            if (gItem.function is not eAggregationFunctions.InColumn and not eAggregationFunctions.AggregationFormula)
            {
                sql += ")";
            }

            if (!string.IsNullOrEmpty(gItem.overFieldName))
            {
                sql += " [" + gItem.overFieldName + "]";
            }

            firstColumn = false;
        }
    }

    private static void GenerateQuery_WriteFields(DataSource dataSource,
        ref NeoStringBuilder sql, ref bool firstColumn)
    {
        foreach (ColumnDefinition item in dataSource.Fields?.Values ?? Enumerable.Empty<ColumnDefinition>())
        {
            if (!firstColumn)
            {
                sql += ",\n";
            }

            sql += SqlFieldName(dataSource, item) + " ";
            if (!string.IsNullOrEmpty(item.overFieldName))
            {
                sql += $"[{item.overFieldName}]";
            }

            firstColumn = false;
        }
    }

    private static string SqlFieldName(DataSource dataSource, BaseFieldDefinition item)
    {
        return SqlFieldName(dataSource, item.Field, item.fieldName);
    }

    private static string SqlFieldName(DataSource dataSource, EntityField field, string dbFieldName)
    {
        string sql = "";
        if (dbFieldName != "*")
        {
            sql += $"{ExtractTableName(dataSource, field)}.[";
        }

        sql += dbFieldName;
        if (dbFieldName != "*")
        {
            sql += "]";
        }

        return sql;
    }

    private static string ExtractTableName(DataSource dataSource, EntityField field)
    {
        if (field == null)
        {
            return dataSource.Name;
        }

        ReferenceField referenceField = field.ReferenceFields?.FirstOrDefault(r => r.Relationship is ParentEntity);
        if (referenceField?.Relationship == null ||
            referenceField.Relationship is not ParentEntity parentEntityRelation)
        {
            return GetTableDbName(dataSource.DataSrcDefinition);
        }

        dataSource.ExtractedParentEntities ??= [];

        string key = parentEntityRelation.ReferEntityKey();
        if (!dataSource.ExtractedParentEntities.TryGetValue(key, out ExtractedParentEntity p))
        {
            p = new ExtractedParentEntity
            {
                ParentEntityRelation = parentEntityRelation,
                TableName = GetTableDbName(parentEntityRelation.DestEntity, dataSource.DataSrcDefinition.connection.DatabaseName)
            };
            dataSource.ExtractedParentEntities.Add(key, p);
        }

        return p.TableName;
    }

    private static void GenerateQuery_WriteFormulaColumns(DataSource dataSource,
        ref NeoStringBuilder sql, ref bool firstColumn)
    {
        if (dataSource.Formulas == null)
        {
            return;
        }

        foreach (FormulaDefinition item in dataSource.Formulas)
        {
            if (!firstColumn)
            {
                sql += ",\n";
            }

            sql += ConvertExpressionToScript(dataSource, item.formula, "null");
            if (!string.IsNullOrEmpty(item.asFieldName))
            {
                sql += $" [{item.asFieldName}]";
            }

            firstColumn = false;
        }
    }

    private static void GenerateQuery_WriteFrom(DataSource dataSource,
        ref NeoStringBuilder sql, ref bool writeFrom,
        bool includeJoins, ref Dictionary<string, string> tables)
    {
        string tableName = GetTableDbName(dataSource.DataSrcDefinition);
        sql += !writeFrom ? "\nFROM " : ",";
        writeFrom = true;
        if (tables.TryAdd(tableName, tableName))
        {
            sql += tableName + " with(nolock) ";
        }
        else
        {
            sql += tableName + " " + dataSource.Name;
        }

        if (!includeJoins)
        {
            return;
        }

        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            GenerateQuery_WriteFrom(sub.dataSource, ref sql, ref writeFrom, true, ref tables);
        }
    }

    private static string GetTableDbName(DataSourceDefinition definition)
    {
        return GetTableDbName(definition.Entity, definition.connection.DatabaseName);
    }
    private static string GetTableDbName(Entity entity, string databaseName)
    {
        return $"[{databaseName}].{EntityDbNameManager.GetTableDbFullName(entity)}";
    }

    private static void GenerateQuery_WriteFilters(DataSource dataSource, FilterItems havingItems, bool writeJoinsInFrom)
    {
        FetchFilterItems(havingItems, dataSource.Filters);
        foreach (SubDataSource sub in dataSource.SubTables?.Values.Where(g => g.isJoin()) ?? [])
        {
            if (writeJoinsInFrom)
            {
                foreach (JoinDefinition.JoinFieldMapping item in sub.fieldMappings)
                {
                    AddFilterItem(havingItems, "", FetchMappingSql(dataSource, sub, item));
                }
            }

            GenerateQuery_WriteFilters(sub.dataSource, havingItems, writeJoinsInFrom);
        }
    }

    private static string FetchMappingSql(DataSource dataSource, SubDataSource sub,
        JoinDefinition.JoinFieldMapping item)
    {
        string joinSql = FetchFieldOrFormula(sub.dataSource, item.JoinTableFieldName, item.JoinTableFormula);
        string mainSql = FetchFieldOrFormula(dataSource, item.MainTableFieldName, item.MainTableFormula);
        return $"({joinSql}={mainSql})";
    }

    private static string FetchMappingSqlOfParent(DataSource dataSource, ExtractedParentEntity parentEntity, EntityRelationMap item)
    {
        EntityField srcField = dataSource.DataSrcDefinition.Entity.GetField(item.SourceField);
        EntityField destField = parentEntity.ParentEntityRelation.GetField(item.DestField);
        string joinSql = $"{parentEntity.TableName}.[{destField?.DbFieldName ?? item.DestField}]";
        string mainSql = SqlFieldName(dataSource, srcField, srcField?.DbFieldName ?? item.SourceField);
        return $"({joinSql}={mainSql})";
    }

    private static void GenerateQuery_FetchOrderByItems(List<OrderByItem> orders, DataSource dataSource)
    {
        if (dataSource.OrderBys != null)
        {
            orders.AddRange(dataSource.OrderBys.Select(orderby =>
                new OrderByItem
                {
                    OrderBy = orderby,
                    DataSource = dataSource
                }));
        }

        foreach (SubDataSource sub in dataSource.SubTables?.Values ?? Enumerable.Empty<SubDataSource>())
        {
            if (sub.joinType == eJoinType.Union ||
                sub.isJoin() ||
                sub.joinType == eJoinType.Concatennate)
            {
                GenerateQuery_FetchOrderByItems(orders, sub.dataSource);
            }
        }
    }

    private static void GenerateQuery_FetchOrderBys(DataSource dataSource, ref NeoStringBuilder sqlOrderBy)
    {
        List<OrderByItem> orders = [];
        GenerateQuery_FetchOrderByItems(orders, dataSource);
        foreach (OrderByItem orderByItem in orders.OrderBy(o => o.OrderBy.orderIndex))
        {
            GenerateQuery_FetchOrderBy(orderByItem.DataSource, orderByItem.OrderBy, ref sqlOrderBy);
        }
    }

    private static void GenerateQuery_FetchOrderBy(DataSource dataSource,
        OrderByDefinition orderBy, ref NeoStringBuilder sqlOrderBy)
    {
        sqlOrderBy += sqlOrderBy.Length == 0 ? "\nORDER BY " : ",";

        if (orderBy.formula != null)
        {
            sqlOrderBy += ConvertExpressionToScript(dataSource, orderBy.formula, "'Error In Formula'");
        }
        else
        {
            sqlOrderBy += orderBy.Field != null
                ? SqlFieldName(dataSource, orderBy)
                : orderBy.fieldName;
        }

        if (orderBy.order == SortType.Descending)
        {
            sqlOrderBy += " DESC";
        }
    }

    private class OrderByItem
    {
        public OrderByDefinition OrderBy;
        public DataSource DataSource;
    }

    private class FilterItems : Dictionary<string, List<string>>
    {
    }
}
