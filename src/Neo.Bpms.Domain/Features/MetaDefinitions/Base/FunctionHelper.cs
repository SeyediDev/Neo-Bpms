namespace Neo.Bpms.Domain.Features.Definitions.Entities;

public abstract partial class BaseModelingDefinition
{
    #region Formula writing Helpers
    /// <summary>
    /// متد‌هایِ زیرمجموعه‌یِ این کلاس اکسپرشن تولید می‌کنند.
    /// با استفاده از آن‌ها اکسپرشن‌ها هم تا حدِ امکان
    /// type-safe
    /// خواهند بود.
    /// </summary>
    public static class Func
    {
        #region Formula writing Helpers
        /// <summary>
        /// IF
        /// </summary>
        /// <param name="condition">شرط</param>
        /// <param name="trueExp">حاصل در صورت درست بودن شرط</param>
        /// <param name="falseExp">حاصل در صورت نادرست بودن شرط</param>
        /// <returns></returns>
        public static string IF(string condition, string trueExp, string falseExp)
        {
            return "IF((" + condition + ");(" + trueExp + ");(" + falseExp + "))";
        }

        /*public static string Switch(string caseValueFormula, string defaultValue, 
				string caseItem1, string caseItem1Value)
			{
				switch (caseValueFormula)
				{
					case "caseItem1":
						return caseItem1Value;
                default:
						return defaultValue;
				}
				return "Switch((" + condition + ");(" + trueExp + ");(" + falseExp + "))";
			}*/

        /// <summary>
        /// Helper to get List of Items in formulas. see sample usages in meta definitions.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string List(params string[] items)
        {
            var txt = "List(";
            var bFirst = true;
            foreach (var item in items)
            {
                if (!bFirst) txt += ",";
                txt += item;
                bFirst = false;
            }

            return txt + ")";
        }

        /// <summary>
        /// دریافت فلیدهای یک رکورد خاص در موجودیت
        /// </summary>
        /// <param name="entityId">مشخصات کامل موجودیت(نام فضای نامی.نام موجودیت)</param>
        /// <param name="id">شناسه رکورد</param>
        /// <param name="requestFormulas">فیلد های درخواستی</param>
        /// <returns></returns>
        public static string DocInfo(string entityId, string id, params string[] requestFormulas)
        {
            return First(Query(entityId, "Id='" + id + "'", null, false, 1, requestFormulas));
        }
        public static string DocInfoZ(string entityId, string id, string requestFormula)
        {
            return First(QueryZ(entityId, "Id=Z1", null, false, 1, requestFormula, id));
        }
        public static string First(string requestFormula)
        {
            return $"First({requestFormula})";
        }

        /// <summary>
        /// دریافت فیلد های رکورد با شرط
        /// </summary>
        /// <param name="entityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
        /// <param name="whereClause">شرط</param>
        /// <param name="orderBy">مرتب سازی بر اساس فیلد</param>
        /// <param name="bDistinct">Distinct</param>
        /// <param name="top">حداکثر تعداد رکورد های خروجی</param>
        /// <param name="requestFormulas">فیلد های درخواستی</param>
        /// <returns></returns>
        public static string Query(string entityId, string whereClause, string orderBy, bool bDistinct, int top,
            params string[] requestFormulas)
        {
            var txt = "Query('" + entityId + "'";
            if (!string.IsNullOrEmpty(whereClause)) txt += ",\"" + whereClause + "\"";
            else txt += ",null";
            if (!string.IsNullOrEmpty(orderBy)) txt += ",'" + orderBy + "'";
            else txt += ",''";
            if (bDistinct) txt += ",(true)";
            else txt += ",(false)";
            if (top != 0) txt += "," + top;
            else txt += ",0";
            txt = (requestFormulas ?? ["Id"]).Aggregate(txt, (current, requestFormula)
                  => current + (",'" + requestFormula + "'"));
            txt += ")";
            return txt;
        }

