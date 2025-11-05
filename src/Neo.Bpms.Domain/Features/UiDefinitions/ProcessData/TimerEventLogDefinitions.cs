namespace Neo.Bpms.Domain.Features.UiDefinitions.ProcessData;

public class TimerEventLogDefinitions : EntityDefinition
{
    protected override void Forms()
    {
        DefineForm<TimerEventLogIndexForm>();
    }
    public class TimerEventLogIndexForm : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("سابقه اجرای زمانبندی رویداد فرآیند", Form.eFormType.Index);
        }

        protected override void ViewModel()
        {
            AddFilterField("BPMNFlowNode");
            AddFilterField("Date");
            AddFilterField("Successful");

            AddColumn("BPMNFlowNode");
            AddColumn("Date");
            AddColumn("Successful");
            AddColumn("ProcessInstance");

            AddOrderBy("Date", SortType.Descending);
        }
    }
}
