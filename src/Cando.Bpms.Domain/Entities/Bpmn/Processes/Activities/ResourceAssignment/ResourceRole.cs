using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;

/// <summary>
/// نقش منابع: کلاس پایه برای نقش‌های مختلف فرآیند و فعالیت 
/// بععضی از نقش‌ها، به عنوان انجام دهنده هستند و بعضی از نفششها به عنوان تعیین کننده ظرفیت و یا تعیین کننده هزینه انجام فعالیت به منظور شبیه سازی، هزینه یابی و یا منظروهای دیگر
/// 
/// TODO: define other Resource roles for other purposes
/// </summary>
public class ResourceRole(IResourceRoleContainer resourceRoleContainer,
    string id, string name, Resource resourceRef,
    ResourceAssignmentExpression resourceAssignmentExpression, ResourceRole.eRoleType type) : BaseElement(resourceRoleContainer as BaseElement, id, name)
{
    //		public string name;

    /// <summary>
    /// The Resource that is associated with Activity. 
    /// Should not be specified when resourceAssignmentExpression is provided.
    /// </summary>
    public Resource resourceRef = resourceRef;

    /// <summary>
    /// This defines the Expression used for the Resource assignment. 
    /// Should not be specified when a resourceRef is provided.
    /// </summary>
    public ResourceAssignmentExpression resourceAssignmentExpression = resourceAssignmentExpression;

    /// <summary>
    /// This defines the Parameter bindings used for the Resource assignment. 
    /// Is only applicable if a resourceRef is specified.
    /// </summary>
    public List<ResourceParameterBinding> resourceParameterBindings { get; set; } = [];

    public eRoleType type { get; set; } = type;

    public enum eRoleType
    {
        None,
        FirstLevelPotentialOwner = 1,//potentialOwner => in lane only this
        DelegatePotentialOwner,//potentialOwner
        SupervisorPotentialOwner,//potentialOwner

        PhysicalPerformerResource = 11,//performer
                                       //humanPerformer?

        AuthorizedUser = 21,//resourceRole 
        Confirmer,//resourceRole

        CostingResource = 31,//resourceRole
        CapacityConstraintResource,//resourceRole

        ProcessManagerResource = 41,//resourceRole => only in process

        NotificationResource = 51
    }
}

public interface IResourceRoleContainer
{
    List<ResourceRole> resources { get; set; }
    void AddResourceRole(ResourceRole resourceRole);
}