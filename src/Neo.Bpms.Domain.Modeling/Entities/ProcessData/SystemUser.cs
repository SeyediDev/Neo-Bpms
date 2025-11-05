namespace Neo.Bpms.Domain.Modeling.Entities.ProcessData;

[Entity_Index("NationalNumber")]
[Entity_Index("UserName", true)]
[DisplayNameAndEnName("کاربر سیستم")]
public class SystemUser : BaseCmmnStateBasedEntityStringKey, ICheckTimeOut
{
    [DisplayNameAndEnName("نام کاربری")]
    [MaxLength(61)]
    public string UserName {  get; set; }

    [DisplayNameAndEnName("رمز عبور", "Password")]
    [DisplayNameAndEnName("PasswordHash")]
    [MaxLength(82)]
    public string PasswordHash { get; set; }

    [DisplayNameAndEnName("نشان امنیتی")]
    [MaxLength(120)]
    public string SecurityStamp { get; set; }

    [DisplayNameAndEnName("غیر فعال", EnName = "Is Disabled")]
    public bool Disable { get; set; }

    [DisplayNameAndEnName("تلفن", EnName = "Phone Number")]
    [MaxLength(81)]
    public string PhoneNumber { get; set; }

    [DisplayNameAndEnName("شماره تلفن تایید شده")]
    public bool PhoneNumberConfirmed { get; set; }

    [DisplayNameAndEnName("آدرس پست الکترونیک", EnName = "Email")]
    [MaxLength(100)]
    public string Email { get; set; }

    [DisplayNameAndEnName("آدرس پست الکترونیک تایید شده")]
    public bool EmailConfirmed { get; set; }

    [DisplayNameAndEnName("AccessFailedCount")]
    public long AccessFailedCount { get; set; }

    [DisplayNameAndEnName("LockoutEnabled")]
    public bool LockoutEnabled { get; set; }

    [DisplayNameAndEnName("HasLockoutEndDateUtc")]
    public bool HasLockoutEndDateUtc { get; set; }

    [DisplayNameAndEnName("LockoutEndDateUtcValue")]
    public DateTime LockoutEndDateUtcValue { get; set; }

    [DisplayNameAndEnName("TwoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }

    [DisplayNameAndEnName("کد ملی", EnName = "National Number")]
    [MaxLength(61)]
    public string NationalNumber { get; set; }

    [DisplayNameAndEnName("تاریخ ایجاد کاربر")]
    public DateTime CreationDate { get; set; }

    [DisplayNameAndEnName("تاریخ شروع کاربری")]
    public DateTime? StartDate { get; set; }

    [DisplayNameAndEnName("تاریخ پایان کاربری")]
    public DateTime? EndDate { get; set; }

    [DisplayNameAndEnName("کاربر جایگزین")]
    [MaxLength(41)]
    public string ReplacementUserId { get; set; }

    [DisplayNameAndEnName("کاربر جایگزین")] public SystemUser ReplacementUser;

    [DisplayNameAndEnName("نام", EnName = "First Name")]
    [MaxLength(61)]
    [InDisplayString]
    public string FirstName { get; set; }

    [DisplayNameAndEnName("نام خانوادگی", EnName = "Last Name")]
    [MaxLength(61)]
    [InDisplayString]
    public string LastName { get; set; }

    [DisplayNameAndEnName("رده سیستمی")]
    public long ElectronicUserLevelId;

    [DisplayNameAndEnName("تلفن داخلی")]
    [MaxLength(81)]
    public string InternalPhone { get; set; }

    [DisplayNameAndEnName("شماره همراه", EnName = "Mobile No")]
    [MaxLength(81)]
    public string MobileNo { get; set; }

    [DisplayNameAndEnName("دورنگار")]
    [MaxLength(81)]
    public string Fax { get; set; }

    [DisplayNameAndEnName("کد پستی")]
    [MaxLength(81)]
    public string PostalCode { get; set; }

    [DisplayNameAndEnName("ضریب تناسب کاردهی")]
    public double WorkSuitability { get; set; }

    [DisplayNameAndEnName("مدیر سیستم")]
    public bool IsAdmin { get; set; }

    [DisplayNameAndEnName("زبان", EnName = "Language")]
    [MaxLength(41)]
    public string Culture { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public long GroupId;

    [DisplayNameAndEnName("گروه کاربری")]
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public SystemUserGroup Group;
}
