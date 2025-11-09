using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

/// <summary>
/// An Event is something that “happens” during the course of a Process. These Events affect the flow of the Process
/// and usually have a cause or an impact. The term “event” is general enough to cover many things in a Process. The start
/// of an Activity, the end of an Activity, the change of state of a document, a Message that arrives, etc., all could be
/// considered Events. However, BPMN has restricted the use of Events to include only those types of Events that will
/// affect the sequence or timing of Activities of a Process.
/// </summary>
/// <remarks>
/// Handling Events
/// 
/// BPMN provides advanced constructs for dealing with Events that occur during the execution of a Process (i.e., the “catching” of an Event). 
/// Furthermore, BPMN supports the explicit creation of an Event in the Process (i.e., the “throwing” of an Event). 
/// Both catching and throwing of an Event as well as the resulting Process behavior is referred to as Event handling. 
/// There are three types of Event handlers: those that start a Process, those that are part of the normal Sequence Flow, 
/// and those that are attached to Activities, either via boundary Events or via separate inline handlers in case of an Event Sub-Process.
/// 
/// Handling Start Events
/// There are multiple ways in which a Process can be started. For single Start Events, handling consists of starting a new 
/// Process instance each time the Event occurs. Sequence Flows leaving the Event are then followed as usual. For
/// multiple Start Events, BPMN supports several modeling scenarios that can be applied depending on the scenario.
/// Exclusive start: the most common scenario for starting a Process is its instantiation by exactly one out of many
/// possible Start Events. Each occurrence of one of these Events will lead to the creation of a new Process instance.
/// The following example shows two Events connected to a single Activity (see Figure 10.97). At runtime, each
/// occurrence of one of the Events will lead to the creation of a new instance of the Process instance and activation of the
/// Activity. Note that a single Multiple Start Event that contains the Message Event Definitions would behave in
/// the same way.
/// 
/// A Process can also be started via an Event-Based Gateway, In that case, the first matching Event will create a new instance of the Process, 
/// and waiting for the other Events originating from the same decision stops, following the usual semantics of the Event-Based Exclusive Gateway. Note
/// that this is the only scenario where a Gateway can exist without an incoming Sequence Flows.
/// It is possible to have multiple groups of Event-Based Gateways starting a Process, provided they participate in the
/// same Conversation and hence share the same correlation information. In that case, one Event out of each group needs
/// to arrive; the first one creates a new Process instance, while the subsequent ones are routed to the existing instance,
/// which is identified through its correlation information.
/// 
/// Event synchronization: if the modeler requires several disjoint Start Events to be merged into a single Process instance
/// The Parallel Start Event MAY group several disjoint Start Events each of which MUST occur once in order for an 
/// instance of the Process to be created. Sequence Flows leaving the Event are then followed as usual.
/// 
/// Handling Events within normal Sequence Flow (Intermediate Events)
/// 
/// For Intermediate Events, the handling consists of waiting for the Event to occur. Waiting starts when the
/// Intermediate Event is reached. Once the Event occurs, it is consumed. Sequence flows leaving the Event are followed as usual.
/// 
/// Handling Events attached to an Activity (Intermediate boundary Events and Event Sub-Processes)
/// For boundary Events, handling consists of consuming the Event occurrence and either canceling the Activity the Event
/// is attached to, followed by normal Sequence Flows leaving that Activity, or by running an Event Handler without
/// canceling the Activity (only for Message, Signal, Timer and Conditional Events, not for Error Events).
/// An interrupting boundary Event is defined by a true value of its cancelActivity attribute. Whenever the Event
/// occurs, the associated Activity is terminated. A downstream token is then generated, which activates the next element of
/// the Process (connected to the Event by an unconditional Sequence Flow called an exception flow).
/// For non-interrupting boundary Events, the cancelActivity attribute is set to false. Whenever the Event occurs, the
/// associated Activity continues to be active. As a token is generated for the Sequence Flow from the boundary Event in
/// parallel to the continuing execution of the Activity, care MUST be taken when this flow is merged into the main flow of
/// the Process – typically it should be ended with its own End Event.
/// 
/// Interrupting Event Handlers (Error, Escalation, Message, Signal, Timer, Conditional, Multiple, and Parallel Multiple)
/// Interrupting Event Handlers are those that have the cancelActivity attribute is set to true. Whenever the Event
/// occurs, regardless of whether the Event is handled inline or on the boundary, the associated Activity is interrupted. If an
/// inline error handler is specified (in case of a Sub-Process), it is run within the context of that Sub-Process. If a
/// boundary Error Event is present, Sequence Flows from that boundary Event are then followed. The parent Activity
/// is canceled after either the error handler completes or Sequence Flow from the boundary Event is followed.
/// In the example above, the “Booking” Sub-Process has an Error handler that defines what should happen in case a
/// “Booking” Error occurs within the Sub-Process, namely, the already performed bookings are canceled using
/// compensation. The Error handler is then continued outside the Sub-Process through a boundary Error Event.
/// 
/// Non-interrupting Event Handlers (Escalation, Message, Signal, Timer, Conditional, Multiple, and Parallel Multiple)
/// Interrupting Event Handlers are those that have the cancelActivity attribute is set to false.
/// For Event Sub-Processes, whenever the Event occurs it is consumed and the associated Event Sub-Process is
/// performed. If there are several Events that happen in parallel, then they are handled concurrently, i.e., several Event
/// Sub-Process instances are created concurrently. The non-interrupting Start Event indicates that the Event Sub-
/// Process instance runs concurrently to the Sub-Process proper.
/// For boundary Events, whenever the Event occurs the handler runs concurrently to the Activity. If an Event Sub-Process 
/// is also specified for that Event (in case of a Sub-Process), it is run within the context of that Sub-Process.
/// Then, Sequence Flows from the boundary Event are followed. As a token is generated for the Sequence Flow from
/// the boundary Event in parallel to the continuing execution of the Activity, care MUST be taken when this flow is
/// merged into the main flow of the Process – typically it should be ended with its own End Event.
/// In the example above, an Event Handler allows to update the credit card information during the “Booking” Sub-
/// Process. It is triggered by a credit card information Message: such a Message can be received whenever the control
/// flow is within the main body of the Sub-Process. Once such a Message is received, the Activities within the
/// corresponding Event Handler run concurrently with the Activities within the body of the Sub-Process.
/// See “Intermediate Events” on page 440 for the exact semantics of boundary Intermediate Events
/// 
/// Handling End Events
/// For a Terminate End Event, all remaining active Activities within the Process are terminated.
/// A Cancel End Event is only allowed in the context of a Transaction Sub-Process and, as such, cancels the Sub-
/// Process and aborts an associated Transaction of the Sub-Process.
/// For all other End Events, the behavior associated with the EventDefinition is performed. When there are no
/// further active Activities, then the Sub-Process or Process instance is completed.
/// </remarks>
public class Event(IFlowElementsContainer flowElementsContainer, string id, string name) : FlowNode(flowElementsContainer, id, name, eFlowNodeType.Event), IPropertyContainer
{
    public List<Property> properties { get; set; }

