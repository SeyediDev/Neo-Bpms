namespace Neo.Bpms.Domain.Models.Base.Utils;

public class IdValueEnum(Enum @enum)
{
    public int Id { get; set; } = @enum.ToInt();
    public string Name { get; set; } = string.IsNullOrEmpty(@enum.ToName()) ? @enum.ToString() : @enum.ToName();
}