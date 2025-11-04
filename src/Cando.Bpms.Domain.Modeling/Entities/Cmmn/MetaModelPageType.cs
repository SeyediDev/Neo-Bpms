namespace Neo.Bpms.Domain.Modeling.Entities.Cmmn;

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
