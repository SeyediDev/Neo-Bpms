namespace Neo.Bpms.Domain.Entities.Base;

public class SBVR
{
    public SBVRModality Modality { get; init; }
    public string Subject { get; init; }
    public string VerbPhrase { get; init; }
    public string Condition { get; init; }
}

public interface ISBVRContainer
{
    List<SBVR> SBVRs { get; set; }
    void AddSBVR(SBVRAttribute sbvr);
}
