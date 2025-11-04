namespace Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;

public class PaintableFileInfo(string mimeType, IList<string> relativePathParts, string id)
{
    public string MimeType { get; } = mimeType;
    public string Id { get; } = id;
    public IList<string> RelativePathParts { get; } = relativePathParts;
    public string RelativeUrl => string.Join("/", RelativePathParts ?? []);

    //TODO
    public string StreamerUrl => DependencyInjectionHolder.Instance.Configuration["MediaStreamingRootUrl"] +
                                 RelativeUrl +
                                 DependencyInjectionHolder.Instance.Configuration["MediaStreamingSuffix"];
    //TODO
    public string FileServerUrl => DependencyInjectionHolder.Instance.Configuration["FileServerRootUrl"] + RelativeUrl;
}
