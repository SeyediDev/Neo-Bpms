namespace Neo.Bpms.Domain.Entities.Base.Audit;
public class AuditDetail(long id, string title,
    long? processVersionId, long? flowNodeId,
    long? processInstanceId, long? activityInstanceId)
{
    public AuditDetail(DetailTypeId id, string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId)
        : this((long)id, title,
            processVersionId, flowNodeId, processInstanceId, activityInstanceId)
    {
    }

    public long Id { get; set; } = id;
    public string Title { get; set; } = title;
    public DateTime DateTime { get; set; } = DateTime.Now;
    public long? ProcessVersionId { get; set; } = processVersionId;
    public long? FlowNodeId { get; set; } = flowNodeId;
    public long? ProcessInstanceId { get; set; } = processInstanceId;
    public long? ActivityInstanceId { get; set; } = activityInstanceId;
}
