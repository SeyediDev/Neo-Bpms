namespace Neo.Bpms.Domain.Entities.Cmmn;

public class MetaModelPageType : BaseStringListCmmnEntity
{
}

public enum MetaModelPageTypeId
{
    None = 0,
    Form = 1,
    Report = 2,
    Dashboard = 3,
    Cartable = 4
}
