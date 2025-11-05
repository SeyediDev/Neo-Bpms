namespace Neo.Bpms.Domain.Entities.Dmn.DecisionRequirements;

/// <summary>
/// A Knowledge Source element denotes an authority for a Business Knowledge Model or Decision.
/// 
/// the class KnowledgeSource is used to model authoritative knowledge sources in a decision model.
/// </summary>
public class KnowledgeSource : DRGElement
{
    public KnowledgeSource(string id) : base(id) { }
    public KnowledgeSource(string id, string name) : base(id, name) { }
    public KnowledgeSource(string id, string name, string desciption) : base(id, name, desciption) { }
    /// <summary>
    /// The URI where this KnowledgeSource is located. The locationURI MUST be specified in a URI format
    /// </summary>
    public string locationURI = null;
    public string type = null;
    public OrganisationalUnit owner = null;
    /// <summary>
    /// the instances of AuthorityRequirement that contribute to this KnowledgeSource
    /// </summary>
    public List<AuthorityRequirement> authorityRequirement = [];
    public void addAuthorityRequirement(AuthorityRequirement authorityRequirement)
    {
        this.authorityRequirement.Add(authorityRequirement);
    }
}
