namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    public class Log
    {
        public static string Error = "danger";
        public static string Warning = "warning";
        public static string Info = "info";
        public string code, comment, className;
    }

    public readonly List<Log> Logs = [];

    protected void AddLog(string code, string className, string comment)
    {
        Logs.Add(new Log { code = code, className = className ?? Log.Error, comment = comment });
    }

    private readonly List<Log> _comments = [];

    private void AddComment(string code, string comment)
    {
        _comments.Add(new Log { code = code, comment = comment });
    }

    private void ReportToFile()
    {
        var date = DateTime.UtcNow;
        var path = Directory.GetCurrentDirectory() + "\\Report";
        Directory.CreateDirectory(path);
        var fn = date.ToString("yyyy-MM-dd hh-mm-ss");
        SaveListToFile(path + "\\" + fn + " DDLCommands" + ".txt", Commands);
        SaveListToFile(path + "\\" + fn + " DDLLogs" + ".txt",
            Logs.Select(l => l.code + " : " + l.comment).ToList());
        if (_comments.Count > 0)
            SaveListToFile(path + "\\" + fn + " DDLComments" + ".txt",
                _comments.Select(l => l.code + " : " + l.comment).ToList());
    }

    private void AddError(ModelNamespace model, Entity entity, Exception e, string code)
    {
        AddLog(code, "Error", e.ToString());
        AddMessage(model, entity, e.Message);
    }
    private void AddMessage(ModelNamespace model, Entity entity, string message, string container = "main")
    {
        var key = entity != null ? entity.model.Id + "." + entity.Id : model?.Id ?? "";
        _messageFunction?.Invoke(key, message, container);
    }

    private static void SaveListToFile(string fileName, IEnumerable<string> lst)
    {
        var file = File.CreateText(fileName);
        file.Write(string.Join("\r\n\r\n", lst));
        file.Close();
    }

    private void AddToCommandList(Entity entity, string ddl, bool changeDdlEntity)
    {
        AddMessage(entity?.model, entity, ddl, "detail");
        if (changeDdlEntity && entity != null)
            ReportEntityChanged(entity);
        Commands.Add(ddl);
    }
}