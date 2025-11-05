using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Artifacts;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Auditing;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.CallActivity;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Monitoring;

namespace Neo.Bpms.Domain.Model.BPMN.Processes;

/// <summary>
/// A Process describes a sequence or flow of Activities in an organization with the objective of carrying out work. 
/// In BPMN a Process is depicted as a graph of Flow Elements, which are a set of Activities, Events, Gateways, and
/// Sequence Flows that define finite execution semantics.
/// 
/// Processes can be defined at any level from enterprise-wide Processes to Processes performed by a single person. 
/// Low-level Processes can be grouped together to achieve a common business goal.
/// 
/// Note that BPMN uses the term Process specifically to mean a set of flow elements. 
/// It uses the terms Collaboration and Choreography when modeling the interaction between Processes.
/// The Process package contains classes that are used for modeling the flow of Activities, Events, and Gateways, and
/// how they are sequenced within a Process (see Figure 10.2). When a Process is defined it is contained within Definitions.
/// 
/// A Process is a CallableElement, allowing it to be referenced and reused by other Processes via the Call Activity construct. 
/// In this capacity, a Process MAY reference a set of Interfaces that define its external behavior.
/// 
/// A Process is a reusable element and can be imported and used within other Definitions.
/// 
/// </summary>
/// <remarks>
/// Private Business Processes are those internal to a specific organization. 
/// These Processes have been generally called workflow or BPM Processes. 
/// Another synonym typically used in the Web services area is the Orchestration of services. 
/// There are two types of private Processes: executable and non-executable. 
/// An executable Process is a Process that has been modeled for the purpose of being executed.
/// Of course, during the development cycle of the Process, there will be stages where the
/// Process does not have enough detail to be “executable.” A non-executable Process is a private Process that has been 
/// modeled for the purpose of documenting Process behavior at a modeler-defined level of detail. 
/// Thus, information needed for execution, such as formal condition Expressions are typically not included in a non-executable Process.
/// 
/// A public Process represents the interactions between a private Business Process and another Process or Participant
/// Only those Activities that are used to communicate to the other Participant(s), plus the order of these Activities, 
/// are included in the public Process. All other “internal” Activities of the private Business Process are not shown in the public Process. 
/// Thus, the public Process shows to the outside world the Messages, and the order of these Messages, that are needed to interact with that Business Process. 
/// Public Processes can be modeled separately or within a Collaboration to show the flow of Messages between the public Process Activities and other Participants.
/// </remarks>
public partial class Process : CallableElement,
    IFlowElementsContainer, IArtifactContainer, IResourceRoleContainer, IPropertyContainer,
    IAuditingContainer, IMonitoringContainer
{
    /// <summary>
    /// The processType attribute Provides additional information about the level of abstraction modeled by this Process.
    /// A public Process shows only those flow elements that are relevant to external consumers. 
    /// Internal details are not modeled. These Processes are publicly visible and can be used within a Collaboration. 
    /// A private Process is one that is internal to a specific organization.
    /// </summary>
    public ProcessType processType { get; set; } = ProcessType.None;

    /// <summary>
    /// An optional Boolean value specifying whether the Process is executable.
    /// 
    /// An executable Process is a private Process that has been modeled for 
    /// the purpose of being executed. Of course, during the development cycle of the Process,
    /// there will be stages where the Process does not have enough detail to be “executable.”
    /// 
    /// A non-executable Process is a private Process that has been modeled
    /// for the purpose of documenting Process behavior at a modeler-defined
    /// level of detail. Thus, information needed for execution, such as formal condition expressions 
    /// are typically not included in a non-executable Process.
    /// 
    /// For public Processes, no value has the same semantics as if the value were false. 
    /// The value MAY not be true for public Processes.
    /// </summary>
    public bool isExecutable { get; set; }

    /// <summary>
    /// provides a hook for specifying audit related properties.
    /// </summary>
    public Auditing auditing { get; set; }

    /// <summary>
    /// provides a hook for specifying monitoring related properties.
    /// </summary>
    public Monitoring monitoring { get; set; }

    public List<Artifact> artifacts { get; set; }

    /// <summary>
    /// A boolean value specifying whether interactions, such as sending andreceiving Messages and Events, 
    /// not modeled in the Process can occur when the Process is executed or performed. If the value is true,
    /// they MAY NOT occur. If the value is false, they MAY occur.
    /// </summary>
    public bool isClosed = false;

    /// <summary>
    /// Modelers can declare that they intend all executions or performances of one Process to also be valid for another Process. 
    /// This means they expect all the executions or performances of the first Processes to also follow the steps laid out in the second Process.
    /// </summary>
    public List<Process> supports;

    /// <summary>
    /// Modeler-defined properties MAY be added to a Process. 
    /// These properties are contained within the Process. 
    /// All Tasks and Sub-Processes SHALL have access to these properties.
    /// </summary>
    public List<Property> properties { get; set; }

    /// <summary>
    /// Defines the resource that will perform or will be responsible for the Process. 
    /// The resource, e.g., a performer, can be specified in the form of a specific individual, a group, an organization role or position, or an organization.
    /// Note that the assigned resources of the Process does not determine the assigned resources of the Activities that are contained by the Process.
    /// </summary>
    public List<ResourceRole> resources { get; set; }

    /// <summary>
    /// correlationSubscriptions are a feature of context-based correlation. 
    /// CorrelationSubscriptions are used to correlate incoming Messages against data in the Process context. 
    /// A Process MAY contain several correlationSubscriptions.
    /// </summary>
    public List<CorrelationSubscription> correlationSubscriptions;

    /// <summary>
    /// For Processes that interact with other Participants, a definitional Collaboration can be referenced by the Process. 
    /// The definitional Collaboration specifies the Participants the Process interacts with, and more specifically, which individual service, Send or Receive Task,
    /// or Message Event, is connected to which Participant through Message Flows. 
    /// The definitional Collaboration need not be displayed. 
    /// Additionally, the definitional Collaboration can be used to include Conversation information within a Process.
    /// </summary>
    public Collaboration definitionalCollaborationRef;

    /// <summary>
    /// Flow elements are Events, Gateways, Sequence Flows, Activities, Data Objects, Data Associations, and Choreography Activities.
    /// Note that: 
    /// • Choreography Activities MUST NOT be included as a flowElement for a Process.
    /// • Activities, Data Associations, and Data Objects MUST NOT be included as a flowElement for a Choreography.
    /// </summary>
    public Dictionary<string, FlowElement> flowElements { get; set; } = [];

    public List<LaneSet> laneSets { get; set; } = [];


    public void AddResourceRole(ResourceRole resourceRole)
    {
        resources ??= [];
        resources.Add(resourceRole);
    }

    public enum ProcessType
    {
        None,
        Private,
        Public
    }
}
