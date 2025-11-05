using Neo.Bpms.Domain.Models.Cmmn;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports.Matrix;

public class MatrixData
{
    public List<MatrixItem> Horizontals { get; internal set; }
    public List<MatrixItem> Verticals { get; internal set; }
}
public class MatrixItem
{
    public ColumnFieldDefinition Column { get; internal set; }
    public List<MatrixValue> Values;
    public int LeafCount => Column.aggrType != eAggregationFunctions.InColumn ? 1 : (Values?.Count ?? 0) > 0 ? Values.Sum(c => c.LeafCount) : 1;
}
public class MatrixValue
{
    public object Value;
    public List<MatrixItem> Children;
    public int LeafCount => (Children?.Count ?? 0) > 0 ? Children.Sum(c => c.LeafCount) : 1;
}
//public class HorizontalItem : MatrixItem
//{
//}
//public class VerticalItem : MatrixItem
//{
//}
