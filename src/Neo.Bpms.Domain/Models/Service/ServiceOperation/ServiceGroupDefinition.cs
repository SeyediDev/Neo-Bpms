namespace Neo.Bpms.Domain.Models.Service.ServiceOperation;

/// <summary>
/// An Interface defines a set of operations that are implemented by Services
/// </summary>
public class ServiceGroupDefinition(string id, string name) : BaseModelClass(null, id, name)
{
    /// <summary>
    /// operations defined as part of the Interface. An Interface has at least one Operation.
    /// </summary>
    public Dictionary<string, ServiceOperationDefinition> Operations =
        [];
}