using Neo.Bpms.Domain.Entities.Bpmn.Iso8601;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.JobScheduler;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public class StartEventTimerEngine(TimerCatchRuntime timerCatchRuntime) 
    : JobTimer
{
    protected override async Task<(bool, ElasticObject)> Take(DateTime dt, Job job)
    {
        (bool, ElasticObject) ret = (false, null);
        lock (LockName())
        {
            ProcessInstance pi = CatchingTimer.TimerReceived(timerCatchRuntime);
            if (pi != null)
            {
                ElasticObject logRecord = new()
                {
                    [nameof(TimerEventLog.BPMNFlowNodeId)] = job.JobSchedule.Id,
                    [nameof(TimerEventLog.Date)] = dt,
                    [nameof(TimerEventLog.ProcessInstanceId)] = pi.Id,
                    [nameof(TimerEventLog.Successful)] = true
                };

                ret = (true, logRecord);
            }
        }
        await Task.CompletedTask;
        return ret;
    }

    protected override void RecordLog(Job job, ElasticObject logRecord)
    {
        lock (LockName())
        {
            _ = ApplyUtility<TimerEventLog>.New().Insert(logRecord);
        }
    }

    protected override DateTime? FetchLatestLogDate(Job job, out long repeatCount)
    {
        lock (LockName())
        {
            repeatCount = 0;
            var record = QueryUtility<TimerEventLog>.Where($"{nameof(TimerEventLog.BPMNFlowNodeId)}=={job.JobSchedule.Id}")
                .GroupBy(nameof(TimerEventLog.BPMNFlowNodeId), eAggregationFunctions.GroupByItem)
                .Max(nameof(TimerEventLog.Date), "Date")
                .Count("*", "repeatCount")
                .FirstOrDefault();
            if (record != null)
            {
                repeatCount = record.GetLong("repeatCount");
            }

            return record?.GetDateTime("Date");
        }
    }

    protected override async Task<IEnumerable<Job>> FetchSchedulesList(DateTime currentDate, CancellationToken ancellationToken)
    {
        await Task.CompletedTask;
        lock (LockName())
        {
            if (timerCatchRuntime.TimerEventDefinition.jobSchedule == null)
            {
                string iso8601 = timerCatchRuntime.TimerEventDefinition.timeCycle.Body();
                timerCatchRuntime.TimerEventDefinition.jobSchedule = Iso8601Interval.ParseToJobSchedule(iso8601);
                if (timerCatchRuntime.TimerEventDefinition.jobSchedule != null)
                {
                    timerCatchRuntime.TimerEventDefinition.jobSchedule.Name = iso8601;
                    timerCatchRuntime.TimerEventDefinition.jobSchedule.Id = timerCatchRuntime.DbId;
                }
            }
            return timerCatchRuntime.TimerEventDefinition.jobSchedule == null || timerCatchRuntime.TimerEventDefinition.jobSchedule.Id == 0
                ? []
                : 
                [
                    new Job
                    {
                        JobSchedule = timerCatchRuntime.TimerEventDefinition.jobSchedule
                    }
                ];
        }
    }
    private string LockName()
    {
        return string.Intern($"{timerCatchRuntime.ProcessVersion.definition.Name}.{timerCatchRuntime.flowNode.Name}");
    }
}
