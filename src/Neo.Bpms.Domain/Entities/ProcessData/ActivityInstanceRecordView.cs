namespace Neo.Bpms.Domain.Entities.ProcessData;

[DontAudit]
[View("SELECT ProcessData.ActivityInstanceRecords.* FROM ProcessData.ActivityInstanceRecords", true)]
public class ActivityInstanceRecordView : ActivityInstanceRecord
{
}
