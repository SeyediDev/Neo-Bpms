using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.Entities;

public static class EntityExtension
{
    public static bool Insert<T>(this T entityRecord, AuditTrail auditTrail = null)
        where T : BpmsBaseEntity, new()
    {
        return ApplyUtility<T>.Insert(entityRecord, false, auditTrail?.User);
    }

    public static bool Update<T>(this T entityRecord, AuditTrail auditTrail = null)
        where T : BpmsBaseEntity, new()
    {
        return ApplyUtility<T>.Update(entityRecord, auditTrail?.User);
    }

    public static bool Delete<T>(this T entityRecord, AuditTrail auditTrail = null)
        where T : BpmsBaseEntity, new()
    {
        return ApplyUtility<T>.Delete(entityRecord, auditTrail);
    }

    public static T Load<T>(this T @this, long id)
        where T : BpmsBaseEntity, new()
    {
        return QueryUtility<T>.Load(id);
    }

    public static List<T> List<T>(this T @this, string filter = null, LocalParameters filterValues = null)
        where T : BpmsBaseEntity, new()
    {
        return QueryUtility<T>.ToList<T>(filter, filterValues);
    }
}
