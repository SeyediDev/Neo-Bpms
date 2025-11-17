namespace Neo.Bpms.Domain.Features.UiDefinitions.Cmmn;

public class MissingIndexesDefinitions : EntityDefinition
{
    public class MissingIndexesReport : ReportDefinition
    {
        public override string Name => "گزارش ایندکس های پیشنهادی پایگاه داده";
        protected override void Filters()
        {
            AddFilterFields("TableName");
            AddFilterFields("equality_columns");
            AddFilterFields("inequality_columns");
            AddFilterFields("included_columns");
        }
        protected override void DataSources()
        {
            AddColumns("TableName");
            AddColumns("equality_columns");
            AddColumns("inequality_columns");
            AddColumns("included_columns");
            AddColumns("user_scans");
            AddColumns("user_seeks");
            AddColumns("avg_total_user_cost");
            AddColumns("avg_user_impact");
            AddColumns("AverageCostSavings");
            AddColumns("TotalCostSavings");
            AddGroupByFields("TableName");
            AddGroupByFields("equality_columns");
            AddGroupByFields("inequality_columns");
            AddGroupByFields("included_columns");
            AddAggregation("user_scans");
            AddAggregation("user_seeks");
            AddAggregation("avg_total_user_cost");
            AddAggregation("avg_user_impact");
            AddAggregation("AverageCostSavings");
            AddAggregation("TotalCostSavings");
        }
    }
}
