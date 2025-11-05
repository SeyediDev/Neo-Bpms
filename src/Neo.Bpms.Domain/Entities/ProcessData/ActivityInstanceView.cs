namespace Neo.Bpms.Domain.Entities.ProcessData;

[DontAudit]
[View(@"SELECT ProcessData.ActivityInstanceRecords.*, 
ProcessModel.BPMNProcesses.ProcessId,
ProcessModel.BPMNProcessVersions.Version,
ProcessModel.BPMNFlowNodes.FlowNodeId,
Cmmn.MetaModelNamespaces.MetaNamespaceId,
Cmmn.MetaModelEntities.MetaEntityId
FROM ProcessData.ActivityInstanceRecords
inner join ProcessModel.BPMNFlowNodes on ProcessModel.BPMNFlowNodes.Id=ProcessData.ActivityInstanceRecords.BPMNFlowNodeId
inner join ProcessModel.BPMNProcessVersions on ProcessModel.BPMNProcessVersions.Id=ProcessModel.BPMNFlowNodes.ProcessVersionId
inner join ProcessModel.BPMNProcesses on ProcessModel.BPMNProcesses.Id=ProcessModel.BPMNProcessVersions.ProcessId
inner join Cmmn.MetaModelEntities on Cmmn.MetaModelEntities.Id=ProcessModel.BPMNProcessVersions.MetaEntityId
inner join Cmmn.MetaModelNamespaces on Cmmn.MetaModelNamespaces.Id=Cmmn.MetaModelEntities.MetaNamespaceId", true)]
public class ActivityInstanceView : ActivityInstanceRecord
{
    [MaxLength(256)]
    [DisplayNameAndEnName("کد فرآیند")]
    public new string ProcessId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نسخه فرآیند")]
    public string Version;

    [MaxLength(256)]
    [DisplayNameAndEnName("فضای نامی")]
    public string MetaNamespaceId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد موجودیت")]
    public string MetaEntityId;
}
