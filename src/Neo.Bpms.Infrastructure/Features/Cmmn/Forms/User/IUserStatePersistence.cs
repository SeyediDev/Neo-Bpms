namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.User;

/// <summary>
/// Interface for managing user state persistence using cache
/// </summary>
public interface IUserStatePersistence
{
    /// <summary>
    /// Persists user state data for a specific form/report/dashboard
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="namespaceId">Namespace identifier</param>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="areaId">Area identifier (Form, Report, Dashboard)</param>
    /// <param name="actionId">Action identifier</param>
    /// <param name="formSubjectId">Form subject identifier</param>
    /// <param name="persistenceObject">Object containing persistence data</param>
    void Persist(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId, PersistenceObject persistenceObject);

    /// <summary>
    /// Retrieves user state data for a specific form/report/dashboard
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="namespaceId">Namespace identifier</param>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="areaId">Area identifier (Form, Report, Dashboard)</param>
    /// <param name="actionId">Action identifier</param>
    /// <param name="formSubjectId">Form subject identifier</param>
    /// <returns>Persistence object or null if not found</returns>
    PersistenceObject? GetPersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId);

    /// <summary>
    /// Removes user state data for a specific form/report/dashboard
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="namespaceId">Namespace identifier</param>
    /// <param name="entityId">Entity identifier</param>
    /// <param name="areaId">Area identifier (Form, Report, Dashboard)</param>
    /// <param name="actionId">Action identifier</param>
    /// <param name="formSubjectId">Form subject identifier</param>
    void RemovePersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId);

    /// <summary>
    /// Clears all persistence data for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    void ClearUserPersistence(string userId);
}
