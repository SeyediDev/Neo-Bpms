namespace Neo.Bpms.Engine.Data.ADODotNet;

/// <summary>
/// The ado dot net database data source that implement dml functions.
/// </summary>
public abstract partial class AdoDotNetDatabaseDataSource
{
    public static string GetSqlValue(object value, EntityField field, Type type, bool exactValue = false)
    {
        while (true)
        {
            if (value is ElasticObject elasticObject)
            {
                value = elasticObject.InternalValue;
                continue;
            }

            break;
        }

        if (value == null) return "null";
        if (value is bool b) return b ? "1" : "0";
        if (type?.IsEnum ?? false)
        {
            return Convert.ToInt32(value).ToString();
        }

        if (value.GetType().IsEnum)
        {
            return Convert.ToInt32(value).ToString();
        }

        if (value is string && type == typeof(DateTime))
        {
            var str = value.ToString();
            var injectionKey = GetInnerInjectionKey();
            try
            {
                if (str == "undefined") return "null";
                if (injectionKey != null && str.Length >= injectionKey.Length &&
                    str[..injectionKey.Length].ToUpper() == injectionKey)
                    str = str[injectionKey.Length..];
                value = Convert.ToDateTime(str);
            }
            catch
            {
                str = value.ToString();
                if (injectionKey != null && str.Length >= injectionKey.Length &&
                    str[..injectionKey.Length].ToUpper() == injectionKey)
                    return str[injectionKey.Length..];
                return "null";
            }
        }

        if (value is TimeSpan timeSpan)
        {
            return "" + timeSpan.Ticks;
        }

        if (value is DateTime || type == typeof(DateTime))
        {
            if (value.ToString() == "undefined") return "null";
            var dt = (DateTime)value;
            if (dt == DateTime.MinValue)
                return "null";
            return "'" + dt.Year + "/" + dt.Month + "/" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second +
                   "'";
        }

        if (value is byte[])
        {
            return "'" + value + "'";
        }

        // ReSharper disable PossibleMultipleEnumeration
        if (value is IEnumerable<string> enumerableString)
        {
            return !enumerableString.Any()
                ? "null"
                : string.Join(",",
                    enumerableString.Select(i => GetSqlStringValue(i, field, typeof(string), exactValue)));
        }

        if (value is IEnumerable<long> enumerableLong)
        {
            return !enumerableLong.Any() ? "null" : string.Join(",", enumerableLong);
        }

        if (value is IEnumerable<object> enumerableObject)
        {
            return !enumerableObject.Any() ? "null" : string.Join(",", enumerableObject);
        }
        // ReSharper restore PossibleMultipleEnumeration

        if (value is string)
        {
            return GetSqlStringValue(value, field, type, exactValue);
        }

        var s = value.ToString();
        if (field != null && field.FieldType == TVariableTypes.String && field.MaxLen > 0)
            s = s.Limit(field.MaxLen);
        if (string.IsNullOrEmpty(s)) return "''";
        if (type == typeof(string))
            return exactValue ? s : "N'" + s + "'";
        if (type == typeof(bool))
            return ConvUtill.ToBoolean(value) ? "1" : "0";
        return s;
    }

    private SqlValueField GetSqlValueField(bool useInInsert, ColumnDefinition fld, Entity entity)
    {
        var field = fld.Field ??
                    entity.GetFieldByDbName(fld.fieldName)
                    ?? entity.GetField(fld.overFieldName);
        if (field == null)
            _fieldInfos?.TryGetValue(fld.overFieldName, out field);
        var sqlValue =
            useInInsert && fld.fieldName == "AutoAudit_RowVersion"//V1
                ? "1"
                : GetSqlValue(fld.formula_value, field, field?.CSharpType);
        return new SqlValueField(field)
        {
            FieldId = fld.fieldName,
            Value = fld.formula_value,
            SqlValue = sqlValue
        };
    }

    private static string GetSqlStringValue(object value, EntityField field, Type type, bool exactValue)
    {
        var str = value.ToString();
        var injectionKey = GetInnerInjectionKey();
        if (injectionKey != null && str.Length >= injectionKey.Length &&
            str[..injectionKey.Length].ToUpper() == injectionKey)
            return str[injectionKey.Length..];
        if (type == typeof(long) || type == typeof(double) || type == typeof(bool))
        {
            if (str == "undefined") return "null";
            if (type == typeof(bool))
            {
                return ConvUtill.ToBoolean(value) ? "1" : "0";
            }

            if (str.Length == 0)
                return "null";
            if (type == typeof(long))
            {
                return long.TryParse(str, out var lv) ? lv.ToString(CultureInfo.InvariantCulture) : str;
            }

            if (type == typeof(double))
            {
                return double.TryParse(str, out var d) ? d.ToString(CultureInfo.InvariantCulture) : str;
            }
        }

        if (string.IsNullOrEmpty(str) || str == "{null}")
            return "null";
        if (field != null && field.FieldType == TVariableTypes.String && field.MaxLen > 0)
            str = str.Limit(field.MaxLen);
        return exactValue
            ? str
            : "N'" + str.Replace("\'", "\'\'") + "'"; // Note N should be added only when column type is an nvarchar
    }

    private static void FetchAggregateAndFields(bool beGenerateGroupBy, DataSource dataSource,
        List<string> insertFields)
    {
        if (beGenerateGroupBy)
        {
            if (dataSource.Aggregates != null)
                insertFields.AddRange(dataSource.Aggregates.Where(g => g.function != eAggregationFunctions.GroupByItem)
                    .Select(qf => qf.overFieldName ?? qf.fieldName));
        }
        else
        {
            if (dataSource.Fields != null)
                insertFields.AddRange(dataSource.Fields.Values.Select(qf => qf.overFieldName ?? qf.fieldName));
        }

        if (dataSource.SubTables != null)
            foreach (var sub in dataSource.SubTables.Values)
            {
                FetchAggregateAndFields(beGenerateGroupBy, sub.dataSource, insertFields);
            }
    }

    private string GenerateDmlCommandWhereClause(string keyFilterValues, Entity entity)
    {
        var commandString = new CandoStringBuilder();
        if ((SubTables?.Count ?? 0) > 0)
        {
            var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
            commandString += $" FROM {dbTableName} ";
            var tables = new Dictionary<string, string>();
            GenerateQuery_WriteJoins(this, ref commandString, ref tables);
        }

        var filter = GetFilterSql();
        if (string.IsNullOrEmpty(filter))
            filter = keyFilterValues;
        else if (!string.IsNullOrEmpty(keyFilterValues))
            filter = $"({filter}) And ({keyFilterValues})";
        if (!string.IsNullOrEmpty(filter))
            commandString += $" WHERE {filter};";
        return commandString.ToString();
    }

    private string GetFilterSql()
    {
        var filterSql = new CandoStringBuilder();
        GenerateQuery_WriteFilter(this, ref filterSql, false, "");
        return filterSql.ToString();
    }
}
