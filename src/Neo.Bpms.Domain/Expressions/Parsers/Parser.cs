namespace Neo.Bpms.Domain.Expressions.Parsers;

public class Parser(string exp)
{
    //		char[] specialChars = { '!', '^', '%', '&', '*', '(', ')', '-', '=', '+', '{', '}', '[', ']', '|', '\\', '\'', '"', '.', '/', '?', ':', ';', ',', '<', '>', '~', }; 

    #region Syntax Processor

    private ExpressionNode expressionTree;

    private int index = 0;
    private int maxindex = exp.Length;
    private readonly char[] exp = exp.ToCharArray();

    public static ExpressionTree ParseTree(string exp)
    {
        if (exp == null) return null;
        ExpressionNode root = exp == string.Empty
            ? new ConstantExpressionNode("")
            : Parse(exp, 0, 0);
        return root != null ? new ExpressionTree { Root = root, ExpressionString = exp } : null;
    }

    public static void ParseTree(ExpressionTree tree)
    {
        tree.Root = Parse(tree.ExpressionString, 0, 0);
    }

    public static ExpressionNode Parse(string exp)
    {
        return Parse(exp, 0, 0);
    }

    public static Dictionary<string, List<ExpressionNode>> FetchNodes<T>(string exp)
        where T : ExpressionNode
    {
        Dictionary<string, List<ExpressionNode>> dic = [];
        return FetchNodes<T>(exp, dic);
    }

    public static Dictionary<string, List<ExpressionNode>> FetchNodes<T>(string exp,
        Dictionary<string, List<ExpressionNode>> dic)
        where T : ExpressionNode
    {
        ExpressionNode root = Parse(exp, 0, 0);
        root.FetchNodes<T>(dic);
        return dic;
    }

    private static ExpressionNode Parse(string exp, int depth, int priority)
    {
        if (string.IsNullOrEmpty(exp)) return null;
        //var result = new VariableNameExpressionNode(exp);
        //try
        //{
        Parser parser = new(exp);
        ExpressionNode result = parser.Complie(depth, priority);
        return result == null ? throw new ParserException(exp, parser.index) : result;
        //}
        //catch(Exception e)
        //{
        //	throw new Exception("Parser Error:\r\n" + exp + "\r\n"+e.Message);
        //}
    }

    //public ExpressionNode SimpleFEELComplie()
    //{
    //	expressionTree = CheckSimpleExpression();
    //	return expressionTree;
    //}
    public ExpressionNode Complie(int depth, int priority)
    {
        //Rule 1. expression = textual expression | boxed expression ;
        expressionTree = CheckExpression(depth, priority);
        return expressionTree;
    }

    private ExpressionNode CheckExpression(int depth, int priority)
    {
        ExpressionNode result = CheckExpressionPart(depth, priority);
        if (result == null) return null;
        bypassWhitespace();
        return index >= maxindex ? result : CheckOperator(depth, priority, result);
    }

    private ExpressionNode CheckOperator(int depth, int priority, ExpressionNode leftExpression)
    {
        bypassWhitespace();
        //checking operands:
        if (index >= maxindex) return leftExpression;
        char c = exp[index];
        switch (c)
        {
            case '+':
                if (index + 1 >= maxindex) return leftExpression;
                switch (exp[index + 1])
                {
                    case '+':
                        if (exp[index + 2] == '=')
                        {
                            index += 3;
                            return new AssignmentExpressionNode(
                                AssignmentExpressionNode.eAssignmentOperator.PreIncrementAssignment,
                                leftExpression, CheckExpression(depth, priority), depth);
                        }
                        else
                        {
                            index += 2;
                            return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Increment,
                                leftExpression);
                        }

                    case '=':
                        index += 2;
                        return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.AddAssignment,
                            leftExpression,
                            CheckExpression(depth, priority), depth);
                }

                index++;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Addition,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '-':
                if (index + 1 >= maxindex) return leftExpression;
                switch (exp[index + 1])
                {
                    case '-':
                        if (exp[index + 2] == '=')
                        {
                            index += 3;
                            return new AssignmentExpressionNode(
                                AssignmentExpressionNode.eAssignmentOperator.PreDecrementAssignment,
                                leftExpression, CheckExpression(depth, priority), depth);
                        }
                        else
                        {
                            index += 2;
                            return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Decrement,
                                leftExpression);
                        }

