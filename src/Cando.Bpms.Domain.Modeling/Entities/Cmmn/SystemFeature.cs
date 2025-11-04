namespace Neo.Bpms.Domain.Modeling.Entities.Cmmn;

[DisplayNameAndEnName(DBName = "SystemFeature", Name = "امکان سیستمی")]
public class SystemFeature : BaseStringListCmmnEntity
{
}
public enum SystemFeatureId
{
    ReportDesign = 1,
    //ReportsFileExport = 2,//todo not used
    //FormsFileExport = 3,	 //todo not used
    //FormsFileImport = 4,	 //todo not used
    DashboardDesign = 5,
    FormDesign = 6,
    PublishConfigs = 7,
    AssignAccess = 8,
    ScheduledReportDesign = 9,
    EntityDesign = 10,
    ProcessDesign = 11
}
