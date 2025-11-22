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
        DrawMatrixBody(matrixData, calendar, sb);

        sb.Append("</table>");
        return new HtmlString(sb.ToString());
    }

    private static void DrawMatrixHeader(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        var horizontals = matrixData.Horizontals ?? new List<MatrixItem>();
        var verticals = matrixData.Verticals ?? new List<MatrixItem>();

        if (horizontals.Count == 0 && verticals.Count == 0)
        {
            return;
        }

        sb.Append("<thead>");

        // محاسبه تعداد سطرهای مورد نیاز برای هدرهای عمودی
        int verticalHeaderRows = CalculateVerticalHeaderRows(verticals);
        
        // سطر اول: سلول خالی + هدرهای بعدهای افقی
        sb.Append("<tr>");
        
        // سلول اول: خالی (برای تقاطع سطر و ستون)
        int emptyCellRowSpan = Math.Max(1, verticalHeaderRows);
        AppendHeaderCell(
            sb,
            "💡",
            emptyCellRowSpan,
            1,
            "matrix-head--empty",
            null,
            null);
        
        // هدرهای ستون: برای هر بعد افقی، مقادیر آن را نمایش می‌دهیم
        DrawHorizontalHeaders(horizontals, 0, verticalHeaderRows, calendar, sb);
        
        sb.Append("</tr>");
        
        // سطرهای بعدی: هدرهای بعدهای عمودی
        if (verticals.Count > 0)
        {
            DrawVerticalHeaders(verticals, 0, calendar, sb);
        }

        sb.Append("</thead>");
    }

    private static int CalculateVerticalHeaderRows(List<MatrixItem> verticals)
    {
        if (verticals == null || verticals.Count == 0)
        {
            return 1;
        }

        // تعداد سطرهای هدر عمودی برابر با تعداد بعدهای عمودی است
        return verticals.Count;
    }

    private static void DrawHorizontalHeaders(List<MatrixItem> horizontals, int index, int verticalRowSpan, string calendar, StringBuilder sb)
    {
        if (horizontals == null || index >= horizontals.Count)
        {
            // اگر به آخر بعدهای افقی رسیدیم، باید برای هر ستون مقدار (value column) یک هدر ایجاد کنیم
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
                colSpan = CalculateHorizontalColSpan(horizontals, index + 1);
            }
            else
            {
                // اگر این آخرین بعد افقی است، باید برای هر ستون مقدار (value column) یک ستون ایجاد کنیم
                // تعداد ستون‌های مقدار از Children آخرین MatrixValue قابل محاسبه است
                if (matrixValue.Children != null && matrixValue.Children.Count > 0)
                {
                    colSpan = matrixValue.Children.Sum(child => child.LeafCount);
                }
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
                DrawHorizontalHeaders(matrixValue.Children, index + 1, verticalRowSpan, calendar, sb);
            }
        }
    }
    
    private static int CalculateHorizontalColSpan(List<MatrixItem> horizontals, int startIndex)
    {
        if (startIndex >= horizontals.Count)
        {
            // اگر به آخر رسیدیم، باید تعداد ستون‌های مقدار را برگردانیم
            // اما اینجا نمی‌توانیم به Children دسترسی داشته باشیم، پس 1 برمی‌گردانیم
            // و در DrawHorizontalHeaders محاسبه می‌کنیم
            return 1;
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
                        colSpan = CalculateHorizontalColSpan(horizontals, startIndex + 1);
                    }
                }
                else
                {
                    // آخرین بعد افقی: تعداد ستون‌های مقدار
                    if (matrixValue.Children != null && matrixValue.Children.Count > 0)
                    {
                        colSpan = matrixValue.Children.Sum(child => child.LeafCount);
                    }
                }
                totalColSpan += colSpan;
            }
        }
        
        return totalColSpan > 0 ? totalColSpan : 1;
    }

    private static void DrawVerticalHeaders(List<MatrixItem> verticals, int verticalIndex, string calendar, StringBuilder sb)
    {
        if (verticals == null || verticalIndex >= verticals.Count)
        {
            return;
        }

        // استفاده از همان منطق DrawVerticalRows برای ایجاد سطرهای هدر
        DrawVerticalHeaderRows(verticals, verticalIndex, calendar, sb, new List<MatrixValue>());
    }

    private static void DrawVerticalHeaderRows(List<MatrixItem> verticals, int verticalIndex, string calendar, 
        StringBuilder sb, List<MatrixValue> currentPath)
    {
        if (verticals == null || verticalIndex >= verticals.Count)
        {
            // به آخر بعدهای عمودی رسیدیم، باید یک سطر کامل رسم کنیم
            DrawVerticalHeaderRow(verticals, calendar, sb, currentPath);
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
            List<MatrixValue> newPath = new List<MatrixValue>(currentPath) { matrixValue };
            
            // ادامه به بعد عمودی بعدی
            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalHeaderRowsRecursive(matrixValue.Children, verticals, verticalIndex + 1, calendar, sb, newPath);
            }
            else
            {
                // به آخر بعدهای عمودی رسیدیم، سطر را رسم می‌کنیم
                DrawVerticalHeaderRow(verticals, calendar, sb, newPath);
            }
        }
    }

    private static void DrawVerticalHeaderRowsRecursive(List<MatrixItem> children, List<MatrixItem> verticals, int verticalIndex, 
        string calendar, StringBuilder sb, List<MatrixValue> currentPath)
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
            List<MatrixValue> newPath = new List<MatrixValue>(currentPath) { matrixValue };
            
            if (verticalIndex < verticals.Count - 1 && matrixValue.Children != null)
            {
                DrawVerticalHeaderRowsRecursive(matrixValue.Children, verticals, verticalIndex + 1, calendar, sb, newPath);
            }
            else
            {
                DrawVerticalHeaderRow(verticals, calendar, sb, newPath);
            }
        }
    }

    private static void DrawVerticalHeaderRow(List<MatrixItem> verticals, string calendar, StringBuilder sb, List<MatrixValue> path)
    {
        sb.Append("<tr>");
        
        if (verticals != null && path != null)
        {
            for (int i = 0; i < path.Count && i < verticals.Count; i++)
            {
                MatrixValue matrixValue = path[i];
                MatrixItem matrixItem = verticals[i];
                
                int rowSpan = CalculateVerticalRowSpan(matrixValue, verticals, i);
                AppendHeaderCell(
                    sb,
                    CreateCelElement(calendar, matrixItem, matrixValue),
                    rowSpan,
                    1,
                    "matrix-head--vertical-value",
                    matrixItem?.Column?.ColumnName,
                    "col");
            }
        }
        
        sb.Append("</tr>");
    }

    private static int CalculateVerticalRowSpan(MatrixValue matrixValue, List<MatrixItem> verticals, int currentIndex)
    {
        if (currentIndex >= verticals.Count - 1)
        {
            // آخرین بعد عمودی: rowspan = 1
            return 1;
        }

        // محاسبه تعداد برگ‌های (leaf) این مقدار
        return matrixValue.LeafCount;
    }

    private static void DrawMatrixBody(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        var verticals = matrixData.Verticals ?? new List<MatrixItem>();
        var horizontals = matrixData.Horizontals ?? new List<MatrixItem>();

        sb.Append("<tbody>");
        
        if (verticals.Count > 0)
        {
            // برای هر ترکیب از مقادیر عمودی، یک سطر ایجاد می‌کنیم
            DrawVerticalRows(verticals, 0, horizontals, calendar, sb, new List<MatrixValue>());
        }
        else if (horizontals.Count > 0)
        {
            // اگر بعد عمودی نداریم، فقط یک سطر با مقادیر افقی
            sb.Append("<tr>");
            DrawHorizontalCells(horizontals, 0, calendar, sb, null, new List<MatrixItem>());
            sb.Append("</tr>");
        }
        
        sb.Append("</tbody>");
    }

    private static void DrawVerticalRows(List<MatrixItem> verticals, int verticalIndex, List<MatrixItem> horizontals, 
        string calendar, StringBuilder sb, List<MatrixValue> currentVerticalPath)
    {
        if (verticals == null || verticalIndex >= verticals.Count)
        {
            // به آخر بعدهای عمودی رسیدیم، باید یک سطر کامل رسم کنیم
            DrawCompleteRow(verticals, horizontals, calendar, sb, currentVerticalPath);
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
                // به آخر بعدهای عمودی رسیدیم، سطر را رسم می‌کنیم
                DrawCompleteRow(verticals, horizontals, calendar, sb, newPath);
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
                DrawCompleteRow(verticals, horizontals, calendar, sb, newPath);
            }
        }
    }

    private static void DrawCompleteRow(List<MatrixItem> verticals, List<MatrixItem> horizontals, 
        string calendar, StringBuilder sb, List<MatrixValue> verticalPath)
    {
        sb.Append("<tr>");
        
        // رسم هدرهای سطر (مقادیر عمودی)
        if (verticals != null && verticalPath != null)
        {
            for (int i = 0; i < verticalPath.Count && i < verticals.Count; i++)
            {
                MatrixValue matrixValue = verticalPath[i];
                MatrixItem matrixItem = verticals[i];
                
                int rowSpan = 1;
                if (i == 0 && matrixValue.LeafCount > 1)
                {
                    rowSpan = matrixValue.LeafCount;
                }
                
                AppendRowHeaderCell(sb, calendar, matrixItem, matrixValue, rowSpan);
            }
        }
        
        // رسم سلول‌های مقدار (مقادیر افقی)
        DrawHorizontalCells(horizontals, 0, calendar, sb, verticalPath, verticals);
        
        sb.Append("</tr>");
    }

    private static void DrawHorizontalCells(List<MatrixItem> horizontals, int horizontalIndex, 
        string calendar, StringBuilder sb, List<MatrixValue> verticalPath, List<MatrixItem> verticals)
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
                    DrawHorizontalCells(matrixValue.Children, horizontalIndex + 1, calendar, sb, verticalPath, verticals);
                }
            }
            else
            {
                // اگر به آخر بعدهای افقی رسیدیم، باید سلول‌های مقدار را رسم کنیم
                if (matrixValue.Children != null)
                {
                    foreach (MatrixItem valueColumn in matrixValue.Children)
                    {
                        // پیدا کردن مقدار مناسب بر اساس مسیر عمودی
                        MatrixValue? matchingValue = FindMatchingValue(valueColumn, verticalPath, verticals);
                        if (matchingValue != null)
                        {
                            AppendValueCell(sb, calendar, valueColumn, matchingValue);
                        }
                        else
                        {
                            // اگر مقدار پیدا نشد، یک سلول خالی
                            AppendValueCell(sb, calendar, valueColumn, new MatrixValue { Value = null });
                        }
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
        // این منطق مشابه GetVerticalLeafIndex در MatrixRoutines است
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
