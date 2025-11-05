namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition : IProcessDefinition
{
    private Entity _processEntity;

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <typeparam name="TIn">TIn is an Entity that define process input information</typeparam>
    /// <typeparam name="TOut">TOut is an Entity that define process output information</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T, TIn, TOut>(string id, string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>() ?? throw new Exception($"Can not find entity {typeof(T).Name}");
        Entity inputStruct = ProjectDefinition.Project.GetEntity<TIn>() ?? throw new Exception($"Can not find entity {typeof(TIn).Name}");
        Entity outputStruct = ProjectDefinition.Project.GetEntity<TOut>();
        return outputStruct == null
            ? throw new Exception($"Can not find entity {typeof(TOut).Name}")
            : DefineProcess(id, name, entity, inputStruct, outputStruct);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <typeparam name="TIn">TIn is an Entity that define process input information</typeparam>
    /// <typeparam name="TOut">TOut is an Entity that define process output information</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T, TIn, TOut>(string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>() ?? throw new Exception($"Can not find entity {typeof(T).Name}");
        Entity inputStruct = ProjectDefinition.Project.GetEntity<TIn>() ?? throw new Exception($"Can not find entity {typeof(TIn).Name}");
        Entity outputStruct = ProjectDefinition.Project.GetEntity<TOut>();
        return outputStruct == null
            ? throw new Exception($"Can not find entity {typeof(TOut).Name}")
            : DefineProcess(GetType().Name, name, entity, inputStruct, outputStruct);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <typeparam name="TIn">TIn is an Entity that define process input information</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T, TIn>(string id, string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>() ?? throw new Exception($"Can not find entity {typeof(T).Name}");
        Entity inputStruct = ProjectDefinition.Project.GetEntity<TIn>();
        return inputStruct == null
            ? throw new Exception($"Can not find entity {typeof(TIn).Name}")
            : DefineProcess(id, name, entity, inputStruct, null);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <typeparam name="TIn">TIn is an Entity that define process input information</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T, TIn>(string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>() ?? throw new Exception($"Can not find entity {typeof(T).Name}");
        Entity inputStruct = ProjectDefinition.Project.GetEntity<TIn>();
        return inputStruct == null
            ? throw new Exception($"Can not find entity {typeof(TIn).Name}")
            : DefineProcess(GetType().Name, name, entity, inputStruct, null);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T>(string id, string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>();
        return entity == null ? throw new Exception($"Can not find entity {typeof(T).Name}") : DefineProcess(id, name, entity, null, null);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <typeparam name="T">T is an Entity that process read and write on it</typeparam>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess<T>(string name)
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>();
        return entity == null
            ? throw new Exception($"Can not find entity {typeof(T).Name}")
            : DefineProcess(GetType().Name, name, entity, null, null);
    }

    /// <summary>
    /// Defines the process.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="namespaceId">The namespace identifier.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    protected BpmnDefinitions DefineProcess(string id, string name, string namespaceId, string entityId)
    {
        Entity entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        return entity == null ? throw new Exception($"Can not find entity {entityId}") : DefineProcess(id, name, entity, null, null);
    }

    public static BpmnDefinitions CreateBpmnDefinitionsAndProcess(string processId, string name,
        Entity entity, out Process process)
    {
        BpmnDefinitions definitions = CreateBpmnDefinitions(processId, name);
        process = new Process(definitions, processId, name, entity);
        definitions.AddRootElement(process);

        Collaboration collaboration = new(definitions, "Collaboration." + processId, name)
        {
            participants = []
        };
        process.definitionalCollaborationRef = collaboration;

        collaboration.participants.Add(
            new Participant(collaboration, "Participant." + collaboration.Id, name)
            {
                processRef = process
            });
        definitions.AddRootElement(collaboration);
        return definitions;
    }

    private BpmnDefinitions DefineProcess(string processIdVersionId, string name,
        Entity entity, Entity inputStruct, Entity outputStruct)
    {
        if (entity == null || processIdVersionId == null)
        {
            return null;
        }

        BusinessProcessVersion.FetchVersionId(processIdVersionId, out string id, out _);

        definitions = CreateBpmnDefinitionsAndProcess(id, name, entity, out Process process);
        currentBaseElement = definitions;
        _flowElementsContainer = process;
        _processEntity = entity;
        process.isExecutable = true;
        process.Status = Process.ProcessStatus.Active;

        if (inputStruct == null && outputStruct == null)
        {
            return definitions;
        }

        InputOutputSpecification ioSpec = new(process, $"{id}_ioSpec");
        ioSpec.Set(inputStruct, outputStruct);
        process.ioSpecification = ioSpec;
        return definitions;
    }
}
