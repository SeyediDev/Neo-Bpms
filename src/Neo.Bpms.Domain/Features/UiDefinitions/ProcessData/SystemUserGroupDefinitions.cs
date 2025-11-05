using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessData;

public class SystemUserGroupDefinitions : EntityDefinition
{
    #region form definitions
    protected override void Forms()
    {
        DefineForm<CreateForm>();
        DefineForm<DetailForm>();
        DefineForm<EditForm>();
        DefineForm<DeleteForm>();
        DefineForm<RolesEditForm>();
        DefineForm<NotificationEditForm>();
        DefineForm<IndexForm>();
        DefineForm<PositionLevelIndexForm>();
    }

    public class CreateForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("ثبت گروه کاربری سیستم", Form.eFormType.Create);
            SetOutputStateId(1);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("Name");
            AddProperty(eControlPropertyId.Required, true);
            AddField("Desc", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.DoubleWidth);
            AddField("Code");
            AddProperty(eControlPropertyId.Required);
            AddField("MembershipMethod");
            AddProperty(eControlPropertyId.Required, true);
            AddField("UserSelctionFormula");
            AddProperties(eControlPropertyId.Required);
            AddProperty(eControlPropertyId.Direction, "ltr");
            AddField("SpecificLevel");
            AddField("EOrganizationPosition");
            AddField("KnowledgeCommunity");
            AddProperties(eControlPropertyId.Editable);

        }
        protected override void UIRules()
        {
            if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            {
                AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
                AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
                AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
                SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
                    "q[MembershipMethod]==1");
                SetTask_SetProperty_ToControl("SystemUser_UserGroup_Group");
            }
            if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            {
                AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
                AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
                AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
                SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
                    "q[MembershipMethod]==3");
                SetTask_SetProperty_ToField("SpecificLevel");
                SetTask_SetProperty_ToField("EOrganizationPosition");
            }
            if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            {
                AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
                AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
                AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
                SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
                    "q[MembershipMethod]==2");
                SetTask_SetProperty_ToField("UserSelctionFormula");
            }

        }
    }
    public class EditForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("اصلاح گروه‌های کاربری سیستم", Form.eFormType.Edit);
            SetInputStateIds(1);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("Name");
            AddProperty(eControlPropertyId.Required);
            AddField("Desc", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.DoubleWidth);
            AddField("Disable");
            AddField("Code");
            AddProperty(eControlPropertyId.Required);
            AddField("MembershipMethod");
            AddProperty(eControlPropertyId.Required);
            AddField("UserSelctionFormula");
            AddProperty(eControlPropertyId.Required);
            AddProperty(eControlPropertyId.Direction, "ltr");
            AddField("SpecificLevel");
            AddField("EOrganizationPosition");
            AddField("KnowledgeCommunity");
            AddProperties(eControlPropertyId.Editable);

        }
        protected override void UIRules()
        {
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==1");
            //    SetTask_SetProperty_ToControl("SystemUser_UserGroup_Group");
            //}
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==3");
            //    SetTask_SetProperty_ToField("SpecificLevel");
            //    SetTask_SetProperty_ToField("EOrganizationPosition");
            //}
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==2");
            //    SetTask_SetProperty_ToField("UserSelctionFormula");
            //}
        }
    }
    public class DeleteForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("حذف گروه‌ کاربری سیستم", Form.eFormType.VirtualDelete);
            SetInputStateIds(1);
            SetOutputStateId(101);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("Name");
            AddField("Desc", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.DoubleWidth);
            AddField("Disable");
            AddField("Code");
            AddProperty(eControlPropertyId.Required);
            AddField("MembershipMethod");
            AddField("UserSelctionFormula");
            AddProperty(eControlPropertyId.Direction, "ltr");
            AddField("SpecificLevel");
            AddField("EOrganizationPosition");
            AddField("KnowledgeCommunity");
        }
        protected override void UIRules()
        {
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==1");
            //    SetTask_SetProperty_ToControl("SystemUser_UserGroup_Group");
            //}
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==3");
            //    SetTask_SetProperty_ToField("SpecificLevel");
            //    SetTask_SetProperty_ToField("EOrganizationPosition");
            //}
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==2");
            //    SetTask_SetProperty_ToField("UserSelctionFormula");
            //}

        }
    }
    public class DetailForm : FormDefinition
    {
        protected override Form Identify()
        {
            var frm = DefineForm("جزئیات گروه‌های کاربری سیستم", Form.eFormType.Detail);
            SetInputStateIds(1);
            return frm;
        }

        protected override void ViewModel()
        {
            AddField("Name");
            AddField("Desc", eControlTypeId.MultilineTextInput);
            AddProperty(eControlPropertyId.DoubleWidth);
            AddField("Disable");
            AddField("Code");
            AddProperty(eControlPropertyId.Required);
            AddField("MembershipMethod");
            AddField("UserSelctionFormula");
            AddProperty(eControlPropertyId.Direction, "ltr");
            AddField("SpecificLevel");
            AddField("EOrganizationPosition");
            AddField("KnowledgeCommunity");
        }
        protected override void UIRules()
        {
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==1");
            //    SetTask_SetProperty_ToControl("SystemUser_UserGroup_Group");
            //}

            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==3");
            //    SetTask_SetProperty_ToField("SpecificLevel");
            //    SetTask_SetProperty_ToField("EOrganizationPosition");
            //}
            //if (AddUIRule("ControllShow/Hide", "کنترل") != null)
            //{
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //    AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "MembershipMethod");
            //    AddUIRuleTask(UIRuleTask.eTaskType.SetProperty);
            //    SetTask_SetProperty(eControlPropertyId.ShowHide, UIRuleTask.eCalcLocation.Client,
            //        "q[MembershipMethod]==2");
            //    SetTask_SetProperty_ToField("UserSelctionFormula");
            //}
        }
    }
    public class RolesEditForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("تعیین فعالیت‌های مجاز گروه‌های کاربری", Form.eFormType.Edit, "Roles");
        }

        protected override void ViewModel()
        {
            AddField("Name", eControlPropertyId.ReadOnly);
            AddField("Desc", eControlTypeId.MultilineTextInput, eControlPropertyId.ReadOnly);
            AddField("Code", eControlPropertyId.ReadOnly);
            AddField("KnowledgeCommunity", eControlPropertyId.ReadOnly);
            //AddSubTable<SystemUserGroup_Activity>("UserGroup", null, "فعالیت‌های مجاز");
        }
    }
    public class NotificationEditForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("تعیین اطلاع رسانی گروه‌های کاربری", Form.eFormType.Edit, "Notification");
        }

        protected override void ViewModel()
        {
            AddField("Name", eControlPropertyId.ReadOnly);
            AddField("Desc", eControlTypeId.MultilineTextInput, eControlPropertyId.ReadOnly);
            AddField("Code", eControlPropertyId.ReadOnly);
            AddField("KnowledgeCommunity", eControlPropertyId.ReadOnly);
            //todo AddSubTable("NotificationAction", "RecipientUserRole", "UserGroup", "فعالیت‌های مجاز", null, true);
        }
    }
    public class IndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("فهرست گروه‌های کاربری سیستم", Form.eFormType.Index);
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
            AddFilterField("Code");
            AddFilterField("Name");
            AddFilterField("KnowledgeCommunity");
        }
        protected override void ViewModel()
        {
            AddColumn("Name");
            AddColumn("Code");
            AddColumn("KnowledgeCommunity");

            AddSubjectColumn("تعیین فعالیت‌های مجاز گروه‌ کاربری", true, false, "Roles");

            //AddSubjectColumn("اطلاع رسانی", true, false, "Notification");todo

            AddControl(eControlTypeId.SpecificLinkColumn, "al1", "فعالیت‌های مجاز کاربر");
            {
                AddProperty(eControlPropertyId.LinkTarget, "Account/AssignAccess");
                AddProperty(eControlPropertyId.LinkParameter, "AssigneeId=Id");
                AddProperty(eControlPropertyId.LinkParameter, "AssigneeType=UserGroup");
            }
        }
    }
    public class PositionLevelIndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("فهرست گروه‌های کاربری سطح", Form.eFormType.Index, "Position");
        }

        protected override void Filters()
        {
            SetInputStateIds(1);
        }
        protected override void ViewModel()
        {
            AddColumn("Name");
            AddColumn("Code");
            AddColumn("KnowledgeCommunity");
            AddSubjectColumn("تعیین فعالیت‌های مجاز گروه‌ کاربری", true, false, "Roles");

            AddControl(eControlTypeId.SpecificLinkColumn, "al1", "فعالیت‌های مجاز کاربر");
            AddProperty(eControlPropertyId.LinkTarget, "Account/AssignAccess");
            AddProperty(eControlPropertyId.LinkParameter, "AssigneeId=Id");
            AddProperty(eControlPropertyId.LinkParameter, "AssigneeType=UserGroup");

        }
    }
    #endregion form definitions
    #region report definitions
    public class SystemUserGroup_75_Report : ReportDefinition
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("فهرست گروه‌های کاربری");
        }
        protected override void Filters()
        {
            SetInputStateIds(1);
            AddFilterFields("Name", "MembershipMethod", "SpecificLevel", "Code", "KnowledgeCommunity");
        }
        protected override void DataSources()
        {
            AddColumns("Name", "EnName", "MembershipMethod", "UserSelctionFormula", "SpecificLevel", "Code", "KnowledgeCommunity");
        }

        protected override void PossibleSubReports()
        {
            //AddPossibleSubReport("SystemUserGroup_Activity", "UserGroup", "SystemUserGroup_Activity_75_Report");
            //AddPossibleSubReport("SystemUser_UserGroup", "Group", "SystemUser_UserGroup_75_Report");
        }
    }
    #endregion report definitions
}
