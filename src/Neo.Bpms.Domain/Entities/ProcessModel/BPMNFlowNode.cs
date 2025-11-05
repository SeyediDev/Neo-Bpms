namespace Neo.Bpms.Domain.Entities.ProcessModel;

[UniqueIndex("ProcessVersionId,FlowNodeId")]
[NotInMeta]
public class BPMNFlowNodeDb : BaseProcessModelStateBasedEntity
{
    [DisplayNameAndEnName("شناسه نسخه فرآیند")]
    public long ProcessVersionId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد المان فرآیند")]
    public string FlowNodeId;

    [InDisplayString]
    [MaxLength(256)]
    [DisplayNameAndEnName("نام المان فرآیند")]
    public string Name;

    [DisplayNameAndEnName("شناسه نوع المان فرآیند")]
    public FlowNodeTypeId FlowNodeTypeId;

    [DisplayNameAndEnName("شناسه عملگر")]
    public long? OperationId;

    [DisplayNameAndEnName("شناسه رویداد")]
    public long? EventTriggerTypeId;

    [DisplayNameAndEnName("شناسه فرم")]
    public long? RenderingFormId;

    [DisplayNameAndEnName("شناسه فرم فهرست")]
    public long? RenderingIndexFormId;

    [DisplayNameAndEnName("startQuantity")]
    public long? startQuantity;

    [DisplayNameAndEnName("مدت زمان اضطراری انجام فعالیت")]
    public TimeSpan? EmergencyTimeToDo;

    [DisplayNameAndEnName("مدت زمان بحرانی انجام فعالیت")]
    public TimeSpan? CriticalTimeToDo;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("المان فرآیند")]
public class BPMNFlowNode : BPMNFlowNodeDb
{
    [DisplayNameAndEnName("وضعیت")]
    public EntityStateName State;

    [DisplayNameAndEnName("نسخه فرآیند")]
    public BPMNProcessVersion ProcessVersion;

    [DisplayNameAndEnName("نوع المان فرآیند")]
    public BPMNFlowNodeType FlowNodeType;

    [DisplayNameAndEnName("Operation")]
    public BPMNOperation Operation;

    [DisplayNameAndEnName("نوع‌فراخوانی رویداد")]
    public BPMNEventType EventTriggerType;

    [DisplayNameAndEnName("فرم")]
    public MetaModelPage RenderingForm;
    [DisplayNameAndEnName("فرم فهرست")]
    public MetaModelPage RenderingIndexForm;
}
