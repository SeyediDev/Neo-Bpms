namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Transaction;

public interface IBatchDataManipulationRunner
{
    int RunCommand(string sqlCommand);
    void Release();
}