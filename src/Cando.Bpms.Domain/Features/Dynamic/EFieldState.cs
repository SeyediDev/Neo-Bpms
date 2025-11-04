namespace Neo.Bpms.Domain.Features.Dynamic;

public enum EFieldState
{
    NotInitialized,
    Invalidated,
    //InvalidatedChanged,
    Initialized,
    Changed,
    ChangeReported,
}
