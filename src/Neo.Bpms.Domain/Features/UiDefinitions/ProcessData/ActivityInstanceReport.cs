using Neo.Bpms.Domain.Entities.ProcessData;

namespace Neo.Bpms.MetaModel.ProcessData;

public partial class ActivityInstanceRecordDefinitions
{
    /// <summary>
    /// امکان تعریف گزارش‎های مختلف و استفاده از گزارش ساز (بجز گزارشات تعریف شده بصورت ثابت) همانند سایر گزارشات امکان پذیر است.
    /// نحوه تعریف گزارش فعالیت‌ها با نمایش دیاگرام فرآیند:
    /// در قسمت “تعریف طراحی جدید برای گزارش” نوع نمایش را “نمودار”   را انتخاب می نماییم سپس در قسمت “نوع نمودار ” از نوع “BPMN Diagram” انتخاب می نماییم.
    /// * امکان تعریف گزارش از نوع دیاگرام فرآیند فقط در موجودیت فعالیت‌ها امکان پذیر است.
    /// * در گزارش از نوع دیاگرام فرآیند می بایست فیلد “کد المان فرآیند” در گروه بندی استفاده شود.
    /// </summary>
    public abstract class AbstractReport : PublicReport
    {
        protected override void Filters()
        {
            //AddControl(eControlTypeId.FieldSet, "fs1", "فیلتر زمان ایجاد فعالیت");
            //{
            //	StartSubControls();
            //AddFilterField(nameof(ActivityInstanceRecord.PastDateTimeFilter");
            //{
            //	AddFilter("DateOf(CreationTime) == DateOf(AddDays(ServerDateTime(),-1))", 
            //		"q[PastDateTimeFilter] == 1");//روز گذشته
            //	AddFilter("", "q[PastDateTimeFilter] == 2");//هفته گذشته
            //	AddFilter("", "q[PastDateTimeFilter] == 3");//ماه گذشته
            //	AddFilter("", "q[PastDateTimeFilter] == 4");//سال گذشته
            AddSpecialFilterField(nameof(ActivityInstanceRecord.FromCreationDate));
            {
                AddFilter("q[FromCreationDate]<=CreationTime", "!IsNullOrEmpty(q[FromCreationDate])");
            }
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ToCreationDate));
            {
                AddFilter("q[ToCreationDate]>=CreationTime", "!IsNullOrEmpty(q[ToCreationDate])");
            }
            //	}
            //	EndSubControls();
            //}
            AddSpecialFilterField(nameof(ActivityInstanceRecord.FromDate));
            {
                AddFilter("q[FromDate]<=CloseTime", "!IsNullOrEmpty(q[FromDate])");
            }
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ToDate));
            {
                AddFilter("q[ToDate]>=CloseTime", "!IsNullOrEmpty(q[ToDate])");
            }
            AddSpecialFilterField(nameof(ActivityInstanceRecord.FromCompletionTime));
            {
                AddFilter("q[FromCompletionTime]<=CompletionTime", "!IsNullOrEmpty(q[FromCompletionTime])");
            }
            AddSpecialFilterField(nameof(ActivityInstanceRecord.ToCompletionTime));
            {
                AddFilter("q[ToCompletionTime]>=CompletionTime", "!IsNullOrEmpty(q[ToCompletionTime])");
            }
            AddFilterField(nameof(ActivityInstanceRecord.Process), eControlPropertyId.RemoteData);
            AddFilterField(nameof(ActivityInstanceRecord.ProcessVersion), eControlPropertyId.RemoteData);
            AddFilterField(nameof(ActivityInstanceRecord.BPMNFlowNode), eControlPropertyId.RemoteData);
            AddFilterField(nameof(ActivityInstanceRecord.MainProcess), eControlPropertyId.RemoteData);
            AddFilterField(nameof(ActivityInstanceRecord.ProcessInstance.MetaModelEntity), eControlPropertyId.RemoteData);
            AddFilterField(nameof(ActivityInstanceRecord.ProcessInstance.EntityPKV));

            AddFilterField(nameof(ActivityInstanceRecord.Closed));
            AddFilterField(nameof(ActivityInstanceRecord.ActualOwner));
            AddFilterField(nameof(ActivityInstanceRecord.UserGroup));
            AddFilterField(nameof(ActivityInstanceRecord.UserTaskState));
            AddProperty(eControlPropertyId.FilterFormula, "Id IN (1,21,31,41,42,101)");
            AddFilterField(nameof(ActivityInstanceRecord.State));
            AddProperty(eControlPropertyId.FilterFormula, "Id IN (1,11,22,33,42,101)");
            AddFilterField(nameof(ActivityInstanceRecord.Priority));
            AddFilterField(nameof(ActivityInstanceRecord.InterfaceName));
            AddFilterField(nameof(ActivityInstanceRecord.OperationName));
            AddFilterField(nameof(ActivityInstanceRecord.MachineId));
            AddFilterField(nameof(ActivityInstanceRecord.Id));
        }

        protected override void UIRules()
        {
            FilterFormula("Process", "ProcessVersion", "ProcessId In (q[Process])");
            FilterFormula("ProcessVersion", nameof(ActivityInstanceRecord.BPMNFlowNode),
                "ProcessVersionId In (q[ProcessVersion])");
            FilterFormula("InterfaceName", "OperationName", "InterfaceId In (q[InterfaceName])");
            //AddUIRule("SetContentUser", "SetContentUser");
            //{todo query result is ActualOwnerId maybe it should be Id
            //	AddUIRuleEvent(UIRuleEvent.eEventType.onPageLoad);
            //	AddUIRuleEvent(UIRuleEvent.eEventType.onUserChange, "Process");
            //	AddUIRuleTask(UIRuleTask.eTaskType.SetContent,UIRuleTask.eControlType.IfCondition,
            //		UIRuleTask.eCalcLocation.Server,"!IsNullOrEmpty(q[Process])");
            //	{
            //		SetTask_SetContent("ActualOwner", UIRuleTask.eCalcLocation.Server,
            //			@"Query('ProcessData.ActivityInstanceRecord', Concat('ProcessId == ',q[Process]),
            //'ActualOwnerId', true, 0, 'ActualOwnerId')");
            //	}
            //}
        }

        protected override void DataSources()
        {
            AddColumn(nameof(ActivityInstanceRecord.Id));
            AddColumn(nameof(ActivityInstanceRecord.Process));
            AddColumn(nameof(ActivityInstanceRecord.ProcessVersion));
            AddColumn(nameof(ActivityInstanceRecord.MainProcess));
            AddColumn(nameof(ActivityInstanceRecord.BPMNFlowNode));
            AddColumn(nameof(ActivityInstanceRecord.ActualOwner));
            AddColumn(nameof(ActivityInstanceRecord.UserGroup));
            AddColumn(nameof(ActivityInstanceRecord.ActiveDuration));
            AddColumn(nameof(ActivityInstanceRecord.DoneDuration));
            AddColumn(nameof(ActivityInstanceRecord.StartTime));
            AddColumn(nameof(ActivityInstanceRecord.CreationTime));
            AddColumn(nameof(ActivityInstanceRecord.CloseTime));
            AddColumn(nameof(ActivityInstanceRecord.CreationDistance));
            AddColumn(nameof(ActivityInstanceRecord.StartDistance));
            AddColumn(nameof(ActivityInstanceRecord.CloseDistance));
            AddColumn(nameof(ActivityInstanceRecord.Description));
            AddColumn(nameof(ActivityInstanceRecord.CompletionTime));
            AddColumn(nameof(ActivityInstanceRecord.Priority));

            AddGroupByField(nameof(ActivityInstanceRecord.Process));
            AddGroupByField(nameof(ActivityInstanceRecord.ProcessVersion));
            AddGroupByField(nameof(ActivityInstanceRecord.BPMNFlowNode));
            AddGroupByField(nameof(ActivityInstanceRecord.MainProcess));
            AddGroupByField(nameof(ActivityInstanceRecord.ActualOwner));
            AddGroupByField(nameof(ActivityInstanceRecord.UserGroup));
            AddGroupByField(nameof(ActivityInstanceRecord.CreationHour));
            AddGroupByField(nameof(ActivityInstanceRecord.StartHour));
            AddGroupByField(nameof(ActivityInstanceRecord.CloseHour));
            AddGroupByField(nameof(ActivityInstanceRecord.DateOfCompletionTime));
            AddGroupByField(nameof(ActivityInstanceRecord.DateOfCloseTime));
            AddGroupByField(nameof(ActivityInstanceRecord.DateOfCreationTime));
            AddGroupByField(nameof(ActivityInstanceRecord.FlowNodeId));
            AddGroupByField(nameof(ActivityInstanceRecord.Description));
            AddGroupByField(nameof(ActivityInstanceRecord.Priority));
            AddAggregation(nameof(ActivityInstanceRecord.ActiveDuration));
            AddAggregation(nameof(ActivityInstanceRecord.DoneDuration));
            AddAggregation(nameof(ActivityInstanceRecord.WaitTime));
            AddAggregation(nameof(ActivityInstanceRecord.CreationDistance));
            AddAggregation(nameof(ActivityInstanceRecord.StartDistance));
            AddAggregation(nameof(ActivityInstanceRecord.CloseDistance));

            AddGroupByField(nameof(ActivityInstanceRecord.InterfaceName));
            AddGroupByField(nameof(ActivityInstanceRecord.OperationName));
        }

        public abstract class AbstractGroupByConfig : GroupByConfigDefinition
        {
            protected void DefineAggs()
            {
                Count();
                Sum(nameof(ActivityInstanceRecord.ActiveDuration));
                Sum(nameof(ActivityInstanceRecord.DoneDuration));
                Sum(nameof(ActivityInstanceRecord.WaitTime));
                Sum(nameof(ActivityInstanceRecord.CreationDistance));
                Sum(nameof(ActivityInstanceRecord.StartDistance));
                Sum(nameof(ActivityInstanceRecord.CloseDistance));
            }
        }
        public abstract class AbstractChartConfig(ChartType chartType) : ChartConfigDefinition(chartType)
        {
            protected void DefineAggs()
            {
                Count();
                Sum(nameof(ActivityInstanceRecord.ActiveDuration));
                Sum(nameof(ActivityInstanceRecord.DoneDuration));
                Sum(nameof(ActivityInstanceRecord.WaitTime));
                Sum(nameof(ActivityInstanceRecord.CreationDistance));
                Sum(nameof(ActivityInstanceRecord.StartDistance));
                Sum(nameof(ActivityInstanceRecord.CloseDistance));
            }
        }
    }

    public class ProcessAnalysis : AbstractReport
    {
        public override string Name => "نظارت بر فعالیت های کسب و کار";

        public class GroupByFlowNodeAndUser : AbstractGroupByConfig
        {
            protected override string Name => "آمار به تفکیک نوع فعالیت و کاربر";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.BPMNFlowNode));
                GroupBy(nameof(ActivityInstanceRecord.ActualOwner));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.BPMNFlowNode));
                DisplayColumn(nameof(ActivityInstanceRecord.ActualOwner));
                DefineAggs();
            }
        }

        public class GroupByFlowNodeAndUserGroup : AbstractGroupByConfig
        {
            protected override string Name => "آمار به تفکیک نوع فعالیت و نقش";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.BPMNFlowNode));
                GroupBy(nameof(ActivityInstanceRecord.UserGroup));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.BPMNFlowNode));
                DisplayColumn(nameof(ActivityInstanceRecord.UserGroup));
                DefineAggs();
            }
        }

        public class DailyWorkDone : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارهای انجام شده روزانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.DateOfCompletionTime));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.DateOfCompletionTime));
                DefineAggs();
            }
        }

        public class DailyWorkDoneChart() : AbstractChartConfig(ChartType.Column)
        {
            protected override string Name => "گزارش کارهای انجام شده روزانه(چارت)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.DateOfCompletionTime));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.DateOfCompletionTime));
                DefineAggs();
            }
        }

        public class MonthlyWorkDone : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارهای انجام شده ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
                DefineAggs();
            }
        }

        public class MonthlyWorkDoneChart() : AbstractChartConfig(ChartType.Line)
        {
            protected override string Name => "گزارش کارهای انجام شده ماهانه(چارت)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
                DefineAggs();
            }
        }
        public class UserWorks : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارهای کاربران در هر فرآیند";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ProcessVersion));
                GroupBy(nameof(ActivityInstanceRecord.ActualOwner));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.ProcessVersion));
                DisplayColumn(nameof(ActivityInstanceRecord.ActualOwner));
                DefineAggs();
            }
        }

        public class UserGroupWorks : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارهای نقش ها در هر فرآیند";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ProcessVersion));
                GroupBy(nameof(ActivityInstanceRecord.UserGroup));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.ProcessVersion));
                DisplayColumn(nameof(ActivityInstanceRecord.UserGroup));
                DefineAggs();
            }
        }

        public class UserWorksChart() : AbstractChartConfig(ChartType.Column)
        {
            protected override string Name => "نمودار کارهای کاربران در هر فرآیند";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ProcessVersion));
                GroupBy(nameof(ActivityInstanceRecord.ActualOwner));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.ProcessVersion));
                DisplayColumn(nameof(ActivityInstanceRecord.ActualOwner));
                DefineAggs();
            }
        }

        public class UserGroupWorksChart() : AbstractChartConfig(ChartType.Column)
        {
            protected override string Name => "نمودار کارهای نقش ها در هر فرآیند";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ProcessVersion));
                GroupBy(nameof(ActivityInstanceRecord.UserGroup));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.ProcessVersion));
                DisplayColumn(nameof(ActivityInstanceRecord.UserGroup));
                DefineAggs();
            }
        }
        public class CompletedActivities() : AbstractChartConfig(ChartType.Bar)
        {
            protected override string Name => "گزارش فعالیت های تکمیل شده";
            protected override string WhereCondition => $"StateId=={ActivityInstanceStateId.Completed:D}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                DefineAggs();
            }
        }

        public class PendingActivities() : AbstractChartConfig(ChartType.Column)
        {
            protected override string Name => "گزارش فعالیت های در انتظار";
            protected override string WhereCondition => $"StateId<{ActivityInstanceStateId.Completed:D}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                DefineAggs();
            }
        }

        public class OverDueActivities() : AbstractChartConfig(ChartType.Column)
        {
            protected override string Name => "گزارش فعالیت های عقب افتاده";
            protected override string WhereCondition => $"StateId<{ActivityInstanceStateId.Completed:D} && daydiff(({nameof(ActivityInstanceRecord.CreationTime)}),(getdate())) > 1";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                DefineAggs();
            }
        }
        public class AvgDurationForActivities() : AbstractChartConfig(ChartType.Area)
        {
            protected override string Name => "گزارش میانگین زمان صرف شده";
            protected override string WhereCondition => $"StateId=={ActivityInstanceStateId.Completed:D}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                Aggregation(nameof(ActivityInstanceRecord.DoneDuration), AggregationType.Average);
                Aggregation(nameof(ActivityInstanceRecord.DoneDuration), AggregationType.Max);
            }
        }
    }

    public class OneProcessAnalysis : AbstractReport
    {
        public override string Name => "نظارت بر فعالیت های یک فرآیند";

        protected override void Filters()
        {
            base.Filters();
            SelectFilterField(nameof(ActivityInstanceRecord.Process), eControlPropertyId.IsNotMultiple, eControlPropertyId.Required);
        }

        public class InstancesChart() : AbstractChartConfig(ChartType.Area)
        {
            protected override string Name => "تعداد فعالیت فعال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.BPMNFlowNode));
                GroupBy(nameof(ActivityInstanceRecord.Closed));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.BPMNFlowNode));
                DisplayColumn(nameof(ActivityInstanceRecord.Closed));
                Count();
                Max(nameof(ActivityInstanceRecord.ActiveDuration));
                Min(nameof(ActivityInstanceRecord.ActiveDuration));
            }
        }

        public class BpmnDiagram() : ChartConfigDefinition(ChartType.BpmnDiagram)
        {
            protected override string Name => "تعداد فعالیت فعال در دیاگرام";
            protected override string WhereCondition => "IsNull(Closed,0)==0";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                Count();
                Max(nameof(ActivityInstanceRecord.ActiveDuration));
                Min(nameof(ActivityInstanceRecord.ActiveDuration));
            }
        }
        public class CompletedBpmnDiagram : BpmnDiagramDefinition
        {
            protected override string Name => "دیاگرام فعالیت های تکمیل شده";
            protected override string WhereCondition => $"StateId=={ActivityInstanceStateId.Completed:D}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.FlowNodeId));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.FlowNodeId));
                Count();
                Max(nameof(ActivityInstanceRecord.DoneDuration));
                Min(nameof(ActivityInstanceRecord.DoneDuration));
            }
        }

    }

    /// <summary>
    /// گزارش کارهای انجام شده توسط من: گزارش فعالیت‌های غیر فعال فعالیت‌ها با فیلتر ثابت (Closed==true) و فیلتر ثابت کاربر انجام دهنده فعالیت برابر کاربر گزارش گیرنده می‌باشد.
    /// </summary>
    public class ReportOfMyWork : AbstractReport
    {
        public override string Name => "نظارت بر فعالیت های من";

        protected override void Filters()
        {
            base.Filters();
            AddFilter("Closed==1");
            AddFilter($"{nameof(ActivityInstanceRecord.ActualOwnerId)} == (UserId(user))");
        }

        public class DailyWork : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارهای انجام شده روزانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.DateOfCompletionTime));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.DateOfCompletionTime));
                DefineAggs();
            }
        }

        public class DailyWorkChart() : ChartConfigDefinition(ChartType.Column)
        {
            protected override string Name => "گزارش کارهای انجام شده روزانه(چارت)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.DateOfCompletionTime));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.DateOfCompletionTime));
                Count();
            }
        }

        public class MonthlyWork : AbstractGroupByConfig
        {
            protected override string Name => "گزارش کارها انجام شده ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
                DefineAggs();
            }
        }

        public class MonthlyWorkChart() : ChartConfigDefinition(ChartType.Line)
        {
            protected override string Name => "گزارش کارها انجام شده ماهانه(چارت)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
            }

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(ActivityInstanceRecord.CompletionTimePersianMonth));
                Count();
            }
        }
    }
    public class MyActiveInstanceReport : AbstractReport
    {
        public override string Name => "گزارش فعالیت های فعال من";
        protected override void Filters()
        {
            base.Filters();
            AddFilter("Closed==0");
            AddFilter($"{nameof(ActivityInstanceRecord.ActualOwnerId)} == (UserId(user))");
        }

        public class AllActiveWorkItemCount() : AbstractChartConfig(ChartType.MetricBox)
        {
            protected override string Name => "گزارش تعداد کار های ایجاد شده";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ActualOwnerId));
            }

            protected override void DefineColumns()
            {
                Count(null, "کارهای من");
            }
        }
        public class TodayCreatedWorkItemCount() : AbstractChartConfig(ChartType.MetricBox)
        {
            protected override string Name => "گزارش تعداد کار های ایجاد شده در روز جاری";
            protected override string WhereCondition => $"DateOf({nameof(ActivityInstanceRecord.CreationTime)}) == ServerDate()";
            protected override void DefineGroupBy()
            {
                GroupBy(nameof(ActivityInstanceRecord.ActualOwnerId));
            }

            protected override void DefineColumns()
            {
                Count(null, "کارهای ایجاد شده امروز");
            }
        }

        /// <summary>
        /// نمایش تعداد کارهای تکمیل شده به صورت Gauge
        /// این ویجت برای نمایش تعداد کارهای بسته شده مناسب است
        /// می‌توانید در تنظیمات گزارش، Range (مثلاً 0-100) و Levels (سطوح رنگی) را تنظیم کنید
        /// </summary>
        public class CompletedActivitiesGauge() : AbstractChartConfig(ChartType.Gauge)
        {
            protected override string Name => "تعداد کارهای تکمیل شده (Gauge)";

            protected override string WhereCondition => $"{nameof(ActivityInstanceRecord.Closed)} == true";

            protected override void DefineGroupBy()
            {
                // بدون گروه‌بندی - نمایش یک مقدار کلی
            }

            protected override void DefineColumns()
            {
                Count(null, "کارهای تکمیل شده");
            }
        }
    }
}
