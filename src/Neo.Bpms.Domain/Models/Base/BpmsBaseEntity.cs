namespace Neo.Bpms.Domain.Models.Base;

public enum BpmsSchema { Cmmn, CmmnConfig, ProcessModel, ProcessData }

public abstract class BpmsBaseEntity : BaseEntity<long>, IEntity<long>
{
    [Key]
    [AutomaticCalculation]
    [DisplayName("شناسه")]
    public new long Id { get; set; }
}

[Schema(nameof(BpmsSchema.Cmmn))]
[FileGroup(nameof(BpmsSchema.Cmmn))]
public abstract class BaseCmmnEntity : BpmsBaseEntity
{
}

[Schema(nameof(BpmsSchema.CmmnConfig))]
[FileGroup(nameof(BpmsSchema.CmmnConfig))]
public abstract class BaseCmmnConfigEntity : BpmsBaseEntity
{
}

[Schema(nameof(BpmsSchema.ProcessModel))]
[FileGroup(nameof(BpmsSchema.ProcessModel))]
public abstract class BaseProcessModelEntity : BpmsBaseEntity
{
}

[Schema(nameof(BpmsSchema.ProcessData))]
[FileGroup(nameof(BpmsSchema.ProcessData))]
public abstract class BaseProcessDataEntity : BpmsBaseEntity
{
}
