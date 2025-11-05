using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.External;

public interface IExternalServiceOperation : IServiceOperation
{
    IExternalServiceOperationRuntime RunTime { get; set; }
}
