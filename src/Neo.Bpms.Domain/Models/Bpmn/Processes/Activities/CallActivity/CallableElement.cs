using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.CallActivity;

/// <summary>
/// When a CallableElement is exposed as a Service, it has to define one or more InputOutputBinding
/// elements. An InputOutputBinding element binds one Input and one Output of the InputOutputSpecification 
/// to an Operation of a Service Interface. 
/// </summary>
public class CallableElement(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name), IIoSpecificationContainer
{
    //public string name { get; set; }
    /// <summary>
    /// The Interfaces describing the external behavior provided by this element
    /// </summary>
    public List<Interface> supportedInterfaceRefs;

    /// <summary>
    /// The InputOutputSpecification defines the inputs and outputs and the InputSets and OutputSets for the Activity.
    /// </summary>
    public InputOutputSpecification ioSpecification { get; set; }

    /// <summary>
    /// The InputOutputBinding defines a combination of one InputSet and one OutputSet in order to bind this to 
    /// an operation defined in an interface.
    /// </summary>
    public List<InputOutputBinding> ioBinding;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not CallableElement newItem) return;
        Name = newItem.Name;
        ioSpecification = newItem.ioSpecification;
        supportedInterfaceRefs = newItem.supportedInterfaceRefs;
        CloneBase(newRootElement);
    }

    public IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems)
    {
        return ItemAwareContainer.GetItemAwareElement(this, itemId, itemName, fromInputItems);
    }
}