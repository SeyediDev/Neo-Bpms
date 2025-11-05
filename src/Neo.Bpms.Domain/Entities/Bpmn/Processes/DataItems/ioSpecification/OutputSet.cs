using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

public class OutputSet(IDataOutputContainer parent, string id, string name) : BaseElement(parent as BaseElement, id, name)
{
    //		public string name;
    public List<DataOutputRef> dataOutputRefs;
}
