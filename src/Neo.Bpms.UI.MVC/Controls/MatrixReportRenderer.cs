using Microsoft.AspNetCore.Html;

namespace Neo.Bpms.UI.MVC.Controls;

public class MatrixReportRenderer : ReportRenderer
{
    public static HtmlString DrawMatrix(MatrixData matrixData, string reportKey, string calendar)
    {
        StringBuilder sb = new();
        sb.Append("<table key=\"" + reportKey +
                     "\" class=\"table reportTbl report-matrix\" style=\"width: 100%; font-size: xx-small; border-radius: 5px; margin-bottom: 50px; border: 1px solid #ddd;\">");
        DrawMatrixHeader(matrixData, calendar, sb);
        DrawMatrixBody(matrixData, calendar, sb);
        sb.Append("</table>");
        return new HtmlString(sb.ToString());
    }

    private static void DrawMatrixBody(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        sb.Append("<tbody>");
        sb.Append("<tr>");
        DrawHorizontalItem(matrixData, matrixData.Horizontals, 0, calendar, sb);
        sb.Append("</tr>");
        sb.Append("</tbody>");
    }

    private static void DrawMatrixHeader(MatrixData matrixData, string calendar, StringBuilder sb)
    {
        sb.Append("<thead>");
        sb.Append("<tr>");
        foreach (MatrixItem horizontalColumn in matrixData.Horizontals)
        {
            sb.Append("<th data-colname=\"" + horizontalColumn.Column.ColumnName + "\" rowspan=\"" +
                      matrixData.Verticals.Count +
                      "\" style=\"text-align: center; vertical-align:bottom; padding: 5px 0px 0px 0px; min-width: 40px; direction: rtl;\" class=\"cColumn\">");
            sb.Append(horizontalColumn.Column.Alias);
            sb.Append("</th>");
        }
        for (int vIndex = 0; vIndex < matrixData.Verticals.Count; vIndex++)
        {
            MatrixItem verticalColumn = matrixData.Verticals[vIndex];
            if (vIndex != 0)
                sb.Append("<tr>");

            sb.Append("<th data-colname=\"" + verticalColumn.Column.ColumnName +
                      "\" style=\"text-align: center; vertical-align:bottom; padding: 5px 0px 0px 0px; min-width: 40px; direction: rtl;\" class=\"cColumn\">");
            sb.Append(verticalColumn.Column.Alias);
            sb.Append("</th>");

            DrawVerticalItem(matrixData.Verticals, verticalColumn.Column, calendar, sb);

            if (vIndex != 0)
                sb.Append("</tr>");
        }
        sb.Append("</tr>");
        sb.Append("</thead>");
    }

    private static void DrawVerticalItem(List<MatrixItem> matrixItems, ColumnFieldDefinition column, string calendar,
        StringBuilder sb)
    {
        foreach (MatrixItem matrixItem in matrixItems)
        {
            if (matrixItem.Values != null)
            {
                foreach (MatrixValue matrixValue in matrixItem.Values)
                {
                    if (matrixItem.Column == column)
                    {
                        sb.Append("<th data-colname=\"" + (matrixItem.Column.ColumnName) + "\" colspan=\"" + matrixValue.LeafCount +
                                  "\" style=\"text - align: center; vertical - align:bottom; padding: 5px 0px 0px 0px; min - width: 40px; direction: rtl; \" class=\"cColumn\">");
                        sb.Append(CreateCelElement(calendar, matrixItem, matrixValue));
                        sb.Append("</th>");
                    }
                    else if (matrixValue.Children != null)
                    {
                        DrawVerticalItem(matrixValue.Children, column, calendar, sb);
                    }
                }
            }
        }
    }

    private static void DrawHorizontalItem(MatrixData matrixData, List<MatrixItem> matrixItems, int index,
        string calendar, StringBuilder sb)
    {
        MatrixItem horizontalItem = (index < matrixData.Horizontals.Count) ? matrixData.Horizontals[index] : null;
        for (int iMatrixItem = 0; iMatrixItem < matrixItems.Count; iMatrixItem++)
        {
            MatrixItem matrixItem = matrixItems[iMatrixItem];
            if (matrixItem.Column.aggrType == eAggregationFunctions.InColumn && horizontalItem != null)
            {
                if (matrixItem.Values == null) continue;
                for (int iValue = 0; iValue < matrixItem.Values.Count; iValue++)
                {
                    if (iValue != 0)
                    {
                        sb.Append("<tr>");
                    }
                    MatrixValue matrixValue = matrixItem.Values[iValue];
                    int leafCount = matrixValue.LeafCount;
                    sb.Append("<th rowspan=\"" + leafCount + "\" data-colname=\"" + (matrixItem.Column.ColumnName) +
                              "\" style=\"text - align: center; vertical - align:bottom; padding: 5px 0px 0px 0px; min - width: 40px; direction: rtl; \" class=\"cColumn\">");
                    sb.Append(CreateCelElement(calendar, matrixItem, matrixValue));
                    sb.Append("</th>");
                    if (matrixValue.Children != null)
                    {
                        // بررسی اینکه آیا بعدهای افقی بعدی وجود دارند
                        if (index + 1 < matrixData.Horizontals.Count)
                        {
                            // اگر بعدهای افقی بعدی وجود دارند، بازگشتی ادامه می‌دهیم
                            DrawHorizontalItem(matrixData, matrixValue.Children, index + 1, calendar, sb);
                        }
                        else
                        {
                            // اگر به آخر بعدهای افقی رسیدیم، باید value columns را نمایش دهیم
                            // value columns در Children آخرین MatrixValue قرار دارند
                            DrawValueColumns(matrixValue.Children, calendar, sb);
                        }
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
                sb.Append("<th><span class=\"cColumn\">" + matrixItem.Column.Alias + "</span></th>");
                if (matrixItem.Values != null)
                {
                    foreach (MatrixValue matrixItemValue in matrixItem.Values)
                        sb.Append("<td><span class=\"table-col-value\">" + CreateCelElement(calendar, matrixItem, matrixItemValue) +
                                  "</span></td>");
                }
                if (iMatrixItem != 0)
                {
                    sb.Append("</tr>");
                }
            }
        }
    }

    private static void DrawValueColumns(List<MatrixItem> children, string calendar, StringBuilder sb)
    {
        if (children == null || children.Count == 0)
        {
            return;
        }

        // value columns آنهایی هستند که aggrType آنها GroupByItem یا InColumn نیست
        foreach (MatrixItem child in children)
        {
            if (child.Column != null &&
                child.Column.aggrType != eAggregationFunctions.GroupByItem &&
                child.Column.aggrType != eAggregationFunctions.InColumn)
            {
                // نمایش مقادیر این value column
                if (child.Values != null)
                {
                    foreach (MatrixValue value in child.Values)
                    {
                        sb.Append("<td><span class=\"table-col-value\">" + CreateCelElement(calendar, child, value) +
                                  "</span></td>");
                    }
                }
            }
        }
    }

    private static string CreateCelElement(string calendar, MatrixItem matrixItem, MatrixValue matrixValue)
    {
        return TableHelper.CreateCellElem(matrixValue.Value, matrixItem.Column,
            true, calendar);
    }
}
