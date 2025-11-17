namespace Neo.Bpms.Domain.Features.UiDefinitions.Cmmn;

public class LongQueriesDefinitions : EntityDefinition
{
    public class LongQueriesReport : ReportDefinition
    {
        public override string Name => "طولانی ترین و پر هزینه ترین کوئری ها";
        
        protected override void Filters()
        {
            _ = AddFilterFields("QueryText");
            _ = AddFilterFields("ExecutionCount");
            _ = AddFilterFields("CpuPerExecution");
            _ = AddFilterFields("CreationTime");
            _ = AddFilterFields("LastExecutionTime");
        }
        protected override void DataSources()
        {
            _ = AddColumn("DatabaseName");
            _ = AddColumn("ExecutionCount");
            _ = AddColumn("CpuPerExecution");
            _ = AddColumn("TotalCPU");
            _ = AddColumn("IOPerExecution");
            _ = AddColumn("TotalIO");
            _ = AddColumn("AverageElapsedTime");
            _ = AddColumn("AverageTimeBlocked");
            _ = AddColumn("AverageRowsReturned");
            _ = AddColumn("QueryText");
            _ = AddColumn("ParentQuery");
            _ = AddColumn("ExecutionPlan");
            _ = AddColumn("CreationTime");
            _ = AddColumn("LastExecutionTime");
        }
    }
}
