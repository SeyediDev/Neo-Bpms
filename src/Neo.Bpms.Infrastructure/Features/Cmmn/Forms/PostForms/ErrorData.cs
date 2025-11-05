namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

public class ErrorData(string message, string actionName, bool inIframe = false)
{
    public string Message { get; set; } = message;
    public string ActionName { get; set; } = actionName;
    public bool InIframe { get; set; } = inIframe;
}
