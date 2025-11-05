using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Components;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Reports;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;
using Neo.Bpms.MetaModel.ProcessData;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions.ProcessData;

public class ProcessInstanceRecordDefinitions : CRUDDefinition
{
    protected override void Forms()
    {
        DefineCRUDForms("R");
    }

    public abstract class AnalysisReport : PublicReport
    {
        protected override void Filters()
        {
            AddControl(eControlTypeId.FieldSet, "fs1");
            {
                StartSubControls();
                AddFilterField("BPMNProcess");
                AddProperty(eControlPropertyId.DoubleWidth);
                AddProperty(eControlPropertyId.RemoteData);
                AddFilterField("ProcessVersion");
                AddProperty(eControlPropertyId.DoubleWidth);
                AddProperty(eControlPropertyId.RemoteData);
                EndSubControls();
            }
            AddControl(eControlTypeId.FieldSet, "fs2");
            {
                StartSubControls();
                AddFilterField("MetaNamespace");
                AddProperty(eControlPropertyId.DoubleWidth);
                AddProperty(eControlPropertyId.RemoteData);
                AddFilterField("MetaModelEntity");
                AddProperty(eControlPropertyId.DoubleWidth);
                AddProperty(eControlPropertyId.RemoteData);
                AddFilterField("EntityPKV");
                AddFilterField("ParentActivityInstanceId");
                EndSubControls();
            }
            AddControl(eControlTypeId.FieldSet, "fs3");
            {
                StartSubControls();
                AddFilterFields("CreationTime");
                AddSpecialFilterField("FromCreationTime");
                {
                    AddFilter("q[FromCreationTime]<=CreationTime", "!IsNullOrEmpty(q[FromCreationTime])");
                }
                AddSpecialFilterField("ToCreationTime");
                {
                    AddFilter("q[ToCreationTime]>=CreationTime", "!IsNullOrEmpty(q[ToCreationTime])");
                }
                AddFilterFields("CloseTime");
                AddSpecialFilterField("FromCloseTime");
                {
                    AddFilter("q[FromCloseTime]<=CloseTime", "!IsNullOrEmpty(q[FromCloseTime])");
                }
                AddSpecialFilterField("ToCloseTime");
                {
                    AddFilter("q[ToCloseTime]>=CloseTime", "!IsNullOrEmpty(q[ToCloseTime])");
                }
                AddFilterField("State");
                AddProperty(eControlPropertyId.FilterFormula, "Id IN (1,22,33,101)");
                AddFilterFields("CreatorUser", "Locked");
                EndSubControls();
            }
        }
        protected override void DataSources()
        {
            base.DataSources();
            AddGroupByField("ProcessVersion");
            AddGroupByField("State");
            AddGroupByField("CreatorUser");
            AddGroupByField("CreationTime");
            AddGroupByField("CloseTime");
            AddGroupByField("Locked");
            AddGroupByField("MetaNamespace");
            AddGroupByField("MetaModelEntity");
            AddGroupByField("EntityPKV");
            AddGroupByField("DateOfCreationTime");
            AddGroupByField("DateOfCloseTime");
            AddAggregation("Duration");
            IncludeEntity("ProcessVersion");
            {
                AddColumns("Process");
                AddGroupByField("Process", null, false);
            }
        }
        protected override void UIRules()
        {
            FilterFormula("BPMNProcess", "ProcessVersion", "ProcessId In(q[BPMNProcess])");
            FilterFormula("MetaNamespace", "MetaModelEntity", "MetaNamespaceId In(q[MetaNamespace])");
        }

