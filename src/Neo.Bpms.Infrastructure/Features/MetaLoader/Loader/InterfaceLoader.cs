using Neo.Bpms.Domain.Features.Definitions.Entities.Processes;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Service.ServiceOperation;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

internal static class InterfaceLoader
{
    //private static MicroServiceManager MicroServiceManager { get; set; }
    private static InternalLibrariesServiceOperationInterface InternalLibrariesInterface { get; set; }
    internal static object GetInterfaceImplementation(ServiceInterfaceProtocol protocol)
    {
        InterfaceRuntime interfaceImplementation = null;
        switch (protocol)
        {
            case ServiceInterfaceProtocol.DirectSOAP:
                break;
            case ServiceInterfaceProtocol.DirectRest:
                break;
            case ServiceInterfaceProtocol.InternalLibrary:
                InternalLibrariesInterface ??= new InternalLibrariesServiceOperationInterface();
                interfaceImplementation = InternalLibrariesInterface;
                break;
            case ServiceInterfaceProtocol.ExternalDotnetLibrary:
                break;
            /*case ServiceInterfaceProtocol.MicroServiceManager:
                MicroServiceManager ??= new MicroServiceManager();
                interfaceImplementation = MicroServiceManager;
                break;*/
            case ServiceInterfaceProtocol.Kafka:
                break;
            case ServiceInterfaceProtocol.NetworkElementRest:
                interfaceImplementation = new ExternalLibrariesServiceOperationInterface();
                break;
        }
        return interfaceImplementation;
    }

    internal static void DefineInterfacesToBPMN(ProjectContext project)
    {
        foreach (var item in project.ServiceInterfaces.UsingInterfaces
            .Values.Where(i => i.Protocol.In(
                ServiceInterfaceProtocol.InternalLibrary,
                ServiceInterfaceProtocol.MicroServiceManager)))
        {
            object interfaceImplementation = GetInterfaceImplementation(item.Protocol);
            if (interfaceImplementation == null)
            {
                throw new Exception($"Interface library not implemented for {item.Interface.Name}");
            }

            var ifc = DefineInterface(item.Interface, interfaceImplementation);
            foreach (var operation in item.Interface.Operations.Values)
            {
                BpmnDefinitionsDefinition.AddOperation(ifc, operation);
            }
        }
    }

    /// <summary>
    /// Defines Interface based on ServiceGroupDefinition
    /// </summary>
    /// <param name="serviceGroup"></param>
    /// <param name="implementation"></param>
    /// <returns></returns>
    private static Interface DefineInterface(ServiceGroupDefinition serviceGroup, object implementation)
    {
        var currentInterface = new Interface(ProjectDefinition.Project.BpmnDefinitions, serviceGroup.Id, serviceGroup.Name, implementation);
        ProjectDefinition.Project.AddInterface(currentInterface);
        return currentInterface;
    }
}
