namespace Neo.Bpms.Domain.Entities.Service.ServiceOperation;

public enum ServiceInterfaceProtocol
{
    DirectSOAP,
    DirectRest,
    InternalLibrary,
    ExternalDotnetLibrary,
    MicroServiceManager,
    Kafka,
    NetworkElementRest
    //todo: other interface protocols definition, and then implementation
}