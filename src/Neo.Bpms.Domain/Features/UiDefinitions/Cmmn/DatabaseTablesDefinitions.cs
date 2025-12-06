namespace Neo.Bpms.Domain.Features.UiDefinitions.Cmmn;

public class DatabaseTablesDefinitions : EntityDefinition
{
    public class PublicReport : ReportDefinition
    {
        public override string Name => "جداول پایگاه داده";

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
            protected override string Name => "جداول پایگاه داده";
            protected override ReportViewType ViewType => ReportViewType.List;

            protected override void DefineColumns()
            {
                DisplayColumns("Name", "SchemaName");
            }
        }
    }
}
