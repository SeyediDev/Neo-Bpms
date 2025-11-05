namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class ProcessComboItem
{
    public string processId { get; set; }
    public string versionNo { get; set; }
    public string displayText { get; set; }
}
public class CommentItem
{
    public string Text { get; set; }
    public long CommentType { get; set; }
    public string File { get; set; }
}
public class CommentsData
{
    public string ProcessName { get; set; }
    public string ProcessDescription { get; set; }
    public List<CommentItem> Comments { get; set; }

}
public class CreateInstanceIds
{
    public string AIId;
}
