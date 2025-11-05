using Neo.Bpms.Domain.Models.Bpmn.Core.Services;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Tasks;

public interface IOperationContainer
{
    Operation operationRef { get; set; }
}
