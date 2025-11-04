namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

public class FormLogicDefinitionItem
{
    public FormLogicEventDefinition TriggeringEvent = new();
    public string RepeatOnTable { get; set; } = string.Empty;
    public List<FormLogicOperationDefinition> Operations { get; set; } = [];
}
