namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;

/// <summary>
/// The DataOutputAssociation can be used to associate a DataOutput contained within an ACTIVITY with any BaseElement accessible in the scope the 
/// association will be executed in. The target of such a DataAssociation can be 
/// every BaseElement accessible in the current scope, e.g., a Data Object, a Property, or an Expression.
/// </summary>
public class DataOutputAssociation : DataAssociation
{
    public DataOutputAssociation(IDataOutputAssociationContainer parent, string id) : base(parent, id)
    {
    }

    public DataOutputAssociation(IDataOutputAssociationContainer parent, string id, IItemAwareElement targetRef, IEnumerable<DataOutput> sourceRef) :
        base(parent, id, sourceRef, targetRef, null)
    {
    }

    public DataOutputAssociation(IDataOutputAssociationContainer parent, string id, IItemAwareElement targetRef, params DataOutput[] sourceRef) :
        base(parent, id, sourceRef.AsEnumerable(), targetRef, null)
    {
    }
}

public interface IDataOutputAssociationContainer : IDataAssociationContainer
{
    List<DataOutputAssociation> dataOutputAssociations { get; set; }
}
