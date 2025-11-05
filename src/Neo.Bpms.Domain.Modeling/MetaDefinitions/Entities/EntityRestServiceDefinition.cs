using Neo.Bpms.Domain.Entities.Cmmn.ServiceOperations;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Entities;

public abstract class EntityRestServiceDefinition : EntityServiceDefinition
{
    protected Entity Entity { get; private set; }
    protected EntityServiceOperation ServiceOperation { get; private set; }
    protected abstract ServiceOperationMethod Method { get; }
    protected abstract EntityServiceOperationType Type { get; }
    protected abstract string RelativePath { get; }

    public interface IServiceOperationRequest { }
    public interface IServiceOperationResponse { }

    public override void DefineAll(Entity entity)
    {
        Entity = entity;
        Type request = ExtractSubs<IServiceOperationRequest>()?.FirstOrDefault();
        Type response = ExtractSubs<IServiceOperationResponse>()?.FirstOrDefault();
        ServiceOperation = new EntityServiceOperation
        (Entity, GetType().Name, GetType().Name, ServiceInterfaceProtocol.DirectRest, GetType().Name, Type,
            Method, RelativePath, request, response);
        Entity.AddServiceOperation(ServiceOperation);
        currentBaseElement = ServiceOperation;
    }
}
