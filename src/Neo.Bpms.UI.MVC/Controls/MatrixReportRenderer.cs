namespace Neo.Bpms.UI.MVC.Controls;

public class MatrixReportRenderer : ReportRenderer
{
    public static HtmlString DrawMatrix(MatrixData matrixData, string reportKey, string calendar)
    {
        if (matrixData == null || (matrixData.Horizontals?.Any() != true && matrixData.Verticals?.Any() != true))
        {
            return new HtmlString("<div class=\"matrix-pivot-empty\">داده‌ای برای نمایش ماتریس موجود نیست.</div>");
        }

        StringBuilder sb = new();
        sb.Append("<table data-report-key=\"")
            .Append(reportKey)
            .Append("\" class=\"table reportTbl report-matrix matrix-pivot-table\">");

        DrawMatrixHeader(matrixData, calendar, sb);
        //DrawMatrixBody(matrixData, calendar, sb);

        sb.Append("</table>");
        return new HtmlString(sb.ToString());
    }

    private static void DrawMatrixHeader(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        var horizontals = matrixData.Horizontals ?? [];
        var verticals = matrixData.Verticals ?? [];

        if (horizontals.Count == 0 && verticals.Count == 0)
        {
            return;
        }

        // پیدا کردن شاخص‌ها (value columns)
        List<MatrixItem> valueColumns = GetValueColumns(matrixData);
        int valueColumnCount = valueColumns.Count;
        bool hasMultipleValueColumns = valueColumnCount > 1;

        sb.Append("<thead>");

        // محاسبه تعداد سطرهای هدر
        // اگر شاخص‌ها بیش از یک باشند، یک سطر اضافی برای نمایش عناوین شاخص‌ها نیاز داریم
        int headerRowCount = verticals.Count + (hasMultipleValueColumns ? 1 : 0);
        var verticalNames = string.Join("/", verticals.Select(v => v.Column.Alias));
        var horizontalNames = string.Join("/", horizontals.Select(v => v.Column.Alias));

        // سطر اول: سلول خالی + هدرهای بعدهای افقی
        sb.Append("<tr>");
        // سلول اول: خالی (برای تقاطع سطر و ستون)
        AppendHeaderCell(sb, "💡 "+ verticalNames, 2, 1, "matrix-head--empty", null, null);
        if (matrixData.Indexs.Count > 0)
        {
            AppendHeaderCell(sb, "💡 شاخص ها", 2, 1, "matrix-head--empty", null, null);
        }
        AppendHeaderCell(sb, "💡 " + horizontalNames, 1, horizontals.Count, "matrix-head--empty", null, null);

        sb.Append("</tr><tr>");
        // هدرهای ستون: برای هر بعد افقی، مقادیر آن را نمایش می‌دهیم
        foreach (var horizontal in horizontals)
        {
            AppendHeaderCell(sb, "💡 " + horizontal.Column.Alias, 1, 1, "matrix-head--empty"/*TODO CURSOR*/, horizontal.Column.ColumnName, null);
        }
        //DrawHorizontalHeaders(horizontals, 0, headerRowCount, calendar, sb, valueColumnCount);

        sb.Append("</tr>");
/*
        // سطرهای بعدی: هدرهای بعدهای عمودی + عناوین شاخص‌ها (اگر بیش از یک باشند)
        if (verticals.Count > 0)
        {
            DrawVerticalHeaders(verticals, 0, calendar, sb, valueColumns, hasMultipleValueColumns);
        }
        else if (hasMultipleValueColumns)
        {
            // اگر بعد عمودی نداریم اما شاخص‌ها بیش از یک هستند، باید سطر عناوین شاخص‌ها را رسم کنیم
            sb.Append("<tr>");
            foreach (MatrixItem valueColumn in valueColumns)
            {
                AppendHeaderCell(
                    sb,
                    valueColumn?.Column?.Alias ?? "",
                    1,
                    1,
                    "matrix-head--value-column",
                    valueColumn?.Column?.ColumnName,
                    "col");
            }
            sb.Append("</tr>");
        }
*/
        sb.Append("</thead>");
    }

