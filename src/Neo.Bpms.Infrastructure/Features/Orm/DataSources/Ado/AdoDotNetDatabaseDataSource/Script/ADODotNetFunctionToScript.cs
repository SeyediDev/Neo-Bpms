namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    private bool ConvertFunctionToScript(bool forWhere,
        FunctionInvocationExpressionNode function,
        out string script, out ExpressionNode output, LocalParameters localParameters)
    {
        output = null;
        script = "";
        var functionName = function.FunctionName.ToLower();
        switch (functionName)
        {
            case "suser_sname":
                script = functionName + "()";
                break;
            case "avg":
            case "checksum_agg":
            case "count":
            case "count_big":
            case "grouping":
            case "grouping_id":
            case "max":
            case "min":
            case "stdev":
            case "stdevp":
            case "sum":
            case "var":
            case "varp":
                script = GenerateAggregationScript(function, localParameters);
                break;
            //case "cast":
            //	script = GenerateCastScript(function, localParameters);
            //	break;
            case "convert":
            case "choose":
                script = GenerateCallFunctionScript(forWhere, function, localParameters, functionName);
                break;
            case "abs":
            case "acos":
            case "asin":
            case "atan":
            case "atn2":
            case "ceiling":
            case "cos":
            case "cot":
            case "degrees":
            case "exp":
            case "floor":
            case "log":
            case "log10":
            case "pi":
            case "power":
            case "radians":
            case "rand":
            case "round":
            case "sign":
                script = GenerateCallFunctionScript(forWhere, function, localParameters, functionName);
                break;
            case "isjson":
            case "json_value":
            case "json_query":
            case "json_modify":
                script = GenerateCallFunctionScript(forWhere, function, localParameters, functionName);
                break;
            case "month":
            case "day":
            case "year":
            case "getdate":
            case "datename":
                script = GenerateCallFunctionScript(forWhere, function, localParameters, functionName);
                break;
            case "ascii":
            case "char":
            case "charindex":
            case "concat":
            case "concat_ws":
            case "difference":
            case "format":
            case "left":
            case "len":
            case "lower":
            case "ltrim":
            case "nchar":
            case "patindex":
            case "quatename":
            case "replace":
            case "replicate":
            case "reverse":
            case "right":
            case "rtrim":
            case "soundex":
            case "space":
            case "str":
            case "string_agg":
            case "string_escape":
            case "string_splite":
            case "stuff":
            case "substring":
            case "translate":
            case "trim":
            case "unicode":
            case "upper":
                script = GenerateCallFunctionScript(forWhere, function, localParameters, functionName);
                break;
            case "isnull": //ISNULL()
                script = GenerateCallFunctionScript(forWhere, function, localParameters, ISNULL_Command());
                break;
            case "hour":
            case "second":
            case "minute":
            case "millisecond":
            case "microsecond":
            case "quarter":
            case "week":
            case "dw":
            case "dayofyear":
                script = $"DATEPART(\"{functionName.ToUpper()}\", {ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "yeardiff":
            case "quarterdiff":
            case "monthdiff":
            case "dayofyeardiff":
            case "daydiff":
            case "weekdiff":
            case "hourdiff":
            case "minutediff":
            case "seconddiff":
            case "milliseconddiff":
                script = $"CONVERT(bigint,DATEDIFF(\"{functionName.ToUpper()[..(functionName.Length - 4)]}\", {ConvertArgumentToScript(forWhere, function, localParameters)}))";
                break;
            case "timespandiff":
                script = $"10000*CONVERT(bigint,DATEDIFF(\"MILLISECOND\", {ConvertArgumentToScript(forWhere, function, localParameters)}))";
                break;
            case "dateof":
                script = $"CONVERT(date,{ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "converttolong":
                script = $"CONVERT(bigint,{ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "converttodouble":
                script = $"CONVERT(float,{ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "string":
            case "converttostring":
                script = $"CONVERT(NVARCHAR(MAX),{ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "autoincrement": //AutoIncrement()
                script = GenerateAutoIncrementScript(localParameters);
                break;
            case "newid": //NewGuid()
                script = "newid()";
                break;
            case "newguid": //NewGuid()
                script = "'" + Guid.NewGuid() + "'";
                break;
            case "hasbit": //HasBit(left,right) جستجوی انتخاب سمت راست در انتخابهای سمت چپ
                script = GenerateHasBitScript(forWhere, function, localParameters);
                break;
            case "equal": //Equal(fieldId;v1;v2;v3;...)
                script = GenerateEqualScript(forWhere, function, localParameters);
                break;
            case "rownumber":
                script = "(ROW_NUMBER() OVER (order by Id))";
                break;
            case "serverdatetime":
            case "serverdate":
                script = GenerateDateScript(functionName);
                break;
            case "dateonly": //DateOnly
                script = GenerateDateOnlyScript(forWhere, function, localParameters);
                break;
            case "strany":
                script = GenerateStrAnyScript(forWhere, function, localParameters);
                break;
            case "strstart":
                script = GenerateStrStartScript(forWhere, function, localParameters);
                break;
            case "strend":
                script = GenerateStrEndScript(forWhere, function, localParameters);
                break;
            case "list": //List()
                script = GenerateListScript(forWhere, function, localParameters);
                break;
            case "space2": //Space( cnt )
                script = GenerateSpaceScript(function);
                break;
            case "char2": //Char( asciCode )
                script = GenerateCharScript(function);
                break;
            case "switch":
                script = GenerateSwitchScript(forWhere, function, localParameters);
                break;

            case "dbselect":
                script = GenerateSelectScript(forWhere, function, localParameters);
                break;
            case "dbconcatrows":
                script = GenerateConcatRowsScript(forWhere, function, localParameters);
                break;
            case "dbnotexists":
                script = "NOT EXISTS(" + ExistsBody(forWhere, function, localParameters) + ")";
                break;
            case "dbexists":
                script = "EXISTS(" + ExistsBody(forWhere, function, localParameters) + ")";
                break;
            case "internalunion":
                script = GenerateInternalUnionScript(forWhere, function, localParameters);
                break;
            case "dbinsert":
                script = GenerateInsertScript(forWhere, function, localParameters);
                break;
            case "fetch":
                script = GenerateFetchScript(forWhere, function, localParameters);
                break;
            case "persiandate":
                script = $"dbo.UDFPersianDate({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "persiandatetime":
                script = $"dbo.UDFPersianDateTime({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "persianyear":
                script = $"dbo.UDFPersianYear({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "persianmonth":
                script = $"dbo.UDFPersianMonth({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "persianday":
                script = $"dbo.UDFPersianDay({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            case "persianyearmonth":
                script = $"dbo.UDFPersianYearMonth({ConvertArgumentToScript(forWhere, function, localParameters)})";
                break;
            default:
                script = GenerateDefaultErrorScript(forWhere);
                output = function;
                break;
        }
        return !string.IsNullOrEmpty(script);
    }

    private static string GenerateDateScript(string functionName)
    {
        var d = DateTime.UtcNow;
        var script = "'" + d.Year + "/" + d.Month + "/" + d.Day + "'";
        if (functionName == "serverdatetime")
            script += " " + d.Hour + ":" + d.Minute + ":" + d.Second + "'";
        return script;
    }

    private string GenerateAutoIncrementScript(LocalParameters localParameters)
    {
        var entity = ProjectDefinition.Project.GetEntity(localParameters.GetString("__DBNamespaceId"), localParameters.GetString("__DBEntityId"));
        var field = entity?.GetField(localParameters.GetString("__DBFieldName"));
        var dbTableName = GetTableDbName(entity, DataSrcDefinition.connection.DatabaseName);
        var dbFieldName = field?.DbFieldName;
        return dbFieldName != null && dbTableName != null
            ? "SELECT (" + ISNULL_Command() + "(Max(" + dbFieldName + "),0)+1) M FROM " + dbTableName
            : "AutoIncrement()";
    }

    private string GenerateHasBitScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        string script;
        if (function.PositionalParameters != null)
        {
            var paramNo = 0;
            string left = "", right = "";
            foreach (var prm in function.PositionalParameters)
            {
                if (ConvertExpressionToScript(forWhere, prm, out var paramFormula, out var paramOut, localParameters) && paramOut == null)
                {
                    if (paramNo == 0)
                        left = paramFormula;
                    else
                        right = paramFormula;
                }
                paramNo++;
            }
            script = "(" + left + "& power(2," + right + ")!=0)"; //todo check power function syntax in oracle
        }
        else
            script = "HasBit()";
        return script;
    }

    private string GenerateEqualScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null) return GenerateDefaultErrorScript(forWhere);
        var paramNo = 0;
        var script = "";
        foreach (var prm in function.PositionalParameters)
        {
            if (ConvertExpressionToScript(forWhere, prm, out var paramFormula, out var paramOut, localParameters) && paramOut == null)
            {
                if (paramNo == 0)
                    script += paramFormula + " IN(";
                else
                {
                    if (paramNo > 1) script += ",";
                    script += paramFormula;
                }
            }
            else
                script += "null";
            paramNo++;
        }
        if (paramNo > 0)
            script += ")";
        return script;
    }

    private string GenerateDateOnlyScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null || function.PositionalParameters.Count <= 0) return GenerateDefaultErrorScript(forWhere);
        var script = "cast(";
        var item = function.PositionalParameters[0];
        if (ConvertExpressionToScript(forWhere, item, out var itemDateParam, out var itemout, localParameters) && itemout == null)
            script += "concat(year(" + itemDateParam + "),'-',month(" + itemDateParam + "),'-',day(" + itemDateParam + "))";
        script += " as date )";
        return script;
    }

    private string GenerateStrAnyScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null || function.PositionalParameters.Count <= 0) return GenerateDefaultErrorScript(forWhere);
        var item = function.PositionalParameters[0];
        var script = "";
        if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) && itemout == null)
        {
            var valueItem = function.PositionalParameters[1];
            if (ConvertExpressionToScript(forWhere, valueItem, out var valuesFormula, out itemout, localParameters) &&
                 itemout == null)
            {
                if (valuesFormula.StartsWith("N'"))
                    valuesFormula = valuesFormula[2..][..(valuesFormula.Length - 3)];
                script += string.Join(" And ",
                    valuesFormula.Split(' ').Select(valueFormula =>
                        $"({itemFormula} Like (N'%{valueFormula}%'))"));
            }
        }

        return script;
    }

    private string GenerateStrStartScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null || function.PositionalParameters.Count <= 0) return GenerateDefaultErrorScript(forWhere);
        var script = "(";
        var item = function.PositionalParameters[0];
        if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) && itemout == null)
        {
            script += itemFormula + " Like (";
            item = function.PositionalParameters[1];
            if (ConvertExpressionToScript(forWhere, item, out itemFormula, out itemout, localParameters) && itemout == null)
            {
                script += itemFormula;
            }
            script += "+'%')";
        }
        script += ")";
        return script;
    }

    private string GenerateStrEndScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null || function.PositionalParameters.Count <= 0) return GenerateDefaultErrorScript(forWhere);
        var script = "(";
        var item = function.PositionalParameters[0];
        if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) && itemout == null)
        {
            script += itemFormula + " Like ('%'+";
            item = function.PositionalParameters[1];
            if (ConvertExpressionToScript(forWhere, item, out itemFormula, out itemout, localParameters) && itemout == null)
            {
                script += itemFormula;
            }
            script += ")";
        }
        script += ")";
        return script;
    }

    private string GenerateListScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        var script = "(";
        foreach (var item in function.PositionalParameters ?? Enumerable.Empty<ExpressionNode>())
        {
            if (!string.IsNullOrWhiteSpace(script)) script += ",";
            if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) && itemout == null)
                script += itemFormula;
            else
                script += "null";
        }
        script += ")";
        return script;
    }

    private static string GenerateSpaceScript(FunctionInvocationExpressionNode function)
    {
        var script = "'";
        if (function.PositionalParameters != null)
        {
            var cntr = Convert.ToInt32(function.PositionalParameters[0].toText());
            for (var i = 0; i < cntr; i++)
            {
                script += " ";
            }
        }
        else script += " ";
        script += "'";
        return script;
    }

    private static string GenerateCharScript(FunctionInvocationExpressionNode function)
    {
        var script = "'";
        if (function.PositionalParameters != null)
        {
            var asciCode = Convert.ToInt32(function.PositionalParameters[0].toText());
            var cnt = function.PositionalParameters.Count > 1
                ? Convert.ToInt32(function.PositionalParameters[1].toText())
                : 1;
            script += new string((char)asciCode, cnt);
        }
        script += "'";
        return script;
    }

    private string GenerateSwitchScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        if (function.PositionalParameters == null) return GenerateDefaultErrorScript(forWhere);
        ConvertExpressionToScript(true, function.PositionalParameters[0], out var switchFormula,
            out var argOutput, localParameters);
        ConvertExpressionToScript(true, function.PositionalParameters[1], out var defaultFormula,
            out argOutput, localParameters);
        var script = "";
        for (var ii = 2; ii + 1 < function.PositionalParameters.Count; ii += 2)
        {
            var caseExpression = function.PositionalParameters[ii];
            var bodyExpression = function.PositionalParameters[ii + 1];
            ConvertExpressionToScript(forWhere, caseExpression, out var caseFormula, out var oexp2,
                localParameters);
            ConvertExpressionToScript(forWhere, bodyExpression, out var bodyFormula, out var oexp3,
                localParameters);
            if (oexp2 != null || oexp3 != null)
                break;
            if (ii == 2)
                script += "(case ";
            script += "\n when " + switchFormula + "=" + caseFormula + " then " + bodyFormula + "";
        }
        if (!string.IsNullOrEmpty(defaultFormula))
            script += "\nelse " + defaultFormula + "";
        script += "\nend ) ";
        return script;
    }

    private static string GenerateDefaultErrorScript(bool forWhere)
    {
        return forWhere ? "1=1" : "null";
    }

    private string GenerateAggregationScript(FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        string argFormula2;
        if (function.PositionalParameters != null && function.PositionalParameters.Count > 0)
        {
            if (ConvertExpressionToScript(true, function.PositionalParameters[0], out argFormula2, out var argOutput,
                localParameters))
            {
                if (function.PositionalParameters.Count > 1)
                    argFormula2 = "distinct " + argFormula2;
            }
            else argFormula2 = "*";
        }
        else
            argFormula2 = "*";
        return function.FunctionName + "(" + argFormula2 + ")";
    }

    private string GenerateCallFunctionScript(bool forWhere, FunctionInvocationExpressionNode function, LocalParameters localParameters, string functionName)
    {
        var argumentsScript = ConvertArgumentToScript(forWhere, function, localParameters);
        return functionName + "(" + argumentsScript + ")";
    }

    protected virtual string ISNULL_Command()
    {
        return "ISNULL";
    }

    private string ConvertArgumentToScript(bool forWhere,
        FunctionInvocationExpressionNode function, LocalParameters localParameters)
    {
        var argumentsScript = "";
        if (function.PositionalParameters != null)
        {
            foreach (var item in function.PositionalParameters)
            {
                if (!string.IsNullOrWhiteSpace(argumentsScript)) argumentsScript += ",";
                if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) && itemout == null)
                    argumentsScript += itemFormula;
                else
                    argumentsScript += "null";
            }
        }
        return argumentsScript;
    }

}
