namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

/// <summary>
/// Column Filter Definition
/// </summary>
public class ColumnFilterDefinition
{
    public eOperator Operator { set; get; }
    public string Operand { set; get; }

    public ColumnFilterDefinition Clone()
    {
        return new ColumnFilterDefinition
        {
            Operand = Operand,
            Operator = Operator
        };
    }
    public enum eOperator
    {
        Equal,
        NotEqual,
        LessThan,
        LessEqual,
        GreaterThan,
        GreaterEqual,
        Includes,
        NotIncludes,
        StartsWith,
        EndsWith
    }
}
