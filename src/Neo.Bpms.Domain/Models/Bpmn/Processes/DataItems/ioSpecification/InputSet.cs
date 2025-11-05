namespace Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

public class InputSet(IDataInputContainer parent, string id, string name) : BaseElement(parent as BaseElement, id, name)
{
    //		public string name;
    public List<DataInputRef> dataInputRefs;

    /// <summary>
    /// Specifies an Input/Output rule that defines which OutputSet is expected to be created by the Activity when this InputSet became valid.
    /// This attribute is paired with the inputSetRefs attribute of OutputSets.
    /// </summary>
    public List<OutputSet> outputSetRefs;
}
