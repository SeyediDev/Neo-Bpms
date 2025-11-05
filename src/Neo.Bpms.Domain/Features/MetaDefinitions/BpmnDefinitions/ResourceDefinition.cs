namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class BpmnDefinitionsDefinition
{
    protected Resource _currentResource;
    protected HumanResource CurrentHumanResource => _currentResource as HumanResource;

    /// <summary>
    /// Adds the human resource.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    protected bool AddHumanResource(string id, string name, HumanResourceType type)
    {
        string namespaceId;
        string entityId;
        switch (type)
        {
            case HumanResourceType.User:
                namespaceId = "UserAndOrganization";
                entityId = "SystemUser";
                break;
            case HumanResourceType.UserGroup:
                namespaceId = "UserAndOrganization";
                entityId = "SystemUserGroup";
                break;
            default:
                return false;
        }
        return AddHumanResource(id, name, namespaceId, entityId);
    }

    /// <summary>
    /// Adds the human resource.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="namespaceId">The namespace identifier</param>
    /// <param name="entityId">The entity identifier</param>
    /// <returns></returns>
    protected bool AddHumanResource(string id, string name, string namespaceId, string entityId)
    {
        _currentResource = ProjectDefinition.Project.GetResource(id);
        if (_currentResource == null)
        {
            _currentResource = ProjectDefinition.Project.AddResource(id, name, namespaceId, entityId);
        }
        else
        {
            if (_currentResource.NamespaceId != namespaceId)
            {
                throw new Exception("conflict in duplicate definition of resource " + id);
            }

            _currentResource.NamespaceId = namespaceId;
            if (_currentResource.EntityId != entityId)
            {
                throw new Exception("conflict in duplicate definition of resource " + id);
            }

            _currentResource.EntityId = entityId;
        }
        return CurrentHumanResource == null ? throw new Exception("Human resource conflict " + id) : true;
    }

    /// <summary>
    /// Adds the physical resource. 
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="namespaceId">The namespace identifier</param>
    /// <param name="entityId">The entity identifier</param>
    /// <returns></returns>
    protected bool AddPhysicalResource(string id, string name, string namespaceId, string entityId)
    {
        return definitions != null && ProjectDefinition.Project.AddResource(id, name, namespaceId, entityId) != null;
    }

    /// <summary>
    /// Adds the resourceParameter. 
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="isRequired">if sets to <c>true</c> [is required]</param>
    /// <returns></returns>
    protected void AddResourceParameter(ResourceParameter.eType type, string name, bool isRequired)
    {
        if (_currentResource == null)
        {
            return;
        }

        ResourceParameter rp = _currentResource.resourceParameters?.FirstOrDefault(p =>
            p.type == type && p.Name == name);
        if (rp != null)
        {
            return;
        }

        _currentResource.resourceParameters ??= [];
        _currentResource.resourceParameters.Add(
            new ResourceParameter(_currentResource,
                $"Resource{_currentResource.Id}.Parameter{name}", name, type, isRequired));
    }
}
