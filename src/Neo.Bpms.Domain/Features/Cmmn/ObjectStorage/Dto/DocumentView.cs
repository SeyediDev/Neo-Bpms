using Neo.Domain.Features.ObjectStore.Dto;

namespace Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;

public record DocumentView(
    long Id, 
    string Title, 
    string Field, 
    ObjectStoreDto? Dto,
    string Base64,
    string MimeType, 
    string FileExtension,
    DateTimeOffset? CreateDate )
{
    public string FullFileName => $"{Id}_{Field}{(Title!=Field? "_"+Title:"")}{FileExtension}";
}
