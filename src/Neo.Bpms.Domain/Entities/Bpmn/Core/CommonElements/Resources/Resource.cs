using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Resources;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Domain.Model.BPMN.Core.CommonElements;

/// <summary>
/// The Resource class is used to specify resources that can be referenced by Activities. 
/// These Resources can be Human Resources as well as any other resource assigned to Activities during Process execution time.
/// The definition of a Resource is “abstract,” because it only defines the Resource, without detailing how e.g., 
/// actual user IDs are associated at runtime. Multiple Activities can utilize the same Resource.
/// 
/// Every Resource can define a set of ResourceParameters. 
/// These parameters can be used at runtime to define query e.g., into an Organizational Directory. 
/// Every Activity referencing a parameterized Resource can bind values available in the scope of the Activity to these parameters.
/// </summary>
public partial class Resource(BpmnDefinitions parent, string id, string name) : RootElement(parent, id, name)
{
    //		public string name;

    /// <summary>
    /// Specifies the type of the query parameter.
    /// </summary>
    public List<ResourceParameter> resourceParameters = [];

    public ResourceParameter GetParameter(ResourceParameter.eType type, string parameterName)
    {
        return resourceParameters?.FirstOrDefault(p => p.type == type && p.Name == parameterName);
    }
}
