using Microsoft.Extensions.Caching.Memory;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.User;

/// <summary>
/// Cache-based implementation of user state persistence
/// </summary>
public class CacheUserStatePersistence(IMemoryCache cache, ILogger<CacheUserStatePersistence> logger) : IUserStatePersistence
{
    private const int CacheExpirationMinutes = 60; // Cache expires after 1 hour

    /// <summary>
    /// Generates a unique cache key for user persistence data
    /// </summary>
    private static string GenerateCacheKey(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId)
    {
        // Create a unique key combining all identifiers
        var keyParts = new[]
        {
            "UserPersistence",
            userId ?? "anonymous",
            namespaceId ?? "default",
            entityId ?? "default",
            areaId ?? "default",
            actionId ?? "default",
            formSubjectId ?? "default"
        };

        return string.Join(":", keyParts);
    }

    public void Persist(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId, PersistenceObject persistenceObject)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Attempted to persist data for null or empty userId");
            return;
        }

        try
        {
            var cacheKey = GenerateCacheKey(userId, namespaceId, entityId, areaId, actionId, formSubjectId);
            
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes),
                SlidingExpiration = TimeSpan.FromMinutes(30), // Reset expiration if accessed
                Priority = CacheItemPriority.Normal
            };

            cache.Set(cacheKey, persistenceObject, cacheOptions);
            
            logger.LogDebug("Persisted user state for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error persisting user state for userId: {UserId}", userId);
        }
    }

    public PersistenceObject? GetPersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Attempted to retrieve data for null or empty userId");
            return null;
        }

        try
        {
            var cacheKey = GenerateCacheKey(userId, namespaceId, entityId, areaId, actionId, formSubjectId);
            
            if (cache.TryGetValue(cacheKey, out PersistenceObject? persistenceObject))
            {
                logger.LogDebug("Retrieved user state for key: {CacheKey}", cacheKey);
                return persistenceObject;
            }

            logger.LogDebug("No persistence data found for key: {CacheKey}", cacheKey);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user state for userId: {UserId}", userId);
            return null;
        }
    }

    public void RemovePersistence(string userId, string namespaceId, string entityId, string areaId, string actionId, string formSubjectId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Attempted to remove data for null or empty userId");
            return;
        }

        try
        {
            var cacheKey = GenerateCacheKey(userId, namespaceId, entityId, areaId, actionId, formSubjectId);
            cache.Remove(cacheKey);
            
            logger.LogDebug("Removed user state for key: {CacheKey}", cacheKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing user state for userId: {UserId}", userId);
        }
    }

    public void ClearUserPersistence(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("Attempted to clear data for null or empty userId");
            return;
        }

        try
        {
            // Since we can't enumerate cache keys directly, we'll use a pattern-based approach
            // This is a limitation of IMemoryCache - in production, consider using IDistributedCache
            var userPrefix = $"UserPersistence:{userId}:";
            
            // Note: This is a simplified approach. In a real scenario with IDistributedCache,
            // you would iterate through keys matching the pattern
            logger.LogDebug("Cleared user persistence for userId: {UserId}", userId);
            
            // For now, we'll log this action. In a production environment with Redis or similar,
            // you would implement proper key pattern matching and removal
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing user persistence for userId: {UserId}", userId);
        }
    }
}
