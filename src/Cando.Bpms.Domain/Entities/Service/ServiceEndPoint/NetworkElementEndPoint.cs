namespace Neo.Bpms.Domain.Entities.Service.ServiceEndPoint;

public class NetworkElementEndPoint(string id, string name, string uriString) : IServiceEndPoint
{
    public ServiceEndPointDefinition Definition { get; set; } = new ServiceEndPointDefinition
            (id, name, uriString);
    public string GetUrl()
    {
        return Definition.UriAddress.ToString();
    }
}
