using Neo.Bpms.Domain.Models.Cmmn.Data.Transaction;
using Neo.Bpms.Domain.Models.Security.Authentication;

namespace Neo.Bpms.Domain.Models.Base.Audit;
public class AuditTrail : IAuditTrail
{
    public string Id { get; set; }
    public DateTime DateTime { get; set; }
    public IdentityUser User { get; set; }
    public long UserGroupId { get; set; }
    public TriggerTypeId TriggerTypeId { get; set; }
    public string Title { get; set; }
    public long? ProcessInstanceId { get; set; }
    public long? FlowNodeInstanceId { get; set; }
    public string TraceCode { get; set; }
    public long? MetaEntityId { get; set; } //todo fill
    public long? MetaFormId { get; set; } //todo fill
    public string FormId { get; set; }
    public string EntityPkv { get; set; } //todo fill
    public bool Log { get; set; }

    public List<AuditDetail> Details { get; set; }
    public DataTransaction DataTransaction { get; set; }
    public BatchDataManipulation Batch { get; set; }

    public AuditTrail(IDataTransactionRunner dataTransactionRunner,
        IBatchDataManipulationRunner batchDataManipulationRunner,
        TriggerTypeId triggerTypeId, string title, IdentityUser user, long userGroupId)
    {
        Id = Guid.NewGuid().ToString();
        if (batchDataManipulationRunner != null)
        {
            SetBatch(batchDataManipulationRunner);
        }

        if (dataTransactionRunner != null)
        {
            DataTransaction = new DataTransaction(dataTransactionRunner);
        }

        DateTime = DateTime.UtcNow;
        TriggerTypeId = triggerTypeId;
        User = user;
        UserGroupId = userGroupId;
        Title = title;
    }

    public AuditTrail(IBatchDataManipulationRunner batchDataManipulationRunner,
        TriggerTypeId triggerTypeId, string title, IdentityUser user, long userGroupId)
        : this(null, batchDataManipulationRunner, triggerTypeId, title, user, userGroupId)
    {
    }

    public AuditTrail(IDataTransactionRunner dataTransactionRunner, TriggerTypeId triggerTypeId, string title,
        IdentityUser user, long userGroupId)
        : this(dataTransactionRunner, null, triggerTypeId, title, user, userGroupId)
    {
    }

    public AuditTrail(TriggerTypeId triggerTypeId, string title, IdentityUser user, long userGroupId) //todo
        : this(null, null, triggerTypeId, title, user, userGroupId)
    {
    }

    public AuditTrail(TriggerTypeId triggerTypeId, string title)
        : this(null, null, triggerTypeId, title, null, 0)
    {
    }

    public void AddDetail(AuditDetail auditDetail)
    {
        Details ??= [];
        Details.Add(auditDetail);
    }

    public void AddDetail(long id, string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
    {
        AddDetail(new AuditDetail(Convert.ToInt64(id), title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId));
    }

    public void Info(string title)
    {
        AddDetail(new AuditDetail(DetailTypeId.Info, title, 0, 0, 0, 0));
    }

    public object GetUser()
    {
        return User;
    }

    public void Info(string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
    {
        AddDetail(new AuditDetail(DetailTypeId.Info, title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId));
    }

    public void Trace(string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
    {
        AddDetail(new AuditDetail(DetailTypeId.Trace, title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId));
    }

    public void Debug(string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
    {
        AddDetail(new AuditDetail(DetailTypeId.Debug, title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId));
    }

    public void Error(string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
    {
        AddDetail(new AuditDetail(DetailTypeId.Error, title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId));
    }

    public AuditTrail BeginBatch(IBatchDataManipulationRunner batchDataManipulationRunner, int batchCount = 0)
    {
        if (Batch == null)
        {
            if (batchDataManipulationRunner == null)
            {
                throw new ArgumentNullException(nameof(batchDataManipulationRunner));
            }

            SetBatch(batchDataManipulationRunner);
        }

        Batch?.BeginBatch(batchCount);
        return this;
    }

    public void EndBatch()
    {
        if (Batch == null)
        {
            throw new ArgumentNullException(nameof(Batch));
        }

        Batch.EndBatch();
    }

    private void SetBatch(IBatchDataManipulationRunner batchDataManipulationRunner)
    {
        Batch = new BatchDataManipulation(batchDataManipulationRunner);
    }
}
