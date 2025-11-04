namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[DisplayNameAndEnName("نظرات و پیوست ها")]
[DontAudit]
public class ProcessInstanceRecord_Attachment: BaseProcessDataEntity
{
    public long ProcessInstanceId;

    [DisplayNameAndEnName("نمونه فرآیند")]
    public ProcessInstanceRecord ProcessInstance;

    public long ActivityInstanceId;

    [DisplayNameAndEnName("نمونه فعالیت")]
    public ActivityInstanceRecord ActivityInstance;

    [DisplayNameAndEnName("توضیحات")]
    [MaxLength(512)]
    public string? Description;

    [DisplayNameAndEnName("زمان")]
    public DateTime date;

    [DisplayNameAndEnName("نوع نظر")]
    public long commentTypeId;

    [AssociationMap("commentTypeId", "Id")]
    [DisplayNameAndEnName("نوع نظر")]
    public CommentType commentType;

    [DisplayNameAndEnName("اعلام کننده")]
    [MaxLength(41)]
    public string UserId;

    [AssociationMap("UserId", "Id")]
    [DisplayNameAndEnName("اعلام کننده")]
    public SystemUser User;
}
