global using Neo.Bpms.Domain.Features.Definitions.Entities;
using Neo.Bpms.Domain.Models.Cmmn.ServiceOperations;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.Entities;

public abstract class EntityServiceDefinition : BaseModelingDefinition
{
    public abstract void DefineAll(Entity entity);
}

public abstract class EntityServiceDefinition<TServiceGroupDefinition, TServiceOperation> : EntityServiceDefinition
    where TServiceGroupDefinition : ServiceGroupDefinition
    where TServiceOperation : IServiceOperation
{
    protected EntityServiceOperation DefineServiceOperation(string name, string serviceName,
        Type inputStructure, Type outputStructure)
    {
        ServiceOperation = new EntityServiceOperation(Entity, GetType().Name, name, ServiceInterfaceProtocol, serviceName,
            Type, Method, RelativePath, inputStructure, outputStructure);
        Entity.AddServiceOperation(ServiceOperation);
        currentBaseElement = ServiceOperation;
        return ServiceOperation;
    }


    public Entity Entity;
    public EntityServiceOperation ServiceOperation;

    protected abstract EntityServiceOperationType Type { get; }
    protected abstract ServiceOperationMethod? Method { get; set; }
    protected abstract ServiceInterfaceProtocol? ServiceInterfaceProtocol { get; set; }
    protected abstract string RelativePath { get; set; }

    public override void DefineAll(Entity entity)
    {
        Entity = entity;

        //todo
    }
}
