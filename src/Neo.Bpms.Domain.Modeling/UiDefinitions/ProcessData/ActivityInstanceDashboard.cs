using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Dashboards;
using Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

namespace Neo.Bpms.MetaModel.ProcessData;

public partial class ActivityInstanceRecordDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }

    public class ProcessDashboard : DashboardDefinition
    {//todo
        protected override Form Identify()
        {
            return DefineDashboard("کابین تحلیل عملکرد فعالیت فرآیند");
        }

        protected override void Filters()
        {
            AddSpecialFilterField(nameof(ActivityInstanceRecord.Process));
            AddProperty(eControlPropertyId.Required);
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ProcessVersion));
            AddSpecialFilterField(nameof(ActivityInstanceRecord.FromDate));
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ToDate));
        }
        protected override void DataSources()
        {
            base.DataSources();
            AddReport<ProcessAnalysis>();
            AddReport<OneProcessAnalysis>();
            AddReport<ProcessInstanceRecord, ProcessInstanceRecordDefinitions.ProcessAnalysis>();
            AddReport<ProcessInstanceRecord, ProcessInstanceRecordDefinitions.ActiveProcessAnalysis>();
        }
    }
}