    //// <summary>
    //// References the reusable EventDefinitions that are triggers expected for a catch Event. These EventDefinitions are only valid inside the current Event.
    //// • If there is no EventDefinition defined, then this is considered a catch None Event and the Event will not have an internal marker.
    //// • If there is more than one EventDefinition defined, this is considered a catch Multiple Event and the Event will have the pentagon internal marker.
    //// This is an ordered set.
    //// </summary>
    // public List<EventDefinition> eventDefinitionRefs;

    /// <summary>
    /// Defines the event EventDefinitions that are triggers expected for a catch Event. These EventDefinitions are only valid inside the current Event.
    /// • If there is no EventDefinition defined, then this is considered a catch None Event and the Event will not have an internal marker.
    /// • If there is more than one EventDefinition defined, this is considered a catch Multiple Event and the Event will have the pentagon internal marker.
    /// This is an ordered set.
    /// </summary>
    public List<EventDefinition> eventDefinitions; //in bpmn this determinate in catch and throw

    public eEventType TriggerType => GetEventType(eventDefinitions);

    public void AddEventDefinitions(params EventDefinition[] newEventDefinitions)
    {
        eventDefinitions ??= [];
        foreach (EventDefinition ev in newEventDefinitions)
        {
            eventDefinitions.Add(ev);
        }
    }

    public IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems)
    {
        return ItemAwareContainer.GetItemAwareElement(this, itemId, itemName, fromInputItems);
    }

    public static eEventType GetEventType(IEnumerable<EventDefinition> eventDefinitions)
    {
        eEventType eventType = eEventType.None;
        foreach (EventDefinition eventDefinition in eventDefinitions ?? [])
        {
            if (eventDefinition.type != eventType)
            {
                eventType = eventType == eEventType.None
                    ? eventDefinition.type
                    : eEventType.Multiple;
            }

            if (eventType == eEventType.Multiple)
                break;
        }

        return eventType;
    }

    public enum eEventType
    {
        None = 1,

        /// <summary>
        /// all catches , throws
        /// </summary>
        Message,

        /// <summary>
        /// all catches , throws
        /// </summary>
        Signal,

        /// <summary>
        /// all catches
        /// </summary>
        Timer,

        /// <summary>
        /// all catches
        /// </summary>
        Condition,

        /// <summary>
        /// both boundary, both event subprocess catches, intermediate throw, end
        /// </summary>
        Escalation,

        /// <summary>
        /// end event(of subprocess), interrupting boundary, interrupting event subprocess
        /// </summary>
        Error,

        /// <summary>
        /// end event(of trnsactional subprocess), interrupting boundary(of trnsactional subprocess activity)
        /// </summary>
        Cancel,

        /// <summary>
        /// only end event
        /// </summary>
        Terminate,

        /// <summary>
        /// intermediate catch, throw
        /// </summary>
        Link,

        /// <summary>
        /// interrupting boundary, interrupting event subprocess, intermediate throw, end
        /// </summary>
        Compensation,

        /// <summary>
        /// all catches, throws
        /// </summary>
        Multiple,

        // /// <summary>
        // /// all catches
        // /// </summary>
        // ParallelMultiple
    }
}
