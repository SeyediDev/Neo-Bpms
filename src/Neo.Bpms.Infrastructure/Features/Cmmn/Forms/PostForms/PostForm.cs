using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.UI.MVC.ControllersMethods.PostForms;
public interface IPostForm
{
    Task PostCreateForm(PostFormData postFormData, long? wid, string taskId, string processId,
        IdentityUser user, eCreateType createType, long userGroupId, CancellationToken cancellationToken=default);
    Task<bool> PostDeleteForm(PostFormData postFormData,
        long? wid/*todo*/, IdentityUser user, string ids, long userGroupId, CancellationToken cancellationToken = default);
    Task PostEditForm(PostFormData postFormData,
        string entityPkv, long? wid, string taskId, string processId,
        bool isApply, IdentityUser user, long userGroupId, CancellationToken cancellationToken = default);
}
public partial class PostForm(IApplyFormData applyFormData, IBpmsEngine bpmsEngine1,
    FormServiceOperation formServiceOperation,
    FormStructRoutines formStructRoutines,
    ILogger<PostForm> logger)
    : IPostForm
{
    private BpmsEngine BpmsEngine => (BpmsEngine)bpmsEngine1;
    public static string GetEntityPkvFromActivityInstance(long? wid)
    {
        QueryUtility q = QueryUtility<ActivityInstanceRecord>
            .Where($"Id='{wid}'");
        q.Include("ProcessInstance")
            .SelectField("EntityPKV");
        string ids = q.FirstOrDefault()?.GetString("EntityPKV");
        return ids;
    }
    
    protected Task<ExceptionInfos> CallApplyServiceOperation(PostFormData postFormData, IdentityUser user)
    {
        if (postFormData.structure.Tables.Count != 0)
        {
            foreach (TableDefinition table in postFormData.structure.Tables)
            {
                if (table.ControlType == eControlTypeId.MultipleSelectableCombo) { }
                else if (table.ControlType == eControlTypeId.IndexTable)
                {
                    List<ElasticObject> tableRecords = ApplyFormData.FindTableRecords(postFormData.FormData, table).Values.ToList();
                    postFormData.FormData.SetField(table.FieldName, tableRecords);
                }
            }
        }
        ServiceOperationFormData result = new(postFormData.FormData);
        AuditTrail auditTrail = new(TriggerTypeId.Business,
            $"{postFormData.form.ApplyServiceOperation} Form {postFormData.form.Id} in entity {postFormData.form.entity.Id}", user, 0)
        {
            MetaEntityId = postFormData.form.entity.DbId,
            MetaFormId = postFormData.form.DbId,
            FormId = postFormData.form.Id
        };
        return formServiceOperation.CallApplyServiceOperation(result, postFormData.form, auditTrail);
    }
}
