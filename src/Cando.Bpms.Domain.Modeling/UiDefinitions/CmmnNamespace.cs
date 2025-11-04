namespace Neo.Bpms.Domain.Modeling.UiDefinitions;

public partial class CmmnNamespace : ModelDefinition
{
    protected override bool Identify()
    {
        return DefineModel(nameof(BpmsSchema.Cmmn), "مدل اطلاعات", nameof(BpmsSchema.Cmmn), nameof(DomainProvider.Domain));
    }
    protected override void Entities()
    {
        DefineEntity<DatabaseTables>();
        DefineEntity<IndexUsage>();
        DefineEntity<MissingIndexes>();
        DefineEntity<LongQueries>();
        DefineEntity<RunTimeQueries>();

        DefineEntity<MetaModelNamespace>();
        DefineEntity<MetaModelEntity>();
        DefineEntity<MetaModelField>();
        DefineEntity<MetaModelPage>();
        DefineEntity<MetaModelPageType>();
        DefineEnumeration<MetaModelPageTypeId>();
    }
}
