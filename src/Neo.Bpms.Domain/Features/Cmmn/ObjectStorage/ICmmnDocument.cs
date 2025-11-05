using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;

namespace Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;

public interface ICmmnDocument
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="subjectField"></param>
    /// <param name="recordId"></param>
    /// <param name="fileData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>documentId</returns>
    Task<string> SaveFileData(Entity entity, string subjectField, int? documentTypeId,
        string recordId, object fileData, CancellationToken cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="subjectField"></param>
    /// <param name="recordId"></param>
    /// <param name="oldDocumentId"></param>
    /// <param name="fileData"></param>
    /// <param name="record"></param>
    /// <param name="auditTrail"></param>
    /// <param name="errors"></param>
    /// <returns>New documentId</returns>
    Task<string> MoveAndSaveFile(Entity entity, string subjectField, int? documentTypeId, string recordId,
        string oldDocumentId, object fileData, CancellationToken cancellationToken);
    Task DeleteFileData(Entity entity, string subjectField, string recordId,
        string oldDocumentId, CancellationToken cancellationToken);
    Task<List<DocumentView>> GetDocuments(Entity entity, string? subjectField, int subjectId, bool loadData, CancellationToken cancellationToken);
    Task<DocumentView> GetDocumentData(int id, CancellationToken cancellationToken);

    string UploadedFilesPath();
    PaintableFileInfo GetPaintableFileInfo(DocumentView document);

}
