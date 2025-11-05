namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;

public class IndexFormSubjectId : FormFieldDefinition
{
    public IndexFormSubjectId(string alias) : base(alias)
    {
    }
    public string Name;
    public bool HasTooltip = false;
    public bool HasText = true;
    public string IconField;
    public string TooltipField;
}
