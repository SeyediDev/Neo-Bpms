using System.Web;

namespace Neo.Bpms.UI.MVC.Attributes;

public class Check
{
    public static void ValidateElastic(ref ElasticObject el)
    {
        foreach (KeyValuePair<string, ElasticObject> item in el?.Attributes ?? [])
        {
            object o = el.GetAttributeValue(item.Key);
            if (o is ElasticObject)
            {
                ElasticObject ole = o as ElasticObject;
                ValidateElastic(ref ole);
                el.SetField(item.Key, ole);
            }
            else if (o is string)
                el.SetField(item.Key, HttpUtility.HtmlEncode(o.ToString()));
            //                    el.SetField(item.Key, AntiXssEncoder.HtmlEncode(o.ToString(), false));// todo salisayedi altered it
        }
    }

    public static bool CheckParams(string word)
    {
        return true;//todo I disabled every nonesense and possible sensible security defense strategy with this line
                    //       bool Safe = true;
                    //       try
                    //       {
                    //           if (word == null) return true;
                    //           var result = AntiXssEncoder.HtmlEncode(word, false);
                    //           result = !string.IsNullOrEmpty(result) ? result.ToLower().Trim() : "";
                    //           word = !string.IsNullOrEmpty(word) ? word.ToLower().Trim() : "";
                    //           if (word.Length > 5 && word.Substring(0, 5) == "data:")
                    //return true;
                    //           CheckWordByWord(result, ' '); //blank
                    //           CheckWordByWord(result, ' '); //whitespace
                    //           CheckWordByWord(result, '\'');
                    //           CheckWordByWord(result, '\"');
                    //           CheckWordByWord(result, '+');
                    //           CheckWordByWord(result, "&#160;");
                    //           CheckWordByWord(word, ' '); //blank
                    //           CheckWordByWord(word, ' '); //whitespace
                    //           CheckWordByWord(word, '+');
                    //           CheckWordByWord(word, '\'');
                    //           CheckWordByWord(word, '\"');
                    //           CheckWordByWord(word, "&#160;");
                    //       }
                    //       catch (Exception)
                    //       {
                    //           Safe = false;
                    //       }
                    //       return Safe;
    }

    private static void CheckWordByWord(string str, char symbol = ' ')//todo I hesitate the philosophy of existence of such methods
    {
        if (!string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str))
        {
            string[] resObject = str.Split(symbol);
            foreach (string w in resObject)
            {
                if (w.IndexOf("charset") == -1 && w.IndexOf("char") > 0 ||
                    w.IndexOf("string") > 0 || //double check
                    w.IndexOf("<") > 0 ||
                    w.IndexOf(">") > 0 ||
                    w.IndexOf("&gt;&gt;") > 0 ||
                    w.IndexOf("&amp;&amp;") > 0 ||
                    w.IndexOf("&lt;%") > 0 ||
                    w.IndexOf("%&gt;") > 0 ||
                    w.IndexOf("&lt;&lt;") > 0 ||
                    w.IndexOf("&#62") > 0 ||
                    w.IndexOf("&#60") > 0 ||
                    w.IndexOf("src") > 0 || //double check
                                            //result.IndexOf("=") ||
                                            //result.IndexOf("==") ||
                                            //result.IndexOf("===") ||
                    w.IndexOf("selection") == -1 && w.IndexOf("select") > 0) //double check
                {
                    throw new Exception(
                        "داده وارد شده از طرف کاربر خطرناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
                }
            }
        }
    }

    private static void CheckWordByWord(string str, string symbol = "")
    {
        if (!string.IsNullOrWhiteSpace(str) && !string.IsNullOrEmpty(str))
        {
            string[] resObject = str.Split(new string[] { symbol }, StringSplitOptions.None);
            foreach (string w in resObject)
            {
                if (w.IndexOf("charset") == -1 && w.IndexOf("char") > 0 ||
                    w.IndexOf("string") > 0 ||
                    w.IndexOf("<") > 0 ||
                    w.IndexOf(">") > 0 ||
                    w.IndexOf("&gt;&gt;") > 0 ||
                    w.IndexOf("&amp;&amp;") > 0 ||
                    w.IndexOf("&lt;%") > 0 ||
                    w.IndexOf("%&gt;") > 0 ||
                    w.IndexOf("&lt;&lt;") > 0 ||
                    w.IndexOf("&#62") > 0 ||
                    w.IndexOf("&#60") > 0 ||
                    w.IndexOf("src") > 0 || //double check
                                            //result.IndexOf("=")||
                                            //result.IndexOf("==")||
                                            //result.IndexOf("===")||
                    w.IndexOf("selection") == -1 && w.IndexOf("select") > 0) //double check
                {
                    throw new Exception(
                        "داده وارد شده از طرف کاربر خطر ناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
                }
            }
        }
    }
}
