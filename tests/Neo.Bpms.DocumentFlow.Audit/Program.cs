using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Minio;
using Neo.Bpms.UI.MVC.Controllers;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;
using Neo.Domain.Features.ObjectStore.Dto;
using Neo.Infrastructure.Features.ObjectStore;
using Hyper.AdminPanel.Web.Infrastructure.Cmmn;
using Hyper.Application.Features.Common.Commands.Documents;
using Hyper.Application.Features.Common.Queries.Documents;

var results = new List<object>();
var failed = 0;
var runRoot = Path.Combine(Path.GetTempPath(), "neo-document-audit", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(runRoot);
async Task Check(string name, Func<Task> run) {
    try { await run(); results.Add(new { name, status="PASS" }); Console.WriteLine("PASS " + name); }
    catch(Exception e) { failed++; results.Add(new { name, status="FAIL", error=e.GetType().Name+": "+e.Message }); Console.WriteLine("FAIL " + name + " — " + e.Message); }
}
void Require(bool condition, string message) { if(!condition) throw new InvalidOperationException(message); }
bool Success(FineUploaderResult result) => (bool)Newtonsoft.Json.Linq.JObject.Parse(result.BuildResponse())["success"];
(UploadController controller, string root) Uploader(string label) {
    var root=Path.Combine(runRoot,label,"uploads"); Directory.CreateDirectory(root);
    return (new UploadController(new TempDocuments(root)) { ControllerContext=new ControllerContext { HttpContext=new DefaultHttpContext() } }, root);
}
FineUploaderResult Upload(UploadController c, string uuid, string filename, byte[] bytes, Dictionary<string,StringValues> extra=null) {
    var fields=new Dictionary<string,StringValues>{{"qquuid",uuid},{"qqfilename",filename}};
    if(extra!=null) foreach(var item in extra) fields[item.Key]=item.Value;
    var files=new FormFileCollection { new FormFile(new MemoryStream(bytes),0,bytes.Length,"qqfile",filename) };
    var form=new FormCollection(fields,files); c.Request.Form=form;
    return c.Upload(form);
}
Dictionary<string,StringValues> Chunk(int index,int total,long size) => new(){{"qqpartindex",index.ToString()},{"qqtotalparts",total.ToString()},{"qqtotalfilesize",size.ToString()}};
FineUploaderResult Complete(UploadController c,string uuid,string filename) => c.Success(new FormCollection(new Dictionary<string,StringValues>{{"qquuid",uuid},{"qqfilename",filename}}));
var sample=Encoding.UTF8.GetBytes("آزمون سند\n");
await Check("Simple upload preserves bytes",()=> {
    var(c,r)=Uploader("simple");Require(Success(Upload(c,"sample","sample.txt",sample)),"Upload rejected");
    Require(File.ReadAllBytes(Path.Combine(r,"sample","sample.txt")).SequenceEqual(sample),"Bytes changed");return Task.CompletedTask;
});
await Check("UUID traversal is rejected",()=> {
    var(c,r)=Uploader("uuid-traversal");var ok=Success(Upload(c,"../outside","sample.txt",sample));
    Require(!ok && !File.Exists(Path.Combine(r,"..","outside","sample.txt")),"UUID escaped upload root inside isolated audit directory");return Task.CompletedTask;
});
await Check("Filename traversal is rejected",()=> {
    var(c,r)=Uploader("name-traversal");var ok=Success(Upload(c,"sample","../../outside.txt",sample));
    Require(!ok && !File.Exists(Path.Combine(r,"..","outside.txt")),"Filename escaped upload root inside isolated audit directory");return Task.CompletedTask;
});
await Check("Empty multipart request returns failure without exception",()=> {
    var(c,_)=Uploader("empty");c.Request.Form=new FormCollection(new Dictionary<string,StringValues>(),new FormFileCollection());
    Require(!Success(c.Upload(c.Request.Form)),"Empty request accepted");return Task.CompletedTask;
});
await Check("Out-of-order chunks combine in numeric order",()=> {
    var(c,r)=Uploader("order");c.Request.QueryString=new QueryString("?isConcurrent=True");
    foreach(var i in Enumerable.Range(0,12).Reverse()) Require(Success(Upload(c,"sample","parts.bin",[(byte)i],Chunk(i,12,12))),"Chunk rejected");
    Require(Success(Complete(c,"sample","parts.bin")),"Finalize failed");
    Require(File.ReadAllBytes(Path.Combine(r,"sample","parts.bin")).SequenceEqual(Enumerable.Range(0,12).Select(i=>(byte)i)),"Chunk order changed");return Task.CompletedTask;
});
await Check("Finalize rejects missing chunks",()=> {
    var(c,_)=Uploader("missing");c.Request.QueryString=new QueryString("?isConcurrent=True");Upload(c,"sample","parts.bin",[1],Chunk(0,2,2));
    Require(!Success(Complete(c,"sample","parts.bin")),"Incomplete file finalized successfully");return Task.CompletedTask;
});
await Check("Finalize rejects declared-size mismatch",()=> {
    var(c,_)=Uploader("length");c.Request.QueryString=new QueryString("?isConcurrent=True");Upload(c,"sample","parts.bin",[1],Chunk(0,1,200));
    Require(!Success(Complete(c,"sample","parts.bin")),"One byte accepted as a 200-byte complete file");return Task.CompletedTask;
});
await Check("Negative chunk index is rejected",()=> {
    var(c,_)=Uploader("negative");c.Request.QueryString=new QueryString("?isConcurrent=True");
    Require(!Success(Upload(c,"sample","parts.bin",[1],Chunk(-1,2,2))),"Negative chunk index accepted");return Task.CompletedTask;
});
await Check("Huge declared size is rejected without allocating large file",()=> {
    var(c,_)=Uploader("size");c.Request.QueryString=new QueryString("?isConcurrent=True");
    Require(!Success(Upload(c,"sample","parts.bin",[1],Chunk(0,2,long.MaxValue))),"Unbounded declared size accepted");return Task.CompletedTask;
});
await Check("Invalid chunk metadata returns failure without exception",()=> {
    var(c,_)=Uploader("metadata");var fields=Chunk(0,1,1);fields["qqpartindex"]="invalid";
    Require(!Success(Upload(c,"sample","parts.bin",[1],fields)),"Invalid metadata accepted");return Task.CompletedTask;
});
await Check("Repeated finalization succeeds after lost success response",()=> {
    var(c,_)=Uploader("retry");c.Request.QueryString=new QueryString("?isConcurrent=True");Upload(c,"sample","parts.bin",[1],Chunk(0,1,1));
    Require(Success(Complete(c,"sample","parts.bin")),"First completion failed");
    Require(Success(Complete(c,"sample","parts.bin")),"Second completion fails after chunks were deleted");return Task.CompletedTask;
});
await Check("Finalize invokes authentication boundary",()=> {
    var(c,_)=Uploader("auth");c.Request.QueryString=new QueryString("?isConcurrent=True");Upload(c,"sample","parts.bin",[1],Chunk(0,1,1));c.Authenticated=false;
    try { Require(!Success(Complete(c,"sample","parts.bin")),"Finalize did not call GetUser"); } catch(UnauthorizedAccessException) { }
    return Task.CompletedTask;
});
await Check("Download of absent document returns 404",async()=> {
    var c=new DownloadController(new TempDocuments(runRoot)) { ControllerContext=new ControllerContext { HttpContext=new DefaultHttpContext() } };
    Require(await c.DL(999,default) is NotFoundResult,"Expected 404");
});
await Check("Private downloads are not indefinitely cached",()=> {
    var attr=(ResponseCacheAttribute)Attribute.GetCustomAttribute(typeof(DownloadController).GetMethod("DL"),typeof(ResponseCacheAttribute));
    Require(attr?.NoStore==true,"Download advertises client caching for int.MaxValue seconds");return Task.CompletedTask;
});
var sender=new CaptureSender();var host=new CmmnDocument(sender);var entity=new Entity();
await Check("Hyper temporary upload path is implemented",()=>{ Require(!string.IsNullOrWhiteSpace(host.UploadedFilesPath()),"Empty path");return Task.CompletedTask; });
await Check("Ordinary form data URL becomes original file bytes",async()=> {
    await host.SaveFileData(entity,"File",null,"12","data:text/plain;base64,"+Convert.ToBase64String(sample)+"|sample.txt",default);
    Require(((AddDocumentCommand)sender.Last).Content.SequenceEqual(sample),"Data URL/filename stored as content instead of decoded bytes");
});
await Check("AttachmentDto preserves original bytes and MIME",async()=> {
    await host.SaveFileData(entity,"File",null,"12",new AttachmentDto {Base64=Convert.ToBase64String(sample),ContentType="text/plain",FileName="sample.txt"},default);
    var cmd=(AddDocumentCommand)sender.Last;Require(cmd.Content.SequenceEqual(sample)&&cmd.FileType=="text/plain","Attachment DTO changed");
});
await Check("Chunked Move uses staged file content instead of UUID",async()=> {
    var uuid=Guid.NewGuid().ToString();await host.MoveAndSaveFile(entity,"File",null,"12",null,uuid,default);
    Require(!((AddDocumentCommand)sender.Last).Content.SequenceEqual(Encoding.UTF8.GetBytes(uuid)),"UUID itself was stored as document content");
});
await Check("Metadata-only form load honors loadData=false",async()=> {
    await host.GetDocuments(entity,"File",12,false,default);Require(!((GetDocumentsQuery)sender.Last).LoadData,"Adapter forced LoadData=true");
});
await Check("Binary document exposes valid Base64 for preview",async()=> {
    var png=new byte[]{137,80,78,71,13,10,26,10,0,255};sender.Document=new DocumentQueryResponse(1,new ObjectStoreDto{Content=png}){SubjectField="File"};
    var view=await host.GetDocumentData(1,default);Require(view.Base64==Convert.ToBase64String(png),"Raw binary decoded as UTF-8 instead of Base64");
});
await Check("Missing host document propagates absence without exception",async()=> {
    sender.Document=null;Require(await host.GetDocumentData(999,default)==null,"Missing document was fabricated");
});
using var transport=new ObjectTransport();
using var client=new MinioClient().WithEndpoint("localhost:19090").WithCredentials("audit-key","audit-secret").WithRegion("us-east-1").WithHttpClient(new HttpClient(transport)).WithSSL(false).Build();
var store=new ObjectStoreService(client,NullLogger<ObjectStoreService>.Instance,new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string>{{"Minio:Bucket","audit-bucket"}}).Build());
await Check("Object store SDK transport roundtrip preserves binary and Persian metadata",async()=> {
    var bytes=Enumerable.Range(0,256).Select(i=>(byte)i).ToArray();
    await store.UploadFileAsync("audit-object",new ObjectStoreDto { Content=bytes,Type="آزمایش",Attributes=[new(){{"title","سند تست"}}] });
    var dto=await store.DownloadFileAsync("audit-object");Require(dto.Content.SequenceEqual(bytes)&&dto.Type=="آزمایش"&&dto.Attributes[0]["title"].ToString()=="سند تست","Object serialization changed content");
});
await Check("Object store SDK stat returns existing checksum",async()=> {Require(await store.HasAsync("audit-object"),"Stored object not found");});
await Check("Object store propagates transport outage",async()=> {
    transport.Fail=true;try {await store.DownloadFileAsync("audit-object");throw new InvalidOperationException("Outage was swallowed");}catch(HttpRequestException){}finally{transport.Fail=false;}
});
var report=new { scope="Linked production source with boundary doubles; no real database or MinIO server; expected requirements, failures are defects", passed=results.Count-failed,failed,runRoot,results };
var output=Path.Combine(runRoot,"results.json");File.WriteAllText(output,JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine($"RESULT {results.Count-failed}/{results.Count} passed; {failed} failed. Report: {output}");
return failed==0?0:1;

sealed class CaptureSender : MediatR.ISender {
    public object Last; public DocumentQueryResponse Document;
    public Task<T> Send<T>(MediatR.IRequest<T> request,CancellationToken token=default) {
        Last=request;object result=request switch {AddDocumentCommand=>1,GetDocumentsQuery=>new List<DocumentItemResponse>(),GetOneDocumentQuery=>Document,_=>new object()};return Task.FromResult((T)result);
    }
}
sealed class TempDocuments(string root) : ICmmnDocument {
    public string UploadedFilesPath()=>root;
    public Task<DocumentView> GetDocumentData(int id,CancellationToken ct)=>Task.FromResult<DocumentView>(null);
    public Task<string> SaveFileData(Entity e,string f,int? d,string r,object data,CancellationToken ct)=>throw new NotSupportedException();
    public Task<string> MoveAndSaveFile(Entity e,string f,int? d,string r,string old,object data,CancellationToken ct)=>throw new NotSupportedException();
    public Task DeleteFileData(Entity e,string f,string r,string old,CancellationToken ct)=>throw new NotSupportedException();
    public Task<List<DocumentView>> GetDocuments(Entity e,string f,int r,bool data,CancellationToken ct)=>throw new NotSupportedException();
    public PaintableFileInfo GetPaintableFileInfo(DocumentView d)=>throw new NotSupportedException();
}
sealed class ObjectTransport : HttpMessageHandler {
    byte[] body=[]; public bool Fail;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct) {
        if(Fail) throw new HttpRequestException("Isolated audit transport unavailable");
        if(request.RequestUri.Host!="localhost"||request.RequestUri.Port!=19090) throw new InvalidOperationException("Unexpected destination");
        if(request.Method==HttpMethod.Put) body=await request.Content.ReadAsByteArrayAsync(ct);
        var response=new HttpResponseMessage(HttpStatusCode.OK) {RequestMessage=request, Content=new ByteArrayContent(request.Method==HttpMethod.Get?body:[])};
        response.Headers.ETag=new System.Net.Http.Headers.EntityTagHeaderValue("\"audit-etag\"");
        response.Content.Headers.ContentType=new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        response.Content.Headers.LastModified=DateTimeOffset.UtcNow;
        if(request.Method==HttpMethod.Head) response.Content.Headers.ContentLength=body.Length;
        return response;
    }
}
