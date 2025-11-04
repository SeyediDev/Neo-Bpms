namespace Neo.Bpms.Domain.Model.UI.Forms;

/// <summary>
/// Form definition
/// </summary>
public partial class Form
{
    [Description("نوع فرم")]
    public enum eFormType
    {
        [Description("فهرست")]
        Index = 1,

        [Description("ثبت")]
        Create,

        [Description("اصلاح")]
        Edit,

        [Description("حذف")]
        Delete,

        [Description("حذف مجازی")]
        VirtualDelete,

        [Description("جزئیات")]
        Detail,

        [Description("ویرایش گروهی")]
        BulkEdit,

        [Description("صفحه‌ی خاص روی رکورد")]
        SpecificURLForRecord = 11,

        [Description("صفحه‌ی خاص")]
        SpecificURL,

        [Description("ایجاد فرآیند")]
        ProcessCreate = 21,

        [Description("فرم فرآیند")]
        WorkItem,

        [Description("فرم پرونده‌ی فعال فرآیند")]
        ActiveProcessInstance,

        [Description("فرم پرونده‌ی فرآیند")]
        ProcessInstance,

        [Description("ایجاد فرآیند روی رکورد موجود")]
        ProcessCreateOnExistingRecord,

        // [Description("فراخوانی سرویس")]
        // ServiceOperation,

        [Description("گزارش")]
        Report = 31,

        [Description("داشبورد")]
        Dashboard = 32,

        [Description("صفحه بدون عملیات پیش‌فرض")]
        CustomPage = 40,

        [Description("فرم اعمال کامند")]
        CommandForm = 41
    }
}
