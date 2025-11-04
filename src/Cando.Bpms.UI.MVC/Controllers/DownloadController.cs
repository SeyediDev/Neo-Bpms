using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;

namespace Neo.Bpms.UI.MVC.Controllers;

public class DownloadController(ICmmnDocument cmmnDocument) : ControllerBaseMVC
{
    /// <summary>
    /// download File using an Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ResponseCache(Duration = int.MaxValue, VaryByQueryKeys = new string[] { "id" },
        Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult> DL(int id, CancellationToken cancellationToken)
    {
        GetUser();
        var document = await cmmnDocument.GetDocumentData(id, cancellationToken);
        System.Net.Mime.ContentDisposition cd = new()
        {
            FileName = document.Title?? document.Field, //Document.FileName;
            Inline = false
        };
        Response.Headers.Append("Content-Disposition", cd.ToString());
        if (document?.Dto == null)
            return NotFound();
        var file = File(document.Dto.Content, document.MimeType, document.FullFileName);
        file.LastModified = document.CreateDate;
        return file;
    }
}
