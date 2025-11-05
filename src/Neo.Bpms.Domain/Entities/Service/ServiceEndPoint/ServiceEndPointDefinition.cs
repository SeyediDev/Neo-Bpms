namespace Neo.Bpms.Domain.Entities.Service.ServiceEndPoint;

public class ServiceEndPointDefinition(string id, string name, string uriString) : BaseModelClass(null, id, name)
{
    public Uri UriAddress { get; set; } = new Uri(uriString);
}
