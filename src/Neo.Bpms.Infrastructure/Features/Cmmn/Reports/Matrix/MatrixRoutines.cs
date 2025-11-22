using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.Matrix;

public class MatrixRoutines(ReportData reportInfo)
{
    public int HorizontalColumnsCount { get; private set; }

    public MatrixData GetMatrixData()
    {
        MatrixData matrixData = new()
        {
            Verticals = [],
            Horizontals = [],
            Indexs = [.. reportInfo.Structure.SelectedColumns.Where(
                sc => sc.aggrType != eAggregationFunctions.InColumn &&
                      sc.aggrType != eAggregationFunctions.GroupByItem)]

        };
        IEnumerable<ColumnFieldDefinition> cols = reportInfo.Structure.SelectedColumns;
        // Only use GroupByItem fields with MatrixType for dimensions (DisplayColumn fields don't have MatrixType)
        foreach (ColumnFieldDefinition col in cols)
        {
            if (col.MatrixType == ConfiguredReport.ReportMatrixType.Vertical &&
                col.aggrType == eAggregationFunctions.GroupByItem)
            {
                MatrixItem matrixItem = new() { Column = col };
                matrixData.Verticals.Add(matrixItem);
            }
            else if (col.MatrixType == ConfiguredReport.ReportMatrixType.Horizontal &&
                     col.aggrType == eAggregationFunctions.GroupByItem)
            {
                MatrixItem matrixItem = new() { Column = col };
                matrixData.Horizontals.Add(matrixItem);
            }
        }
        foreach (ReportRowInfo row in reportInfo.Rows)
        {
            ReadVerticalValue(matrixData, row, null, 0);
        }
        int rowIndex = 0;
        foreach (ReportRowInfo row in reportInfo.Rows)
        {
            ReadHorizontalValue(matrixData, row, null, 0);
            rowIndex++;
            //if (rowIndex > 13) break;
        }
        return matrixData;
    }
    private static void ReadVerticalValue(MatrixData matrixData, ReportRowInfo row, MatrixValue parentMatrixValue, int vIndex)
    {
        if (matrixData.Verticals.Count == vIndex) return;
        MatrixItem matrixItem = matrixData.Verticals[vIndex];
        if (parentMatrixValue != null)
        {
            parentMatrixValue.Children ??= [];
            matrixItem = parentMatrixValue.Children.FirstOrDefault(c => c.Column.ColumnTypeName == matrixData.Verticals[vIndex].Column.ColumnTypeName);
            if (matrixItem == null)
            {
                matrixItem = new MatrixItem() { Column = matrixData.Verticals[vIndex].Column };
                parentMatrixValue.Children.Add(matrixItem);
            }
        }
        object value = ReportRenderer.GetCelValue(matrixItem.Column, row);

        matrixItem.Values ??= [];
        MatrixValue matrixValue = matrixItem.Values.FirstOrDefault(v => CompareValue(v, value));
        if (matrixValue == null)
        {
            matrixValue = new MatrixValue() { Value = value };
            matrixItem.Values.Add(matrixValue);
            matrixItem.Values = [.. matrixItem.Values.OrderBy(v => v.Value?.ToString() ?? "")];
        }
        ReadVerticalValue(matrixData, row, matrixValue, vIndex + 1);
    }
    private void ReadHorizontalValue(MatrixData matrixData, ReportRowInfo row, MatrixValue parentMatrixValue, int vIndex)
    {
        if (matrixData.Horizontals.Count == 0)
            return; // No horizontal dimensions
        
        if (vIndex >= matrixData.Horizontals.Count)
            return; // Index out of range
        
        MatrixItem matrixItem = matrixData.Horizontals[vIndex];
        if (parentMatrixValue != null)
        {
            parentMatrixValue.Children ??= [];
            matrixItem = parentMatrixValue.Children.FirstOrDefault(c => c.Column.ColumnTypeName == matrixData.Horizontals[vIndex].Column.ColumnTypeName);
            if (matrixItem == null)
            {
                matrixItem = new MatrixItem() { Column = matrixData.Horizontals[vIndex].Column };
                parentMatrixValue.Children.Add(matrixItem);
            }
        }
        object value = ReportRenderer.GetCelValue(matrixItem.Column, row);
        matrixItem.Values ??= [];
        MatrixValue matrixValue = matrixItem.Values.FirstOrDefault(v => CompareValue(v, value));
        if (matrixValue == null)
        {
            matrixValue = new MatrixValue() { Value = value };
            matrixItem.Values.Add(matrixValue);
        }
        if (vIndex + 1 < matrixData.Horizontals.Count)
            ReadHorizontalValue(matrixData, row, matrixValue, vIndex + 1);
        else
        {
            IEnumerable<ColumnFieldDefinition> cols = reportInfo.Structure.SelectedColumns;
            foreach (ColumnFieldDefinition col in cols.Where(col =>
                    col.aggrType != eAggregationFunctions.InColumn &&
                    col.aggrType != eAggregationFunctions.GroupByItem &&
                    col.MatrixType != ConfiguredReport.ReportMatrixType.Vertical &&
                    col.MatrixType != ConfiguredReport.ReportMatrixType.Horizontal))
            {
                matrixValue.Children ??= [];
                MatrixItem child = matrixValue.Children.FirstOrDefault(c => c.Column.ColumnTypeName == col.ColumnTypeName);
                if (child == null)
                {
                    if (matrixData.Verticals.Count == 0)
                        continue; // Skip if no vertical dimensions
                    int leafCount = matrixData.Verticals[0].LeafCount;
                    child = new MatrixItem() { Column = col, Values = new List<MatrixValue>(leafCount) };
                    for (int i = 0; i < leafCount; i++)
                        child.Values.Add(new MatrixValue());
                    matrixValue.Children.Add(child);
                }
                value = ReportRenderer.GetCelValue(col, row);
                int leafIndex = 0;
                GetVerticalLeafIndex(matrixData.Verticals, row, ref leafIndex);
                if (leafIndex < child.Values.Count)
                    child.Values[leafIndex].Value = value;
            }
        }
    }

    private static bool CompareValue(MatrixValue v, object value)
    {
        return (v.Value?.ToString() ?? "") == (value?.ToString() ?? "");
    }

    private int GetVerticalLeafIndex(List<MatrixItem> verticals, ReportRowInfo row, ref int leafIndex)
    {
        foreach (MatrixItem matrixItem in verticals)
        {
            if (matrixItem.Values != null)
            {
                object value = ReportRenderer.GetCelValue(matrixItem.Column, row);
                foreach (MatrixValue matrixValue in matrixItem.Values)
                {
                    if (CompareValue(matrixValue, value))
                    {
                        return (matrixValue.Children?.Count ?? 0) == 0 ? leafIndex : GetVerticalLeafIndex(matrixValue.Children, row, ref leafIndex);
                    }
                    else
                        leafIndex += matrixValue.LeafCount;
                }
            }
        }
        return leafIndex;
    }
}
