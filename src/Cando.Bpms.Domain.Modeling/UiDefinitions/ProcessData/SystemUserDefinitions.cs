using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

public class SystemUserDefinitions : EntityDefinition
{
    public class AvatarPicture : DocumentControlDefinitions
    {
        public override string Name => "عکس";
    }
    #region form definitions

    protected override void Forms()
    {
        DefineForm<SystemUserIndexForm>();
        DefineForm<SystemUserDetailsForm>();
        DefineForm<SystemUserActivityForm>();
        DefineForm<SystemUserProfileForm>();
        DefineForm<AdminCreateForm>();
        DefineForm<AdminIndexForm>();
        DefineForm<AdminDetailsForm>();
        DefineForm<AdminEditForm>();
        DefineForm<AdminDeleteForm>();
        DefineForm<SystemUserActivityAdminForm>();
    }

    public class SystemUserIndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("فهرست کاربران زیرمجموعه جهت تعیین سطح دسترسی", Form.eFormType.Index);
            SetInputStateIds(1);
            return frm;
        }

        protected override void Filters()
        {
            AddFilterFields("UserName", "FirstName", "LastName");
            AddSpecialFilterField("Group");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In GroupUsers(q[Group])", "!IsNullOrEmpty(q[Group])");
            }
            AddSpecialFilterField("Position");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In PositionSubUsers(q[Position])", "!IsNullOrEmpty(q[Position])");
            }
            AddFilter("Id In SubUsers(user)", "!user.IsAdmin");
        }

        protected override void ViewModel()
        {
            AddColumns("FirstName", "LastName", "UserName");
            AddSubjectColumn("فعالیت مجاز کاربر و گروه‌های کاربر", true, false, "RolesAndUserGroups");
            AddProperty(eControlPropertyId.EnLabelName, "Allowed Activities and UserGroups");

            AddControl(eControlTypeId.SpecificLinkColumn, "al1", "فعالیت‌های مجاز کاربر", "Allowed Activities");
            {
                AddProperty(eControlPropertyId.LinkTarget, "Account/AssignAccess");
                AddProperty(eControlPropertyId.LinkParameter, "assigneeId=Id");
                AddProperty(eControlPropertyId.LinkParameter, "assigneeType=User");
            }
            AddProperty(eControlPropertyId.EnLabelName, "Allowed Activities");
            AddOrderBy("AutoAudit_CreatedDate", SortType.Descending);
        }
    }

    public class SystemUserProfileForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("اصلاح اطلاعات شخصی کاربر", Form.eFormType.Edit, "EditSelfProfile");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
        }

        protected override void ViewModel()
        {
            SetInputRecordIdFormula("userId");
            AddFilter("Id=userId");
            AddField("UserName");
            AddProperty(eControlPropertyId.ReadOnly);
            AddField("FirstName");
            AddProperty(eControlPropertyId.Required);
            AddField("LastName");
            AddProperty(eControlPropertyId.Required);
            AddField("NationalNumber");
            AddProperty(eControlPropertyId.Required);
            AddField("Email");
            AddField("MobileNo");
            AddField("ReplacementUser");
            AddField("AvatarPictureFileInfo", eControlTypeId.AdvancedUpload);
            AddProperty(eControlPropertyId.ScaleImageSizeKb, 100);
        }
    }

    public class SystemUserDetailsForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("جزئیات کاربر سیستم", Form.eFormType.Detail);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("UserName");
            AddField("FirstName");
            AddField("LastName");
            AddField("NationalNumber");
            AddField("Email");
            AddField("MobileNo");
            AddField("Disable");
        }
    }

    public class SystemUserActivityForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("تعیین دسترسی کاربر سیستم", Form.eFormType.Edit, "RolesAndUserGroups");
        }

        protected override void Filters()
        {
            AddFilter("Id In SubUsers(user)", "!user.IsAdmin");
        }

        protected override void ViewModel()
        {
            AddField("UserName", eControlPropertyId.ReadOnly);
            AddField("FirstName", eControlPropertyId.ReadOnly);
            AddField("LastName", eControlPropertyId.ReadOnly);
            AddField("NationalNumber", eControlPropertyId.ReadOnly);
        }
    }

    public class AdminCreateForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Create System User",
                "ثبت کاربر سیستم", Form.eFormType.Create, "Admin");
            SetOutputStateId(1);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("UserName");
            AddProperty(eControlPropertyId.Required);
            AddField("FirstName");
            AddProperty(eControlPropertyId.Required);
            AddField("LastName");
            AddProperty(eControlPropertyId.Required);
            AddField("NationalNumber");
            AddProperty(eControlPropertyId.Required);
            AddField(nameof(SystemUser.PasswordHash), eControlPropertyId.IsPassword);
            AddProperty(eControlPropertyId.Required);
            AddSubTable("SystemUser_UserGroup", "User", null, "گروه کاربری", "Id", true,
                eControlTypeId.None, "UserGroup");
        }
        protected override void DataOperations()
        {
            AddAutoCalc(nameof(SystemUser.CreationDate), "serverdatetime()");
        }

        protected override void UIRules()
        {
            SetContentOnPageLoad(nameof(SystemUser.PasswordHash), "12345");
        }
    }

    public class AdminIndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("Users List For Admin", "فهرست کاربران سیستم جهت تعیین سطح دسترسی",
                Form.eFormType.Index, "Admin");
            SetInputStateIds(1);
            return frm;
        }

        protected override void Filters()
        {
            AddFilterFields("UserName", "FirstName", "LastName");
            AddSpecialFilterField("Group");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In GroupUsers(q[Group])", "!IsNullOrEmpty(q[Group])");
            }
            AddSpecialFilterField("Position");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In PositionSubUsers(q[Position])", "!IsNullOrEmpty(q[Position])");
            }
        }

        protected override void ViewModel()
        {
            AddColumns("FirstName", "LastName", "UserName");
            AddSubjectColumn("فعالیت مجاز کاربر و گروه‌های کاربر", true, false, "AdminRolesAndUserGroups");
            AddProperty(eControlPropertyId.EnLabelName, "Allowed Activities and UserGroups");

            AddControl(eControlTypeId.SpecificLinkColumn, "al1", "فعالیت‌های مجاز کاربر", "Allowed Activities");
            {
                AddProperty(eControlPropertyId.LinkTarget, "Account/AssignAccess");
                AddProperty(eControlPropertyId.LinkParameter, "AssigneeId=Id");
                AddProperty(eControlPropertyId.LinkParameter, "AssigneeType=User");
            }
            AddOrderBy("AutoAudit_CreatedDate", SortType.Descending);
        }
    }

    public class AdminEditForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("اصلاح کاربر سیستم", Form.eFormType.Edit, "Admin");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
        }

        protected override void ViewModel()
        {
            AddField("UserName");
            AddProperty(eControlPropertyId.Required);
            AddField("FirstName");
            AddProperty(eControlPropertyId.Required);
            AddField("LastName");
            AddProperty(eControlPropertyId.Required);
            AddField("NationalNumber");
            AddProperty(eControlPropertyId.Required);
            AddField("Email");
            AddField("MobileNo");
            AddField("StartDate");
            AddField("EndDate");
            AddField("ReplacementUser");
            //AddField("Picture", eControlTypeId.File);
            AddField("Disable");
        }
    }

    public class AdminDeleteForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("حذف کاربر سیستم", Form.eFormType.VirtualDelete, "Admin");
            SetOutputStateId(101);
            SetInputStateIds(1);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("UserName");
            AddField("FirstName");
            AddField("LastName");
            AddField("NationalNumber");
            AddField("Disable");
        }
    }

    public class AdminDetailsForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("جزئیات کاربر سیستم", Form.eFormType.Detail, "Admin");
        }

        protected override void ViewModel()
        {
            AddField("UserName");
            AddField("FirstName");
            AddField("LastName");
            AddField("NationalNumber");
            AddField("Email");
            AddField("MobileNo");
            AddField("Disable");
        }
    }

    public class SystemUserActivityAdminForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("تعیین دسترسی کاربر سیستم", Form.eFormType.Edit, "AdminRolesAndUserGroups");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
        }

        protected override void ViewModel()
        {
            AddField("UserName", eControlPropertyId.ReadOnly);
            AddField("FirstName", eControlPropertyId.ReadOnly);
            AddField("LastName", eControlPropertyId.ReadOnly);
            AddField("NationalNumber", eControlPropertyId.ReadOnly);
        }
    }

    #endregion form definitions

    #region report definitions

    public class SystemUser_75_Report : ReportDefinition
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("فهرست کاربران");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);

            AddFilterField("UserName");
            AddFilterField("FirstName");
            AddFilterField("LastName");
            AddSpecialFilterField("Group");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In GroupUsers(q[Group])", "!IsNullOrEmpty(q[Group])");
            }
            AddSpecialFilterField("Position");
            {
                AddProperties(eControlPropertyId.IsNotMultiple);
                AddFilter("Id In PositionSubUsers(q[Position])", "!IsNullOrEmpty(q[Position])");
            }
        }

        protected override void DataSources()
        {
            AddColumns("UserName", "FirstName", "LastName", "NationalNumber", "ElectronicUserLevel", "Disable");
            AddColumns("Email", "MobileNo");
            AddGroupByFields("ElectronicUserLevel");
            AddPossibleSubReport("SystemUser_Claim", "User", "SystemUser_Claim_75_Report");
            AddPossibleSubReport("SystemUser_EPosition", "User", "SystemUserEPositionReport1");
            AddPossibleSubReport("SystemUser_UserGroup", "User", "SystemUser_UserGroup_75_Report");
            AddPossibleSubReport("SystemUser_Activity", "User", "SystemUser_Activity_75_Report");
        }
    }

    public class SystemUserSubUsersReport : ReportDefinition
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("فهرست کاربران زیر مجموعه");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
            AddFilter("Id In SubUsers(user)");
        }

        protected override void DataSources()
        {
            AddColumns("UserName", "FirstName", "LastName", "NationalNumber", "ElectronicUserLevel", "Disable");
            AddColumns("Email", "MobileNo");
            AddGroupByFields("ElectronicUserLevel");
            AddPossibleSubReport("SystemUser_Claim", "User", "SystemUser_Claim_75_Report");
            AddPossibleSubReport("SystemUser_EPosition", "User", "SystemUserEPositionReport1");
            AddPossibleSubReport("SystemUser_UserGroup", "User", "SystemUser_UserGroup_75_Report");
            AddPossibleSubReport("SystemUser_Activity", "User", "SystemUser_Activity_75_Report");
        }
    }

    #endregion report definitions
}
