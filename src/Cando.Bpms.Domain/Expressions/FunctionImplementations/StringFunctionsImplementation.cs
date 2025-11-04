using System.Text.RegularExpressions;

namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions
{
    #region feel string functions

    /// <summary>
    ///  بازگرداندن قسمت بریده شده از رشته ورودی از نقطه شروع تا طول ورودی
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="startPosition">شماره کاراکتر شروع</param>
    /// <param name="length">به طول</param>
    /// <returns></returns>
    public static string feel_substring(string s, int startPosition, int length)
    {
        return s.Substring(startPosition, length);
    }

    /// <summary>
    ///  بازگرداندن قسمت بریده شده از رشته ورودی از نقطه شروع تا انتها
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="startPosition">شماره کاراکتر شروع</param>
    /// <returns></returns>
    public static string feel_substring(string s, int startPosition)
    {
        return s[startPosition..];
    }

    /// <summary>
    /// بازگرداندن طول رشته ورودی
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <returns></returns>
    public static int feel_length(string s)
    {
        return s.Length;
    }

    /// <summary>
    /// تبدیل حروف رشته ورودی به حروف بزرگ
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <returns></returns>
    public static string feel_uppercase(string s)
    {
        return s.ToUpper();
    }

    /// <summary>
    /// تبدیل حروف رشته ورودی به حروف کوچک
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <returns></returns>
    public static string feel_lowercase(string s)
    {
        return s.ToLower();
    }

    /// <summary>
    /// برش رشته ورودی از ابتدا رشته تا رشته مورد نظر یافت شود
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="match">رشته مورد نظر</param>
    /// <returns></returns>
    public static string feel_substringbefor(string s, string match)
    {
        int i = s.IndexOf(match, StringComparison.Ordinal);
        return (i >= 0) ? s[..i] : "";
    }

    /// <summary>
    /// برش رشته ورودی از یافت رشته مورد نظر تا انتهای رشته ورودی
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="match">رشته مورد نظر</param>
    /// <returns></returns>
    public static string feel_substringafter(string s, string match)
    {
        int i = s.IndexOf(match, StringComparison.Ordinal);
        return (i >= 0) ? s[i..] : "";
    }

    public static string feel_replace(string input, string pattern, string replacement, string flags)
    {
        RegexOptions ro =
            ((flags.IndexOf('i') >= 0) ? RegexOptions.IgnoreCase : RegexOptions.None) |
            ((flags.IndexOf('r') >= 0) ? RegexOptions.RightToLeft : RegexOptions.None) |
            ((flags.IndexOf('m') >= 0) ? RegexOptions.Multiline : RegexOptions.None);
        Regex rx = new(pattern, ro);
        return rx.Replace(input, replacement);
    }

    public static bool feel_matches(string input, string pattern, string flags)
    {
        RegexOptions ro =
            ((flags.IndexOf('i') >= 0) ? RegexOptions.IgnoreCase : RegexOptions.None) |
            ((flags.IndexOf('r') >= 0) ? RegexOptions.RightToLeft : RegexOptions.None) |
            ((flags.IndexOf('m') >= 0) ? RegexOptions.Multiline : RegexOptions.None);
        Regex rx = new(pattern, ro);
        return rx.Matches(input).Count > 0;
    }

    public static bool feel_contains(string s, string match)
    {
        return s.IndexOf(match, StringComparison.Ordinal) >= 0;
    }

    /// <summary>
    /// مشخص میکند رشته ورودی با رشته مورد نظر شروع میشود یا خیر
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="match">رشته مورد نظر</param>
    /// <returns></returns>
    public static bool feel_startswith(string s, string match)
    {
        return s.StartsWith(match);
    }

    /// <summary>
    /// مشخص میکند رشته ورودی با رشته مورد نظر اتمام میابد یا خیر
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <param name="match">رشته مورد نظر</param>
    /// <returns></returns>
    public static bool feel_endswith(string s, string match)
    {
        return s.EndsWith(match);
    }

    #endregion feel string functions

    #region extended string functions

    public static int feel_compareTo(string s, object match)
    {
        return s.CompareTo(match);
    }

    public static int feel_indexof(string s, string match)
    {
        return s.IndexOf(match, StringComparison.Ordinal);
    }

    public static int feel_indexofany(string s, string match)
    {
        return s.IndexOfAny(match.ToCharArray());
    }

    public static string feel_insert(string s, int index, string value)
    {
        return s.Insert(index, value);
    }

    public static int feel_lastindexof(string s, string match)
    {
        return s.LastIndexOf(match, StringComparison.Ordinal);
    }

    public static int feel_lastindexofany(string s, string match)
    {
        return s.LastIndexOfAny(match.ToCharArray());
    }

    public static string feel_normalize(string s)
    {
        return s.Normalize();
    }

    public static string feel_padleft(string s, int totalWidth)
    {
        return s.PadLeft(totalWidth);
    }

    public static string feel_padright(string s, int totalWidth)
    {
        return s.PadRight(totalWidth);
    }

    public static string feel_remove(string s, int startIndex)
    {
        return s[..startIndex];
    }

    public static string feel_remove(string s, int startIndex, int count)
    {
        return s.Remove(startIndex, count);
    }

    public static string[] feel_split(string s, params string[] seperator)
    {
        return s.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
    }
    public static string SubSplit(string str, string seperator, int requestedIndex)
    {
        if (string.IsNullOrEmpty(str) || str == "undefined") return null;
        string[] s = feel_split(str, seperator);
        return (s == null || s.Length <= requestedIndex) ? null : s[requestedIndex];
    }

    public static string feel_trim(string s)
    {
        return s.Trim();
    }

    public static string feel_trimend(string s)
    {
        return s.TrimEnd();
    }

    public static string feel_trimstart(string s)
    {
        return s.TrimStart();
    }

    public static string feel_repeat(string s, int count)
    {
        string s1 = "";
        for (int i = 0; i < count; i++)
        {
            s1 += s;
        }

        return s1;
    }

    #endregion extended string functions

    #region backward compatibily

    /// <summary>
    /// طول رشته ورودی
    /// </summary>
    /// <param name="s">رشته ورودی</param>
    /// <returns></returns>
    public static int feel_strlen(object s)
    {
        return s?.ToString().Length ?? 0;
    }

    public static bool feel_strany(string s, string match)
    {
        return s.IndexOf(match, StringComparison.Ordinal) >= 0;
    }

    public static bool feel_strstart(string s, string match)
    {
        return s.StartsWith(match);
    }

    public static bool feel_strend(string s, string match)
    {
        return s.EndsWith(match);
    }

    public static string feel_mid(string s, int startPosition, int length)
    {
        return s.Substring(startPosition, length);
    }

    public static string feel_left(string s, int length)
    {
        return s[..length];
    }

    public static string feel_right(string s, int length)
    {
        int len = s.Length;
        return (length <= len) ? s.Substring(len - length, length) : s;
    }

    #endregion backward compatibility

    /// <summary>
    /// دریافت یک GUID جدید
    /// </summary>
    /// <returns></returns>
    public static string NewGuid()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// دریافت یک رشته تصادفی
    /// </summary>
    /// <returns></returns>
    public static string RandomString()
    {
        return Guid.NewGuid().ToString("n");
        //		    Convert.ToBase64String(Guid.NewGuid().ToByteArray()) alternative solution for longer strings!
    }

    /// <summary>
    /// به هم چسباندن رشته های ورودی 
    /// </summary>
    /// <param name="strs"></param>
    /// <returns></returns>
    public static string Concat(params object[] strs)
    {
        return strs.Aggregate("", (current, str) => current + str?.ToString());
    }

    /// <summary>
    /// تبدیل عبارت ورودی به رشته
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Constant(object str)
    {
        return str?.ToString();
    }

    /// <summary>
    /// تبدیل عبارت به 'عبارت'
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Quotify(object str)
    {
        return $"'{str}'";
    }
    public static bool StartsWith(string s, string match)
    {
        return s.StartsWith(match);
    }

    public static bool RegexIsMatch(string input, string pattern)
    {
        return Regex.IsMatch(input, pattern);
    }
}
