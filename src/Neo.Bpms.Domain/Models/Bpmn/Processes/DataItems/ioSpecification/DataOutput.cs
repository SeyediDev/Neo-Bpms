using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

public class DataOutput : DataElement
{
    public bool isCollection;

    public DataOutput(IBaseDataOutputContainer parent, string id, string name, ItemDefinition item, bool isCollection) :
        base(parent, id, name, eDataElementTypes.DataOutput)
    {
        itemSubjectRef = item;
        this.isCollection = isCollection;
    }
}

public interface IBaseDataOutputContainer : IDataElementContainer
{
}
public interface IDataOutputContainer : IBaseDataOutputContainer
{
    List<DataOutput> dataOutputs { get; set; }
    List<OutputSet> outputSets { get; set; }
}
