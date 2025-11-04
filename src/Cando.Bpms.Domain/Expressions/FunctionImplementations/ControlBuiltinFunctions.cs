using Neo.Bpms.Domain.Expressions.FunctionImplementations;
using Neo.Bpms.Domain.Expressions.Parsers;

namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    #region feel boolean functions
    /// <summary>
    /// معکوس کردن مقدار فیلد بولین
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool not(bool e)
    {
        return !e;
    }
    #endregion feel boolean functions
    #region farasa public functions
    //public static object func_if(bool condition, object thenValue, object elseValue)
    //{
    //	return condition ? thenValue : elseValue;
    //}
    //public static object func_switch(object criteria, object defaultValue, params object[] caseValues)
    //{
    //	for (int i = 0; i < caseValues.Length; i += 2)
    //	{
    //		if (criteria == caseValues[i])
    //			return caseValues[i + 1];
    //	}
    //	return defaultValue;
    //}
    public static int equal(object criteria, params object[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (criteria == values[i])
                return i + 1;//it is better than 1
        }
        return 0;
    }
    public static object eval(object obj, string expression)
    {
        ExpressionNode expTree = Parser.Parse(expression);

        return expTree.Eval(obj, []);//todo
    }
    public static object eval(string expression)
    {
        return eval(null, expression);
    }
    public static string checkformula(string expression)
    {
        Parser parser = new(expression);
        try
        {
            parser.Complie(0, 0);
        }
        catch (ParserException e)
        {
            return e.Message;
        }
        //TODO: output structure must be checked
        return "";
    }
    #endregion farasa public functions
    /// <summary>
    /// بررسی null ‌یا خالی بودن عبارت
    /// </summary>
    /// <param name="expression">عبارت</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(object expression) => ControlFunctionsImplementation.IsNullOrEmpty(expression);
    /// <summary>
    /// بازگشت مقدار پیشفرض در صورت null بودن عبارت
    /// </summary>
    /// <param name="expression">عبارت</param>
    /// <param name="defaultValue">مقدار پیشفرض</param>
    /// <returns></returns>
    public static object IsNull(object expression, object defaultValue) => ControlFunctionsImplementation.IsNull(expression, defaultValue);
    public static object ISNULL(object expression, object defaultValue) => ControlFunctionsImplementation.IsNull(expression, defaultValue);
    /// <summary>
    /// تبدیل عبارت ورودی به عدد صحیح 
    /// </summary>
    /// <param name="v">عبارت</param>
    /// <returns></returns>
    public static long ToInt(object v) => ControlFunctionsImplementation.ToInt(v);
    /// <summary>
    /// تبدیل عبارت ورودی به عدد اعشاری
    /// </summary>
    /// <param name="v">عبارت</param>
    /// <returns></returns>
    public static double ToDouble(object v) => ControlFunctionsImplementation.ToDouble(v);
    /// <summary>
    /// مجموع دو عبارت ورودی
    /// </summary>
    /// <param name="i1">عبارت اول</param>
    /// <param name="i2">عبارت دوم</param>
    /// <returns></returns>
    public static double Plus(object i1, object i2) => ControlFunctionsImplementation.Plus(i1, i2);
}
