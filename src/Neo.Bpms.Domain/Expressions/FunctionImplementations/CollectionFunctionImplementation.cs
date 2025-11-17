namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    #region feel list functions
    public static bool feel_contains(IEnumerable<object> list, object element)
    {
        foreach (object item in list)
        {
            if (item == element)
                return true;
        }
        return false;
    }
    public static int feel_count(IEnumerable<object> list)
    {
        return list.Count();
    }
    public static object feel_min(IEnumerable<object> list)
    {
        return list.Min();
    }
    public static object feel_max(IEnumerable<object> list)
    {
        return list.Max();
    }
    public static double feel_sum(IEnumerable<object> list)
    {
        return list.Sum(o => (double)o);
    }
    public static double feel_mean(IEnumerable<object> list)
    {
        return list.Average(o => (double)o);
    }
    public static bool feel_and(IEnumerable<object> list)
    {
        foreach (object item in list)
        {
            if (!feel_bool(item))
                return false;
        }
        return true;
    }
    public static bool feel_or(IEnumerable<object> list)
    {
        foreach (object item in list)
        {
            if (feel_bool(item))
                return true;
        }
        return false;
    }
    public static IEnumerable<object> feel_sublist(IEnumerable<object> list, int startPosition, int length)
    {
        return list.Skip(startPosition).Take(length);
    }
    public static IEnumerable<object> feel_sublist(IEnumerable<object> list, int startPosition)
    {
        return list.Skip(startPosition);
    }
    /// <summary>
    /// concatenate in feel
    /// </summary>
    public static IEnumerable<object> feel_append(IEnumerable<object> list, params IEnumerable<object>[] second)
    {
        foreach (IEnumerable<object> item in second)
            list = list.Concat(item);
        return list;
    }
    public static IEnumerable<object> feel_append(IEnumerable<object> list, params object[] second)
    {
        foreach (object item in second)
        {
            List<object> lst = [item];
            list = list.Concat(lst);
        }
        return list;
    }
    /// <summary>
    /// insertBefore in feel
    /// </summary>
    public static List<object> feel_insert(IEnumerable<object> list, int index, params object[] second)
    {
        List<object> first = [.. list];
        foreach (object item in second)
        {
            first.Insert(index, item);
            index++;
        }
        return first;
    }
    public static List<object> feel_insert(IEnumerable<object> list, int index, params IEnumerable<object>[] second)
    {
        List<object> first = [.. list];
        foreach (IEnumerable<object> item in second)
        {
            first.InsertRange(index, item);
            index += item.Count();
        }
        return first;
    }
    public static List<object> feel_remove(IEnumerable<object> list, int index)
    {
        List<object> first = [.. list];
        first.RemoveAt(index);
        return first;
    }
    public static IEnumerable<object> feel_reverse(IEnumerable<object> list)
    {
        return list.Reverse();
    }
    public static List<int> feel_indexof(IEnumerable<object> list, object match)
    {
        List<int> idxes = [];
        int i = 0;
        foreach (object item in list)
        {
            if (item == match)
                idxes.Add(i);
            i++;
        }
        return idxes;
    }
    public static IEnumerable<object> feel_union(IEnumerable<object> list, params IEnumerable<object>[] second)
    {
        foreach (IEnumerable<object> item in second)
            list.Union(item);
        return list;
    }
    public static IEnumerable<object> feel_distinct(IEnumerable<object> list)
    {
        return list.Distinct();
    }
    public static IEnumerable<object> feel_flatten(IEnumerable<object> list)
    {
        List<int> idxes = [];
        List<IEnumerable<object>> lsts = [];
        int i = 0;
        List<object> lists = [.. list];
        foreach (object item in lists)
        {
            IEnumerable<object> lst = item as IEnumerable<object>;
            if (lst != null)
            {
                lst = feel_flatten(lst);
                lsts.Add(lst);
                idxes.Add(i);
            }
            i++;
        }
        int j = 0;
        i = 0;
        foreach (IEnumerable<object> item in lsts)
        {
            int idx = idxes[i];
            lists.RemoveAt(idx + j);
            foreach (object subitem in item)
            {
                lists.Insert(idx + j, subitem);
                j++;
            }
            j--;
            i++;
        }
        return lists;
    }
    #endregion feel list functions
    #region extended list functions
    public static IEnumerable<object> feel_except(IEnumerable<object> list, params IEnumerable<object>[] second)
    {
        foreach (IEnumerable<object> item in second)
            list = list.Intersect(item);
        return list;
    }
    public static IEnumerable<object> feel_intersect(IEnumerable<object> list, params IEnumerable<object>[] second)
    {
        foreach (IEnumerable<object> item in second)
            list = list.Intersect(item);
        return list;
    }
    public static IEnumerable<object> feel_sort(IEnumerable<object> list, string key)
    {
        return list.OrderBy(o => ExpressionNode.GetPropertyValue(o, key));
    }
    public static object feel_aggregate(IEnumerable<object> list, ExpressionNode expression)
    {
        LocalParameters localVariables = new() { { "aggr", null }, { "value", null } }; //todo
        return list.Aggregate((o, o1) => { localVariables["aggr"] = o; localVariables["value"] = o1; return expression.Eval(o, localVariables); });
    }
    public static IEnumerable<object> feel_where(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.Where(o => (bool)filter.Eval(o, localVariables));
    }
    public static int feel_count(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.Count(o => (bool)filter.Eval(o, localVariables));
    }
    public static object feel_first(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.FirstOrDefault(o => (bool)filter.Eval(o, localVariables));
    }
    public static object feel_last(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.LastOrDefault(o => (bool)filter.Eval(o, localVariables));
    }
    public static IEnumerable<object> feel_takewhile(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.TakeWhile(o => (bool)filter.Eval(o, localVariables));
    }
    public static IEnumerable<object> feel_skipwhile(IEnumerable<object> list, ExpressionNode filter)
    {
        LocalParameters localVariables = [];//todo
        return list.SkipWhile(o => (bool)filter.Eval(o, localVariables));
    }
    public static IEnumerable<object> feel_groupby(IEnumerable<object> list, string key)
    {
        LocalParameters localVariables = [];//todo
        return list.GroupBy(o => ExpressionNode.GetPropertyValue(o, key));
    }
    public static void feel_foreach(IEnumerable<object> list, ExpressionNode func)
    {
        LocalParameters localVariables = [];//todo
        object outVal = null;
        foreach (object item in list)
        {
            outVal = func.Eval(item, localVariables);
        }
    }
    #endregion extended list functions
    public static string Join(string separator, object value, string fieldId)
    {
        if (value is IEnumerable<object> list)
        {
            return string.Join(separator, list.Select(item =>
                {
                    dynamic d = item;
                    return d != null ? d[fieldId] : "";
                }
            ).ToList());
        }
        dynamic dy = value;
        return Convert.ToString(dy?[fieldId] ?? "");
    }
}
