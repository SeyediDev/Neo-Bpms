using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Entities.Cmmn.UI.Reports;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.Reports;

namespace Neo.Bpms.Domain.Modeling.UiDefinitions.Cmmn;

public class DatabaseTablesDefinitions : EntityDefinition
{
    public class PublicReport : ReportDefinition
    {
        protected override Report IdentifyReport()
        {
            return DefineReport("جداول پایگاه داده");
        }

        protected override void Filters()
        {
            AddFilterField("Name");
            AddFilterField("SchemaName");
        }

        protected override void DataSources()
        {
            AddColumn("Name");
            AddColumn("SchemaName");

            AddGroupByField("SchemaName");
        }
        public class List : ReportConfigDefinition
        {
            protected override void Identify()
            {
                DefineConfig("جداول پایگاه داده", ReportViewType.List);
            }

            protected override void DefineColumns()
            {
                DisplayColumns("Name", "SchemaName");
            }
        }
    }
}
