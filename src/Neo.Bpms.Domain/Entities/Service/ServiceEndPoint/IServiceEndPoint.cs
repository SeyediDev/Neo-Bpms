namespace Neo.Bpms.Domain.Entities.Service.ServiceEndPoint;

public interface IServiceEndPoint
{
    ServiceEndPointDefinition Definition { get; set; }
    string GetUrl();
}