    private static List<MatrixItem> GetValueColumns(MatrixData matrixData)
    {
        List<MatrixItem> valueColumns = [];
        var horizontals = matrixData.Horizontals ?? [];

        if (horizontals.Count == 0)
        {
            return valueColumns;
        }

        // پیدا کردن آخرین بعد افقی و استخراج value columns از آن
        MatrixItem? lastHorizontal = horizontals.LastOrDefault();
        if (lastHorizontal?.Values != null && lastHorizontal.Values.Count > 0)
        {
            // از اولین MatrixValue، Children را می‌گیریم که value columns هستند
            MatrixValue? firstValue = lastHorizontal.Values.FirstOrDefault();
            if (firstValue?.Children != null)
            {
                foreach (MatrixItem child in firstValue.Children)
                {
                    // value columns آنهایی هستند که aggrType آنها GroupByItem یا InColumn نیست
                    if (child.Column != null &&
                        child.Column.aggrType != eAggregationFunctions.GroupByItem &&
                        child.Column.aggrType != eAggregationFunctions.InColumn)
                    {
                        valueColumns.Add(child);
                    }
                }
            }
        }

        return valueColumns;
    }

    private static void DrawHorizontalHeaders(List<MatrixItem> horizontals, int index, int verticalRowSpan, 
        string calendar, StringBuilder sb, int valueColumnCount)
    {
        if (horizontals == null || index >= horizontals.Count)
        {
            return;
        }

        MatrixItem horizontalItem = horizontals[index];
        if (horizontalItem?.Values == null || horizontalItem.Values.Count == 0)
        {
            return;
        }

        // برای هر مقدار در بعد افقی فعلی
        foreach (MatrixValue matrixValue in horizontalItem.Values)
        {
            int colSpan = 1;

            // محاسبه colspan: اگر بعدهای افقی بعدی وجود دارند
            if (index < horizontals.Count - 1)
            {
                // تعداد کل ترکیبات بعدهای بعدی
                colSpan = CalculateHorizontalColSpan(horizontals, index + 1, valueColumnCount);
            }
            else
            {
                // اگر این آخرین بعد افقی است، باید برای هر ستون مقدار (value column) یک ستون ایجاد کنیم
                colSpan = valueColumnCount;
            }

            AppendHeaderCell(
                sb,
                CreateCelElement(calendar, horizontalItem, matrixValue),
                verticalRowSpan,
                colSpan,
                "matrix-head--horizontal",
                horizontalItem?.Column?.ColumnName,
                "col");

            // اگر بعدهای افقی بعدی وجود دارند، آنها را هم رسم کنیم
            if (index < horizontals.Count - 1 && matrixValue.Children != null)
            {
                DrawHorizontalHeaders(matrixValue.Children, index + 1, verticalRowSpan, calendar, sb, valueColumnCount);
            }
        }
    }

    private static int CalculateHorizontalColSpan(List<MatrixItem> horizontals, int startIndex, int valueColumnCount)
    {
        if (startIndex >= horizontals.Count)
        {
            return valueColumnCount;
        }

        int totalColSpan = 0;
        MatrixItem horizontalItem = horizontals[startIndex];
        if (horizontalItem?.Values != null)
        {
            foreach (MatrixValue matrixValue in horizontalItem.Values)
            {
                int colSpan = 1;
                if (startIndex < horizontals.Count - 1)
                {
                    if (matrixValue.Children != null)
                    {
                        colSpan = CalculateHorizontalColSpan(horizontals, startIndex + 1, valueColumnCount);
                    }
                }
                else
                {
                    // آخرین بعد افقی: تعداد ستون‌های مقدار
                    colSpan = valueColumnCount;
                }
                totalColSpan += colSpan;
            }
        }

        return totalColSpan > 0 ? totalColSpan : valueColumnCount;
    }

