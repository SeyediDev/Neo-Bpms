using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataAssociation;

/// <summary>
/// The DataInputAssociation can be used to associate an DataFlowElement element with a DataInput contained in an Activity. 
/// The source of such a DataAssociation can be every DataFlowElement accessible in the current scope, e.g., a Data Object, a Property, or an Expression.
/// </summary>
public class DataInputAssociation : DataAssociation
{
    public DataInputAssociation(IDataInputAssociationContainer container, string id)
        : base(container, id)
    {
    }

    public DataInputAssociation(IDataInputAssociationContainer container, string id,
        IEnumerable<IItemAwareElement> sourceRef, IItemAwareElement targetRef,
        FormalExpression transformation)
        : base(container, id, sourceRef, targetRef, transformation)
    {
    }
}

public interface IDataInputAssociationContainer : IDataAssociationContainer
{
    List<DataInputAssociation> dataInputAssociations { get; set; }
}
