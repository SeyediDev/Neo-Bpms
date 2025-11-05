namespace Neo.Bpms.Domain.Models.Service.ServiceEndPoint;

public class ServiceEndPointDefinition(string id, string name, string uriString) : BaseModelClass(null, id, name)
{
    public Uri UriAddress { get; set; } = new Uri(uriString);
}
