using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Resources;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels;

public class ResourceViewModel
{
    public ResourceViewModel()
    {
    }

    public ResourceViewModel(Resource resource)
    {
        id = resource.Id;
        name = resource.Name;
        namespaceId = resource.NamespaceId;
        entityId = resource.EntityId;
        resourceParameters = resource.resourceParameters?.Select(p => new ResourceParameterViewModel(p)).ToList();
    }

    public string id { get; set; }
    public string name { get; set; }
    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public List<ResourceParameterViewModel> resourceParameters { get; set; }

    public void Modify(Resource resource)
    {
        resource.Id = id;
        resource.Name = name;

        resource.NamespaceId = namespaceId;
        resource.EntityId = entityId;
        resource.resourceParameters = resourceParameters?.Select(p => p.ToResourceParameter(resource)).ToList();
    }
}

public class ResourceParameterViewModel
{
    public ResourceParameterViewModel()
    {

    }

    public ResourceParameterViewModel(ResourceParameter resourceParameter)
    {
        id = resourceParameter.Id;
        name = resourceParameter.Name;
        type = resourceParameter.type;
        isRequired = resourceParameter.isRequired;
    }

    public string id { get; set; }
    public string name { get; set; }
    public ResourceParameter.eType type { get; set; }
    public bool isRequired { get; set; }

    public ResourceParameter ToResourceParameter(Resource resource)
    {
        return new ResourceParameter(resource, id, name, type, isRequired);
    }
}