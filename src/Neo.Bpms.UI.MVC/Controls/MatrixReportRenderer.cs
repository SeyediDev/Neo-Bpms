using Microsoft.AspNetCore.Html;
using System.Linq;
using System.Text;

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

    private static void DrawMatrixBody(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        var horizontals = matrixData.Horizontals ?? new List<MatrixItem>();

        sb.Append("<tbody>");
        if (horizontals.Count > 0)
        {
            sb.Append("<tr>");
            DrawHorizontalItem(matrixData, horizontals, 0, calendar, sb);
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
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
        sb.Append("<tr>");

        foreach (MatrixItem horizontalColumn in horizontals)
        {
            AppendHeaderCell(
                sb,
                horizontalColumn?.Column?.Alias,
                Math.Max(1, verticals.Count),
                1,
                "matrix-head--horizontal",
                horizontalColumn?.Column?.ColumnName,
                "colgroup");
        }

        for (int vIndex = 0; vIndex < verticals.Count; vIndex++)
        {
            MatrixItem verticalColumn = verticals[vIndex];
            if (vIndex != 0)
            {
                sb.Append("<tr>");
            }

            AppendHeaderCell(
                sb,
                verticalColumn?.Column?.Alias,
                1,
                1,
                "matrix-head--vertical",
                verticalColumn?.Column?.ColumnName,
                "col");

            DrawVerticalItem(verticals, verticalColumn?.Column, calendar, sb);

            if (vIndex != 0)
            {
                sb.Append("</tr>");
            }
        }

        sb.Append("</tr>");
        sb.Append("</thead>");
    }

    private static void DrawVerticalItem(List<MatrixItem> matrixItems, ColumnFieldDefinition column, string calendar,
        StringBuilder sb)
    {
        if (matrixItems == null || column == null)
        {
            return;
        }

        foreach (MatrixItem matrixItem in matrixItems)
        {
            if (matrixItem?.Values == null)
            {
                continue;
            }

            foreach (MatrixValue matrixValue in matrixItem.Values)
            {
                if (matrixItem.Column == column)
                {
                    AppendHeaderCell(
                        sb,
                        CreateCelElement(calendar, matrixItem, matrixValue),
                        1,
                        Math.Max(1, matrixValue.LeafCount),
                        "matrix-head--vertical-value",
                        matrixItem.Column?.ColumnName,
                        "col");
                }
                else if (matrixValue.Children != null)
                {
                    DrawVerticalItem(matrixValue.Children, column, calendar, sb);
                }
            }
        }
    }

    private static void DrawHorizontalItem(MatrixData matrixData, List<MatrixItem> matrixItems, int index,
        string calendar, StringBuilder sb)
    {
        if (matrixItems == null || matrixItems.Count == 0)
        {
            return;
        }

        var horizontals = matrixData.Horizontals ?? new List<MatrixItem>();
        MatrixItem horizontalItem = (index < horizontals.Count) ? horizontals[index] : null;

        for (int iMatrixItem = 0; iMatrixItem < matrixItems.Count; iMatrixItem++)
        {
            MatrixItem matrixItem = matrixItems[iMatrixItem];
            if (matrixItem == null)
            {
                continue;
            }

            var column = matrixItem.Column;
            if (column?.aggrType == eAggregationFunctions.InColumn && horizontalItem != null)
            {
                if (matrixItem.Values == null)
                {
                    continue;
                }

                for (int iValue = 0; iValue < matrixItem.Values.Count; iValue++)
                {
                    if (iValue != 0)
                    {
                        sb.Append("<tr>");
                    }

                    MatrixValue matrixValue = matrixItem.Values[iValue];
                    AppendRowHeaderCell(sb, calendar, matrixItem, matrixValue, matrixValue.LeafCount);

                    if (matrixValue.Children != null)
                    {
                        DrawHorizontalItem(matrixData, matrixValue.Children, index + 1, calendar, sb);
                    }

                    if (iValue != 0)
                    {
                        sb.Append("</tr>");
                    }
                }
            }
            else
            {
                if (iMatrixItem != 0)
                {
                    sb.Append("<tr>");
                }

                AppendHeaderCell(
                    sb,
                    column?.Alias,
                    1,
                    1,
                    "matrix-head--row-title",
                    column?.ColumnName,
                    "row");

                if (matrixItem.Values != null)
                {
                    foreach (MatrixValue matrixItemValue in matrixItem.Values)
                    {
                        AppendValueCell(sb, calendar, matrixItem, matrixItemValue);
                    }
                }

                if (iMatrixItem != 0)
                {
                    sb.Append("</tr>");
                }
            }
        }
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

