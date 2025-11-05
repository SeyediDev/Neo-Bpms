namespace Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports.Actions;

public class ReportActionFileInfo : ReportActionFile
{
    public override async Task<bool> DoAction(LocalParameters outParameters)
    {
        bool isFtp = IsFtp();
        string uploadedFilesRoot = isFtp ? GetUploadedFilesRoot() : "";
        string fileId = Guid.NewGuid().ToString();
        outParameters.Add("FileId", fileId);
        string extension = GetFileExtension();
        string fileName = fileId + extension;
        outParameters.Add("FileName", fileName);
        string path = uploadedFilesRoot;
        CopyFile(path, fileName);
        outParameters.Add("FileName", (isFtp ? "" : path) + "\\" + fileName);
        await Task.CompletedTask;
        return true;
    }
}