                    case '=':
                        index += 2;
                        return new AssignmentExpressionNode(
                            AssignmentExpressionNode.eAssignmentOperator.SubtractAssignment,
                            leftExpression, CheckExpression(depth, priority), depth);
                }

                index++;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Subtraction,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '*':
                if (index + 1 >= maxindex) return leftExpression;
                switch (exp[index + 1])
                {
                    case '*':
                        if (exp[index + 2] == '=')
                        {
                            index += 3;
                            return new AssignmentExpressionNode(
                                AssignmentExpressionNode.eAssignmentOperator.PowerAssignment,
                                leftExpression, CheckExpression(depth, priority), depth);
                        }
                        else
                        {
                            index += 2;
                            return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Power,
                                leftExpression,
                                CheckExpression(depth, priority), depth).AdjustByPriority();
                        }

                    case '=':
                        index += 2;
                        return new AssignmentExpressionNode(
                            AssignmentExpressionNode.eAssignmentOperator.MultiplicationAssignment,
                            leftExpression, CheckExpression(depth, priority), depth);
                }

                index++;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Multiplication,
                    leftExpression, CheckExpression(depth, priority), depth).AdjustByPriority();
            case '/':
                if (index + 1 >= maxindex) return leftExpression;
                if (exp[index + 1] == '=')
                {
                    index += 2;
                    return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.DivisionAssignment,
                        leftExpression, CheckExpression(depth, priority), depth);
                }

                index += 1;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Division,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '%':
                index += 1;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Reminder,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '\\':
                if (index + 1 >= maxindex) return leftExpression;
                if (exp[index + 1] == '=')
                {
                    index += 2;
                    return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.ModuloAssignment,
                        leftExpression,
                        CheckExpression(depth, priority), depth);
                }

                index += 1;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Modulo,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case 'm':
            case 'M':
                if (checkString("mod"))
                {
                    return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Modulo,
                        leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case 'e':
            case 'E':
                if (checkString("e"))
                {
                    return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.Exponentiation,
                        leftExpression, CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case '.': //path
                index++;
                return new PathExpressionNode(leftExpression, CheckExpression(depth, priority));
            case ',':
            case ';':
                {
                    index++;
                    int idxcoma = 1;
                    ListExpressionNode listExp = null;

                    while (idxcoma > 0)
                    {
                        idxcoma = lookFor(index, c.ToString());
                        string s = idxcoma < 0
                            ? new string(exp, index, maxindex - index)
                            : new string(exp, index, idxcoma - 1 - index);
                        if (string.IsNullOrEmpty(s))
                            break;
                        ExpressionNode item = s == "*"
                            ? new ConstantExpressionNode(s)
                            : Parse(s);
                        if (listExp == null)
                            listExp = new ListExpressionNode(leftExpression, item);
                        else
                            listExp.Items.Add(item);
                        index = idxcoma < 0 ? maxindex : idxcoma;
                    }

                    return listExp;
                }

            case 'n':
            case 'N':
                if (checkString("notin"))
                    return ParseIn(depth, priority, leftExpression, ComparisonExpressionNode.eComparisonType.NotIn);
                return CheckWhiteSpaceNameContinution(leftExpression);
            case 'i':
            case 'I':
                if (checkString("in"))
                    return ParseIn(depth, priority, leftExpression, ComparisonExpressionNode.eComparisonType.In);
                if (checkString("instance"))
                {
                    bypassWhitespace();
                    if (checkString("of"))
                    {
                        index += 2;
                        bypassWhitespace();
                        return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.InstanceOf,
                            leftExpression,
                            CheckExpression(depth, priority), depth).AdjustByPriority();
                    }
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case '[': //index or filter
                {
                    index++;
                    int idx = lookFor(index, "]");
                    if (idx > 0)
                    {
                        int maxindex0 = maxindex;
                        maxindex = idx - 1;
                        ExpressionNode variable = CheckExpression(depth + 1, priority);
                        switch (variable)
                        {
                            case ConstantExpressionNode node:
                                variable = new VariableNameExpressionNode(node.Value?.ToString());
                                break;
                            case PathExpressionNode node:
                                variable = new VariableNameExpressionNode(node.toText());
                                break;
                        }
                        IndexExpressionNode result = new(leftExpression, variable);
                        index = idx;
                        maxindex = maxindex0;
                        return CheckOperator(depth, priority, result);
                    }
                }
                return null;
            case 'o':
            case 'O':
                //					if (((exp[index + 1] == 'r') || (exp[index + 1] == 'R')) && Char.IsWhiteSpace(exp[index + 2]) || exp[index + 2] == '(')
                if (checkString("or"))
                {
                    return new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.Or, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case 'a':
            case 'A':
                if (checkString("and"))
                {
                    return new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case '&':
                if (index + 1 >= maxindex) return leftExpression;
                switch (exp[index + 1])
                {
                    case '&':
                        if (exp[index + 2] == '=')
                        {
                            index += 3;
                            return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.AndAssignment,
                                leftExpression,
                                CheckExpression(depth, priority), depth);
                        }

                        index += 2;
                        return new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And, leftExpression,
                            CheckExpression(depth, priority), depth).AdjustByPriority();
                    case '=':
                        index += 2;
                        return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.BitAndAssignment,
                            leftExpression, CheckExpression(depth, priority), depth);
                }

                index++;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.BitAnd,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '|':
                if (index + 1 >= maxindex) return leftExpression;
                switch (exp[index + 1])
                {
                    case '|':
                        if (exp[index + 2] == '=')
                        {
                            index += 3;
                            return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.OrAssignment,
                                leftExpression,
                                CheckExpression(depth, priority), depth);
                        }

                        index += 2;
                        return new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.Or, leftExpression,
                            CheckExpression(depth, priority), depth).AdjustByPriority();
                    case '=':
                        index += 2;
                        return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.BitOrAssignment,
                            leftExpression,
                            CheckExpression(depth, priority), depth);
                }

                index++;
                return new ArithmeticExpressionNode(ArithmeticExpressionNode.eArithmeticOperatorType.BitOr,
                    leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '<': //comparision
                if (index + 1 >= maxindex) return leftExpression;
                if (exp[index + 1] == '=')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.LessEqual,
                        leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                if (exp[index + 1] == '>')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.NotEqual, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }
                else
                {
                    index++;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.LessThan, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

            case '>': //comparision
                if (index + 1 >= maxindex) return leftExpression;
                if (exp[index + 1] == '=')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.GreaterEqual,
                        leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }
                else if (exp[index + 1] == '<')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.NotEqual, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                index++;
                return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.GreaterThan, leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case '=':
                if (index + 1 < exp.Length && exp[index + 1] == '=')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.Equal, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                index++;
                return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.Equal, leftExpression,
                    CheckExpression(depth, priority), depth).AdjustByPriority();
            case ':':
                index++;
                return new AssignmentExpressionNode(AssignmentExpressionNode.eAssignmentOperator.SimpleAssignment,
                    leftExpression,
                    CheckExpression(depth, priority), depth);
            case '!': //comparision
                if (index + 1 >= maxindex) return leftExpression;
                if (exp[index + 1] == '=')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.NotEqual, leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                if (exp[index + 1] == '>')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.NotGreaterThan,
                        leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                if (exp[index + 1] == '<')
                {
                    index += 2;
                    return new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.NotLessThan,
                        leftExpression,
                        CheckExpression(depth, priority), depth).AdjustByPriority();
                }

                return null; //expression!expression
            case 'b':
            case 'B':
                if (checkString("between"))
                {
                    int idx = lookFor(index, "and");
                    if (idx > 0)
                    {
                        int maxindex0 = maxindex;
                        maxindex = idx - 3;
                        ExpressionNode middle = CheckExpression(depth, priority);
                        if (middle == null) return null;
                        maxindex = maxindex0;
                        index = idx;
                        ExpressionNode right = CheckExpression(depth, priority);
                        return right == null
                            ? null
                            : (ExpressionNode)new LogicalExpressionNode(LogicalExpressionNode.eLogicalExpressionType.And,
                            new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.GreaterThan,
                                leftExpression, middle,
                                depth),
                            new ComparisonExpressionNode(ComparisonExpressionNode.eComparisonType.LessThan, leftExpression,
                                right,
                                depth), depth);
                    }
                }

                return CheckWhiteSpaceNameContinution(leftExpression);
            case '(': //function invocation
                string funcName;
                switch (leftExpression)
                {
                    case VariableNameExpressionNode expressionNode:
                        funcName = expressionNode.Name;
                        break;
                    case FunctionInvocationExpressionNode node:
                        funcName = node.FunctionName;
                        break;
                    default:
                        return null;
                }

                index++;
                int idx1 = lookFor(index, ")");
                if (idx1 > 0)
                {
                    ExpressionNode result;
                    string s = new(exp, index, idx1 - 1 - index);
                    if (string.IsNullOrEmpty(s))
                    {
                        result = new FunctionInvocationExpressionNode(funcName);
                    }
                    else if (s == "*")
                    {
                        ConstantExpressionNode prmExps0 = new(s);
                        result = new FunctionInvocationExpressionNode(funcName, prmExps0);
                    }
                    else
                    {
                        ExpressionNode prmExps0 = Parse(s);
                        result = prmExps0 is ListExpressionNode node ? new FunctionInvocationExpressionNode(funcName, node.Items) : new FunctionInvocationExpressionNode(funcName, prmExps0);
                    }

                    index = idx1;
                    return CheckOperator(depth, priority, result);

                }

                return null;
        }

        return leftExpression;
    }

    private ExpressionNode ParseIn(int depth, int priority, ExpressionNode leftExpression,
        ComparisonExpressionNode.eComparisonType comparisonType)
    {
        bypassWhitespace();
        if (CurrentCharEquals('('))
        {
            index++;
            int idx5 = lookFor(index, ")");
            if (idx5 > 0)
            {
                string s = new(exp, index, idx5 - 1 - index);
                index = idx5;
                if (string.IsNullOrEmpty(s))
                {
                    return new ConstantExpressionNode(false);
                }

                Parser parser = new(s);
                List<ExpressionNode> items = [];
                while (parser.index < parser.maxindex)
                {
                    int storeIndex = parser.index;
                    ExpressionNode item = parser.Complie(depth + 1, priority);
                    if (item is ListExpressionNode node)
                        items.AddRange(node.Items);
                    else if (item != null)
                        items.Add(item);
                    else if (storeIndex == parser.index)
                        throw new ParserException(
                            $"Invalid parsing input. {new string(exp, parser.index, parser.maxindex)}");
                }

                ListExpressionNode listExps = new(items, depth + 1);
                return new ComparisonExpressionNode(comparisonType, leftExpression, listExps,
                    depth);
            }

            return null;
        }

        return new ComparisonExpressionNode(comparisonType, leftExpression,
            CheckExpression(depth, priority), depth).AdjustByPriority();
    }

    private bool CurrentCharEquals(char c)
    {
        return index == maxindex ? throw new ParserException($"Expected {c} but reached end of string") : exp[index] == c;
    }

    private int GetOperatorPriority()
    {
        switch (exp[index])
        {
            case '(':
            case ')':
                return 1;
            case '!':
                if (exp[index + 1] == '=') return 7;
                return 2;
            case '~':
                //case 'sizeof'
                return 2;
            case '+':
                if (exp[index + 1] == '+') return 2;
                if (exp[index + 1] == '=') return 14;
                return 4;
            case '-':
                if (exp[index + 1] == '-') return 2;
                return exp[index + 1] == '=' ? 14 : 4;
            case '*':
            case '/':
            case '%':
                return exp[index + 1] == '=' ? 14 : 3;
            case '<':
                if (exp[index + 1] != '<') return 6;
                return exp[index + 2] == '=' ? 14 : 5;

            case '>':
                if (exp[index + 1] != '>') return 6;
                return exp[index + 2] == '=' ? 14 : 5;

            case '=':
                return exp[index + 1] == '=' ? 7 : 14;
            case '^':
                return exp[index + 1] == '=' ? 14 : 9;
            case '&':
                if (exp[index + 1] != '&') return 8;
                return exp[index + 2] == '=' ? 14 : 11;

            case '|':
                if (exp[index + 1] != '|') return 10;
                return exp[index + 2] == '=' ? 14 : 12;

            case '?':
            case ':':
                return 13;
            //case 'in': return 6;
            //case 'is': return 6;
            //case 'as': return 6;
            default:
                return 0;
        }
    }

    private ExpressionNode CheckWhiteSpaceNameContinution(ExpressionNode leftExpression)
    {
        //todo
        throw new NotImplementedException();
    }

    private ExpressionNode CheckExpressionPart(int depth, int priority)
    {
        ExpressionNode result;
        bypassWhitespace();
        if (index >= maxindex) return null;
        char c = exp[index];
        switch (exp[index])
        {
            //case '<'://unary tests
            //	if (exp[index + 1] == '=')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.LessEqual, CheckExpression(depth, priority));
            //	}
            //	else if (exp[index + 1] == '>')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.NotEqual, CheckExpression(depth, priority));
            //	}
            //	else
            //	{
            //		index++;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.LessThan, CheckExpression(depth, priority));
            //	}
            //case '>': //unary tests
            //	if (exp[index + 1] == '=')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.GreaterEqual, CheckExpression(depth, priority));
            //	}
            //	else if (exp[index + 1] == '<')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.NotEqual, CheckExpression(depth, priority));
            //	}
            //	else
            //	{
            //		index++;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.GreaterThan, CheckExpression(depth, priority));
            //	}
            //case '!': //unary tests
            //	if (exp[index + 1] == '>')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.NotGreaterThan, CheckExpression(depth, priority));
            //	}
            //	else if (exp[index + 1] == '<')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.NotLessThan, CheckExpression(depth, priority));
            //	}
            //	else if (exp[index + 1] == '=')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.NotEqual, CheckExpression(depth, priority));
            //	}
            //	break;
            //case '=': //unary tests
            //	if (exp[index + 1] == '=')
            //	{
            //		index += 2;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.Equal, CheckExpression(depth, priority));
            //	}
            //	else
            //	{
            //		index++;
            //		return new UnaryTestExpressionNode(UnaryTestExpressionNode.eUnaryTestType.Equal, CheckExpression(depth, priority));
            //	}
            case 't':
            case 'T':
                if (checkString("true"))
                    return new ConstantExpressionNode(true);
                break;
            case 'n':
            case 'N':
                if (checkString("null"))
                    return new ConstantExpressionNode(null);
                if (checkString("not"))
                {
                    bypassWhitespace();
                    if (CurrentCharEquals('('))
                    {
                        index++;
                        int idx = lookFor(index, ")");
                        if (idx > 0)
                        {
                            int maxindex0 = maxindex;
                            maxindex = idx - 2;
                            result = new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Not,
                                CheckExpression(depth + 1, priority));
                            maxindex = maxindex0;
                            return result;
                        }
                    }

                    return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Not,
                        CheckExpression(depth, priority));
                }

                break;
            case '-': //arithmetic negation
                index++;
                return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Negation,
                    CheckExpression(depth, priority));
            case '!': //arithmetic negation
                index++;
                return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.Not,
                    CheckExpression(depth, priority));
            case '~': //arithmetic negation
                index++;
                return new UnaryExpressionNode(UnaryExpressionNode.eUnaryOperationType.BitNot,
                    CheckExpression(depth, priority));
            case '{':
                {
                    index++;
                    int idx = lookFor(index, "}");

                    if (idx > 0)
                    {
                        int maxindex0 = maxindex;
                        maxindex = idx - 1;
                        result = CheckExpression(depth + 1, priority);
                        maxindex = maxindex0;
                        return result;
                    }
                }
                break;
            case '"':
            case '\'': //string literal
                break;
            case 's':
            case 'S': //quantified
                if (checkString("some"))
                {
                    //todo
                }

                break;
            case 'e':
            case 'E':
                if (checkString("every")) //quantified
                {
                    //todo
                }

                break;
            case 'f':
            case 'F':
                if (checkString("for")) //quantified
                {
                    //todo
                }
                else if (checkString("function")) //function definition
                {
                    int idx4 = CheckName();
                    if (idx4 > 0)
                    {
                        string funcName = new(exp, index, idx4);
                        index += idx4;
                        bypassWhitespace();
                        if (CurrentCharEquals('('))
                        {
                            index++;
                            int idx1 = lookFor(index, ")");
                            if (idx1 > 0)
                            {
                                string[] prms = new string(exp, index, idx1 - 1 - index).Split(',', ';');
                                index = idx1;
                                return new FunctionDefinitionExpressionNode(funcName, prms,
                                    CheckExpression(depth, priority));
                            }
                        }
                    }
                }
                else if (checkString("false"))
                    return new ConstantExpressionNode(false);

                break;
            case 'i':
            case 'I':
                if (checkString("if")) //if (exp[index + 1] == 'f' || exp[index + 1] == 'F')
                {
                    bypassWhitespace();
                    if (CurrentCharEquals('('))
                    {
                        index++;
                        int idx1 = lookFor(index, ")");
                        if (idx1 > 0)
                        {
                            ExpressionNode prmExps0 = Parse(new string(exp, index, idx1 - 1 - index), depth + 1, priority);
                            index = idx1;
                            if (prmExps0 is ListExpressionNode node)
                            {
                                List<ExpressionNode> prms = node.Items;
                                if (prms.Count == 3 || prms.Count == 2)
                                    return new IfExpressionNode(prms[0], prms[1], prms.Count > 2 ? prms[2] : null);
                            }
                            else
                            {
                                ExpressionNode thenexp = CheckExpression(depth, priority);
                                bypassWhitespace();
                                return checkString("else")
                                    ? new IfExpressionNode(prmExps0, thenexp, CheckExpression(depth, priority))
                                    : new IfExpressionNode(prmExps0, thenexp, null);
                            }
                        }

                    }
                }

                break;
            case '[': //interval  //todo: interval???
                index++;
                int idx2 = lookFor(index, "]");
                if (idx2 > 0)
                {
                    int maxindex0 = maxindex;
                    maxindex = idx2 - 1;
                    result = CheckExpression(depth + 1, priority);
                    index = idx2;
                    maxindex = maxindex0;
                    return result;
                }

                break;
            case ']':
            case ')':
                //todo: interval???
                break;
            case '(':
                index++;
                int idx3 = lookFor(index, ")");
                if (idx3 > 0)
                {
                    int maxindex0 = maxindex;
                    maxindex = idx3 - 1;
                    result = CheckExpression(depth + 1, priority);
                    index = idx3;
                    maxindex = maxindex0;
                    return result;
                }

                break;
            case ',':
                index++;
                return CheckExpressionPart(depth, priority);
        }

        int i = CheckName();
        if (i > 0)
        {
            result = new VariableNameExpressionNode(new string(exp, index, i));
            index += i;
            return result;
        }

        i = CheckLiteral(out object obj);
        if (i <= 0) return null;
        result = new ConstantExpressionNode(obj);
        index += i;
        return result;

    }

    private void bypassWhitespace()
    {
        for (; index < maxindex && char.IsWhiteSpace(exp[index]); index++) { }
    }

    private bool checkString(string s)
    {
        bypassWhitespace();

        int i = 0, j = s.Length, k;
        for (; i < j; i++)
        {
            k = index + i;
            if (k >= maxindex) return false;
            if (char.ToLower(exp[k]) != s[i])
                return false;
        }

        if (index + i >= maxindex)
        {
            index += i;
            return true;
        }

        char ch = exp[index + i];
        if (!(char.IsWhiteSpace(ch) || ch == '(')) return false;
        index += i;
        return true;
    }

    private bool CheckChar(char c)
    {
        bypassWhitespace();
        if (index >= maxindex || exp[index] != c) return false;
        index++;
        return true;

    }

    private bool nameStartChar(char c)
    {
        return
            c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' ||
            c == '?' || c == '_' ||
            c >= 0xC0 && c <= 0xD6 || c >= 0xD8 && c <= 0xF6 ||
            c >= 0xF8 && c <= 0x2FF || c >= 0x370 && c <= 0x37D ||
            c >= 0x37F && c <= 0x1FFF || c >= 0x200C && c <= 0x200D ||
            c >= 0x2070 && c <= 0x218F || c >= 0x2C00 && c <= 0x2FEF ||
            c >= 0x3001 && c <= 0xD7FF || c >= 0xF900 && c <= 0xFDCF ||
            c >= 0xFDF0 && c <= 0xFFFD; //||(c>=0x10000&&c<=0xEFFFF)
    }

    private bool namePartChar(char c)
    {
        return c >= '0' && c <= '9' || c == 0xB7 || c >= 0x0300 && c <= 0x036F || c >= 0x203F && c <= 0x2040;
    }

    private int lookForEndString(int start, char endchar)
    {
        for (int i = start; i < maxindex; i++)
        {
            if (exp[i] == endchar)
            {
                return i + 1;
            }

            /*if (exp[i] == '\\' && (i + 1) < maxindex && exp[i + 1] == '\\')
					i+=2;
				if (exp[i] == '\\' && (i+1)<maxindex && exp[i+1]==endchar)
					i++;*/
        }

        return -1;
    }

    private int lookFor(int start, string s)
    {
        int slen = s.Length;
        int matchcount = 0;
        for (int i = start; i < maxindex; i++)
        {
            if (char.ToLower(exp[i]) == s[matchcount])
            {
                matchcount++;
                if (matchcount >= slen) return i + 1;
            }
            else
            {
                matchcount = 0;
                switch (exp[i])
                {
                    case '\"':
                    case '\'':
                        i = lookForEndString(i + 1, exp[i]) - 1;
                        if (i < 0) return i;
                        break;
                    case '(':
                        i = lookFor(i + 1, ")") - 1;
                        if (i < 0) return i;
                        break;
                    case '{':
                        i = lookFor(i + 1, "}") - 1;
                        if (i < 0) return i;
                        break;
                    case '[':
                    case ']':
                        int j = lookFor(i + 1, "]") - 1;
                        if (j < 0)
                        {
                            i = lookFor(i + 1, "[") - 1;
                            if (i < 0) return i;
                        }
                        else i = j;

                        break;
                }
            }
        }

        return -1;
    }

    private int CheckLiteral(out object obj)
    {
        obj = null;
        bypassWhitespace();
        int index0 = index;
        if (CurrentCharEquals('\"'))
        {
            return StringLiteral(out obj, index0, '\"');
        }

        if (CurrentCharEquals('\''))
        {
            return StringLiteral(out obj, index0, '\'');
        }

        int i = 0;
        for (; i + index < maxindex; i++)
        {
            char c = exp[i + index];
            bool namepartchar = namePartChar(c);
            if (!namepartchar) break;
        }

        if (i > 0)
        {
            string s = new(exp, index0, i);
            if (int.TryParse(s, out int outi))
                obj = outi;
            else if (double.TryParse(s.Replace("/", "."), out double outd))
                obj = outd;
            else
                obj = s;
        }

        return i;
    }

    private int StringLiteral(out object obj, int index0, char quotationMark)
    {
        int i1 = lookForEndString(index + 1, quotationMark);
        if (i1 == -1)
            throw new ParserException(new string(exp), index, "String literal not closed.");
        obj = new string(exp, index + 1, i1 - (index + 1) - 1);
        return i1 - index0;
    }

    private int CheckName()
    {
        //Rule 27. name = name start , { name part | additional name symbols } ;
        //Rule 28. name start = name start char, { name part char } ;
        //Rule 29. name part = name part char , { name part char } ;
        //Rule 30. name start char = "?" | [A-Z] | "_" | [a-z] | [\uC0-\uD6] | [\uD8-\uF6] | [\uF8-\u2FF] | [\u370-\u37D] | [\u37F-\u1FFF] | [\u200C-\u200D] | [\u2070-\u218F] | [\u2C00-\u2FEF] | [\u3001-\uD7FF] | [\uF900-\uFDCF] | [\uFDF0-\uFFFD] | [\u10000-\uEFFFF] ;
        //Rule 31. name part char = name start char | digit | \uB7 | [\u0300-\u036F] | [\u203F-\u2040] ;
        // I disagree: //Rule 32. additional name symbols = "." | "/" | "-" | "’" | "+" | "*" ;
        bypassWhitespace();
        int i = 0;
        for (; i + index < maxindex; i++)
        {
            char c = exp[i + index];
            bool namestartchar = nameStartChar(c);
            if (i == 0 && !namestartchar) return 0;
            bool namepartchar = namestartchar || namePartChar(c);
            if (!namepartchar) return i;
        }

        return i;
    }


    //private ExpressionNode CheckSimpleExpressions()
    //{
    //	//Rule 6. simple expressions = simple expression , { "," , simple expression } ;
    //	ExpressionNode result = null;
    //	bypassWhitespace();
    //	do
    //	{
    //		ExpressionNode result1 = null;
    //		result1 = CheckSimpleExpression();
    //		if (result1 == null)
    //			return null;
    //		if (result == null)
    //			result = result1;
    //		else
    //			result = ExpressionNode.Block(result, result1);
    //		if (!checkChar(','))
    //			return null;
    //		bypassWhitespace();
    //	} while (exp[index] != 0);
    //	return result;
    //}

    //private ExpressionNode CheckSimpleExpression()
    //{
    //	bypassWhitespace();
    //	//Rule 5. simple expression = arithmetic expression | simple value ;
    //	ExpressionNode result = null;
    //	if (result == null)
    //		result = CheckArithmeticExpression();
    //	if (result == null)
    //		result = CheckSimpleValue();
    //	return result;
    //}


    //private ExpressionNode CheckTextualExpressions()
    //{

    //	//Rule 3. textual expressions = textual expression , { "," , textual expression } ;
    //	ExpressionNode result = null;
    //	bypassWhitespace();
    //	do
    //	{
    //		ExpressionNode result1 = null;
    //		result1 = CheckTextualExpression();
    //		if (result1 == null)
    //			return null;
    //		if (result == null)
    //			result = result1;
    //		else
    //			result = ExpressionNode.Block(result, result1);
    //		if (!checkChar(','))
    //			return null;
    //		bypassWhitespace();
    //	} while (exp[index] != 0);
    //	return result;
    //}




    //private ExpressionNode CheckSimpleValue()
    //{
    //	//Rule 19. simple value = qualified name | simple literal ;
    //	ExpressionNode result = null;
    //	if (result == null)
    //		result = CheckQuantifiedName();
    //	if (result == null)
    //		result = CheckSimpleLiteral();
    //	return result;
    //}

    //private ExpressionNode CheckSimpleLiteral()
    //{
    //	//Rule 34. simple literal = numeric literal | string literal | Boolean literal ;
    //	ExpressionNode result = null;
    //	if (result == null)
    //		result = CheckNumericLiteral();
    //	if (result == null)
    //		result = CheckStringLiteral();
    //	if (result == null)
    //		result = CheckBooleanLiteral();
    //	return result;
    //}

    //private ExpressionNode CheckStringLiteral()
    //{
    //	//Rule 35. string literal = '"' , { character – ('"' | vertical space) }, '"' ;
    //	int index0 = index;
    //	if (!checkChar('"'))
    //		return null;
    //	for (int i = 0; exp[index + i] != 0; i++)
    //	{
    //		char c = exp[index + i];
    //		if (c == '"')
    //		{
    //			ExpressionNode result = ExpressionNode.Constant(new string(exp, index, i));
    //			index += i + 1;
    //			return result;
    //		}
    //		if (c == '\r' || c == '\n')//vertical space
    //		{
    //			index = index0;
    //			return null;
    //		}
    //	}
    //	index = index0;
    //	return null;
    //}

    //private ExpressionNode CheckNumericLiteral()
    //{
    //	//Rule 37. numeric literal = digits , [ ".", digits ] | "." , digits ;
    //	//Rule 38. digit = [0-9] ;
    //	//Rule 39. digits = digit , {digit} ;
    //	bool pointVisited = false;
    //	bypassWhitespace();
    //	for (int i = 0; exp[index + i] != 0; i++)
    //	{
    //		char c = exp[index + i];
    //		if (c == '.' && !pointVisited)
    //			continue;
    //		if (c >= '0' && c <= '9')
    //			continue;
    //		if (i > 0)
    //		{
    //			ExpressionNode result = ExpressionNode.Constant(double.Parse(new string(exp, index, i)));
    //			index += i;
    //			return result;
    //		}
    //		break;
    //	}
    //	return null;
    //}

    #endregion
}
