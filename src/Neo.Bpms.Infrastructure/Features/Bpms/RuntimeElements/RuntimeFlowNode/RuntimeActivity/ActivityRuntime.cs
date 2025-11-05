using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public abstract partial class ActivityRuntime(ProcessVersionRuntime processVersion, Activity activity) 
    : FlowNodeRunTime(processVersion, activity)
{
    public Activity Activity => flowNode as Activity;

    #region token
    internal abstract Task StartActivity(ActivityInstance ai, LocalParameters inputData);

    internal override void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null)
    {
        AuditTrace($"Receive token {Activity.ActivityType} {Activity.Id}", pi, null);
        switch (Activity.loopCharacteristics?.loopType)
        {
            case LoopCharacteristics.eLoopType.Standard:
                ReceiveTokenOnStandardLoop(pi, inputData);
                break;
            case LoopCharacteristics.eLoopType.MultiInstance:
                ReceiveTokenOnMultiInstanceLoop(pi, inputData);
                break;
            default:
                ActivityInstance ai = CreateActivityInstance(pi, 0);
                CheckDataInputAvailabilityAndStartIfNeeded(ai, true, inputData);
                break;
        }
    }

    private bool CheckStartQuantityIsValid(ActivityInstance ai)
    {
        return ai.ReceivedTokenCount + 1 >= Activity.startQuantity;
    }

    public virtual void CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance ai,
        bool fetchInputData, LocalParameters inputData)
    {
        CheckDataInputAvailabilityAndStart(ai, fetchInputData, inputData);
    }

    public void CheckDataInputAvailabilityAndStart(ActivityInstance ai, bool fetchInputData,
        LocalParameters inputData)
    {
        if (ai.state != ActivityInstanceStateId.Ready)
        {
            return;
        }

        if (!CheckStartQuantityIsValid(ai))
        {
            return;
        }

        inputData ??= new LocalParameters(ai.AuditTrail.User);
        LocalParameters data = fetchInputData ? FetchInputData(ai.pi, ai, inputData) : inputData;
        if (IsValidInput(ai, data))
        {
            ai.Activate();
            ai.Save();
            _ = StartActivity(ai, data);
        }
    }

    internal void Complete(ActivityInstance ai, LocalParameters outputData)
    {
        Complete(ai, outputData, TokenPattern.Inclusive);
    }

    protected override bool IsCompleted(FlowNodeInstance ai, LocalParameters outputData)
    {
        return IsCompletedLoop((ActivityInstance)ai, outputData);
    }

    internal override void SendTokenToOutgoing(ProcessInstance pi, LocalParameters inputData,
        TokenPattern pattern, string defaultSeqFlowId = null)
    {
        int completionQuantity = Activity.completionQuantity;
        if (completionQuantity < 1)
        {
            completionQuantity = 1;
        }

        for (int i = 0; i < completionQuantity; i++)
        {
            base.SendTokenToOutgoing(pi, inputData, pattern, defaultSeqFlowId);
        }
    }
    #endregion token

    #region data
    private bool IsValidInput(ActivityInstance ai, LocalParameters inputData)
    {
        if (Activity?.ioSpecification?.inputSets == null)
        {
            return true;
        }

        foreach (InputSet inputSet in Activity.ioSpecification.inputSets)
        {
            if (IsValid(inputSet, inputData))
            {
                ai.SelectedInputSet = inputSet;
                return true;
            }
        }

        return false;
    }
    #endregion data

    #region ActivityInstance
    protected virtual ActivityInstance CreateActivityInstance(ProcessInstance pi, long loopCounter)
    {
        ActivityInstance ai = null;
        if (Activity.startQuantity > 1)
        {
            ai = DataStorage.FetchActivityInstancesOfProcessInstance(pi, this,
                    $"IsNull(LoopCounter,0)=={loopCounter}")
                ?.FirstOrDefault();
            if (ai != null)
            {
                ai.ReceivedTokenCount++;
            }
        }

        if (ai == null)
        {
            ai = new ActivityInstance(0, this, pi);
            ai.Init(loopCounter);
        }

        ai.Save();
        ai.AddAuditDetail(BPMNAuditDetailTypeId.CreateActivityInstance, "Create Activity Instance");
        return ai;
    }

    internal override void WithdrawObsolete()
    {
        //todo check logic
        /*foreach (var pi in process.PIs.Values)
			{
				foreach (var ai in pi.FlowNodeInstances.Instances.Values)
				{
					if (ai.FlowNode.id != flowNode.id) continue;
					var instance = ai as ActivityInstance;
					if (instance != null)
						Interrupt(auditTrail, instance, InterruptType.Withdraw);
					else
						ai.Cancel();
					CheckAndClosePiAndParentAiIfNotExecuting(auditTrail, pi, ai.ParentAi);
				}
			}*/
    }
    #endregion ActivityInstance
}
