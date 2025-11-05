namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;
public abstract class ActivityInstanceRecordDb: BaseProcessDataEntity
{
    [DisplayNameAndEnName("شناسه فرآیند اصلی")]
    public long MainProcessId;

    [DisplayNameAndEnName("نمونه فرآیند")]
    public long ProcessInstanceId;

    [DisplayNameAndEnName("شناسه المان فرآیند")]
    public long BPMNFlowNodeId;

    [DisplayNameAndEnName("شرح")]
    public string? Description;

    [DisplayNameAndEnName("تاریخ ایجاد")]
    public DateTime CreationTime;

    [DisplayNameAndEnName("زمان شروع")]
    public DateTime? StartTime;

    [DisplayNameAndEnName("زمان تکمیل")]
    public DateTime? CompletionTime;

    [DisplayNameAndEnName("زمان بسته شدن")]
    public DateTime? CloseTime;

    [DisplayNameAndEnName("وضعیت")]
    [InDisplayString]
    public long StateId;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public ActivityInstanceStateId InstanceStateId => (ActivityInstanceStateId)StateId;

    [DisplayNameAndEnName("بسته شده")]
    public bool Closed;

    [DisplayNameAndEnName("وضعیت کار")]
    public UserTaskInstanceStateId userTaskStateId;

    [DisplayNameAndEnName("اولویت")]
    public double Priority;

    [MaxLength(41)]
    [DisplayNameAndEnName("شناسه کاربر")]
    public string ActualOwnerId;

    [DisplayNameAndEnName("شناسه نقش")]
    public long UserGroupId;

    [DisplayNameAndEnName("شمارنده تکرار")]
    public long LoopCounter;

    [DisplayNameAndEnName("تعداد توکن دریافتی")]
    public long ReceivedTokenCount;

    [DisplayNameAndEnName("activationCount")]
    public long activationCount;

    [DisplayNameAndEnName("waitingForStart")]
    public bool waitingForStart;

    [DisplayNameAndEnName("ضریب کاردهی")]
    public double WorkAllocationFactor;

    [DisplayNameAndEnName("مرحله تایید سلسله مراتبی")]
    public long ConfirmationStepId;

    [DisplayNameAndEnName("توکن های دریافتی")]
    public string ReceivedTokens;

    [DisplayNameAndEnName("AllowedExecutionTime")]
    public TimeSpan AllowedExecutionTime;

    [DisplayNameAndEnName("AllowedActiveTime")]
    public TimeSpan AllowedActiveTime;

    [MaxLength(40)]
    public string MachineId;
}

public abstract class ActivityInstanceRecordAs : ActivityInstanceRecordDb
{
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("المان فرآیند")]
    public BPMNFlowNode BPMNFlowNode;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("فرآیند اصلی")]
    public BPMNProcess MainProcess;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("نمونه فرآیند")]
    public ProcessInstanceRecord ProcessInstance;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("کاربر")]
    public SystemUser ActualOwner;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("نقش")]
    public SystemUserGroup UserGroup;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("وضعیت نمونه المان فرآیند")]
    public ActivityInstanceState State;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [AssociationMap("userTaskStateId", "Id")]
    [DisplayNameAndEnName("وضعیت نمونه تسک")]
    public UserTaskInstanceState UserTaskState;
}

