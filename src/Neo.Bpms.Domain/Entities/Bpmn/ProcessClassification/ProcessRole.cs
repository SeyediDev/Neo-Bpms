namespace Neo.Bpms.Domain.Entities.Bpmn.ProcessClassification;

public class ProcessRole(BusinessProcess businessProcess, string Id, string name) : BaseModelClass(businessProcess, Id, name)
{

    //		public List<SubProcessRole> subRoles = null;
    public void SetSubRoles(BaseModelClass subRoleEntity, string filter, int nameFieldId, int enNameFieldId)
    {
        this.subRoleEntity = subRoleEntity;
        subRoleEntityFilter = filter;
        subRoleNameFieldId = nameFieldId;
        subRoleEnNameFieldId = enNameFieldId;
    }
    public BaseModelClass subRoleEntity;
    public string subRoleEntityFilter;
    public int subRoleNameFieldId;
    public int subRoleEnNameFieldId;
    public BusinessProcess businessProcess = businessProcess;
}
