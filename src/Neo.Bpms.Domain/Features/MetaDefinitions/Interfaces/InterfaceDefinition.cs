namespace Neo.Bpms.Domain.Features.MetaDefinitions.Interfaces;

/// <summary>
/// This class is the base class for all service interface definitions.
/// Normally we have only one service interface definition for a project.
/// The inherited class wil be called in the project definition.
/// </summary>
public abstract class ServiceDefinition : BaseModelingDefinition
{
    protected readonly ServiceInterfaceDefinition ProjectInterfaceDefinition;
    private ServiceGroupDefinition _currentServiceGroup;
    private ServiceOperationDefinition _currentServiceOperation;

    /// <summary>
    /// Constructor
    /// </summary>
    protected ServiceDefinition()
    {
        ProjectInterfaceDefinition = ProjectDefinition.Project.ServiceInterfaces;
    }

    /// <summary>
    /// All the service and interface definitions(which uses the following design tool methods)
    /// must be called inside this method (or sub calls).
    /// </summary>
    public abstract void DefineAll();

    /// <summary>
    /// Service group is a group of service operations which are provided in a single interface.
    /// We define both providing and using service operations and interfaces( as service groups).
    /// </summary>
    /// <param name="name">unique name of the interface in the scope of project</param>
    /// <returns></returns>
    public ServiceGroupDefinition AddServiceGroup(string name)
    {
        _currentServiceGroup = new ServiceGroupDefinition(name, name);
        return _currentServiceGroup;
    }

    private IEnumerable<Type> GetTypesWithInterface<T>(Assembly asm)
    {
        var it = typeof(T);
        return asm.GetLoadableTypes().Where(it.IsAssignableFrom).ToList();
    }

    private IEnumerable<Type> GetTypesWithInterface2<T>(Assembly asm)
    {
        return asm.GetLoadableTypes().Where(ReflectionTools.IsInBaseInterface<T>);
    }

    /// <summary>
    /// Internal service operations and groups can be added based on its internal definitions
    /// </summary>
    /// <param name="serviceGroup"></param>
    /// <returns></returns>
    public ServiceGroupDefinition AddInternalServiceOperationGroup(InternalServiceGroupDefinition serviceGroup)
    {
        _currentServiceGroup = serviceGroup;
        var inheritedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetTypesWithInterface<IInternalServiceOperationRuntime>)
            //typeof(IInternalServiceOperationRuntime).GetTypeInfo().ImplementedInterfaces
            .Select(t =>
            {
                //var names = t.Name?.Split('.');
                //var name = namespaceIds?[namespaceIds.Length - 1] ?? "";
                var name = t.Name;

                if (!name.EndsWith("Runtime")) return null;
                name = t.Name.Replace("Runtime", "");

                return new
                {
                    type = t,
                    Name = name
                };
            }).ToList();
        serviceGroup.AddOperations();
        foreach (var item in serviceGroup.Operations.Values)
        {
            if (item is not IInternalServiceOperation serviceOperation) continue;
            if (serviceOperation.RunTimeType != null) continue;
            item.InParamsEntity = FetchProcessEntity(serviceOperation.InputStructure);
            item.OutParamsEntity = FetchProcessEntity(serviceOperation.OutputStructure);

            var type = inheritedTypes.FirstOrDefault(t => t?.Name == item.GetType().Name)?.type;
            if (type == null || !type.IsAssignableTo(typeof(IInternalServiceOperationRuntime)))
            {
                continue;
            }

            serviceOperation.RunTimeType = type;
        }