    /*private static void DrawVerticalHeaders(List<MatrixItem> verticals, int verticalIndex, string calendar, 
        StringBuilder sb, List<MatrixItem> valueColumns, bool hasMultipleValueColumns)
    {
        if (verticals == null || verticalIndex >= verticals.Count)
        {
            return;
        }

        MatrixItem verticalItem = verticals[verticalIndex];
        if (verticalItem?.Values == null || verticalItem.Values.Count == 0)
        {
            return;
        }

        // برای هر مقدار در بعد عمودی فعلی
        foreach (MatrixValue matrixValue in verticalItem.Values)
        {
            // اگر بعدهای عمودی بعدی وجود دارند، بازگشتی ادامه می‌دهیم
            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalHeadersRecursive(matrixValue.Children, verticals, verticalIndex + 1, calendar, sb, 
                    matrixValue, verticalItem, valueColumns, hasMultipleValueColumns);
            }
            else
            {
                // اگر به آخر بعدهای عمودی رسیدیم، یک سطر کامل می‌سازیم
                sb.Append("<tr>");
                DrawVerticalHeaderCell(verticalItem, matrixValue, verticals, verticalIndex, calendar, sb);
                
                // اگر شاخص‌ها بیش از یک باشند، عناوین آنها را در این سطر نمایش می‌دهیم
                if (hasMultipleValueColumns && verticalIndex == verticals.Count - 1)
                {
                    foreach (MatrixItem valueColumn in valueColumns)
                    {
                        AppendHeaderCell(
                            sb,
                            valueColumn?.Column?.Alias ?? "",
                            1,
                            1,
                            "matrix-head--value-column",
                            valueColumn?.Column?.ColumnName,
                            "col");
                    }
                }
                
                sb.Append("</tr>");
            }
        }
    }*/
    /*
    private static void DrawVerticalHeadersRecursive(List<MatrixItem> children, List<MatrixItem> verticals, int verticalIndex,
        string calendar, StringBuilder sb, MatrixValue parentValue, MatrixItem parentItem, 
        List<MatrixItem> valueColumns, bool hasMultipleValueColumns)
    {
        if (children == null || children.Count == 0 || verticalIndex >= verticals.Count)
        {
            return;
        }

        MatrixItem currentVertical = verticals[verticalIndex];
        MatrixItem matchingChild = children.FirstOrDefault(c => c.Column?.ColumnTypeName == currentVertical.Column?.ColumnTypeName);

        if (matchingChild?.Values == null || matchingChild.Values.Count == 0)
        {
            return;
        }

        foreach (MatrixValue matrixValue in matchingChild.Values)
        {
            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalHeadersRecursive(matrixValue.Children, verticals, verticalIndex + 1, calendar, sb,
                    matrixValue, matchingChild, valueColumns, hasMultipleValueColumns);
            }
            else
            {
                // اگر به آخر بعدهای عمودی رسیدیم، یک سطر کامل می‌سازیم
                sb.Append("<tr>");
                
                // رسم هدرهای تمام بعدهای عمودی از ریشه تا این مقدار
                DrawVerticalHeaderCell(parentItem, parentValue, verticals, verticalIndex - 1, calendar, sb);
                DrawVerticalHeaderCell(matchingChild, matrixValue, verticals, verticalIndex, calendar, sb);
                
                // اگر شاخص‌ها بیش از یک باشند، عناوین آنها را در این سطر نمایش می‌دهیم
                if (hasMultipleValueColumns)
                {
                    foreach (MatrixItem valueColumn in valueColumns)
                    {
                        AppendHeaderCell(
                            sb,
                            valueColumn?.Column?.Alias ?? "",
                            1,
                            1,
                            "matrix-head--value-column",
                            valueColumn?.Column?.ColumnName,
                            "col");
                    }
                }
                
                sb.Append("</tr>");
            }
        }
    }

    private static void DrawVerticalHeaderCell(MatrixItem verticalItem, MatrixValue matrixValue, List<MatrixItem> verticals,
        int verticalIndex, string calendar, StringBuilder sb)
    {
        int rowSpan = CalculateVerticalRowSpan(matrixValue, verticals, verticalIndex);
        AppendHeaderCell(
            sb,
            CreateCelElement(calendar, verticalItem, matrixValue),
            rowSpan,
            1,
            "matrix-head--vertical-value",
            verticalItem?.Column?.ColumnName,
            "col");
    }

    private static int CalculateVerticalRowSpan(MatrixValue matrixValue, List<MatrixItem> verticals, int currentIndex)
    {
        if (currentIndex >= verticals.Count - 1)
        {
            // آخرین بعد عمودی: rowspan = 1 (مگر اینکه شاخص‌ها بیش از یک باشند)
            return 1;
        }

        // محاسبه تعداد برگ‌های (leaf) این مقدار
        return matrixValue.LeafCount;
    }*/

