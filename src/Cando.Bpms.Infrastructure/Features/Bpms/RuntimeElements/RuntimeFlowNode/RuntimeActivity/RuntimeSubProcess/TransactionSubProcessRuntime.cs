using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeSubProcess;

public class TransactionSubProcessRuntime(ProcessVersionRuntime processVersion, 
    TransactionSubProcess subProcess) : SubProcessRuntime(processVersion, subProcess)
{
    public TransactionSubProcess transactionSubProcessDefinition = subProcess;
    //internal override void StartActivity(AuditTrail auditTrail, ProcessInstance pi, ActivityInstance ai, LocalParameters inputData)
    //{
    //	base.StartActivity(auditTrail, pi, ai, inputData);
    //}
}
