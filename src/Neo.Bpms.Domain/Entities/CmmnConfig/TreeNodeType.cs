namespace Neo.Bpms.Domain.Entities.CmmnConfig;

[DisplayNameAndEnName("نوع نود درخت")]
public class TreeNodeType : BaseStringListCmmnConfigEntity
{
}

[DisplayNameAndEnName("نوع نود درخت")]
public enum TreeNodeTypeId
{
    [DisplayNameAndEnName("طبقه‌بندی فرآیند")]
    ProcessCategory = 1,
    [DisplayNameAndEnName("گروه فرآیند")]
    ProcessGroup = 2,
    [DisplayNameAndEnName("فرآیند")]
    Process = 3,
    [DisplayNameAndEnName("نسخه فرآیند")]
    ProcessVersion = 4,
    [DisplayNameAndEnName("منابع")]
    Resource = 5,
    [DisplayNameAndEnName("فضای نامی")]
    Namespace = 6,
    [DisplayNameAndEnName("موجودیت")]
    Entity = 7,
    [DisplayNameAndEnName("طبقه‌بندی موجودیت")]
    EntityCategory = 8,
    [DisplayNameAndEnName("پیام")]
    Message = 9,
    [DisplayNameAndEnName("سیگنال")]
    Signal = 10,
    [DisplayNameAndEnName("خطا")]
    Error = 11,
    [DisplayNameAndEnName("خبر دهی")]
    Escalation = 12,
    [DisplayNameAndEnName("رابط")]
    Interface = 13,
    [DisplayNameAndEnName("مخزن داده")]
    DataStore = 14,
    [DisplayNameAndEnName("Main Process")]
    MainProcess = 15,
    [DisplayNameAndEnName("Sub Process")]
    SubProcess = 16,
    [DisplayNameAndEnName("System")]
    System = 17,
    [DisplayNameAndEnName("Sub System")]
    SubSystem = 18,
    [DisplayNameAndEnName("Business Segment")]
    BusinessSegment = 19,
    [DisplayNameAndEnName("Business Unit")]
    BusinessUnit = 20,
    [DisplayNameAndEnName("Department")]
    Department = 21,
    [DisplayNameAndEnName("Organization Unit")]
    OrganizationUnit = 22,

    [DisplayNameAndEnName("Business Rule")]
    BusinessRule = 23,
    [DisplayNameAndEnName("Business Rule Version")]
    BusinessRuleVersion = 24,

    [DisplayNameAndEnName("Business Domain")]
    BusinessDomain = 25,
    [DisplayNameAndEnName("Business Policy")]
    BusinessPolicy = 26,
    [DisplayNameAndEnName("Business Policy Group")]
    BusinessPolicyGroup = 27,
    [DisplayNameAndEnName("Role Group")]
    RoleGroup = 28,
    [DisplayNameAndEnName("Metrics")]
    Metrics = 29,

    [DisplayNameAndEnName("Business Aria")]
    BusinessAria = 30,
    [DisplayNameAndEnName("Service Domain")]
    ServiceDomain = 31,
}
