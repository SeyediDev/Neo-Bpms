namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    private string GenerateSelectScript(bool forWhere, FunctionInvocationExpressionNode function,
        LocalParameters localParameters)
    {
        var argIndex = 0;

        var script = "(SELECT";
        var argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null")
            script += " TOP " + argFormula; //todo : top n not supported in oracle.

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;
        else script += " *"; //blank star

        script += " FROM " + GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " WHERE " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " GROUP BY " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " HAVING " + argFormula;

        argFormula = GetFunctionArgument(argIndex, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " ORDER BY " + argFormula;
        script += ")";
        return script;
    }

    private string GenerateConcatRowsScript(bool forWhere, FunctionInvocationExpressionNode function,
        LocalParameters localParameters)
    {
        var argIndex = 0;

        var script = "(SELECT ";
        var argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null")
            script += " TOP " + argFormula; //todo : top n not supported in oracle.

        script += " concat(";
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;
        else script += " *"; //blank star

        script += ",";
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;
        else script += "-";

        script += ") as 'data()' FROM " + GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " WHERE " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " GROUP BY " + argFormula;

        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " HAVING " + argFormula;

        argFormula = GetFunctionArgument(argIndex, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") script += " ORDER BY " + argFormula;
        script += " FOR XML PATH ('') )";
        return script;
    }

    private string ExistsBody(bool forWhere, FunctionInvocationExpressionNode function,
        LocalParameters localParameters)
    {
        var formula = "";
        var argIndex = 3;

        formula += "SELECT 1 FROM " + GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        //Tables Name
        var argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") formula += " " + argFormula;
        //WHERE
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") formula += " WHERE " + argFormula;
        //GROUP BY
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") formula += " GROUP BY " + argFormula;
        //HAVING 
        argFormula = GetFunctionArgument(argIndex++, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") formula += " HAVING " + argFormula;
        //ORDER BY
        argFormula = GetFunctionArgument(argIndex, forWhere, function, localParameters);
        if (!string.IsNullOrWhiteSpace(argFormula) && argFormula != "null") formula += " ORDER BY " + argFormula;
        return formula;
    }

    private string GenerateInternalUnionScript(bool forWhere,
        FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        var tableName = GetFunctionArgument(0, forWhere, function, localParameters, true);
        var startTime = GetFunctionArgument(1, forWhere, function, localParameters, true);
        var endTime = GetFunctionArgument(2, forWhere, function, localParameters, true);
        var filter = GetFilterSql();
        var joinBuilder = new CandoStringBuilder();
        var tables = new Dictionary<string, string> { { Name, Name } };
        GenerateQuery_WriteJoins(this, ref joinBuilder, ref tables);
        var join = joinBuilder.ToString();
        var script = new CandoStringBuilder();
        script += $@"(Select Sum(G.{endTime}-G.{startTime}) From (
			Select Distinct B.{startTime}, B.{endTime}
				From {tableName} A ";
        if (joinBuilder.Length > 0)
            script += $"\r\n {join.Replace(Name, "A")}";
        script += $@"
				Cross Apply( Select Min(C.{startTime}) {startTime}, Max(C.{endTime}) {endTime}
				From {tableName} C ";
        if (joinBuilder.Length > 0)
            script += join.Replace(Name, "C");
        script += $"\r\n Where C.{startTime} <= A.{endTime} And C.{endTime} >= A.{startTime}";
        foreach (var groupByItem in Aggregates?.Where(agg => agg.function == eAggregationFunctions.GroupByItem) ?? [])
        {
            if (groupByItem.fieldName == null) continue;
            var groupByField = groupByItem.fieldName;
            script += $"\r\n And C.{groupByField} = A.{groupByField}";
            script += $"\r\n And {Name}.{groupByField} = A.{groupByField}";
        }
        if (!string.IsNullOrEmpty(filter))
            script += $" And {filter.Replace(Name, "C")}";
        script += " ) B";
        if (!string.IsNullOrEmpty(filter))
            script += $" Where {filter.Replace(Name, "A")}";
        script += ") G )";
        return script.ToString();
    }

    private string GenerateInsertScript(bool forWhere, FunctionInvocationExpressionNode function,
        LocalParameters localParameters)
    {
        var argIndex = 0;
        var script = "INSERT INTO " + GetFunctionArgument(argIndex++, forWhere, function, localParameters) +
                     " (" + GetFunctionArgument(argIndex++, forWhere, function, localParameters) + ") ";
        var argFormula = GetFunctionArgument(argIndex, forWhere, function, localParameters);
        if (argFormula != null)
        {
            //if (argFormula.Substring(0, 6).ToLower() == "select")
            //	formula += argFormula;
            //else
            //var argId = "Id";//GetFunctionArgument(argIndex++, forWhere, function, localParameters);
            script += "OUTPUT Id VALUES(" + argFormula + ")";
        }
        return script;
    }
    private string GenerateFetchScript(bool forWhere, FunctionInvocationExpressionNode function,
        LocalParameters localParameters)
    {
        var argIndex = 0;
        var formula = $"Fetch('{GetFunctionArgument(argIndex++, forWhere, function, localParameters)}',({GetFunctionArgument(argIndex++, forWhere, function, localParameters, true)}),'{GetFunctionArgument(argIndex++, forWhere, function, localParameters)}')";
        var exp = Parser.Parse(formula);
        var lc = new LocalParameters(localParameters);
        var value = exp.Eval(this, lc);
        return $"'{value?.ToString()}'";
    }
    private string GetFunctionArgument(int argIndex, bool forWhere,
        FunctionInvocationExpressionNode function, LocalParameters localParameters, bool exactValue = false)
    {
        if (function.PositionalParameters == null) return null;
        if (argIndex < 0 || argIndex >= function.PositionalParameters.Count) return null;
        var arg = function.PositionalParameters[argIndex];
        if (ConvertExpressionToScript(forWhere, arg, out var paramFormula, out var paramOut, localParameters, exactValue) && paramOut == null)
            return paramFormula;
        return null;
    }
}