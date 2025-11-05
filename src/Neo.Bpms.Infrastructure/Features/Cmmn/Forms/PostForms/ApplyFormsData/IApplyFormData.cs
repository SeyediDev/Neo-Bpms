using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Cmmn;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

public interface IApplyFormData
{
    Task<bool> CreateRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, IList<string> validFieldIds, ExceptionInfos errors,
        ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken=default);
    Task<bool> UpdateRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, ElasticObject keyValues, IList<string> validFieldIds,
        ExceptionInfos errors, bool forVirtualDelete = false,
        ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteRecord(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record,
        ExceptionInfos errors, ApplyUtility apply = null, LocalParameters localParameters = null, CancellationToken cancellationToken = default);
    Task<bool> Delete(AuditTrail auditTrail, string namespaceId, string entityId,
        Form form, ElasticObject record, ExceptionInfos errors, CancellationToken cancellationToken = default);
    Task<bool> UpdateTables(AuditTrail auditTrail, string namespaceId, string entityId, string formSubjectId,
        ElasticObject record, string ids, string culture,
        List<TableDefinition> tables, ExceptionInfos errors, CancellationToken cancellationToken);
    void ConvertPostedElasticToRecord(ref ElasticObject postedRecord, CommonFormStructure structure);
}
