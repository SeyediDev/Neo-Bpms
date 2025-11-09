using System.Collections;

namespace Neo.Bpms.Infrastructure.Features.Orm.EntityExpressionFuncs;

public partial class BuiltInFunctions : IFunctionImplementations
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

    /// <summary>
    /// بررسی وجود رکورد
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط بررسی</param>
    /// <returns></returns>
    public static bool ExistsWhere(string fullEntityId, string whereClause)
    {
        return CountWhere(fullEntityId, whereClause) > 0;
    }

    /// <summary>
    /// دریافت تعداد رکوردهای موجودیت با شرایط مشخص
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط بررسی</param>
    /// <returns></returns>
    public static long CountWhere(string fullEntityId, string whereClause)
    {
        (string namespaceId, string entityId) = GetNamespaceEntity(fullEntityId);
        return new QueryUtility(namespaceId, entityId).Count("*", "c")
            .Where(whereClause).FirstOrDefault()?.GetLong("c") ?? 0;
    }

    public static long ListCount(object listObject)
    {
        if (listObject is IList list)
        {
            return list.Count;
        }
        return 0;
    }
    public static object First(object listObject)
    {
        if (listObject is IList list)
        {
            return list.Count > 0 ? list[0] : null;
        }
        return null;
    }

    /// <summary>
    /// دریافت فیلد رکورد خاص
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="id">شناسه رکورد</param>
    /// <param name="requestFormula">فیلد درخواستی</param>
    /// <returns></returns>
    public static object Fetch(string fullEntityId, object id, string requestFormula)
    {
        ElasticObject docInfo = DocInfo(fullEntityId, id, requestFormula) as ElasticObject;
        object value = null;
        docInfo?.GetField("F1", out value);
        return value;
    }

    /// <summary>
    /// دریافت فیلد رکورد با شرط
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط</param>
    /// <param name="requestFormula">فیلد درخواستی</param>
    /// <param name="orderBy">مرتب سازی بر اساس فیلد</param>
    /// <returns></returns>
    public static object FetchWhere(string fullEntityId, string whereClause,
        string requestFormula, string orderBy = null)
    {
        List<object> list = Query(fullEntityId, whereClause,
            string.IsNullOrEmpty(orderBy) ? null : orderBy, false, 1, requestFormula);
        if (list == null || list.Count == 0)
        {
            return null;
        }

        ElasticObject result = list[0] as ElasticObject;
        object value = null;
        result?.GetField("F1", out value);
        return value;
    }

    public static object FetchAggregration(string fullEntityId, string whereClause, string requestFormula)
    {
        List<object> list = Query(fullEntityId, whereClause, null, false, 0, requestFormula);
        if (list == null || list.Count == 0)
        {
            return null;
        }

        ElasticObject result = list[0] as ElasticObject;
        object value = null;
        result?.GetField("F1", out value);
        return value;
    }
    /// <summary>
    /// دریافت فیلد های رکورد خاص
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="id">شناسه رکورد</param>
    /// <param name="requestFormulas">فیلد های درخواستی</param>
    /// <returns></returns>
    public static object DocInfo(string fullEntityId, object id, params string[] requestFormulas)
    {
        if (id?.ToString() == "undefined")
        {
            return null;
        }

        return Find(fullEntityId, "Id='" + id + "'", requestFormulas);
    }

    /// <summary>
    /// دریافت فیلد های رکورد با شرط
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط</param>
    /// <param name="requestFormulas">فیلد های درخواستی</param>
    /// <returns></returns>
    public static object Find(string fullEntityId, string whereClause, params string[] requestFormulas)
    {
        List<object> list = Query(fullEntityId, whereClause, null, false, 1, requestFormulas);
        if (list == null || list.Count == 0)
        {
            ElasticObject e = new();
            int formulaIndex = 1;
            foreach (string requestFormula in requestFormulas)
            {
                string f = "F" + formulaIndex++;
                e[f] = null;
                e[requestFormula] = null;
            }

            return e;
        }

        return list[0];
    }

    /// <summary>
    /// دریافت فیلد های رکورد با شرط
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط</param>
    /// <param name="orderBy">
    /// مرتب سازی بر اساس فیلد
    /// با الگویِ
    /// FieldName
    /// یا
    /// FieldName DESC
    /// یا
    /// FieldName,FieldName2
    /// </param>
    /// <param name="bDistinct">Distinct</param>
    /// <param name="top">حداکثر تعداد رکورد های خروجی</param>
    /// <param name="requestFormulas">فیلد های درخواستی</param>
    /// <returns></returns>
    public static List<object> Query(string fullEntityId, string whereClause,
        string orderBy, object bDistinct, int top, params string[] requestFormulas)
    {
        try
        {
            (string namespaceId, string entityId) = GetNamespaceEntity(fullEntityId);
            QueryUtility q = new(namespaceId, entityId);
            int formulaIndex = 1;
            foreach (string requestFormula in requestFormulas)
            {
                q.SelectFormulaField("F" + formulaIndex++, requestFormula);
            }

            if (!string.IsNullOrEmpty(whereClause))
            {
                q.AddFilter(whereClause);
            }

            AddOrderBy(orderBy, q);
            if (bDistinct?.ToString() == "undefined")
            {
                q.SetDistinct(false);
            }
            else if (ConvUtill.ToBoolean(bDistinct))
            {
                q.SetDistinct(true);
            }

            if (top > 0)
            {
                q.topRows = top;
            }

            LocalParameters localVariables = [];
            if (!q.GetDocuments(localVariables))
            {
                return null;
            }

            List<object> values = [];
            while (true)
            {
                ElasticObject r = q.GetRecord();
                if (r == null)
                {
                    break;
                }

                formulaIndex = 1;
                foreach (string requestFormula in requestFormulas)
                {
                    string f = "F" + formulaIndex++;
                    if (!r.GetField(f, out object v))
                    {
                        r.SetField(f, null);
                    }

                    r.SetField(requestFormula, v);
                }

                values.Add(r);
            }

            q.ReleaseQuery();
            return values;
        }
        catch (Exception e)
        {
            Logger.LogError(
                $"can not load entity {fullEntityId} in formula.(Query, Find, DocInfo, Fetch, FetchWhere, ...)");
            Logger.LogError(e, e.Message);
        }

        return null;
    }
    public static List<object> QueryZ(string fullEntityId, string whereClause,
        string orderBy, object bDistinct, int top, string requestFormula, params string[] zs)
    {
        try
        {
            (string namespaceId, string entityId) = GetNamespaceEntity(fullEntityId);
            QueryUtility q = new(namespaceId, entityId);
            string resName = "F1";
            q.SelectFormulaField(resName, requestFormula);
            if (!string.IsNullOrEmpty(whereClause))
            {
                q.AddFilter(whereClause);
            }

            AddOrderBy(orderBy, q);
            if (bDistinct?.ToString() == "undefined")
            {
                q.SetDistinct(false);
            }
            else if (ConvUtill.ToBoolean(bDistinct))
            {
                q.SetDistinct(true);
            }

            if (top > 0)
            {
                q.topRows = top;
            }

            LocalParameters localVariables = [];
            int zIndex = 1;
            foreach (string z in zs)
            {
                localVariables.Add("Z" + zIndex++, z);
            }

            if (!q.GetDocuments(localVariables))
            {
                return null;
            }

            List<object> values = [];
            while (true)
            {
                ElasticObject r = q.GetRecord();
                if (r == null)
                {
                    break;
                }

                if (!r.GetField(resName, out object v))
                {
                    v = null;
                }

                values.Add(v);
            }

            q.ReleaseQuery();
            return values;
        }
        catch (Exception e)
        {
            Logger.LogError(
                $"can not load entity {fullEntityId} in formula.(Query, Find, DocInfo, Fetch, FetchWhere, ...)");
            Logger.LogError(e, e.Message);
        }

        return null;
    }

    private static (string namespaceId, string entityId) GetNamespaceEntity(string fullEntityId)
    {
        string[] entityIds = fullEntityId.Split('.');
        string namespaceId = entityIds.Length == 2 ? entityIds[0] : "";
        string entityId = entityIds.Length == 2 ? entityIds[1] : entityIds[0];
        return (namespaceId, entityId);
    }

    private static void AddOrderBy(string orderBy, QueryUtility q)
    {
        if (!string.IsNullOrEmpty(orderBy))
        {
            string[] sfs = orderBy.Split(',');
            foreach (string item in sfs)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }

                string[] sf = item.Split(' ');
                q.OrderBy(sf[0],
                    sf.Length > 1 && sf[1].ToUpper() == "DESC"
                        ? SortType.Descending
                        : SortType.Ascending);
            }
        }
    }

    /// <summary>
    /// دریافت فیلد های از رکورد با شرط بصورت گروه بندی
    /// </summary>
    /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
    /// <param name="whereClause">شرط</param>
    /// <param name="orderBy">مرتب سازی بر اساس فیلد</param>
    /// <param name="bDistinct">Distinct</param>
    /// <param name="top">حداکثر تعداد رکورد های خروجی</param>
    /// <param name="groupByField">فیلد گروه بندی</param>
    /// <param name="requestFormulas">فیلد های درخواستی</param>
    /// <returns></returns>
    public static List<object> GroupByQuery(string fullEntityId, string whereClause,
        object orderBy, object bDistinct, int top, string groupByField, params string[] requestFormulas)
    {
        string[] entityIds = fullEntityId.Split('.');
        string namespaceId = entityIds.Length == 2 ? entityIds[0] : "";
        string entityId = entityIds.Length == 2 ? entityIds[1] : entityIds[0];
        QueryUtility q = new(namespaceId, entityId);
        int formulaIndex = 1;
        if (!string.IsNullOrEmpty(groupByField))
        {
            q.GroupBy(groupByField, true);
        }

        foreach (string requestFormula in requestFormulas)
        {
            q.GroupByFormula(requestFormula, eAggregationFunctions.Formula, eAggregateScope.All, "F" + formulaIndex++);
        }

        if (!string.IsNullOrEmpty(whereClause))
        {
            q.AddFilter(whereClause);
        }

        if (!string.IsNullOrEmpty(orderBy?.ToString()))
        {
            q.OrderBy(orderBy.ToString());
        }

        if (bDistinct?.ToString() == "undefined")
        {
            q.SetDistinct(false);
        }
        else if (ConvUtill.ToBoolean(bDistinct))
        {
            q.SetDistinct(true);
        }

        if (top > 0)
        {
            q.topRows = top;
        }

        LocalParameters localVariables = [];
        if (!q.GetDocuments(localVariables))
        {
            return null;
        }

        List<object> values = [];
        while (true)
        {
            ElasticObject r = q.GetRecord();
            if (r == null)
            {
                break;
            }

            formulaIndex = 1;
            foreach (string requestFormula in requestFormulas)
            {
                string f = "F" + formulaIndex++;
                if (!r.GetField(f, out object v))
                {
                    r.SetField(f, null);
                }

                r.SetField(requestFormula, v);
            }

            values.Add(r);
        }

        q.ReleaseQuery();
        return values;
    }

    public static object DeleteRecords(LocalParameters lp, string fullEntityId, string whereClause)
    {
        string[] entityIds = fullEntityId.Split('.');
        string namespaceId = entityIds.Length == 2 ? entityIds[0] : "";
        string entityId = entityIds.Length == 2 ? entityIds[1] : entityIds[0];
        ApplyUtility au = new(namespaceId, entityId, (IdentityUser)lp.AuditUser, lp);
        return whereClause != null && au.DeleteWithFilter(whereClause);
    }

    public static object UpdateRecords(LocalParameters lp, string fullEntityId, string whereClause, object record)
    {
        //todo unSecure bayad choni andishid barqash
        string[] entityIds = fullEntityId.Split('.');
        string namespaceId = entityIds.Length == 2 ? entityIds[0] : "";
        string entityId = entityIds.Length == 2 ? entityIds[1] : entityIds[0];
        ElasticObject r = record as ElasticObject ?? new ElasticObject("", record);
        ApplyUtility au = new(namespaceId, entityId, (IdentityUser)lp.AuditUser, lp);
        return au.UpdateWithFilter(whereClause, r);
    }

    private LocalParameters _localVariables;

    public object Select(string entities, string requestFormula, string whereClause)
    {
        QueryUtility q = new("", entities);
        q.SelectFormulaField("SJVS", requestFormula);
        if (!string.IsNullOrEmpty(whereClause))
        {
            q.AddFilter(whereClause);
        }

        _localVariables ??= [];

        if (!q.GetDocuments(_localVariables))
        {
            return null;
        }

        List<object> values = [];
        while (true)
        {
            ElasticObject r = q.GetRecord();
            if (r == null)
            {
                break;
            }

            values.Add(r["SJVS"]);
        }

        q.ReleaseQuery();
        if (values.Count == 1)
        {
            return values[0];
        }
        else if (values.Count > 0)
        {
            return values;
        }

        return "";
    }
}
