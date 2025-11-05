using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Newtonsoft.Json.Linq;

namespace Neo.Bpms.UI.MVC.Controllers;

public class UploadController(ICmmnDocument cmmnFileManager) : ControllerBaseMVC
{
    private const string ChunkDirName = "chunks";

    [HttpPost]
    public FineUploaderResult Upload(IFormCollection fields)
    {
        GetUser();
        return HandleUpload(Request.Form.Files[0], fields); //todo try catch and so            
    }

    [HttpPost]
    public FineUploaderResult Success(IFormCollection fields)
    {
        return CombineChunks(null, fields["qquuid"], fields["qqfilename"])
            ? new FineUploaderResult(true)
            : new FineUploaderResult(false, null, "Problem conbining the chunks!");
    }

    [HttpDelete]
    public FineUploaderResult Delete(IFormCollection fields)
    {
        GetUser();
        _ = Request.Query["uuid"];
        throw new NotImplementedException();
    }

    public string Filename { get; set; }
    public Stream InputStream { get; set; }

    private FineUploaderResult HandleUpload(IFormFile file, IFormCollection fields)
    {
        Microsoft.Extensions.Primitives.StringValues partIndex = fields["qqpartindex"];

        return string.IsNullOrEmpty(partIndex) ? HandleSimpleUpload(fields, file) : HandleChunkedUpload(fields, file);
    }

    private FineUploaderResult HandleChunkedUpload(IFormCollection fields, IFormFile file)
    {
        long size = Convert.ToInt64(fields["qqtotalfilesize"]);
        Microsoft.Extensions.Primitives.StringValues uuid = fields["qquuid"];
        long index = Convert.ToInt64(fields["qqpartindex"]);
        long totalParts = Convert.ToInt64(fields["qqtotalparts"]);
        Microsoft.Extensions.Primitives.StringValues fileName = fields["qqfilename"];
        bool isConcurrent = Request.Query.TryGetValue("isConcurrent", out Microsoft.Extensions.Primitives.StringValues value) && Convert.ToBoolean(value);

        if (SizeIsValid(size))
        {
            if (StoreChunk(file, uuid, index, totalParts))
            {
                if (isConcurrent || index < totalParts - 1)
                    return new FineUploaderResult(true);
                return CombineChunks(file, uuid, fileName)
                    ? new FineUploaderResult(true)
                    : new FineUploaderResult(false, null, "Problem combining the chunks!");
            }

            return new FineUploaderResult(false, null, "Problem storing the chunk!");
        }

        return new FineUploaderResult(false, null, "Too big!", true);
    }

    private bool CombineChunks(IFormFile file, string uuid, string fileName)
    {
        try
        {
            //TODO MRSH MinIo
            string uploadedFilesPath = cmmnFileManager.UploadedFilesPath();
            string chunksDir = System.IO.Path.Combine(uploadedFilesPath, uuid, ChunkDirName);
            string destinationDir = System.IO.Path.Combine(uploadedFilesPath, uuid);
            string fileDestination = System.IO.Path.Combine(destinationDir, fileName);

            IOrderedEnumerable<string> chunkedFilePaths = Directory.GetFiles(chunksDir).OrderBy(f => f);
            using (FileStream outputStream = System.IO.File.Create(fileDestination))
            {
                foreach (string chunkPath in chunkedFilePaths)
                {
                    using (FileStream inputStream = System.IO.File.OpenRead(chunkPath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }

                Directory.Delete(chunksDir, true);
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    private bool StoreChunk(IFormFile file, string uuid, long index, long totalParts)
    {
        string destinationDir = System.IO.Path.Combine(cmmnFileManager.UploadedFilesPath(), uuid, ChunkDirName);
        string fileDestination = System.IO.Path.Combine(destinationDir, GetChunkFileName(index, totalParts));
        return MoveFile(destinationDir, file, fileDestination);
    }

    private string GetChunkFileName(long index, long totalParts)
    {
        int digits = totalParts.ToString().Length;
        return index.ToString($"D{digits}");
    }

    private bool MoveFile(string destinationDir, IFormFile file, string fileDestination)
    {
        try
        {
            Directory.CreateDirectory(destinationDir);
            using FileStream stream = System.IO.File.Create(fileDestination);
            file.CopyTo(stream);
        }
        catch
        {
            return false;
        }

        return true;
    }

    private FineUploaderResult HandleSimpleUpload(IFormCollection fields, IFormFile file)
    {
        Microsoft.Extensions.Primitives.StringValues uuid = fields["qquuid"];
        Microsoft.Extensions.Primitives.StringValues fileName = fields["qqfilename"];

        return SizeIsValid(file.Length)
            ? MoveUploadedFile(file, uuid, fileName)
                ? new FineUploaderResult(true)
                : new FineUploaderResult(false, null, "Problem copying the file!")
            : new FineUploaderResult(false, null, "Too big!", true);
    }

    private bool SizeIsValid(long size)
    {
        //            return size < MAX_FILE_SIZE;
        return true; //todo obviously
    }

    private bool MoveUploadedFile(IFormFile file, string uuid, string fileName)
    {
        string destinationDir = System.IO.Path.Combine(cmmnFileManager.UploadedFilesPath(), uuid);
        string fileDestination = System.IO.Path.Combine(destinationDir, fileName);

        return MoveFile(destinationDir, file, fileDestination);
    }


    //        public void SaveAs(string destination, bool overwrite = false, bool autoCreateDirectory = true)
    //        {
    //            if (autoCreateDirectory)
    //            {
    //                var directory = new FileInfo(destination).Directory;
    //                directory?.Create();
    //            }
    //
    //            using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
    //                InputStream.CopyTo(file);
    //        }        
}

public class FineUploaderResult : ActionResult
{
    public const string ResponseContentType = "text/plain";

    private readonly bool _success;
    private readonly string _error;
    private readonly bool? _preventRetry;
    private readonly JObject _otherData;

    public FineUploaderResult(bool success, object otherData = null, string error = null, bool? preventRetry = null)
    {
        _success = success;
        _error = error;
        _preventRetry = preventRetry;

        if (otherData != null)
            _otherData = JObject.FromObject(otherData);
    }

    public override void ExecuteResult(ActionContext context)
    {
        HttpResponse response = context.HttpContext.Response;
        response.ContentType = ResponseContentType;

        response.WriteAsync(BuildResponse());
    }

    public string BuildResponse()
    {
        JObject response = _otherData ?? [];
        response["success"] = _success;

        if (!string.IsNullOrWhiteSpace(_error))
            response["error"] = _error;

        if (_preventRetry.HasValue)
            response["preventRetry"] = _preventRetry.Value;

        return response.ToString();
    }
}
