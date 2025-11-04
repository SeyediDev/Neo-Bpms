using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.TestEngine;

public class CatchTestSendTaskRuntime :
    InternalServiceOperationRuntime<CatchTestSendTaskInput, CatchTestSendTaskOutput>
{
    protected override Task<CatchTestSendTaskOutput> RunOperationAsync(IAuditTrail auditHeader, CatchTestSendTaskInput input)
    {
        Task.Factory.StartNew(() =>
        {
            auditHeader.AddDetail((long)ServiceDetailTypeId.RunOperation, $"RunOperation of CatchTestSendTask. input:{input}", null, null, null, null);
            Thread.Sleep(3000);
            var messageName = input.MessageName;
            AuditTrail auditTrail = new(TriggerTypeId.SendMessage, messageName);
            LocalParameters lp = new() { { "MessageName", input.MessageName } };
            ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).MessageReceived(messageName, lp, auditTrail);
            DataStorage.SaveAudit(auditTrail);
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        return Task.FromResult(new CatchTestSendTaskOutput());
    }
}
