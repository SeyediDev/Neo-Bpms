using Neo.Bpms.Domain.Features.Dynamic;

namespace Neo.Bpms.Application.Features;
public interface ISendFormCommand
{
    public Task<(bool result, string message)> Send(ElasticObject record, Type commandType);
}