        public static string QueryZ(string entityId, string whereClause, string orderBy, bool bDistinct, int top,
            string requestFormula, params string[] zs)
        {
            var txt = "QueryZ('" + entityId + "'";
            if (!string.IsNullOrEmpty(whereClause)) txt += ",\"" + whereClause + "\"";
            else txt += ",null";
            if (!string.IsNullOrEmpty(orderBy)) txt += ",'" + orderBy + "'";
            else txt += ",''";
            if (bDistinct) txt += ",(true)";
            else txt += ",(false)";
            if (top != 0) txt += "," + top;
            else txt += ",0";
            txt += ",'" + requestFormula + "'";
            txt = (zs ?? ["-"]).Aggregate(txt, (current, z)
                  => current + ("," + z));
            txt += ")";
            return txt;
        }
        /// <summary>
        /// Helper to write database select command in formulas. see sample usages in meta definitions.
        /// </summary>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="orderby">The orderBy.</param>
        /// <param name="bDistinct">if set to <c>true</c> [b distinct].</param>
        /// <param name="top">The top.</param>
        /// <param name="requestFormulas"></param>
        /// <returns></returns>
        public static string Query<T>(string whereClause, string orderby, bool bDistinct, int top,
            params string[] requestFormulas)
        {
            var t = typeof(T);
            var txt = "Query('" + t.Name + "'";
            if (!string.IsNullOrEmpty(whereClause)) txt += ",\"" + whereClause + "\"";
            else txt += ",null";
            if (!string.IsNullOrEmpty(orderby)) txt += ",'" + orderby + "'";
            else txt += ",''";
            if (bDistinct) txt += ",(true)";
            else txt += ",(false)";
            if (top != 0) txt += "," + top;
            else txt += ",0";
            txt = (requestFormulas ?? ["Id"]).Aggregate(txt, (current, requestFormula)
                  => current + (",'" + requestFormula + "'"));
            txt += ")";
            return txt;
        }

