namespace Neo.Bpms.Domain.Features.UiDefinitions.Cmmn;

public class RunTimeQueriesDefinitions : EntityDefinition
{
    public class RunTimeQueriesReport : ReportDefinition
    {
        public override string Name => "کوئری های در حال اجرا";

        protected override void Filters()
        {
            AddFilterFields("DatabaseName");
            AddFilterFields("status");
            AddFilterFields("SqlStatement");
            AddFilterFields("ClientHost");
            AddFilterFields("ClientProgram");
            AddFilterFields("ClientProcessId");
            AddFilterFields("SqlLoginUser");
            AddFilterFields("start_time");
            AddFilterFields("cpu_time");
            AddFilterFields("wait_type");
            AddFilterFields("BlockingSessionId");
            AddFilterFields("BlockingHostname");
            AddFilterFields("BlockingProgram");
            AddFilterFields("BlockingClientProcessId");
        }

        protected override void DataSources()
        {
            AddColumn("DatabaseName");
            AddColumn("session_id");
            AddColumn("status");
            AddColumn("SqlStatement");
            AddColumn("ClientHost");
            AddColumn("ClientProgram");
            AddColumn("ClientProcessId");
            AddColumn("SqlLoginUser");
            AddColumn("DurationInSeconds");
            AddColumn("start_time");
            AddColumn("cpu_time");
            AddColumn("logical_reads");
            AddColumn("writes");
            AddColumn("ParentStatement");
            AddColumn("query_plan");
            AddColumn("wait_type");
            AddColumn("BlockingSessionId");
            AddColumn("BlockingHostname");
            AddColumn("BlockingProgram");
            AddColumn("BlockingClientProcessId");
            AddColumn("BlockingSql");
        }
    }
}
