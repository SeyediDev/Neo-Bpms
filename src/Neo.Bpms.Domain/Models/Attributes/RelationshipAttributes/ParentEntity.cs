namespace Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

/// <summary>
/// Parent Entity Map
/// 
/// entity can have multiple Parent Entity Maps.
/// Parent Entity Map is for mapping fields between entity and parent entity if hierarchy implementation type is relation.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class ParentEntityAttribute(string booleanFieldIdInParent) : Attribute
{
    public RelationDeleteUpdateBehavior Behaviour { get; set; } = RelationDeleteUpdateBehavior.Cascade;
    public RelationDeleteUpdateBehavior OnUpdateBehaviour { get; set; } = RelationDeleteUpdateBehavior.Cascade;
    public string BooleanFieldIdInParent { get; set; } = booleanFieldIdInParent;
}

/*
	[AttributeUsage(AttributeTargets.Class)]
	public class ParentEntityModificationAttribute : Attribute
	{
		public ParentEntityModificationAttribute(string parent, string booleanFieldIdInParent)
		{
			Parent = parent;
			BooleanFieldIdInParent = booleanFieldIdInParent;
		}

		public string Parent { get; set; }

		public string BooleanFieldIdInParent { get; set; }
	}*/