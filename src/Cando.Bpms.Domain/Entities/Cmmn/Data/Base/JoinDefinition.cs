namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Base;

public abstract class JoinDefinition
{
    public eJoinType joinType;
    public List<JoinFieldMapping> fieldMappings { get; set; } = [];

    public bool isJoin()
    {
        return joinType == eJoinType.InnerJoin || joinType == eJoinType.LeftOuterJoin || joinType == eJoinType.RightOuterJoin;
    }
    public class JoinFieldMapping(
        string mainTableFieldName, ExpressionNode mainTableFormula,
        string joinTableFieldName, ExpressionNode joinTableFormula)
    {
        public string MainTableFieldName { get; set; } = mainTableFieldName;
        public ExpressionNode MainTableFormula { get; set; } = mainTableFormula;
        public string JoinTableFieldName { get; set; } = joinTableFieldName;
        public ExpressionNode JoinTableFormula { get; set; } = joinTableFormula;
        public JoinFieldMapping(string mainTableFieldName, string joinTableFieldName)
            : this(mainTableFieldName, null, joinTableFieldName, null)
        {
        }
    }
}
public enum eJoinType
{
    Concatennate,
    Union,
    Intersect,
    //Include and Shape:
    ShapeLists,
    ShapePerItemByItem,
    GroupShapeLists,
    GroupShapeItemByItem,
    InnerJoin,
    LeftOuterJoin,
    RightOuterJoin,
    //Exists,
    GroupJoin,
}