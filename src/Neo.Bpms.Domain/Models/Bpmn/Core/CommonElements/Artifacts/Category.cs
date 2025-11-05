namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Artifacts;

/// <summary>
/// Categories, which have user-defined semantics, can be used for documentation or analysis purposes. 
/// For example, FlowElements can be categorized as being customer oriented vs. support oriented. 
/// Furthermore, the cost and time of Activities per Category can be calculated.
/// </summary>
public class Category(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name)
{
    //		public string name;

    /// <summary>
    /// The categoryValue attribute specifies one or more values of the Category. 
    /// For example, the Category is “Region” then this Category could specify values like “North,” “South,” “West,” and “East.”
    /// </summary>
    public List<CategoryValue> categoryValues;

    public override void Copy(RootElement newRootElement)
    {
        Category newItem = newRootElement as Category;
        if (newItem == null) return;
        // todo
    }
}
