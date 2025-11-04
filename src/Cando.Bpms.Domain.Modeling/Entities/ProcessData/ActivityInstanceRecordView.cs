namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[DontAudit]
[View("SELECT ProcessData.ActivityInstanceRecords.* FROM ProcessData.ActivityInstanceRecords", true)]
public class ActivityInstanceRecordView : ActivityInstanceRecord
{
}