    private static void DrawMatrixBody(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        var verticals = matrixData.Verticals ?? [];
        var horizontals = matrixData.Horizontals ?? [];

        sb.Append("<tbody>");

        if (verticals.Count > 0)
        {
            // برای هر ترکیب از مقادیر عمودی، سطر(های) ایجاد می‌کنیم
            DrawVerticalRows(verticals, 0, horizontals, calendar, sb, []);
        }
        else if (horizontals.Count > 0)
        {
            // اگر بعد عمودی نداریم، فقط یک سطر با مقادیر افقی
            List<MatrixItem> valueColumns = GetValueColumnsFromHorizontals(horizontals);
            sb.Append("<tr>");
            DrawHorizontalCells(horizontals, 0, calendar, sb, null, [], valueColumns, 0);
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
    }

    private static void DrawVerticalRows(List<MatrixItem> verticals, int verticalIndex, List<MatrixItem> horizontals,
        string calendar, StringBuilder sb, List<MatrixValue> currentVerticalPath)
    {
        if (verticals == null || verticalIndex >= verticals.Count)
        {
            // به آخر بعدهای عمودی رسیدیم، باید سطر(های) کامل رسم کنیم
            DrawCompleteRows(verticals, horizontals, calendar, sb, currentVerticalPath);
            return;
        }

        MatrixItem verticalItem = verticals[verticalIndex];
        if (verticalItem?.Values == null || verticalItem.Values.Count == 0)
        {
            return;
        }

        // برای هر مقدار در بعد عمودی فعلی
        foreach (MatrixValue matrixValue in verticalItem.Values)
        {
            List<MatrixValue> newPath = new List<MatrixValue>(currentVerticalPath) { matrixValue };

            // ادامه به بعد عمودی بعدی
            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalRowsRecursive(matrixValue.Children, verticals, verticalIndex + 1, horizontals, calendar, sb, newPath);
            }
            else
            {
                // به آخر بعدهای عمودی رسیدیم، سطر(های) را رسم می‌کنیم
                DrawCompleteRows(verticals, horizontals, calendar, sb, newPath);
            }
        }
    }

