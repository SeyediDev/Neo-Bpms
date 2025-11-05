using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.ResourceRoles;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Lanes;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Resources;

/// <summary>
/// 14.5
/// </summary>
internal static class ResourceRoleXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, dynamic node, IResourceRoleContainer resourceRoleContainer)
    {
        var containerIsLane = resourceRoleContainer is Lane;
        var @namespace = containerIsLane ? "teta" : "";
        foreach (var resourceRole in resourceRoleContainer.resources ?? Enumerable.Empty<ResourceRole>())
        {
            dynamic element = null;
            switch (resourceRole.type)
            {
                case ResourceRole.eRoleType.FirstLevelPotentialOwner:
                case ResourceRole.eRoleType.DelegatePotentialOwner:
                case ResourceRole.eRoleType.SupervisorPotentialOwner:
                    element = containerIsLane ? node.performer() : node.potentialOwner();
                    break;
                case ResourceRole.eRoleType.PhysicalPerformerResource:
                    element = node.performer();
                    break;
                case ResourceRole.eRoleType.Confirmer:
                case ResourceRole.eRoleType.CostingResource:
                case ResourceRole.eRoleType.CapacityConstraintResource:
                case ResourceRole.eRoleType.AuthorizedUser:
                case ResourceRole.eRoleType.ProcessManagerResource:
                case ResourceRole.eRoleType.NotificationResource:
                    element = node.resourceRole();
                    switch (resourceRole.type)
                    {
                        case ResourceRole.eRoleType.Confirmer:
                            element.confirmStepId = (resourceRole as Confirmer)?.confirmStepId;
                            break;
                        case ResourceRole.eRoleType.CostingResource:
                            element.isDirect = (resourceRole as CostingResource)?.isDirect;
                            element.quantity = (resourceRole as CostingResource)?.quantity;
                            break;
                        case ResourceRole.eRoleType.CapacityConstraintResource:
                            element.quantity = (resourceRole as CapacityConstraintResource)?.quantity;
                            break;
                        case ResourceRole.eRoleType.NotificationResource:
                            ExportNotificationResource(resourceRole, element);
                            break;
                    }
                    break;
            }
            if (element == null)
                return;
            element.name = resourceRole.Name ?? resourceRole.resourceRef?.Name ?? resourceRole.Id;
            BaseElementXmlConvertor.Export(element, resourceRole, @namespace);
            var resourceRefElement = containerIsLane ? element.performerRef() : element.resourceRef();
            if (resourceRole.resourceRef != null)
                BaseElementXmlConvertor.ExportRef(resourceRefElement, resourceRole.resourceRef, @namespace);
            element.type = resourceRole.type;
            ResourceAssignmentExpressionXmlConvertor.Export(element, resourceRole, @namespace);
            ResourceParameterBindingXmlConvertor.Export(bpmnDefinitions, element, resourceRole, @namespace);
            Validate(bpmnDefinitions, resourceRole);
        }
    }

    internal static void Imports(BpmnDefinitions bpmnDefinitions,
        ElasticObject node, IResourceRoleContainer resourceRoleContainer)
    {
        resourceRoleContainer.resources = null;
        foreach (var elements in node.ElementCollections ?? Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
        {
            switch (elements.Key.ToLower())
            {
                case "resourceRole":
                case "performer":
                case "humanPerformer":
                case "potentialOwner":
                    foreach (var element in elements.Value ?? Enumerable.Empty<ElasticObject>())
                        Import(bpmnDefinitions, resourceRoleContainer, element, elements.Key);
                    break;
            }
        }
    }

    private static void Import(BpmnDefinitions bpmnDefinitions,
        IResourceRoleContainer resourceRoleContainer, ElasticObject element, string elementName)
    {
        var resourceRefElement = resourceRoleContainer is Lane
            ? element.GetElement("performerRef")
            : element.GetElement("resourceRef");
        var resource = ProjectDefinition.Project.GetResource(resourceRefElement?.InternalValue?.ToString());

        var id = element.GetString("id");
        var name = element.GetString("name");
        var resourceAssignmentExpression = ResourceAssignmentExpressionXmlConvertor.Import(resourceRoleContainer, element);
        var roleType = element.GetEnumText("type", ResourceRole.eRoleType.FirstLevelPotentialOwner);
        ResourceRole resourceRole = null;
        switch (elementName)
        {
            case "resourceRole":
                switch (roleType)
                {
                    case ResourceRole.eRoleType.PhysicalPerformerResource:
                        resourceRole = new PhysicalPerformerResource(resourceRoleContainer, id, name, resource, resourceAssignmentExpression);
                        break;
                    case ResourceRole.eRoleType.AuthorizedUser:
                        resourceRole = new AuthorizedUser(resourceRoleContainer, id, name, resource as HumanResource, resourceAssignmentExpression);
                        break;
                    case ResourceRole.eRoleType.Confirmer:
                        var confirmStepId = element.GetInteger("confirmStepId", 1);
                        resourceRole = new Confirmer(resourceRoleContainer, id, name, resource as HumanResource, resourceAssignmentExpression, confirmStepId);
                        break;
                    case ResourceRole.eRoleType.CostingResource:
                        var isDirect = element.GetBool("isDirect", true);
                        var quantity = element.GetDouble("quantity");
                        resourceRole = new CostingResource(resourceRoleContainer, id, name, resource as HumanResource, resourceAssignmentExpression, isDirect,
                            quantity);
                        break;
                    case ResourceRole.eRoleType.CapacityConstraintResource:
                        var quantity1 = element.GetDouble("quantity");
                        resourceRole = new CapacityConstraintResource(resourceRoleContainer, id, name, resource as HumanResource, resourceAssignmentExpression,
                            quantity1);
                        break;
                    case ResourceRole.eRoleType.ProcessManagerResource:
                        resourceRole = new ProcessManagerResource(resourceRoleContainer, id, name, resource as HumanResource, resourceAssignmentExpression);
                        break;
                    case ResourceRole.eRoleType.NotificationResource:
                        resourceRole = ImportNotificationResource(resourceRoleContainer, element, resource, id, name, resourceAssignmentExpression);
                        break;
                    default:
                        resourceRole =
                            new PotentialOwner(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, (PotentialOwnerType)roleType);
                        break;
                }
                break;
            case "performer":
                if (resourceRoleContainer is Lane)//todo
                    resourceRole = new PotentialOwner(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, (PotentialOwnerType)roleType);
                else
                    resourceRole = new PhysicalPerformerResource(resourceRoleContainer, id, name, resource, resourceAssignmentExpression);
                break;
            case "humanPerformer":
            case "potentialOwner":
                resourceRole = new PotentialOwner(resourceRoleContainer, id, name, resource, resourceAssignmentExpression, (PotentialOwnerType)roleType);
                break;
        }
        if (resourceRole == null) return;
        if (resourceAssignmentExpression != null)
            resourceAssignmentExpression.Parent = resourceRole;
        ResourceParameterBindingXmlConvertor.Import(bpmnDefinitions, resourceRoleContainer, element, resourceRole);
        resourceRoleContainer.AddResourceRole(resourceRole);
        Validate(bpmnDefinitions, resourceRole);
    }

    private static void Validate(BpmnDefinitions bpmnDefinitions, ResourceRole resourceRole)
    {
        if (resourceRole.resourceRef == null)
            bpmnDefinitions.ErrorInfos.AddError($"The ResourceRole {resourceRole.Id}, doesn't contain resource!", "ResourceRole",
                "14.5.0", "Resource roles should contain resource.");
    }

    private static void ExportNotificationResource(ResourceRole resourceRole, dynamic element)
    {
        var notification = resourceRole as NotificationResourceRole;
        element.onNoResource = (notification?.notificationTimes & NotificationTimes.onNoResource) != 0;
        element.onAllocate = (notification?.notificationTimes & NotificationTimes.onAllocate) != 0;
        element.onDo = (notification?.notificationTimes & NotificationTimes.onDo) != 0;
        element.onComplete = (notification?.notificationTimes & NotificationTimes.onComplete) != 0;
        element.onDueTimeExpiration = (notification?.notificationTimes & NotificationTimes.onDueTimeExpiration) != 0;
        element.onError = (notification?.notificationTimes & NotificationTimes.onError) != 0;
        element.onCancel = (notification?.notificationTimes & NotificationTimes.onCancel) != 0;
    }

    private static ResourceRole ImportNotificationResource(IResourceRoleContainer resourceRoleContainer, ElasticObject element, Resource resource, string id, string name, ResourceAssignmentExpression resourceAssignmentExpression)
    {
        var notificationTime = NotificationTimes.none;
        if (element.GetBool("onNoResource"))
            notificationTime |= NotificationTimes.onNoResource;
        if (element.GetBool("onAllocate"))
            notificationTime |= NotificationTimes.onAllocate;
        if (element.GetBool("onDo"))
            notificationTime |= NotificationTimes.onDo;
        if (element.GetBool("onComplete"))
            notificationTime |= NotificationTimes.onComplete;
        if (element.GetBool("onDueTimeExpiration"))
            notificationTime |= NotificationTimes.onDueTimeExpiration;
        if (element.GetBool("onError"))
            notificationTime |= NotificationTimes.onError;
        if (element.GetBool("onCancel"))
            notificationTime |= NotificationTimes.onCancel;
        var resourceRole = new NotificationResourceRole(resourceRoleContainer, id,
            name, resource as HumanResource, resourceAssignmentExpression,
            notificationTime);
        return resourceRole;
    }
}
