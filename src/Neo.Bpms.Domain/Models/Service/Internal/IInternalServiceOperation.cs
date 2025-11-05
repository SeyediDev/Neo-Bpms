namespace Neo.Bpms.Domain.Models.Service.Internal;

public interface IInternalServiceOperation : IServiceOperation
{
    Type RunTimeType { get; set; }
}
