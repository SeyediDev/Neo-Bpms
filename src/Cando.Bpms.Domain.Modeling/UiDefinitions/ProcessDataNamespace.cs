using Neo.Bpms.Domain.Modeling.Entities.ProcessData;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions;

public partial class ProcessDataNamespace : ModelDefinition
{
    protected override bool Identify()
    {
        return DefineModel("ProcessData", "داده فرآیند", nameof(BpmsSchema.ProcessData), nameof(DomainProvider.Domain));
    }

    protected override void Entities()
    {
        DefineEntity<SystemUserGroup>();
        DefineEntity<SystemUser>();

        ProcessInstanceEntities();
        ActivityInstanceEntities();
        DefineEntity<ProcessOfferList>();
        DefineEntity<ProcessOfferUser>();
        DefineEntity<TimerEventLog>();
        DefineCommentType();
    }

    private void ActivityInstanceEntities()
    {
        DefineEntity<ActivityInstanceRecord>();
        DefineEntity<ActivityInstanceRecordView>();
        DefineEntity<ActivityInstanceView>();
        DefineEntity<ActivityInstanceState>();
        DefineEnumeration<ActivityInstanceStateId>();
        DefineEntity<UserTaskInstanceState>();
        DefineEnumeration<UserTaskInstanceStateId>();
    }

    private void ProcessInstanceEntities()
    {
        DefineEntity<ProcessInstanceRecord>();
        DefineEntity<ProcessInstanceView>();
        DefineEntity<ProcessInstanceState>();
        DefineEnumeration<ProcessInstanceStateId>();
        DefineEntity<ProcessInstanceRecord_Attachment>();
    }

    protected override void Enumerations()
    {
        DefineEnumeration<Form.eFormType>();
    }

    private void DefineCommentType()
    {
        DefineEntity<CommentType>();
        DefineEnumeration("CommentType", "نوع یادداشت");
        {
            AddEnumItem(1, "تایید", "", "", "");
            AddEnumItem(2, "عدم تایید", "", "", "");
            AddEnumItem(3, "درخواست", "Request", "", "");
            //AddEnumItem(4, "درخواست", "", "", "");
            AddEnumItem(11, "اعلام اشکال", "", "", "");
            AddEnumItem(12, "اعلام نقص", "", "", "");
            AddEnumItem(13, "اعلام رفع اشکال", "", "", "");
            AddEnumItem(14, "اعلام رفع نقص", "", "", "");
            //AddEnumItem(15, "اعلام رفع نقص", "", "", "");
            AddEnumItem(21, "توضیح", "", "", "");
            AddEnumItem(22, "توجیه", "", "", "");
            AddEnumItem(23, "تشریح", "", "", "");
            AddEnumItem(24, "پیشنهاد", "", "", "");
            AddEnumItem(25, "راهکار", "", "", "");
            AddEnumItem(26, "تجربه", "", "", "");
            AddEnumItem(27, "راهنما", "", "", "");
            AddEnumItem(28, "درس آموخته شده", "", "", "");
            AddEnumItem(29, "درس آموخته شده", "", "", "");
            AddEnumItem(30, "خطا-دوباره‌کاری", "", "", "");
            AddEnumItem(31, "محض اطلاع", "", "", "");
            AddEnumItem(32, "یادآوری", "", "", "");
            AddEnumItem(33, "هشدار", "", "", "");
            AddEnumItem(34, "اخطار", "", "", "");
            AddEnumItem(35, "تذکر", "", "", "");
            AddEnumItem(36, "انتقاد", "", "", "");
            //				AddEnumItem(37, "انتقاد", "", "", "");
            AddEnumItem(41, "دستورالعمل", "", "", "");
            AddEnumItem(42, "اعتراض", "", "", "");
            AddEnumItem(43, "تصمیم", "", "", "");
            AddEnumItem(44, "مصوبه", "", "", "");
            AddEnumItem(45, "مستند پیشنهادی", "", "", "");
            //				AddEnumItem(46, "مستند پیشنهادی", "", "", "");
        }
    }
}