        protected override void PossibleSubReports()
        {
            AddPossibleSubReport(nameof(ActivityInstanceRecord), nameof(ActivityInstanceRecord.ProcessInstance),
                nameof(ActivityInstanceRecordDefinitions.ProcessAnalysis));
        }
    }
    /// <summary>
    /// گزارش کابین تحلیل عملکرد فرآیند غیر فعال: گزارش فرآیندهای غیر فعال با فیلتر ثابت فرآیندها در وضعیت تکمیل شده، لغو شده و حذف دستی می‌باشد.
    /// </summary>
    public class ProcessAnalysis : AnalysisReport
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("نظارت بر فرآیندهای غیرفعال");
        }

        protected override void Filters()
        {
            base.Filters();
            AddFilter($"StateId>={ProcessInstanceStateId.Completed:D}");
        }
        public class CompletedProcessCount : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("تعداد فرآیند های انجام شده", ReportViewType.Chart);
                SetChartType(Report.ChartType.Bar);
            }

            protected override void DefineColumns()
            {
                Count();
                DisplayColumns(nameof(ProcessInstanceRecord.ProcessVersion));
            }
            protected override void DefineGroupBy()
            {
                GroupBys(nameof(ProcessInstanceRecord.ProcessVersion));
            }
        }
        public class CompletedProcessDuration : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش مدت زمان انجام فرآیندها", ReportViewType.Chart);
                SetChartType(Report.ChartType.Bar);
            }

            protected override void DefineColumns()
            {
                Count();
                Max(nameof(ProcessInstanceRecord.Duration));
                Min(nameof(ProcessInstanceRecord.Duration));
                DisplayColumns(nameof(ProcessInstanceRecord.ProcessVersion));
            }
            protected override void DefineGroupBy()
            {
                GroupBys(nameof(ProcessInstanceRecord.ProcessVersion));
            }

            protected override void DefineOrderBy()
            {
                OrderBy("Count");
            }
        }
        public class List : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش جزئیات فرآیندها", ReportViewType.List);
            }

            protected override void DefineColumns()
            {
                DisplayColumns("Id", "State", "CreationTime", "CloseTime", "ProcessVersion");
                DisplayColumns("BPMNProcess", "CreatorUser", "ParentActivityInstanceId", "BPMNEngine");
                DisplayColumns("MetaNamespace", "MetaModelEntity", "Duration");
                DisplayColumns("Locked", "AllowedActiveTime");
            }
        }
    }
    /// <summary>
    /// گزارش نظارت بر فرآیند کسب و کار: گزارش فرآیند های فعال با فیلتر ثابت فرآیندها در وضعیت فعال می‌باشد.
    /// </summary>
    public class ActiveProcessAnalysis : AnalysisReport
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("نظارت بر فرآیندهای فعال");
        }
        protected override void Filters()
        {
            base.Filters();
            AddFilter($"StateId<{ProcessInstanceStateId.Completed:D}");
        }
        public class ActivatedProcess : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش فرآیند های در حال اجرا", ReportViewType.Chart);
                SetChartType(Report.ChartType.Column);
            }

            protected override void DefineColumns()
            {
                Count();
                DisplayColumns("ProcessVersion");
            }
            protected override void DefineGroupBy()
            {
                GroupBys("ProcessVersion");
            }
        }
        public class List : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش جزئیات فرآیند های در حال اجرا", ReportViewType.List);
            }

            protected override void DefineColumns()
            {
                DisplayColumns("Id", "State", "CreationTime", "ProcessVersion", "BPMNProcess", "MetaNamespace");
                DisplayColumns("MetaModelEntity", "CreatorUser", "ParentActivityInstanceId", "BPMNEngine");
                DisplayColumns("Locked", "AllowedActiveTime");
            }
        }
    }
    /// <summary>
    /// گزارش کابین تحلیل فرآیندها: گزارش تمام فرآیند های ایجاد شده بدون فیلتر ثابت.
    /// </summary>
    public class TotalProcessAnalysis : AnalysisReport
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("نظارت بر فرآیندها");
        }
        public class CreatedProcess : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش فرآیند های ایجاد شده", ReportViewType.Chart);
                SetChartType(Report.ChartType.Bar);
            }

            protected override void DefineColumns()
            {
                Count();
                DisplayColumns("ProcessVersion");
            }
            protected override void DefineGroupBy()
            {
                GroupBys("ProcessVersion");
            }
        }
        public class CompletedProcess : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش فرآیند های تکمیل شده", ReportViewType.Chart);
                SetChartType(Report.ChartType.Bar);
            }

            protected override void DefineFilter()
            {
                SetWhereCondition($"StateId == {ProcessInstanceStateId.Completed:D}");
            }

            protected override void DefineColumns()
            {
                Count();
                DisplayColumns("ProcessVersion");
            }
            protected override void DefineGroupBy()
            {
                GroupBys("ProcessVersion");
            }
        }
        public class ActivatedProcess : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش فرآیند های در حال اجرا", ReportViewType.Chart);
                SetChartType(Report.ChartType.Column);
            }

            protected override void DefineFilter()
            {
                SetWhereCondition($"StateId < {ProcessInstanceStateId.Completed:D}");
            }

            protected override void DefineColumns()
            {
                Count();
                DisplayColumns("ProcessVersion");
            }
            protected override void DefineGroupBy()
            {
                GroupBys("ProcessVersion");
            }
        }
        public class CompletedProcessDuration : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش مدت زمان انجام فرآیندها", ReportViewType.Chart);
                SetChartType(Report.ChartType.Bar);
            }

            protected override void DefineFilter()
            {
                SetWhereCondition($"StateId == {ProcessInstanceStateId.Completed:D}");
            }

            protected override void DefineColumns()
            {
                Count();
                Max("Duration");
                Min("Duration");
                DisplayColumns("ProcessVersion");
            }
            protected override void DefineGroupBy()
            {
                GroupBys("ProcessVersion");
            }

            protected override void DefineOrderBy()
            {
                OrderBy("Count");
            }
        }
        public class List : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("گزارش جزئیات فرآیندها", ReportViewType.List);
            }

            protected override void DefineColumns()
            {
                DisplayColumns("Id", "State", "CreationTime", "CloseTime", "Locked", "AllowedActiveTime");
                DisplayColumns("ProcessVersion", "CreatorUser", "ParentActivityInstanceId", "BPMNEngine");
                DisplayColumns("BPMNProcess", "MetaNamespace", "MetaModelEntity", "Duration");
            }
        }
    }
}
