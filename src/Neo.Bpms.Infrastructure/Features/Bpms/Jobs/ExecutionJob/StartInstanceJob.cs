namespace Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;

internal class StartInstanceJob : ExecutionJob
{
    public ActivityInstance ActivityInstance { get; set; }
    public LocalParameters InputData { get; set; }

    internal override void Execute()
    {
        ActivityInstance.ReBorn();
        ActivityInstance.ActivityRuntime
            .CheckDataInputAvailabilityAndStartIfNeeded(ActivityInstance, false, InputData);
    }
}
