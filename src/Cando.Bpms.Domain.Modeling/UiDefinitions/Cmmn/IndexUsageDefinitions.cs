using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions.Cmmn;

public class IndexUsageDefinitions : EntityDefinition
{
    public class IndexUsageReport : ReportDefinition
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("ایندکس های استفاده شده");
        }
        protected override void Filters()
        {
            AddFilterFields("DatabaseName");
            AddFilterFields("TableName");
            AddFilterFields("IndexName");
            AddFilterFields("IndexType");
        }
        protected override void DataSources()
        {
            AddColumn("DatabaseName");
            AddColumn("TableName");
            AddColumn("IndexName");
            AddColumn("IndexType");
            AddColumn("TotalUsage");
            AddColumn("UserSeeks");
            AddColumn("UserScans");
            AddColumn("UserLookups");
            AddColumn("UserUpdates");

            AddGroupByFields("DatabaseName");
            AddGroupByFields("TableName");
            AddGroupByFields("IndexName");
            AddGroupByFields("IndexType");

            AddAggregation("TotalUsage");
            AddAggregation("UserSeeks");
            AddAggregation("UserScans");
            AddAggregation("UserLookups");
            AddAggregation("UserUpdates");
        }
    }
}
