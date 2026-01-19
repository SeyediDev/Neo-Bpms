using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices
{
    public enum ResourceStates
    {
        Idle,
        Working
    }

    public class ServiceResource(FunctionInMachineDefinition func, int index) : OperationResourceRuntime
    {
        public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
        private volatile ResourceStates _state = ResourceStates.Idle;

        public FunctionInMachineDefinition Function { get; } = func;
        public int ResourceIndex { get; } = index;

        public ResourceStates State => _state;

        public long CurrentRequestId { get; private set; }
        public IOperationUserParams UserParams { get; set; }

        private Action<LocalParameters, IOperationUserParams, bool> Done { get; set; }
        private Action<string, LocalParameters, IOperationUserParams, bool> Fail { get; set; }
        private Action<IOperationUserParams> _takeBackToQueue;
        public string Detail { get; private set; }
        private DateTime LastNotifiedTime { get; set; }

        private TimeSpan TimeoutCriteria
        {
            get
            {
                var oneMinute = TimeSpan.FromMinutes(1);
                var functionTimeout = TimeSpan.FromMilliseconds(Function.RefreshTime);
                return functionTimeout > oneMinute ? functionTimeout : oneMinute;
            }
        }

        public override bool IsWorking()
        {
            return _state == ResourceStates.Working;
        }

        public override string GetMachineId()
        {
            return Function?.Machine?.MachineId;
        }

        public void AssignWork(long currentRequestId,
            Action<LocalParameters, IOperationUserParams, bool> done,
            Action<string, LocalParameters, IOperationUserParams, bool> fail,
            Action<IOperationUserParams> takeBackToQueue, IOperationUserParams userParams)
        {
            if (_state == ResourceStates.Working)
                Logger.LogCritical("Assigning work to an already working resource! {0}",
                    Function.FuncName); //todo What else should I do? Exception?
            Logger.LogTrace("AssignWork currentRequestId {0} parameter {1}", currentRequestId, userParams?.ToString());

            _state = ResourceStates.Working;
            CurrentRequestId = currentRequestId;
            Done = done;
            Fail = fail;
            _takeBackToQueue = takeBackToQueue;
            UserParams = userParams;
            LastNotifiedTime = DateTime.UtcNow;
        }

        public void FinishWork(LocalParameters output, bool isImmediate)
        {
            Logger.LogTrace("Finish CurrentRequestId {0} {1} immediate {2}", CurrentRequestId, UserParams?.ToString(),
                isImmediate);
            if (Done == null)
            {
                Logger.LogCritical("Finish CurrentRequestId {0} {1} immediate {2} output {3}",
                    CurrentRequestId, UserParams?.ToString(), isImmediate, output);
                throw new InvalidOperationException("Can not finish a work when the Done action is null.");
            }

            Done(output, UserParams, isImmediate);
        }

        public void FailWork(string errorCode, LocalParameters output, bool isImmediate)
        {
            Logger.LogTrace("Fail CurrentRequestId {0} {1} immediate {2}", CurrentRequestId, UserParams?.ToString(),
                isImmediate);
            if (Fail == null)
            {
                Logger.LogCritical("Fail CurrentRequestId {0} Operation {1} immediate {2} output {3} code {4}",
                    CurrentRequestId, UserParams, isImmediate, output, errorCode);
                throw new InvalidOperationException("Can not fail a work when the Fail action is null.");
            }

            Fail(errorCode, output, UserParams, isImmediate);
        }

        public void UpdateProgress(int progressValue, string detail)
        {
            if (progressValue < Progress) return;
            Progress = progressValue;
            Detail = detail;
            LastNotifiedTime = DateTime.UtcNow;
        }

        public void QueueIfTimedOut()
        {
            if (DateTime.UtcNow.Subtract(LastNotifiedTime) > TimeoutCriteria)
            {
                Logger.LogInformation("Timeout {0} {1} last:{2} now:{3} criteria:{4}", Function.FuncName, CurrentRequestId,
                    LastNotifiedTime, DateTime.UtcNow, TimeoutCriteria);
                //                FailWork(new LocalParameters(), null, false);
                TakeBackToQueue(dontRun: false); //todo IMPORTANT! number of tries
            }
        }

        public void TakeBackToQueue(bool dontRun = true)
        {
            Logger.LogTrace("Taking Back To Queue CurrentRequestId {0} {1}", CurrentRequestId, UserParams?.ToString());
            _takeBackToQueue(UserParams);
        }

        public override void Free()
        {
            Logger.LogTrace("Free {0} {1}", CurrentRequestId, UserParams?.ToString());
            _state = ResourceStates.Idle;
            CurrentRequestId = 0; //May need change based on ServiceManager's way of generating request ids
                                  //make other fields null?
            Done = null;
            UserParams = null;
            Progress = 0;
            Detail = null;
            //	        LastNotifiedTime = null;	        
        }

        public void HadUnsuccessfulConnection()
        {
            Function.Machine.HadUnsuccessfulConnection();
        }
    }
}
