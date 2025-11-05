using CultureInfo = System.Globalization.CultureInfo;

namespace Neo.Bpms.Domain.Extensions;

/// <summary>
/// Utility functions for work with numbers
/// </summary>
public class NumberUtill
{
    /// <summary>
    /// Strings to byte array.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns></returns>
    public static byte[] StrToByteArray(string str)
    {
        System.Text.UTF8Encoding encoding = new();
        return encoding.GetBytes(str);
    }


    /// <summary>
    /// Determines whether the specified type is number.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static bool IsNumber(Type type)
    {
        return type.FullName == null
            ? false
            : type.FullName.ToLower().Replace("system.", "") switch
        {
            "decimal" or "int32" or "double" => true,
            _ => false,
        };
    }
    /// <summary>
    /// Determines whether [is na n] [the specified in value].
    /// </summary>
    /// <param name="inVal">The in value.</param>
    /// <returns></returns>
    public static bool IsNaN(object inVal)
    {
        try
        {
            string temp = inVal.ToString();
            if (string.IsNullOrEmpty(temp))
                return false;
        }
        catch
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// To the double.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static double ToDouble(string ParameterValue)
    {
        double outVal;
        try
        {
            int negative = ParameterValue.IndexOf("(", StringComparison.Ordinal) != -1 ? -1 : 1;
            ParameterValue = ParameterValue.Replace("(", "").Replace(")", "").Replace("/", ".");
            outVal = Convert.ToDouble(ParameterValue) * negative;
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }
    /// <summary>
    /// To the double.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static double ToDouble(object ParameterValue)
    {
        double outVal;
        try
        {
            outVal = Convert.ToDouble(ParameterValue.ToString().Replace("'", ""));
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }
    /// <summary>
    /// To the int.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static int ToInt(object ParameterValue)
    {
        int outVal;
        try
        {
            double TEMP = Convert.ToDouble(ParameterValue);
            outVal = Convert.ToInt32(TEMP);
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }
    /// <summary>
    /// To the short.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static short ToShort(object ParameterValue)
    {
        short outVal;
        try
        {
            double TEMP = Convert.ToDouble(ParameterValue);
            outVal = Convert.ToInt16(TEMP);
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }
    /// <summary>
    /// To the long.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static long ToLong(object ParameterValue)
    {
        long outVal;
        try
        {
            double TEMP = Convert.ToDouble(ParameterValue);
            outVal = Convert.ToInt64(TEMP);
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }

    /// <summary>
    /// Groups the digits by comma seperator.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <returns></returns>
    public static string DigitGroup(string Text)
    {
        return DigitGroup(Text, ',', '.', true);
    }
    /// <summary>
    /// Groups the digits by specific seperator and dot characters
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <param name="Seperator">The seperator.</param>
    /// <param name="dot">The dot.</param>
    /// <param name="showNegativeInParanteses">if set to <c>true</c> [show negative in paranteses].</param>
    /// <returns></returns>
    public static string DigitGroup(string Text, char Seperator, char dot, bool showNegativeInParanteses)
    {
        if (string.IsNullOrEmpty(Text)) return "";
        double num = ToDouble(Text);
        bool isnegative = num < 0;

        string[] dddddddd = Text.Split('.');
        string firstSection = dddddddd.Length > 0 ? dddddddd[0].Replace(".", "").Replace(",", "").Replace("-", "") : "";
        string secondSection = dddddddd.Length > 1 ? dddddddd[1] : "";
        string rvseStr = Reverse(firstSection);
        string rvseStrDigitGroup = "";
        int len = rvseStr.Length;
        for (int i = 0; i < len; i += 3)
        {
            if (i + 3 >= len)
                rvseStrDigitGroup += rvseStr[i..len];
            else
                rvseStrDigitGroup += rvseStr.Substring(i, 3) + Seperator;
        }
        return (isnegative ? showNegativeInParanteses ? "(" : "-" : "") + Reverse(rvseStrDigitGroup) + (secondSection != "" ? dot + secondSection : "") + (isnegative ? showNegativeInParanteses ? ")" : "" : "");
    }
    /// <summary>
    /// Reverses the specified instr.
    /// </summary>
    /// <param name="instr">The instr.</param>
    /// <returns></returns>
    public static string Reverse(string instr)
    {
        string outval = "";
        for (int i = instr.Length - 1; i >= 0; i--)
            outval += instr[i];
        return outval;
    }
    /// <summary>
    /// Parses the number.
    /// </summary>
    /// <param name="v">The v.</param>
    /// <returns></returns>
    public static string ParseNumber(string v)
    {
        try
        {
            decimal ParseNumber = Convert.ToInt64(v);
            return ParseNumber.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            return "0";
        }
    }

    /// <summary>
    /// Removes the comma.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="nochange">if set to <c>true</c> [nochange].</param>
    /// <returns></returns>
    public static int RemoveCama(string value, bool nochange)
    {
        //	alert(String(value) + '\n' + String(value).length)
        if (value == "")
            return 0;
        value = value.Replace("/", ".");
        value = value.Replace("&8237;", "");
        value = value.Replace("&8238;", "");
        int negative = 1;
        if (value.Contains("("))
        {
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            negative = -1;
        }
        value = value.Replace("/", "");
        return nochange ? ToInt(value) : ToInt(value) * negative;
    }
    /// <summary>
    /// Maximums the specified a.
    /// </summary>
    /// <param name="a">A.</param>
    /// <param name="b">The b.</param>
    /// <param name="c">The c.</param>
    /// <param name="d">The d.</param>
    /// <returns></returns>
    public static int Max(int a, int b, int c, int d)
    {
        int max = a;
        if (b > max)
            max = b;
        if (c > max)
            max = c;
        if (d > max)
            max = d;
        return max;
    }
    public static double Round(double value, int ResolutionType)
    {
        double OutVal = value;

        int Type = ResolutionType > 40 ? 1 : ResolutionType > 20 ? 3 : ResolutionType > 0 ? 2 : 0;//تعیین کننده نوع روند
        int Type1 = ResolutionType % 20;//تعیین کننده تعداد صفر یا تعداد اعشار
        int FZ = Type1 < 7 ? 1 : 0;//صفر یا اعشار؟
        int Resolution = FZ == 1 ? 7 - Type1 : Type1 - 7;//تعداد صفر یا تعداد اعشار
        int res = 1;
        for (int i = 0; i < Resolution; res *= 10, i++) { }//ده به توان تعداد صفر یا تعداد اعشار

        switch (Type)//نوع روند
        {
            case 1://round
                if (FZ == 1)//تعداد صفر
                    OutVal = Math.Round(value / res) * res;
                else//تعداد اعشار
                    OutVal = Math.Round(value * res) / res;
                break;
            case 2://up
                if (FZ == 1)//تعداد صفر
                    OutVal = Math.Ceiling(value / res) * res;
                else//تعداد اعشار
                    OutVal = Math.Ceiling(value * res) / res;

                break;
            case 3://down
                if (FZ == 1)//تعداد صفر
                    OutVal = Math.Truncate(value / res) * res;
                else//تعداد اعشار
                    OutVal = Math.Truncate(value * res) / res;
                break;
        }
        return OutVal;
    }

    public static bool IsDigit(string text, bool validDouble)
    {
        foreach (char ch in text)
        {
            if (ch >= '0' && ch <= '9') continue;
            if (validDouble && ch == '.') continue;
            return false;
        }
        return true;
    }



    /// <summary>
    /// To the unsigned integer.
    /// </summary>
    /// <param name="ParameterValue">The parameter value.</param>
    /// <returns></returns>
    public static uint ToUInt(string ParameterValue)
    {
        uint outVal;
        try
        {
            outVal = Convert.ToUInt32(ParameterValue);
        }
        catch
        {
            outVal = 0;
        }
        return outVal;
    }
}
/// <summary>
/// string utilities
/// </summary>
public class StringUtill
{
    /// <summary>
    /// Normalaizes the specified string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns></returns>
    public static string Normalaize(string str)
    {
        return str.Replace('ک', 'ك');
    }

    /// <summary>
    /// Capitalizes the specified s.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    public static string Capitalize(string s)
    {
        string outval = "";
        if (string.IsNullOrEmpty(s)) return outval;
        string temp = s.Trim();
        string[] arrtemp = temp.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        temp = "";
        foreach (string ss in arrtemp)
            temp += ss[..1].ToUpper() + ss[1..];
        outval += temp;
        return outval;
    }
}
