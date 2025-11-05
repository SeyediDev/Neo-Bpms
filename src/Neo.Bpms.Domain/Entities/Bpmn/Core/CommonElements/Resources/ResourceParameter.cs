using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

namespace Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Resources;

public class ResourceParameter(Resource parent, string id, string name, ResourceParameter.eType type, bool isRequired) : BaseElement(parent, id, name)
{
    //		public string name;

    /// <summary>
    /// Specifies the type of the query parameter.
    /// </summary>
    public eType type = type;

    public bool isRequired = isRequired;

    public enum eType
    {
        UserField,
        EntityField,
        Claim,
        Role
    }
}
