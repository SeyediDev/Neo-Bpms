namespace Neo.Bpms.Engine.Data.ADODotNet;

public abstract partial class AdoDotNetDatabaseDataSource
{
    public override bool ConvertExpressionToScript(bool forWhere, ExpressionNode input,
        out string formula, out ExpressionNode output,
        LocalParameters localParameters, bool exactValue = false)
    {
        formula = null;
        output = null;
        if (input == null) return true;
        var injectionKey = "";
        switch (input.NodeType)
        {
            case ExpressionNode.eNodeType.IfExpression:
                formula = ConvertIfToScript(forWhere, input, localParameters, ref injectionKey);
                break;
            case ExpressionNode.eNodeType.Switch:
                formula = ConvertSwitchToScript(forWhere, input, localParameters, ref injectionKey);
                break;
            case ExpressionNode.eNodeType.LiteralConstant:
                {
                    var coexp = input as ConstantExpressionNode;
                    output = null;
                    if (coexp != null)
                        formula = GetSqlValue(coexp.Value, null, coexp.Value?.GetType() ?? typeof(object), exactValue);
                }
                break;
            case ExpressionNode.eNodeType.Variable:
                {
                    var coexp = input as VariableNameExpressionNode;
                    output = null;
                    formula = coexp?.Name;
                }
                break;
            case ExpressionNode.eNodeType.Convert:
                {
                    var convexp = input as ConvertExpressionNode;
                    ConvertExpressionToScript(forWhere, convexp.Expression, out var formula1, out var oexp, localParameters);
                    if (oexp != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    if (convexp.Type == typeof(int))
                        formula = "Int(" + formula1 + ")";
                    else if (convexp.Type == typeof(double) || convexp.Type == typeof(float))
                        formula = "Number(" + formula1 + ")";
                    else if (convexp.Type == typeof(string))
                        formula = "String(" + formula1 + ")";
                    else
                        break;
                    output = null;
                }
                break;
            case ExpressionNode.eNodeType.Unary:
                {
                    var uexp = input as UnaryExpressionNode;
                    ConvertExpressionToScript(forWhere, uexp.Expression, out var formula1, out var oexp, localParameters);
                    if (oexp != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    switch (uexp?.UnaryOperationType)
                    {
                        case UnaryExpressionNode.eUnaryOperationType.Negation:
                            output = null;
                            formula = "-(" + formula1 + ")";
                            break;
                        case UnaryExpressionNode.eUnaryOperationType.BitNot:
                            output = null;
                            formula = "~(" + formula1 + ")";
                            break;
                        case UnaryExpressionNode.eUnaryOperationType.Not:
                            output = null;
                            formula = "not (" + formula1 + ")";
                            break;
                    }
                }
                break;
            case ExpressionNode.eNodeType.ArithmeticExpressions:
                {
                    var aexp = input as ArithmeticExpressionNode;
                    if (aexp == null) break;
                    ConvertExpressionToScript(forWhere, aexp.LeftExpression, out var formula1, out var oexp1, localParameters);
                    if (oexp1 != null) break;
                    ConvertExpressionToScript(forWhere, aexp.RightExpression, out var formula2, out var oexp2, localParameters);
                    if (oexp2 != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    CheckInjectionKey(ref injectionKey, ref formula2);
                    switch (aexp.OperatorType)
                    {
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Addition:
                            output = null;
                            formula = "(" + formula1 + ")+(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Subtraction:
                            output = null;
                            formula = "(" + formula1 + ")-(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Multiplication:
                            output = null;
                            formula = "(" + formula1 + ")*(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Division:
                            output = null;
                            formula = "(" + formula1 + ")/(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Modulo:
                            output = null;
                            formula = "(" + formula1 + ") Mod (" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.Reminder:
                            output = null;
                            formula = "(" + formula1 + ")%(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.BitAnd:
                            output = null;
                            formula = "(" + formula1 + ")&(" + formula2 + ")";
                            break;
                        case ArithmeticExpressionNode.eArithmeticOperatorType.BitOr:
                            output = null;
                            formula = "(" + formula1 + ")|(" + formula2 + ")";
                            break;
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.Power:
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.Exponentiation:
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.Negation:
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.BitXOr:
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.LeftShift:
                            //case ArithmeticExpressionNode.eArithmeticOperatorType.RightShift:
                    }
                }
                break;
            case ExpressionNode.eNodeType.ComparisionExpression:
                {
                    var compexp = input as ComparisonExpressionNode;
                    if (compexp == null) break;
                    ConvertExpressionToScript(forWhere, compexp.LeftExpression, out var formula1, out var oexp1,
                        localParameters);
                    if (oexp1 != null) break;
                    ConvertExpressionToScript(forWhere, compexp.RightExpression, out var formula2, out var oexp2,
                        localParameters);
                    if (oexp2 != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    CheckInjectionKey(ref injectionKey, ref formula2);
                    switch (compexp.ComparisonType)
                    {
                        case ComparisonExpressionNode.eComparisonType.Equal:
                            output = null;
                            if (formula2 == "null")
                                formula = "(" + (formula1 ?? "0") + ") is null";
                            else if (formula1 == "null")
                                formula = "(" + (formula2 ?? "0") + ") is null";
                            else
                                formula = "(" + (formula1 ?? "0") + ")=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.NotEqual:
                            output = null;
                            if (formula2 == "null")
                                formula = "(" + (formula1 ?? "0") + ") is not null";
                            else if (formula1 == "null")
                                formula = "(" + (formula2 ?? "0") + ") is not null";
                            else
                                formula = "(" + (formula1 ?? "0") + ")!=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.LessThan:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")<(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.LessEqual:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")<=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.GreaterThan:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")>(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.GreaterEqual:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")>=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.NotGreaterThan:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")<=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.NotLessThan:
                            output = null;
                            formula = "(" + (formula1 ?? "0") + ")>=(" + (formula2 ?? "0") + ")";
                            break;
                        case ComparisonExpressionNode.eComparisonType.In:
                        case ComparisonExpressionNode.eComparisonType.NotIn:
                            output = null;
                            if (string.IsNullOrEmpty(formula2))
                                formula = "(1=0)";
                            else if (formula2 == "null" || formula2 == "N'null'")
                                formula = "(" + (formula1 ?? "0") + ") is null";
                            else if (formula1 == "null" || formula1 == "N'null'")
                                formula = "(" + formula2 + ") is null";
                            else
                            {
                                var stringifyValues = string.Join(",", formula2.Split(',')
                                    .Select(f =>
                                    {
                                        if (!f.StartsWith("N'"))
                                            return !double.TryParse(f, out _) ? Endify($"N'{f}") : Endify($"'{f}");

                                        return Endify(f);
                                    }));
                                formula =
                                    $"({formula1}) {(((compexp.ComparisonType == ComparisonExpressionNode.eComparisonType.In) ? "in" : "not in"))} (" +
                                    stringifyValues + ")";
                            }

                            break;
                            //case ComparisonExpressionNode.eComparisonType.InstanceOf:
                    }
                }
                break;
            case ExpressionNode.eNodeType.LogicalExpressions:
                {
                    if (input is not LogicalExpressionNode logicExp) break;
                    ConvertExpressionToScript(forWhere, logicExp.LeftExpression, out var formula1, out var oexp1,
                        localParameters);
                    if (oexp1 != null) break;
                    ConvertExpressionToScript(forWhere, logicExp.RightExpression, out var formula2, out var oexp2,
                        localParameters);
                    if (oexp2 != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    CheckInjectionKey(ref injectionKey, ref formula2);
                    switch (logicExp.LogicalExpressionType)
                    {
                        case LogicalExpressionNode.eLogicalExpressionType.And:
                            output = null;
                            formula = "(" + formula1 + ") and (" + formula2 + ")";
                            break;
                        case LogicalExpressionNode.eLogicalExpressionType.Or:
                            output = null;
                            formula = "(" + formula1 + ") or (" + formula2 + ")";
                            break;
                            //case LogicalExpressionNode.eLogicalExpressionType.Not:
                            //	output = null;
                            //	formula = "!(" + formula1 + ")";
                            //	break;
                            //case LogicalExpressionNode.eLogicalExpressionType.XOr:
                            //case LogicalExpressionNode.eLogicalExpressionType.InstanceOf:
                    }
                }
                break;
            case ExpressionNode.eNodeType.PathExpression:
                {
                    var pathpexp = input as PathExpressionNode;
                    if (pathpexp == null) break;
                    ConvertExpressionToScript(forWhere, pathpexp.Expression, out var formula1, out var oexp1, localParameters);
                    if (oexp1 != null) break;
                    ConvertExpressionToScript(forWhere, pathpexp.RightExpression, out var formula2, out var oexp2, localParameters);
                    if (oexp2 != null) break;
                    CheckInjectionKey(ref injectionKey, ref formula1);
                    CheckInjectionKey(ref injectionKey, ref formula2);
                    output = null;
                    formula = formula1 + "." + formula2;
                }
                break;
            //case ExpressionNode.eNodeType.Block:
            //case ExpressionNode.eNodeType.FunctionDefinition:
            //case ExpressionNode.eNodeType.ForExpression:
            //case ExpressionNode.eNodeType.Switch:
            //case ExpressionNode.eNodeType.BlockControl:
            //case ExpressionNode.eNodeType.UnaryTest:
            //case ExpressionNode.eNodeType.Interval:
            //case ExpressionNode.eNodeType.Dictionary:
            //case ExpressionNode.eNodeType.AssignmentOperator:
            //case ExpressionNode.eNodeType.QuantifiedExpression:
            case ExpressionNode.eNodeType.List:
                {
                    var listExp = input as ListExpressionNode;
                    if (listExp.Items != null)
                    {
                        foreach (var item in listExp.Items)
                        {
                            if (!string.IsNullOrWhiteSpace(formula)) formula += ",";
                            if (ConvertExpressionToScript(forWhere, item, out var itemFormula, out var itemout, localParameters) &&
                                itemout == null)
                            {
                                CheckInjectionKey(ref injectionKey, ref itemFormula);
                                if (itemFormula.StartsWith("N'{"))
                                    itemFormula = itemFormula[3..^2];
                                formula += itemFormula;
                            }
                            else
                                formula += "null";
                        }
                    }
                }
                break;
            case ExpressionNode.eNodeType.FunctionInvocation:
                {
                    var funcInvocExp = input as FunctionInvocationExpressionNode;
                    if (
                        !ConvertFunctionToScript(forWhere, funcInvocExp, out formula, out output, localParameters) ||
                        output != null)
                        return false;
                    CheckInjectionKey(ref injectionKey, ref formula);
                    if (!forWhere) formula = GetInnerInjectionKey() + "(" + formula + ")";
                    else formula = "" + formula + "";
                }
                break;
        }

        if (!string.IsNullOrEmpty(injectionKey))
            formula = injectionKey + formula;
        return true;
    }

    private static string Endify(string f)
    {
        return !f.EndsWith("'") ? $"{f}'" : f;
    }

    private string ConvertSwitchToScript(bool forWhere, ExpressionNode input, LocalParameters localParameters,
        ref string injectionKey)
    {
        var switchExpressionNode = input as SwitchExpressionNode;
        if (switchExpressionNode == null) throw new Exception("Invalid node for switch.");
        ConvertExpressionToScript(forWhere, switchExpressionNode.DefaultExpression, out var defaultFormula, out var oexp1,
            localParameters);
        CheckInjectionKey(ref injectionKey, ref defaultFormula);
        //?if (oexp1 != null) break;
        var formula = "";
        if (switchExpressionNode.Cases != null)
        {
            foreach (var caseExpressionNode in switchExpressionNode.Cases)
            {
                ConvertExpressionToScript(forWhere, caseExpressionNode.CaseExpression, out var caseFormula, out var oexp2,
                    localParameters);
                ConvertExpressionToScript(forWhere, caseExpressionNode.BodyExpression, out var bodyFormula, out var oexp3,
                    localParameters);
                //?if (oexp2 != null || oexp3 != null) break;
                CheckInjectionKey(ref injectionKey, ref caseFormula);
                CheckInjectionKey(ref injectionKey, ref bodyFormula);
                if (string.IsNullOrEmpty(formula))
                    formula = "(case ";
                formula += "\n when " + caseFormula + " then " + bodyFormula + "";
            }

            if (!string.IsNullOrEmpty(defaultFormula))
                formula += "\nelse " + defaultFormula + "";
            formula += "\nend ) ";
        }

        return formula;
    }

    private string ConvertIfToScript(bool forWhere, ExpressionNode input, LocalParameters localParameters,
        ref string injectionKey)
    {
        var ifexp = input as IfExpressionNode;
        if (ifexp == null) return "";
        string formula3 = null;
        ConvertExpressionToScript(forWhere, ifexp.IfExpression, out var formula1, out var oexp1, localParameters);
        ConvertExpressionToScript(forWhere, ifexp.ThenExpression, out var formula2, out var oexp2, localParameters);
        CheckInjectionKey(ref injectionKey, ref formula1);
        CheckInjectionKey(ref injectionKey, ref formula2);
        if (ifexp.ElseExpression != null)
        {
            ConvertExpressionToScript(forWhere, ifexp.ElseExpression, out formula3, out var oexp3, localParameters);
            CheckInjectionKey(ref injectionKey, ref formula3);
            if (oexp3 != null) return "";
        }

        if (oexp1 != null || oexp2 != null) return "";
        return GenerateIIfScript(formula1, formula2, formula3);
    }

    private static string GenerateCaseWhenThenElseScript(string formula1, string formula2, string formula3)
    {
        var formula = "(case when " + formula1 + " then " + formula2 + "";
        if (!string.IsNullOrEmpty(formula3))
            formula += " else " + formula3;
        formula += " end ) ";
        return formula;
    }

    private static string GenerateIIfScript(string formula1, string formula2, string formula3)
    {
        var formula = "IIF(" + formula1 + "," + formula2 + "";
        if (!string.IsNullOrEmpty(formula3)) formula += "," + formula3;
        else formula += ",null";
        formula += ")";
        return formula;
    }

    private static void CheckInjectionKey(ref string injectionKey, ref string formula)
    {
        if (formula == null) return;
        var k = GetInnerInjectionKey();
        if (formula.Length >= k.Length && formula[..k.Length].ToUpper() == k)
        {
            injectionKey = k;
            formula = formula[k.Length..];
        }
    }
}