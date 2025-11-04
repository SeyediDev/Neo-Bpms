namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[DontAudit]
[DisplayNameAndEnName("سابقه اجرای رویداد زمانبندی شده فرآیند")]
public class TimerEventLog : BaseProcessDataEntity
{
    [DisplayNameAndEnName("شناسه المان فرآیند")]
    public long BPMNFlowNodeId;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [DisplayNameAndEnName("المان فرآیند")]
    public BPMNFlowNode BPMNFlowNode;

    [DisplayNameAndEnName("تاریخ")]
    public DateTime Date;

    [DisplayNameAndEnName("موفق")]
    [FAttr_Boolean("موفق", "ناموفق", "هر دو")]
    public bool Successful;

    [DisplayNameAndEnName("شناسه نمونه فرآیند", DBName = "ProcessInstanceIdL")]
    public long ProcessInstanceId;
    [DisplayNameAndEnName("نمونه فرآیند")]
    public ProcessInstanceRecord ProcessInstance;
}
