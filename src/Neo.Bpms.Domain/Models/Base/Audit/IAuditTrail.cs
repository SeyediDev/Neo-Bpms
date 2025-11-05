namespace Neo.Bpms.Domain.Models.Base.Audit;

public interface IAuditTrail
{
    void AddDetail(long id, string title,
        long? processVersionId, long? flowNodeId,
        long? processInstanceId, long? activityInstanceId);

    void Info(string infoText);
    object GetUser();
}