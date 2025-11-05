using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;

public interface IOperationContainer
{
    Operation operationRef { get; set; }
}
