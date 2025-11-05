namespace Neo.Bpms.Domain.Models.Service.ServiceOperation;

/// <summary>
/// An Operation defines Messages that are consumed and, optionally, produced when the Operation is called. 
/// It can also define zero or more errors that are returned when operation fails
/// </summary>
public class ServiceOperationDefinition(ServiceGroupDefinition @interface, string id, string name,
    IModelEntity inParamsStruct, IModelEntity outParamsStruct, ServiceOperationMethod method = ServiceOperationMethod.POST) : BaseModelClass(@interface, id, name)
{
    public IModelEntity InParamsEntity = inParamsStruct;

    public IModelEntity OutParamsEntity = outParamsStruct;

    /// <summary>
    /// This attribute specifies errors that the Operation may return. 
    /// </summary>
    public List<ExceptionInformation> PossibleErrors;

    public ServiceGroupDefinition Interface => Parent as ServiceGroupDefinition;
    public virtual string RelativePath { get; }
    public ServiceOperationMethod Method { get; } = method;
}