        /// <summary>
        /// به پیوستن چند رشته
        /// </summary>
        /// <param name="items">رشته های ورودی</param>
        /// <returns></returns>
        public static string Concat(params string[] items)
        {
            var txt = "Concat(" + string.Join(",", items.Select(item => "(" + item + ")")) + ")";
            return txt;
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

        /// <summary>
        /// Helper to write database select command in formulas. see sample usages in meta definitions.
        /// </summary>
        /// <param name="entityId">نام موجودیت</param>
        /// <param name="requestFormula">فیلد درخواستی</param>
        /// <param name="whereClause">شرط</param>
        /// <param name="orderby">مرتب سازی با فیلد</param>
        /// <param name="bDistinct">Distinct</param>
        /// <param name="top">تعداد حداکثر رکورد خروجی</param>
        /// <returns></returns>
        public static string Select(string entityId, string requestFormula, string whereClause = null,
            string orderby = null, bool bDistinct = false, int top = 0)
        {
            var txt = "Select((" + entityId + ")," + requestFormula + " ";
            var tmp = "";
            if (!string.IsNullOrEmpty(whereClause)) txt += ",(" + whereClause + ")";
            else tmp += ",null";
            if (!string.IsNullOrEmpty(orderby)) txt += tmp + ",(" + orderby + ")";
            else tmp += ",null";
            if (bDistinct) txt += tmp + ",DISTINCT";
            else tmp += ",null";
            if (top != 0) txt += tmp + "," + top;
            //else tmp += ",null"; //todo: this did not added to the txt
            txt += ")";
            return txt;
        }

        /// <summary>
        /// دریافت فیلد رکورد خاص
        /// </summary>
        /// <param name="fullEntityId">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
        /// <param name="id">شناسه رکورد</param>
        /// <param name="requestFormula">فیلد درخواستی</param>
        /// <returns></returns>
        public static string Fetch(string fullEntityId, string id, string requestFormula)
        {
            return "Fetch((" + fullEntityId + "),(" + id + "),(" + requestFormula + "))";
        }

        /// <summary>
        /// دریافت فیلد های از رکورد با شرط به صورت گروه بندی
        /// </summary>
        /// <param name="entities">نام کامل موجودیت 'فضای نامی.نام موجودیت'</param>
        /// <param name="whereClause">شرط</param>
        /// <param name="orderby">مرتب سازی بر اساس فیلد</param>
        /// <param name="bDistinct">Distinct</param>
        /// <param name="top">حداکثر تعداد رکورد های خروجی</param>
        /// <param name="groupBy">فیلد گروه بندی</param>
        /// <param name="requestFormula">فیلد های درخواستی</param>
        /// <param name="having">The Having</param>
        /// <returns></returns>
        public static string GroupBy(string entities, string requestFormula, string whereClause = null,
            string groupBy = null, string having = null, string orderby = null, bool bDistinct = false, int top = 0)
        {
            var txt = "GroupBy((" + entities + "),(" + requestFormula + ")";
            txt += GroupByBody(whereClause, groupBy, having, orderby, bDistinct, top);
            txt += ")";
            return txt;
        }

        /// <summary>
        /// Helper to write exists condition for database select command in formulas. see sample usages in meta definitions.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="groupBy">The group by.</param>
        /// <param name="having">The having.</param>
        /// <param name="orderby">The orderBy.</param>
        /// <param name="bDISTINCT">if set to <c>true</c> [b distinct].</param>
        /// <param name="top">The top.</param>
        /// <returns></returns>
        public static string Exists(string entities, string whereClause = null, string groupBy = null,
            string having = null, string orderby = null, bool bDISTINCT = false, int top = 0)
        {
            var txt = "Exists((" + entities + ")";
            txt += GroupByBody(whereClause, groupBy, having, orderby, bDISTINCT, top);
            txt += ")";
            return txt;
        }
        /// <summary>
        /// Helper to write not exists condition for database select command in formulas. see sample usages in meta definitions.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="groupBy">The group by.</param>
        /// <param name="having">The having.</param>
        /// <param name="orderby">The orderBy.</param>
        /// <param name="bDISTINCT">if set to <c>true</c> [b distinct].</param>
        /// <param name="top">The top.</param>
        /// <returns></returns>
        public static string NotExists(string entities, string whereClause = null, string groupBy = null,
            string having = null, string orderby = null, bool bDISTINCT = false, int top = 0)
        {
            var txt = "NotExists((" + entities + ")";
            txt += GroupByBody(whereClause, groupBy, having, orderby, bDISTINCT, top);
            txt += ")";
            return txt;
        }

        private static string GroupByBody(string whereClause, string groupBy, string having, string orderby,
            bool bDISTINCT, int top)
        {
            string txt = "";
            var tmp = "";
            if (!string.IsNullOrEmpty(whereClause)) txt += ",(" + whereClause + ")";
            else tmp += ",null";
            if (!string.IsNullOrEmpty(groupBy)) txt += tmp + ",(" + groupBy + ")";
            else tmp += ",null";
            if (!string.IsNullOrEmpty(having)) txt += tmp + ",(" + having + ")";
            else tmp += ",null";
            if (!string.IsNullOrEmpty(orderby)) txt += tmp + ",(" + orderby + ")";
            else tmp += ",null";
            if (bDISTINCT) txt += tmp + ",DISTINCT";
            else tmp += ",null";
            if (top != 0) txt += tmp + "," + top;
            //else tmp += ",null"; //todo: this did not added to the txt
            return txt;
        }
        #endregion Formula writing Helpers

        /// <summary>
        /// بررسی کلیم کاربر جاری با مقادیر ورودی
        /// </summary>
        /// <param name="claimId">کلیم ورودی</param>
        /// <returns></returns>
        public static string UserClaim(string claimId)
        {
            return $"UserClaim(user,\"{claimId}\")";
        }

        //todo: usage
        /// <summary>
        /// بررسی کلیم کاربر های جاری با مقادیر ورودی
        /// </summary>
        /// <param name="claimId">کلیم ورودی</param>
        /// <returns></returns>
        public static string UserClaims(string claimId)
        {
            return $"UserClaims(user,\"{claimId}\")";
        }

        //todo: name correction
        //todo: usage
        public static string LoginedUserId => "user.Id";

        /// <summary>
        /// تاریخ و زمان سرور
        /// </summary>
        public static string ServerDateTime => "serverdatetime()";

        public static string Constant(string str)
        {
            return $"Constant(\"{str}\")";
        }
    }
    #endregion Formula writing Helpers
}
