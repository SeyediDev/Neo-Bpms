using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

/// <summary>
/// A Message represents the content of a communication between two Participants. In BPMN 2.0, a Message is a 
/// graphical decorator. An ItemDefinition is used to specify the Message structure.
/// </summary>
public class Message(BpmnDefinitions parent, string id, string name, ItemDefinition itemRef) : RootElement(parent, id, name)
{
    //		public string name;

    /// <summary>
    /// An ItemDefinition is used to define the “payload” of the Message.
    /// </summary>
    public ItemDefinition itemRef = itemRef;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Message newItem)
        {
            return;
        }

        Name = newItem.Name;
        itemRef = newItem.itemRef;
        CloneBase(newRootElement);
    }
}
