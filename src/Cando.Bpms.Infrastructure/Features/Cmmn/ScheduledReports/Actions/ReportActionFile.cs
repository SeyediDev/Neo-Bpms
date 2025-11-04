namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public abstract class ReportActionFile : ReportActionType
{
    protected bool IsFtp()
    {
        return ReportAction.ScheduledReport.DestinationPath?.TrimStart().ToLower().StartsWith("ftp:") ?? false;
    }
    protected void CopyFile(string path, string fileName)
    {
        if (!Directory.Exists(path))
        {
            DirectoryInfo info = Directory.CreateDirectory(path);
            if (!info.Exists)
                return;
        }

        if (ReportAction.ReportView.Content is not MemoryStream memoryStream)
            return;
        memoryStream.Seek(0, SeekOrigin.Begin);
        using (FileStream fileStream = File.Create(path + "\\" + fileName))
        {
            memoryStream.CopyTo(fileStream);
        }
    }
    protected static string GetUploadedFilesRoot()
    {
        string uploadedFilesRoot = "";
        try
        {
            uploadedFilesRoot = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        }
        catch
        {
            try
            {
                uploadedFilesRoot = AppDomain.CurrentDomain.BaseDirectory;
            }
            catch
            {
                // ignored
            }
        }
        if (!uploadedFilesRoot.EndsWith("\\"))
            uploadedFilesRoot += "\\";
        uploadedFilesRoot += "ScheduledReportLog\\";
        return uploadedFilesRoot;
    }
}
