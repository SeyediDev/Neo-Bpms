namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

public class DataInput : DataElement
{
    public bool isCollection;

    public DataInput(IBaseDataInputContainer parent, string id, string name, ItemDefinition item, bool isCollection) :
        base(parent, id, name, eDataElementTypes.DataInput)
    {
        itemSubjectRef = item;
        this.isCollection = isCollection;
    }
}

public interface IBaseDataInputContainer : IDataElementContainer
{
}

public interface IDataInputContainer : IBaseDataInputContainer
{
    List<DataInput> dataInputs { get; set; }
    List<InputSet> inputSets { get; set; }
}
