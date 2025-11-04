namespace Neo.Bpms.Domain.Entities.Base;

public interface ICanBeDisable
{
    bool Disable { get; set; }
}

public interface ICheckTimeOut : ICanBeDisable
{
    DateTime? StartDate { get; set; }
    DateTime? EndDate { get; set; }
}