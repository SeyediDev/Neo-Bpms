using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.Internal;

public interface IInternalServiceOperation : IServiceOperation
{
    Type RunTimeType { get; set; }
}
