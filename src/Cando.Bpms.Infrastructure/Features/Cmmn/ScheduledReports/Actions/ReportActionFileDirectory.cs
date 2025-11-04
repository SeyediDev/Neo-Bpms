using Neo.Bpms.Infrastructure.Utility.FTP;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public class ReportActionFileDirectory : ReportActionFile
{
    public override async Task<bool> DoAction(LocalParameters outParameters)
    {
        bool isFtp = IsFtp();
        string uploadedFilesRoot = isFtp ? GetUploadedFilesRoot() : "";
        string extension = GetFileExtension();
        string remotePath = "";
        bool ret = CopyToFileDirectory(isFtp, uploadedFilesRoot, out string path, ref remotePath, out string fileName, extension);
        outParameters.Add("FileName", (isFtp ? remotePath : path) + "\\" + fileName);
        await Task.CompletedTask;
        return ret;
    }

    protected bool CopyToFileDirectory(bool isFtp, string uploadedFilesRoot,
        out string path, ref string remotePath, out string fileName, string extension)
    {
        bool success = true;
        path = null;
        fileName = (ReportAction.ScheduledReport.ActionName ?? ReportAction.ScheduledReportName) + "_" +
                   ReportAction.date.ToString("yyyy-MM-dd hh-mm") + extension;
        if (ReportAction.ScheduledReport.DestinationPath == null)
            return false;
        path = ReportAction.ScheduledReport.DestinationPath + "\\" +
               ReportAction.date.ToString("yyyy-MM\\dd");
        if (isFtp)
        {
            remotePath = path;
            path = uploadedFilesRoot;
        }
        CopyFile(path, fileName);
        if (isFtp)
        {
            success = TransferFileToFtp(remotePath, path, fileName);
            File.Delete(path + "\\" + fileName);
        }
        return success;
    }

    private bool TransferFileToFtp(string remotePath, string localPath, string fileName)
    {
        try
        {
            if (fileName == null)
                return false;
            CP_FTP ftpDir = new(ReportAction.ScheduledReport.DestinationPath,
                ReportAction.ScheduledReport.DestinationUserName,
                ReportAction.ScheduledReport.DestinationPassword);

            if (!Directory.Exists(remotePath))
            {
                DirectoryInfo dirinfo = Directory.CreateDirectory(remotePath);
                if (!dirinfo.Exists)
                    return false;
            }
            try
            {
                ftpDir.upload(remotePath + "\\" + fileName, localPath + "\\" + fileName);
            }
            catch
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
        return true;
    }
}
