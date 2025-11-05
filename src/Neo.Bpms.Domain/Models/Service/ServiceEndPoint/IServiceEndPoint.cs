namespace Neo.Bpms.Domain.Models.Service.ServiceEndPoint;

public interface IServiceEndPoint
{
    ServiceEndPointDefinition Definition { get; set; }
    string GetUrl();
}
