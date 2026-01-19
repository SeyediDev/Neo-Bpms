using Neo.Bpms.Domain.Entities.CmmnConfig;

namespace Neo.Bpms.Domain.Entities.ProcessData;

[DontAudit]
[EntityIndex("ProcessVersionId,EntityPKV")] //todo
[EntityIndex("BPMNEngineId,Locked,Id#I")] //todo
[DisplayNameAndEnName("نمونه فرآیند")]
public abstract class ProcessInstanceRecordDb : BaseProcessDataEntity
{
    [DisplayNameAndEnName("شناسه نسخه فرآیند")]
    public long ProcessVersionId;

    [MaxLength(112)]
    [InDisplayString]
    [DisplayNameAndEnName("شناسه پرونده")]
    public string EntityPKV;

    [DisplayNameAndEnName("شناسه نمونه فعالیت مافوق")]
    public long? ParentActivityInstanceId;

    [InDisplayString]
    [DisplayNameAndEnName("شناسه وضعیت نمونه فرآیند")]
    public long StateId;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public ProcessInstanceStateId InstanceStateId => (ProcessInstanceStateId)StateId;

    [DisplayNameAndEnName("زمان ایجاد")]
    public DateTime CreationTime;

    [DisplayNameAndEnName("زمان بسته شدن")]
    public DateTime? CloseTime;

    [MaxLength(41)]
    [DisplayNameAndEnName("شناسه کاربر ایجاد کننده")]
    public string CreatorUserId;


    //[Display("وزن")]
    //public double Weight;
    [DisplayNameAndEnName("درحال پردازش")]
    public bool Locked;

    [DisplayNameAndEnName("شناسه موتور جریان کار")]
    public long BPMNEngineId;

    [DisplayNameAndEnName("مدت زمان مجاز فعال بودن")]
    public TimeSpan AllowedActiveTime;
}

public abstract class ProcessInstanceRecordAs : ProcessInstanceRecordDb
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("نسخه فرآیند")]
    public BPMNProcessVersion ProcessVersion;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("کاربر اقدام کننده")]
    public SystemUser CreatorUser;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("نمونه فعالیت مافوق")]
    public ActivityInstanceRecord ParentActivityInstance;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("وضعیت نمونه فرآیند")]
    public ProcessInstanceState State;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("موتور جریان کار")]
    public BPMNEngine BPMNEngine;
}

public class ProcessInstanceRecord : ProcessInstanceRecordAs
{
    [FormulaAttribute("ProcessVersion.ProcessId")]
    [DisplayNameAndEnName("شناسه فرآیند")]
    public long BPMNProcessId;

    [DisplayNameAndEnName("فرآیند")]
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public BPMNProcess BPMNProcess;

    [FormulaAttribute("ProcessVersion.MetaEntity.MetaNamespaceId")]
    [DisplayNameAndEnName("شناسه فضای نامی")]
    public long MetaNamespaceId;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("فضای نامی")]
    public MetaModelNamespace MetaNamespace;

    [FormulaAttribute("ProcessVersion.MetaEntityId")]
    [DisplayNameAndEnName("شناسه موجودیت")]
    public long MetaModelEntityId;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("موجودیت")]
    public MetaModelEntity MetaModelEntity;

    [FormulaAttribute("10000000 * SecondDiff(CreationTime,CloseTime)")]
    [DisplayNameAndEnName("مدت زمان")]
    public TimeSpan Duration;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("از تاریخ بسته شدن")]
    public DateTime FromCloseTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("تا تاریخ بسته شدن")]
    public DateTime ToCloseTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("از تاریخ ایجاد")]
    public DateTime FromCreationTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("تا تاریخ ایجاد")]
    public DateTime ToCreationTime;

    [FAttr_IsFormulaAttribute("DateOf(CreationTime)")]
    [DisplayNameAndEnName("تاریخ ایجاد")]
    public DateTime DateOfCreationTime;

    [FAttr_IsFormulaAttribute("DateOf(CloseTime)")]
    [DisplayNameAndEnName("تاریخ بسته شدن")]
    public DateTime DateOfCloseTime;
}
