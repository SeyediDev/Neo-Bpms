namespace Neo.Bpms.Domain.Modeling.UiDefinitions.CmmnConfig;

public class ScheduledReportLogDefinitions : CRUDDefinition
{
    public class File : DocumentControlDefinitions
    {
        public override string Name => "فایل";
    }
    protected override void AdditionalForms()
    {
        DefineForm<ScheduledReportLogIndexForm>();
        DefineForm<ScheduledReportLogIndexForm2>();
    }

    public class ScheduledReportLogIndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("سابقه اجرای گزارش زمانبندی", Form.eFormType.Index);
        }

        protected override void ViewModel()
        {
            AddFilterFields("ScheduledReport", "Date", "Successfull");

            AddColumn("ScheduledReport");
            AddColumn("Date");
            AddColumn("Successfull");
            AddColumn("FileName");
            AddColumn("File");

            AddOrderBy("Date", SortType.Descending);
        }
    }
    public class ScheduledReportLogIndexForm2 : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("سابقه گزارش گیری", Form.eFormType.Index, "FilteredByUserGroup");
        }

        protected override void ViewModel()
        {
            AddFilter(Func.Select("UserAndOrganization.SystemUser_UserGroup",
                "1", "((UserId==(UserId(user))))AND((ScheduledReport.UserGroupId)==Group)", null, false, 1) + ">0");

            AddFilterFields("Date", "ScheduledReport", "Successfull");

            AddColumn("Date");
            AddColumn("ScheduledReport");
            AddColumn("Successfull");
            AddColumn("FileName");
            AddColumn("File");

            AddOrderBy("Date", SortType.Descending);
        }
    }
}
