namespace Neo.Bpms.Domain.Models.Cmmn.Data.Transaction;

public interface IBatchDataManipulationRunner
{
    int RunCommand(string sqlCommand);
    void Release();
}