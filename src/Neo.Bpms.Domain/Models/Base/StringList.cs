namespace Neo.Bpms.Domain.Models.Base;

public abstract class StringList : BpmsBaseEntity
{
    [DisplayName("شناسه")] [Key] public new long Id { get; set; }

    [DisplayName("نام")]
    [InDisplayString(Culture = "fa")]
    [MaxLength(81)]
    public string Name { get; set; }

    [InDisplayString(Culture = "en")]
    [DisplayName("نام انگلیسی")]
    [MaxLength(81)]
    public string EnName { get; set; }

    [DisplayName("شرح")]
    [MaxLength(512)]
    public string Description { get; set; }

    [DisplayName("شرح انگلیسی")]
    [MaxLength(512)]
    public string EnDescription { get; set; }

    [DisplayName("نام آیکن")]
    [MaxLength(60)]
    public string IconName { get; set; }
}

[Schema(nameof(BpmsSchema.Cmmn))]
[FileGroup(nameof(BpmsSchema.Cmmn))]
public abstract class BaseStringListCmmnEntity : StringList
{
}

[Schema(nameof(BpmsSchema.CmmnConfig))]
[FileGroup(nameof(BpmsSchema.CmmnConfig))]
public abstract class BaseStringListCmmnConfigEntity : StringList
{
}

[Schema(nameof(BpmsSchema.ProcessModel))]
[FileGroup(nameof(BpmsSchema.ProcessModel))]
public abstract class BaseStringListProcessModelEntity : StringList
{
}

[Schema(nameof(BpmsSchema.ProcessData))]
[FileGroup(nameof(BpmsSchema.ProcessData))]
public abstract class BaseStringListProcessDataEntity : StringList
{
}
