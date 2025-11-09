using Neo.Bpms.Domain.Models.Base;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

internal class DeleteCascadeRuntime
{
    internal DeleteCascadeRuntime(long parentId, long stateId, 
        Entity entity, AuditTrail auditTrail)
    {
        ParentId = parentId;
        StateId = stateId;
        Entity = entity;
        AuditTrail = auditTrail;
    }
    internal long ParentId { get; set; }
    internal long StateId { get; set; }
    internal Entity Entity { get; set; }
    internal AuditTrail AuditTrail { get; set; }
    internal ExceptionInfos Errors { get; set; }

    internal async Task CheckAndDeleteCascade(IApplyFormData applyFormData, FormStructRoutines formStructRoutines, CancellationToken cancellationToken = default)
    {
        if (Entity.IsStateBase)
        {
            EntityState state = Entity.GetState(StateId.ToString());
            if (state != null && (state.category & (uint)EntityStateCategory.BackupNode) ==
                (uint)EntityStateCategory.BackupNode)
            {
                await DeleteCascade(applyFormData, formStructRoutines, cancellationToken );
            }
        }
    }

    internal async Task DeleteCascade(IApplyFormData applyFormData, FormStructRoutines formStructRoutines, CancellationToken cancellationToken = default)
    {
        foreach (Entity referEntity in ProjectDefinition.Entities.Values)
        {
            foreach (WeakEntityAssociation association in referEntity.Associations.OfType<WeakEntityAssociation>()
                .Where(a => Equals(a.Entity(), Entity)))
            {
                await DeleteCascadeAssociation(applyFormData, formStructRoutines, referEntity, association, cancellationToken);
            }
        }
    }

    private Task DeleteCascadeAssociation(IApplyFormData applyFormData,
        FormStructRoutines formStructRoutines, Entity referEntity, Association association, 
        CancellationToken cancellationToken = default)
    {
        Form referDeleteForm =
            FormStructRoutines.GetForm(referEntity.NamespaceId, referEntity.Id, null, Form.eFormType.Delete, null);
        QueryUtility q = new QueryUtility(referEntity, "DeleteCascadeAssociationQuery")
            .Where($"{association.Id}=='{ParentId}'")
            .ActiveStates();
        ExceptionInfos errors = [];
        q.GetDocuments();
        var records = q.GetRecords();
        return Task.WhenAll(records.Select(referRecord =>
        {
            return applyFormData.Delete(AuditTrail, referEntity.NamespaceId, referEntity.Id,
            referDeleteForm, referRecord, errors, cancellationToken);
            
        }));
    }
}
