namespace Neo.Bpms.Domain.Models.Service.ServiceOperation;

public class ServiceInterfaceDefinition
{
    public ServiceInterfaceDefinition()
    {
        ProvidingInterfaces = [];
        UsingInterfaces = [];
    }

    public Dictionary<string, ServiceInterfaceDefinitionItem>
        ProvidingInterfaces
    { get; }

    public Dictionary<string, ServiceInterfaceDefinitionItem>
        UsingInterfaces
    { get; }

    public void AddProvidingInterface(
        string id, string _name,
        ServiceGroupDefinition _interface,
        ServiceInterfaceProtocol protocol
    )
    {
        ServiceInterfaceDefinitionItem item = new(id, _name, null, _interface, protocol);
        ProvidingInterfaces.Add(id, item);
    }
    public void AddUsingInterface(
        string id, string _name,
        ServiceEndPointDefinition endPoint,
        ServiceGroupDefinition _interface,
        ServiceInterfaceProtocol protocol
    )
    {
        ServiceInterfaceDefinitionItem item = new(id, _name, endPoint, _interface, protocol);
        UsingInterfaces.Add(id, item);
    }

    public ServiceOperationDefinition GetUsingOperation(string uri,
        ServiceInterfaceProtocol protocol, string _name)
    {
        //var _uri = new Uri(uri);
        return UsingInterfaces.Values.FirstOrDefault(ifc =>
            //ifc.EndPoint.UriAddress == _uri &&
            ifc.Protocol == protocol &&
            ifc.Interface.Operations.ContainsKey(_name))?.Interface.Operations[_name];
    }
    public ServiceOperationDefinition GetProvidingOperation(
        ServiceInterfaceProtocol protocol, string _name)
    {
        return ProvidingInterfaces.Values.FirstOrDefault(ifc =>
            ifc.Protocol == protocol &&
            ifc.Interface.Operations.ContainsKey(_name))?.Interface.Operations[_name];
    }
}