        AddServiceGroupAsProvidingInterface(ServiceInterfaceProtocol.InternalLibrary);
        AddServiceGroupAsUsingInterface("bpms://", ServiceInterfaceProtocol.InternalLibrary);
        return _currentServiceGroup;
    }

    private static Entity FetchProcessEntity(Type structure)
    {
        return ProjectDefinition.Project.GetEntityFromType(structure) ??
               ProcessEntityDefinition.DefineProcessEntity(structure);
    }

    /// <summary>
    /// A service operation is an atomic part of an interface.
    /// The service operation can be called in a single call.
    /// It may be one way, both synchronous or asynchronous.
    /// We define the service operations which are provided by the BPMS and also the
    /// ones that will be used(consumed by) the BPMS.
    /// </summary>
    /// <param name="id">unique id of the operation in the scope of project</param>
    /// <param name="name">unique name of the operation in the scope of interface(service group)</param>
    /// <param name="inParamsStruct">The input parameters of the operation needs to be defined in an entity which is defined in the namespace with the ending part of : '.ProcessEntities'</param>
    /// <param name="outParamsStruct">The output parameters of the operation needs to be defined in an entity which is defined in the namespace with the ending part of : '.ProcessEntities'</param>
    public void AddServiceOperation(string id, string name, Entity inParamsStruct, Entity outParamsStruct)
    {
        if (_currentServiceGroup == null)
            throw new Exception("No Service Group defined");
        _currentServiceOperation =
            new ServiceOperationDefinition(_currentServiceGroup, id, name, inParamsStruct, outParamsStruct);
        _currentServiceGroup.Operations.Add(id, _currentServiceOperation);
    }

    /// <summary>
    /// This method define Typed service operations and the entities relating input and output parameters.
    /// </summary>
    /// <typeparam name="TIn">The corresponding class defining the structure of input parameters. The Process Entity will be define based on this class.</typeparam>
    /// <typeparam name="TOut">The corresponding class defining the structure of output parameters. The Process Entity will be define based on this class.</typeparam>
    /// <param name="name"></param>
    public void AddServiceOperation<TIn, TOut>(string name)
    {
        if (_currentServiceGroup == null)
            throw new Exception("No Service Group defined");
        var inEntity = ProcessEntityDefinition.GetOrDefineEntity<TIn>();
        var outEntity = ProcessEntityDefinition.GetOrDefineEntity<TOut>();
        _currentServiceOperation =
            new ServiceOperationDefinition(_currentServiceGroup, name, name, inEntity, outEntity);
        _currentServiceGroup.Operations.Add(name, _currentServiceOperation);
    }

    ///// <summary>
    ///// Add internal service operations and the related entities for input and output structure
    ///// </summary>
    ///// <param name="operation"></param>
    //public void AddServiceOperation(InternalServiceOperationModelBase operation)
    //{
    //	dynamic op1 = operation;
    //	Type InputStructure = op1.InputStructure;
    //	Type OutputStructure = op1.OutputStructure;
    //	if( _currentServiceGroup == null )
    //		throw new Exception("No Service Group defined");
    //	var inEntity = ProcessDefinition.DefineProcessEntity(InputStructure);
    //	var outEntity = ProcessDefinition.DefineProcessEntity(OutputStructure);
    //	_currentServiceOperation = operation;
    //	operation.InParamsEntity = inEntity;
    //	operation.OutParamsEntity = outEntity;
    //	operation.Parent = _currentServiceGroup;
    //	_currentServiceGroup.Operations.Add(operation.name, _currentServiceOperation);
    //}
    /// <summary>
    /// Each operation may cause multiple errors. This errors can be documented by this method.
    /// </summary>
    /// <param name="ex"></param>
    public void AddPossibleError(ExceptionInformation ex)
    {
        if (_currentServiceOperation == null)
            throw new Exception("No Service defined");
        _currentServiceOperation.PossibleErrors.Add(ex);
    }

    /// <summary>
    /// Defines a new interface with service operations defined in a service group to be used
    /// as service operations in process service tasks, send and receive task and also catch
    /// and throw messages with the protocol specified.
    /// We assume that the other system or party supports these service operations
    /// with the specified uri and protocol.
    /// </summary>
    /// <param name="uri">the uri of the endpoint of the interface</param>
    /// <param name="protocol">The protocol. This protocol can be selected only from the supported protocols</param>
    public void AddServiceGroupAsUsingInterface(string uri, ServiceInterfaceProtocol protocol)
    {
        if (_currentServiceGroup == null)
            throw new Exception("No Service Group defined");
        var endpoint = new ServiceEndPointDefinition(_currentServiceGroup.Id, _currentServiceGroup.Name, uri);
        ProjectInterfaceDefinition.AddUsingInterface(_currentServiceGroup.Id, _currentServiceGroup.Name, endpoint,
            _currentServiceGroup, protocol);
    }

    /// <summary>
    /// Defines a new interface with service operations defined in a service group to provide
    /// these service operations with the protocol specified to the other systems and parties.
    /// </summary>
    /// <param name="protocol">The protocol. This protocol can be selected only from the supported protocols</param>
    public void AddServiceGroupAsProvidingInterface(ServiceInterfaceProtocol protocol)
    {
        if (_currentServiceGroup == null)
            throw new Exception("No Service Group defined");
        ProjectInterfaceDefinition.AddProvidingInterface(_currentServiceGroup.Id, _currentServiceGroup.Name,
            _currentServiceGroup, protocol);
    }

    public ServiceGroupDefinition AddExternalServiceOperationGroup(ExternalServiceGroupDefinition serviceGroup,
        ServiceInterfaceProtocol protocol)
    {
        if (protocol == ServiceInterfaceProtocol.InternalLibrary)
            throw new Exception("Internal library protocol is invalid.");
        _currentServiceGroup = serviceGroup;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var interfaceTypes = assemblies.SelectMany(GetTypesWithInterface2<IExternalServiceOperationRuntime>)
            .ToList();
        var inheritedTypes = interfaceTypes
            .Select(t =>
            {
                var name = t.Name;

                if (!name.EndsWith("Runtime")) return null;
                name = t.Name.Replace("Runtime", "");

                return new
                {
                    type = t,
                    Name = name
                };
            }).Where(t => t != null).ToList();
        serviceGroup.AddOperations();
        foreach (var item in serviceGroup.Operations.Values)
        {
            if (item is not IExternalServiceOperation serviceOperation) continue;
            if (serviceOperation.RunTime != null) continue;
            var itemName = item.GetType().Name;
            item.InParamsEntity = FetchProcessEntity(serviceOperation.InputStructure);
            item.OutParamsEntity = FetchProcessEntity(serviceOperation.OutputStructure);
            var type = inheritedTypes.FirstOrDefault(t => t?.Name == itemName)?.type;
            if (type == null) continue;
            var ctr = type.GetConstructor([]);
            var runtime = ctr?.Invoke([]);
            if (runtime == null) continue;
            serviceOperation.RunTime = runtime as IExternalServiceOperationRuntime;
            //type.GetConstructor()
        }

        AddServiceGroupAsProvidingInterface(protocol);
        AddServiceGroupAsUsingInterface("bpms://", protocol);
        return _currentServiceGroup;
    }
}
