namespace Neo.Bpms.Domain.Models.Base;

public abstract class BpmsBaseEntityStringKey : BaseEntity<string>, IEntity<string>
{
    [Key]
    [DisplayName("شناسه")]
    [AutomaticCalculation("NewGuid()", GenerationType = eGenerationType.Insert)]
    [MaxLength(41)]
    public new string Id { get; set; }
}

[Schema(nameof(BpmsSchema.Cmmn))]
[FileGroup(nameof(BpmsSchema.Cmmn))]
public abstract class BaseCmmnEntityStringKey : BpmsBaseEntityStringKey
{
}

[Schema(nameof(BpmsSchema.ProcessModel))]
[FileGroup(nameof(BpmsSchema.ProcessModel))]
public abstract class BaseProcessModelEntityStringKey : BpmsBaseEntityStringKey
{
}

[Schema(nameof(BpmsSchema.ProcessData))]
[FileGroup(nameof(BpmsSchema.ProcessData))]
public abstract class BaseProcessDataEntityStringKey : BpmsBaseEntityStringKey
{
}
