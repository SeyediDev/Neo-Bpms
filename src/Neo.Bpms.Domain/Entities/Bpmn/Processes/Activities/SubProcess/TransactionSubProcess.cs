using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;

/// <summary>
/// A Transaction is a specialized type of Sub-Process(Embeded) that will have a special behavior that is controlled through a
/// transaction protocol (such as WS-Transaction). The boundary of the Sub-Process will be double-lined to indicate that it
/// is a Transaction
/// </summary>
/// <remarks>
/// There are three basic outcomes of a Transaction:
/// 
/// 1. Successful completion: this will be shown as a normal Sequence Flow that leaves the Transaction Sub-Process.
/// 2. Failed completion (Cancel): When a Transaction is cancelled, the Activities inside the Transaction will be
/// subjected to the cancellation actions, which could include rolling back the Process and compensation 
/// for specific Activities. 
/// Note that other mechanisms for interrupting a Transaction Sub-Process will not cause compensation 
/// (e.g., Error, Timer, and anything for a non-Transaction Activity). 
/// A Cancel Intermediate Event, attached to the boundary of the Activity, will direct the flow after the Transaction 
/// has been rolled back and all compensation has been completed. The Cancel Intermediate Event
/// can only be used when attached to the boundary of a Transaction Sub-Process. It cannot be used in any normal
/// flow and cannot be attached to a non-Transaction Sub-Process. There are two mechanisms that can signal the
/// cancellation of a Transaction:
/// • A Cancel End Event is reached within the transaction Sub-Process. A Cancel End Event can only
/// be used within a transaction Sub-Process.
/// • A cancel Message can be received via the transaction protocol that is supporting the execution of the
/// Transaction Sub-Process.
/// 3. Hazard: This means that something went terribly wrong and that a normal success or cancel is not possible. 
/// Error Intermediate Events are used to show Hazards. When a Hazard happens, the Activity is interrupted 
/// (without compensation) and the flow will continue from the Error Intermediate Event.
/// 
/// 
/// The behavior at the end of a successful Transaction Sub-Process is slightly different than that of a 
/// normal Sub-Process. When each path of the Transaction Sub-Process reaches a non-Cancel End Event(s), 
/// the flow does not immediately move back up to the higher-level parent Process, as does a normal Sub-Process. 
/// First, the transaction protocol needs to verify that all the Participants have successfully completed their 
/// end of the Transaction. Most of the time this will be true and the flow will then move up to the higher-level 
/// Process. But it is possible that one of the Participants can end up with a problem that causes a Cancel or a Hazard. 
/// In this case, the flow will then move to the appropriate Intermediate Event, even though it had apparently finished successfully.
/// </remarks>
public class TransactionSubProcess(IFlowElementsContainer flowElementsContainer,
    string id, string name) : SubProcess(flowElementsContainer, id, name, eActivityType.TransactionSubProcess)
{
    /// <summary>
    /// The method is an attribute that defines the Transaction method used to commit or cancel a Transaction. 
    /// For executable Processes, it SHOULD be set to a technology specific URI, e.g., http://schemas.xmlsoap.org/ws/2004/10/wsat for WSAtomicTransaction,
    /// or http://docs.oasis-open.org/ws-tx/wsba/2006/06/AtomicOutcome for WS-BusinessActivity. 
    /// For compatibility with BPMN 1.1, it can also be set to "##compensate", "##store", or "##image".
    /// </summary>
    public string method;//TransactionMethod
}
