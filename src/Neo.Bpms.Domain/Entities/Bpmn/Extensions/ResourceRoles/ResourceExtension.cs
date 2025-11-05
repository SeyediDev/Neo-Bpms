using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

namespace Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

public partial class Resource
{
    public string NamespaceId;
    public string EntityId;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not Resource newItem) return;
        Name = newItem.Name;
        resourceParameters = newItem.resourceParameters;

        NamespaceId = newItem.NamespaceId;
        EntityId = newItem.EntityId;
        CloneBase(newItem);
    }
}