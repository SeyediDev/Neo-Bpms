namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels;

public class FilterViewModel
{
    public FilterViewModel()
    {

    }
    public FilterViewModel(EntityFormFilter filter)
    {
        filterFormula = filter.filter?.ExpressionString;
        condition = filter.condition?.ExpressionString;
        entityId = filter.entityId;
        associationId = filter.Association;
    }
    public string filterFormula { get; set; }
    public string condition { get; set; }
    public string entityId { get; set; }
    public string associationId { get; set; }

    public EntityFormFilter ToFilter()
    {
        return new EntityFormFilter(Parser.ParseTree(filterFormula), Parser.ParseTree(condition), entityId,
            associationId);
    }
}