    private static void DrawVerticalRowsRecursive(List<MatrixItem> children, List<MatrixItem> verticals, int verticalIndex,
        List<MatrixItem> horizontals, string calendar, StringBuilder sb, List<MatrixValue> currentVerticalPath)
    {
        if (children == null || children.Count == 0 || verticalIndex >= verticals.Count)
        {
            return;
        }

        MatrixItem currentVertical = verticals[verticalIndex];
        MatrixItem matchingChild = children.FirstOrDefault(c => c.Column?.ColumnTypeName == currentVertical.Column?.ColumnTypeName);

        if (matchingChild?.Values == null || matchingChild.Values.Count == 0)
        {
            return;
        }

        foreach (MatrixValue matrixValue in matchingChild.Values)
        {
            List<MatrixValue> newPath = new List<MatrixValue>(currentVerticalPath) { matrixValue };

            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalRowsRecursive(matrixValue.Children, verticals, verticalIndex + 1, horizontals, calendar, sb, newPath);
            }
            else
            {
                DrawCompleteRows(verticals, horizontals, calendar, sb, newPath);
            }
        }
    }

    private static void DrawCompleteRows(List<MatrixItem> verticals, List<MatrixItem> horizontals,
        string calendar, StringBuilder sb, List<MatrixValue> verticalPath)
    {
        // پیدا کردن شاخص‌ها (value columns)
        List<MatrixItem> valueColumns = GetValueColumnsFromHorizontals(horizontals);
        bool hasMultipleValueColumns = valueColumns.Count > 1;

        if (hasMultipleValueColumns)
        {
            // اگر شاخص‌ها بیش از یک باشند، برای هر شاخص یک سطر جداگانه ایجاد می‌کنیم
            for (int valueIndex = 0; valueIndex < valueColumns.Count; valueIndex++)
            {
                MatrixItem valueColumn = valueColumns[valueIndex];
                sb.Append("<tr>");

                // رسم هدرهای سطر (مقادیر عمودی) - فقط در سطر اول rowspan می‌دهیم
                if (verticals != null && verticalPath != null)
                {
                    for (int i = 0; i < verticalPath.Count && i < verticals.Count; i++)
                    {
                        MatrixValue matrixValue = verticalPath[i];
                        MatrixItem matrixItem = verticals[i];

                        int rowSpan = 1;
                        if (i == 0 && valueIndex == 0)
                        {
                            // فقط در اولین سطر و اولین شاخص، rowspan را محاسبه می‌کنیم
                            rowSpan = valueColumns.Count;
                        }

                        AppendRowHeaderCell(sb, calendar, matrixItem, matrixValue, rowSpan);
                    }
                }

                // رسم عنوان شاخص
                AppendHeaderCell(
                    sb,
                    valueColumn?.Column?.Alias ?? "",
                    1,
                    1,
                    "matrix-head--value-column",
                    valueColumn?.Column?.ColumnName,
                    "row");

                // رسم سلول‌های مقدار (مقادیر افقی) برای این شاخص
                DrawHorizontalCells(horizontals, 0, calendar, sb, verticalPath, verticals, valueColumns, valueIndex);

                sb.Append("</tr>");
            }
        }
        else
        {
            // اگر فقط یک شاخص داریم، یک سطر ساده می‌سازیم
            sb.Append("<tr>");

            // رسم هدرهای سطر (مقادیر عمودی)
            if (verticals != null && verticalPath != null)
            {
                for (int i = 0; i < verticalPath.Count && i < verticals.Count; i++)
                {
                    MatrixValue matrixValue = verticalPath[i];
                    MatrixItem matrixItem = verticals[i];
                    AppendRowHeaderCell(sb, calendar, matrixItem, matrixValue, 1);
                }
            }

            // رسم سلول‌های مقدار (مقادیر افقی)
            DrawHorizontalCells(horizontals, 0, calendar, sb, verticalPath, verticals, valueColumns, 0);

            sb.Append("</tr>");
        }
    }

    private static List<MatrixItem> GetValueColumnsFromHorizontals(List<MatrixItem> horizontals)
    {
        List<MatrixItem> valueColumns = [];

        if (horizontals == null || horizontals.Count == 0)
        {
            return valueColumns;
        }

        // پیدا کردن آخرین بعد افقی و استخراج value columns از آن
        MatrixItem? lastHorizontal = horizontals.LastOrDefault();
        if (lastHorizontal?.Values != null && lastHorizontal.Values.Count > 0)
        {
            // از اولین MatrixValue، Children را می‌گیریم که value columns هستند
            MatrixValue? firstValue = lastHorizontal.Values.FirstOrDefault();
            if (firstValue?.Children != null)
            {
                foreach (MatrixItem child in firstValue.Children)
                {
                    // value columns آنهایی هستند که aggrType آنها GroupByItem یا InColumn نیست
                    if (child.Column != null &&
                        child.Column.aggrType != eAggregationFunctions.GroupByItem &&
                        child.Column.aggrType != eAggregationFunctions.InColumn)
                    {
                        valueColumns.Add(child);
                    }
                }
            }
        }

        return valueColumns;
    }

    private static void DrawHorizontalCells(List<MatrixItem> horizontals, int horizontalIndex,
        string calendar, StringBuilder sb, List<MatrixValue> verticalPath, List<MatrixItem> verticals,
        List<MatrixItem> valueColumns, int valueColumnIndex)
    {
        if (horizontals == null || horizontalIndex >= horizontals.Count)
        {
            return;
        }

        MatrixItem horizontalItem = horizontals[horizontalIndex];
        if (horizontalItem?.Values == null || horizontalItem.Values.Count == 0)
        {
            return;
        }

        // برای هر مقدار در بعد افقی فعلی
        foreach (MatrixValue matrixValue in horizontalItem.Values)
        {
            if (horizontalIndex < horizontals.Count - 1)
            {
                // اگر بعدهای افقی بعدی وجود دارند، بازگشتی ادامه می‌دهیم
                if (matrixValue.Children != null)
                {
                    DrawHorizontalCells(matrixValue.Children, horizontalIndex + 1, calendar, sb, verticalPath, verticals, valueColumns, valueColumnIndex);
                }
            }
            else
            {
                // اگر به آخر بعدهای افقی رسیدیم، باید سلول مقدار را برای شاخص مشخص شده رسم کنیم
                if (matrixValue.Children != null && valueColumnIndex < valueColumns.Count)
                {
                    MatrixItem valueColumn = valueColumns[valueColumnIndex];
                    MatrixItem? matchingValueColumn = matrixValue.Children.FirstOrDefault(c => c.Column?.ColumnTypeName == valueColumn.Column?.ColumnTypeName);

                    if (matchingValueColumn != null)
                    {
                        // پیدا کردن مقدار مناسب بر اساس مسیر عمودی
                        MatrixValue? matchingValue = FindMatchingValue(matchingValueColumn, verticalPath, verticals);
                        if (matchingValue != null)
                        {
                            AppendValueCell(sb, calendar, matchingValueColumn, matchingValue);
                        }
                        else
                        {
                            // اگر مقدار پیدا نشد، یک سلول خالی
                            AppendValueCell(sb, calendar, matchingValueColumn, new MatrixValue { Value = null });
                        }
                    }
                    else
                    {
                        // اگر value column پیدا نشد، یک سلول خالی
                        AppendValueCell(sb, calendar, valueColumn, new MatrixValue { Value = null });
                    }
                }
            }
        }
    }

    private static MatrixValue? FindMatchingValue(MatrixItem valueColumn, List<MatrixValue> verticalPath, List<MatrixItem> verticals)
    {
        if (valueColumn.Values == null || valueColumn.Values.Count == 0)
        {
            return null;
        }

        if (verticalPath == null || verticalPath.Count == 0 || verticals == null || verticals.Count == 0)
        {
            // اگر مسیر عمودی نداریم، اولین مقدار را برمی‌گردانیم
            return valueColumn.Values[0];
        }

        // محاسبه index مناسب بر اساس مسیر عمودی
        int leafIndex = CalculateVerticalLeafIndex(verticals, verticalPath, 0, 0);

        if (leafIndex >= 0 && leafIndex < valueColumn.Values.Count)
        {
            return valueColumn.Values[leafIndex];
        }

        // اگر index معتبر نبود، اولین مقدار را برمی‌گردانیم
        return valueColumn.Values[0];
    }

    private static int CalculateVerticalLeafIndex(List<MatrixItem> verticals, List<MatrixValue> verticalPath, int verticalIndex, int currentLeafIndex)
    {
        if (verticalIndex >= verticals.Count || verticalIndex >= verticalPath.Count)
        {
            return currentLeafIndex;
        }

        MatrixItem verticalItem = verticals[verticalIndex];
        MatrixValue pathValue = verticalPath[verticalIndex];

        if (verticalItem.Values == null)
        {
            return currentLeafIndex;
        }

        // پیدا کردن index این مقدار در Values
        foreach (MatrixValue matrixValue in verticalItem.Values)
        {
            if (CompareMatrixValue(matrixValue, pathValue))
            {
                // اگر این آخرین بعد عمودی است، index فعلی را برمی‌گردانیم
                if (verticalIndex == verticals.Count - 1)
                {
                    return currentLeafIndex;
                }

                // اگر children دارد، بازگشتی ادامه می‌دهیم
                if (matrixValue.Children != null && matrixValue.Children.Count > 0)
                {
                    return CalculateVerticalLeafIndex(verticals, verticalPath, verticalIndex + 1, currentLeafIndex);
                }

                return currentLeafIndex;
            }
            else
            {
                // اگر مطابقت نداشت، LeafCount این مقدار را به currentLeafIndex اضافه می‌کنیم
                currentLeafIndex += matrixValue.LeafCount;
            }
        }

        return currentLeafIndex;
    }

    private static bool CompareMatrixValue(MatrixValue v1, MatrixValue v2)
    {
        return (v1.Value?.ToString() ?? "") == (v2.Value?.ToString() ?? "");
    }

    private static void AppendHeaderCell(StringBuilder sb, string? content, int rowSpan, int colSpan, string cssModifier,
        string? columnName, string? scope = null)
    {
        sb.Append("<th");

        if (!string.IsNullOrWhiteSpace(columnName))
        {
            sb.Append(" data-colname=\"").Append(columnName).Append("\"");
        }

        if (rowSpan > 1)
        {
            sb.Append(" rowspan=\"").Append(rowSpan).Append("\"");
        }

        if (colSpan > 1)
        {
            sb.Append(" colspan=\"").Append(colSpan).Append("\"");
        }

        if (!string.IsNullOrWhiteSpace(scope))
        {
            sb.Append(" scope=\"").Append(scope).Append("\"");
        }

        sb.Append(" class=\"matrix-head");
        if (!string.IsNullOrWhiteSpace(cssModifier))
        {
            sb.Append(' ').Append(cssModifier);
        }

        sb.Append("\"><div class=\"matrix-head__content\">")
            .Append(string.IsNullOrWhiteSpace(content) ? "&mdash;" : content)
            .Append("</div></th>");
    }

    private static void AppendRowHeaderCell(StringBuilder sb, string calendar, MatrixItem matrixItem,
        MatrixValue matrixValue, int rowSpan)
    {
        string? columnName = matrixItem.Column?.ColumnName;
        sb.Append("<th class=\"matrix-head matrix-head--row-value\"");
        if (!string.IsNullOrWhiteSpace(columnName))
        {
            sb.Append(" data-colname=\"").Append(columnName).Append("\"");
        }

        sb.Append(" scope=\"row\"");
        if (rowSpan > 1)
        {
            sb.Append(" rowspan=\"").Append(Math.Max(1, rowSpan)).Append("\"");
        }

        sb.Append("><div class=\"matrix-head__content\">")
            .Append(CreateCelElement(calendar, matrixItem, matrixValue))
            .Append("</div></th>");
    }

    private static void AppendValueCell(StringBuilder sb, string calendar, MatrixItem matrixItem, MatrixValue matrixValue)
    {
        string? columnName = matrixItem.Column?.ColumnName;
        sb.Append("<td class=\"matrix-cell\"");
        if (!string.IsNullOrWhiteSpace(columnName))
        {
            sb.Append(" data-colname=\"").Append(columnName).Append("\"");
        }

        sb.Append("><div class=\"matrix-cell__value\">")
            .Append(CreateCelElement(calendar, matrixItem, matrixValue))
            .Append("</div></td>");
    }

    private static string CreateCelElement(string calendar, MatrixItem matrixItem, MatrixValue matrixValue)
    {
        return TableHelper.CreateCellElem(matrixValue.Value, matrixItem.Column,
            true, calendar);
    }
}
