using Neo.Bpms.Domain.Entities.Service.ServiceEndPoint;

namespace Neo.Bpms.Domain.Entities.Service.ServiceOperation;

public class ServiceInterfaceDefinitionItem(
    string id, string name,
    ServiceEndPointDefinition endPoint,
    ServiceGroupDefinition _interface,
    ServiceInterfaceProtocol protocol
        ) : BaseModelClass(id, name)
{
    public ServiceEndPointDefinition EndPoint { get; set; } = endPoint;
    public ServiceGroupDefinition Interface { get; set; } = _interface;
    public ServiceInterfaceProtocol Protocol { get; set; } = protocol;
}