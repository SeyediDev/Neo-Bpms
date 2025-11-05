using Neo.Bpms.Domain.Entities.Dmn.DecisionLogic;

namespace Neo.Bpms.Domain.Entities.Dmn.FEEL;

/// <summary>
/// A Relation is convenient shorthand for a list of similar contexts. A Relation has a column instead of repeated ContextEntrys, 
/// and a List is used for every row, with one of the List’s expression for each column value.
/// </summary>
public class Relation(string id, IEnumerable<InformationItem> inputVariable, ItemDefinition itemDefinition) 
    : DMNExpression(id, inputVariable, itemDefinition)
{
    /// <summary>
    /// the instances of List that compose the rows of this Relation
    /// </summary>
    public List<FEELList> row = [];
    /// <summary>
    /// the instances of InformationItem that define the columns in this Relation.
    /// </summary>
    public List<InformationItem> column = [];
    public void addColumn(InformationItem column, FEELList row)
    {
        this.column.Add(column);
        this.row.Add(row);
    }
}
