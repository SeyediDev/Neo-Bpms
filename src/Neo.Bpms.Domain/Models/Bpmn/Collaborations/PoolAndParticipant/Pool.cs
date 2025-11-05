namespace Neo.Bpms.Domain.Models.Bpmn.Collaborations.PoolAndParticipant;

/// <summary>
/// A Pool is the graphical representation of a Participant in a Collaboration. 
/// A Participant can be a specific PartnerEntity (e.g., a company) or can be a more general PartnerRole (e.g., a buyer, seller, or manufacturer). 
/// A Pool MAY or MAY NOT reference a Process. A Pool is NOT REQUIRED to contain a Process, i.e., it can be a “black box.”
/// 
/// The Sequence Flows can cross the boundaries between Lanes of a Pool but cannot cross the boundaries of a Pool. 
/// 
/// A Collaboration can contain two (2) or more Pools (i.e., Participants). 
/// However, a Process that represents the work performed from the point of view of the modeler or the modeler’s organization can be considered “internal” and is NOT REQUIRED to be surrounded by the boundary of the Pool, while the other Pools in the Diagram MUST have their boundary.
/// </summary>
//public class Pool : Participant
//{
//	public Pool(string id) :
//		base(id)
//	{

//	}
//	public enum TOrientation  { Vertical = 1 , Horizental = 2 };
//	public FlowNode.NodeGraphicsInfo nodeGraphicsInfo;
//	public bool BoundaryVisible;
//	public bool IsMainPool;
//	public int Id;
//	public string Name;
//	public TOrientation Orientation;


//}
