using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Cmmn.ServiceOperations;

//public class RestEntityServiceOperation<TIn, TOut> : EntityServiceOperation<TIn, TOut>
//{
//	public RestEntityServiceOperation(Entity entity, string id, string name,
//		string serviceName, EntityServiceOperationType entityServiceOperationType)
//		: base(entity, id, name, ServiceInterfaceProtocol.DirectRest, 
//			serviceName, entityServiceOperationType)
//	{
//	}
//}
//public class EntityServiceOperation<TIn, TOut> : EntityServiceOperation
//{
//	public EntityServiceOperation(Entity entity, string id, string name,
//		ServiceInterfaceProtocol serviceInterfaceProtocol, string serviceName, 
//		EntityServiceOperationType entityServiceOperationType)
//		: base(entity, id, name, serviceInterfaceProtocol, 
//			serviceName, entityServiceOperationType, typeof(TIn), typeof(TOut))
//	{
//	}
//}
public class EntityServiceOperation(Entity entity, string id, string name,
    ServiceInterfaceProtocol? serviceInterfaceProtocol, string serviceName,
    EntityServiceOperationType entityServiceOperationType,
    ServiceOperationMethod? method, string relativePath, Type inputStructure, Type outputStructure) : BaseModelClass(entity, id, name)
{
    public EntityServiceOperation(Entity entity, string id, string name,
        string serviceName, EntityServiceOperationType entityServiceOperationType,
        Type inputStructure, Type outputStructure)
        : this(entity, id, name, null, serviceName, entityServiceOperationType, null,
            null, inputStructure, outputStructure)
    {
    }

    public Type InputStructure { get; } = inputStructure;
    public Type OutputStructure { get; } = outputStructure;
    public string ServiceName { get; set; } = serviceName;
    public EntityServiceOperationType EntityServiceOperationType { get; set; } = entityServiceOperationType;
    public ServiceInterfaceProtocol? ServiceInterfaceProtocol { get; set; } = serviceInterfaceProtocol;
    public ServiceOperationMethod? Method { get; set; } = method;
    public string RelativePath { get; set; } = relativePath;

    //service address : service group service name  / in service enum that defines types
    //Tolist contains subject and const filters like paging
    // get next nadarad !!
}