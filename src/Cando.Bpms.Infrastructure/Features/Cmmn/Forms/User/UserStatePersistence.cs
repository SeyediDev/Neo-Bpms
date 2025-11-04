using Microsoft.AspNetCore.Http;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.User;

/// <summary>
/// Persistence object containing user state data
/// </summary>
public class PersistenceObject
{
    public int Page { get; set; }
    public string SortFields { get; set; } = string.Empty;
    public ElasticObject FilterValues { get; set; } = new();
}

/// <summary>
/// Legacy wrapper for backward compatibility
/// This class maintains the old static interface while delegating to the new service
/// </summary>
public static class UserStatePersistence
{
    private static IUserStatePersistence? _service;
    
    /// <summary>
    /// Gets the persistence service instance
    /// </summary>
    private static IUserStatePersistence GetService()
    {
        if (_service == null)
        {
            // Try to get from current HTTP context if available
            var httpContextAccessor = ServiceProviderAccessor.ServiceProvider?.GetService<IHttpContextAccessor>();
            if (httpContextAccessor?.HttpContext != null)
            {
                _service = httpContextAccessor.HttpContext.RequestServices.GetService<IUserStatePersistence>();
            }
            
            // Fallback to static service provider if available
            if (_service == null)
            {
                _service = ServiceProviderAccessor.ServiceProvider?.GetService<IUserStatePersistence>();
            }
            
            // Last resort: create a default implementation
            if (_service == null)
            {
                throw new InvalidOperationException("IUserStatePersistence service is not registered. Please ensure it's registered in DI container.");
            }
        }
        
        return _service;
    }

    /// <summary>
    /// Persists user state data
    /// </summary>
    public static void Persist(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId, PersistenceObject persistenceObject)
    {
        GetService().Persist(userId, namespaceId, entityId, areaId, actionId, formSubjectId, persistenceObject);
    }

    /// <summary>
    /// Retrieves user state data
    /// </summary>
    public static PersistenceObject? GetPersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId)
    {
        return GetService().GetPersistence(userId, namespaceId, entityId, areaId, actionId, formSubjectId);
    }

    /// <summary>
    /// Removes user state data
    /// </summary>
    public static void RemovePersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId)
    {
        GetService().RemovePersistence(userId, namespaceId, entityId, areaId, actionId, formSubjectId);
    }

    /// <summary>
    /// Clears all persistence data for a user
    /// </summary>
    public static void ClearUserPersistence(string userId)
    {
        GetService().ClearUserPersistence(userId);
    }
}

/// <summary>
/// Helper class to access service provider in static context
/// </summary>
public static class ServiceProviderAccessor
{
    public static IServiceProvider? ServiceProvider { get; set; }
}
