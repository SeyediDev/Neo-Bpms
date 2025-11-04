namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[DontAudit]
[View(@"SELECT ProcessData.ProcessInstanceRecords.*, 
ProcessModel.BPMNProcesses.ProcessId,
ProcessModel.BPMNProcessVersions.Version,
Cmmn.MetaModelNamespaces.MetaNamespaceId,
Cmmn.MetaModelEntities.MetaEntityId
FROM ProcessData.ProcessInstanceRecords
inner join ProcessModel.BPMNProcessVersions on ProcessModel.BPMNProcessVersions.Id=ProcessData.ProcessInstanceRecords.ProcessVersionId
inner join ProcessModel.BPMNProcesses on ProcessModel.BPMNProcesses.Id=ProcessModel.BPMNProcessVersions.ProcessId
inner join Cmmn.MetaModelEntities on Cmmn.MetaModelEntities.Id=ProcessModel.BPMNProcessVersions.MetaEntityId
inner join Cmmn.MetaModelNamespaces on Cmmn.MetaModelNamespaces.Id=Cmmn.MetaModelEntities.MetaNamespaceId", true)]
public class ProcessInstanceView : ProcessInstanceRecord
{
    [MaxLength(256)]
    [DisplayNameAndEnName("کد فرآیند")]
    public string ProcessId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نسخه فرآیند")]
    public string Version;

    [MaxLength(256)]
    [DisplayNameAndEnName("فضای نامی")]
    public new string MetaNamespaceId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد موجودیت")]
    public string MetaEntityId;
}
