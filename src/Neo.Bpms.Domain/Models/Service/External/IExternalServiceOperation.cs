namespace Neo.Bpms.Domain.Models.Service.External;

public interface IExternalServiceOperation : IServiceOperation
{
    IExternalServiceOperationRuntime RunTime { get; set; }
}