[TableIndex("BPMNFlowNodeId,StateId,userTaskStateId,ActualOwnerId")]
[TableIndex("ActualOwnerId,StateId,userTaskStateId")]
[TableIndex("ProcessInstanceId")]
[DontAudit]
[DisplayNameAndEnName("نمونه المان فرآیند")]
public class ActivityInstanceRecord : ActivityInstanceRecordAs
{
    [Formula("BPMNFlowNode.ProcessVersion.ProcessId")]
    [DisplayNameAndEnName("شناسه فرآیند")]
    public long ProcessId;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("فرآیند")]
    public BPMNProcess Process;

    [Formula("BPMNFlowNode.ProcessVersion")]
    [DisplayNameAndEnName("شناسه نسخه فرآیند")]
    public long ProcessVersionId;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("نسخه فرآیند")]
    public BPMNProcessVersion ProcessVersion;

    [Formula("BPMNFlowNode.ActivityInstanceTypeId")]
    [DisplayNameAndEnName("شناسه نوع نمونه المان")]
    public long ActivityInstanceTypeId; //enum: AiType

    [Formula("BPMNFlowNode.Operation.InterfaceId")]
    public long InterfaceNameId;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public BPMNInterface InterfaceName;


    [Formula("BPMNFlowNode.OperationId")]
    public long OperationNameId;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public BPMNOperation OperationName;

    [Formula("gethour(CreationTime)")]
    [DisplayNameAndEnName("ساعت ایجاد")]
    public long CreationHour;

    [Formula("gethour(StartTime)")]
    [DisplayNameAndEnName("ساعت ایجاد")]
    public long StartHour;

    [Formula("gethour(CloseTime)")]
    [DisplayNameAndEnName("ساعت اتمام")]
    public long CloseHour;

    [Formula("10000000 * SecondDiff(CreationTime,CloseTime)")]
    [DisplayNameAndEnName("مدت زمان فعالیت")]
    public TimeSpan ActiveDuration;//active

    [Formula("10000000 * SecondDiff(StartTime,CompletionTime)")]
    [DisplayNameAndEnName("مدت زمان انجام")]
    public TimeSpan DoneDuration;

    [Formula("10000000 * SecondDiff((ProcessInstance.CreationTime),CreationTime)")]
    [DisplayNameAndEnName("فاصله زمانی ایجاد فعالیت در فرآیند")]
    public TimeSpan CreationDistance;

    [Formula("10000000 * SecondDiff((ProcessInstance.CreationTime),StartTime)")]
    [DisplayNameAndEnName("فاصله زمانی شروع فعالیت در فرآیند")]
    public TimeSpan StartDistance;

    [Formula("10000000 * SecondDiff((ProcessInstance.CreationTime),CloseTime)")]
    [DisplayNameAndEnName("فاصله زمانی بسته شدن فعالیت در فرآیند")]
    public TimeSpan CloseDistance;

    [Formula("10000000 * SecondDiff(CreationTime,StartTime)")]
    [DisplayNameAndEnName("زمان انتظار/تاخیر")]
    public TimeSpan WaitTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("از تاریخ بسته شدن")]
    public DateTime FromDate;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("تا تاریخ بسته شدن")]
    public DateTime ToDate;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("از تاریخ ایجاد")]
    public DateTime FromCreationDate;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("تا تاریخ ایجاد")]
    public DateTime ToCreationDate;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("از تاریخ تکمیل")]
    public DateTime FromCompletionTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("تا تاریخ تکمیل")]
    public DateTime ToCompletionTime;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("شناسه فیلتر تاریخ")]
    public long PastDateTimeFilterId;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("فیلتر تاریخ")]
    public PastDateTimeFilter PastDateTimeFilter;

    [DisplayNameAndEnName("ماه", EnName = "Month")]
    [FAttr_IsFormula(Formula = "datename(mm, CompletionTime)")]
    public string CompletionTimeMonth;

    [DisplayNameAndEnName("سال-ماه شمسی", EnName = "Month")]
    [FAttr_IsFormula(Formula = "PersianYearMonth(CompletionTime)")]
    public string CompletionTimePersianMonth;

    [Formula("BPMNFlowNode.FlowNodeId")]
    [DisplayNameAndEnName("کد المان فرآیند")]
    public string FlowNodeId;

    [DisplayNameAndEnName("تاریخ تکمیل", EnName = "Completion Time")]
    [FAttr_IsFormula(Formula = "DateOf(CompletionTime)")]
    public DateTime DateOfCompletionTime;

    [DisplayNameAndEnName("تاریخ بسته شدن", EnName = "Close Time")]
    [FAttr_IsFormula(Formula = "DateOf(CloseTime)")]
    public DateTime DateOfCloseTime;

    [DisplayNameAndEnName("تاریخ ایجاد", EnName = "Creation Time")]
    [FAttr_IsFormula(Formula = "DateOf(CreationTime)")]
    public DateTime DateOfCreationTime;
}
