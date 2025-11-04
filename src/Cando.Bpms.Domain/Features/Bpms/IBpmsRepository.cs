using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;

namespace Neo.Bpms.Domain.Features.Bpms;

public interface IBpmsRepository
{
    bool LoadBpmnDefinitions(BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion, bool forceObsolete = false);
}
