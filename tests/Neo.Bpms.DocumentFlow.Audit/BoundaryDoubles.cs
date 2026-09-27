// Only boundary types are doubled. The uploader, downloader, Hyper adapter,
// object-store service and MIME detector compile directly from their source files.
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;
global using Neo.Bpms.Domain.Models.Cmmn.Entities;
global using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
namespace Neo.Bpms.Domain.Models.Cmmn.Entities { public class Entity { public string Id { get; set; } = "Audit"; } }
namespace Neo.Bpms.Domain.Features.Cmmn.ObjectStorage { public class PaintableFileInfo { } }
namespace Neo.Bpms.UI.MVC.Controllers {
    public class ControllerBaseMVC : Controller {
        public bool Authenticated { get; set; } = true;
        protected object GetUser() => Authenticated ? new object() : throw new UnauthorizedAccessException();
    }
}
namespace MassTransit.Initializers { }
namespace MediatR {
    public interface IRequest<T> { }
    public interface ISender { Task<T> Send<T>(IRequest<T> request, CancellationToken token = default); }
}
namespace Hyper.Application.Features.Common.Commands.Documents {
    public record AddDocumentCommand(string SubjectTitle, string SubjectField, long SubjectId) : MediatR.IRequest<int?> {
        public int? DocumentTypeId { get; set; } public byte[] Content { get; set; } public string FileType { get; set; }
    }
    public class AttachmentDto { public string FileName { get; set; } public string ContentType { get; set; } public string Base64 { get; set; } }
    public record RemoveDocumentCommand : MediatR.IRequest<object> { public int DocumentId { get; set; } }
}
namespace Hyper.Application.Features.Common.Queries.Documents {
    public record DocumentQueryResponse(int DocumentId, Neo.Domain.Features.ObjectStore.Dto.ObjectStoreDto Dto) {
        public string SubjectField { get; set; } public string DocumentType { get; set; } public DateTimeOffset? CreateDate { get; set; }
    }
    public record DocumentItemResponse(int DocumentId, Neo.Domain.Features.ObjectStore.Dto.ObjectStoreDto Dto) : DocumentQueryResponse(DocumentId, Dto);
    public record GetOneDocumentQuery : MediatR.IRequest<DocumentQueryResponse> { public int DocumentId { get; set; } }
    public record GetDocumentsQuery : MediatR.IRequest<List<DocumentItemResponse>> {
        public string SubjectTitle { get; set; } public int SubjectId { get; set; }
        public string SubjectField { get; set; } public bool LoadData { get; set; }
    }
}
