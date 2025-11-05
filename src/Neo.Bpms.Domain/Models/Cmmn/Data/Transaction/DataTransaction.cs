namespace Neo.Bpms.Domain.Models.Cmmn.Data.Transaction;

public class DataTransaction(IDataTransactionRunner runner)
{
    private readonly IDataTransactionRunner _runner = runner;

    //public Transaction Transaction { get; set; }

    public void BeginTransaction()
    {
        _runner.BeginTransaction();
    }

    public void EndTransaction()
    {
        _runner.EndTransaction();
    }

    public void Commit()
    {
        _runner.Commit();
    }

    public void Rollback()
    {
        _runner.Rollback();
    }


    private void Release()
    {
        _runner?.Release();
    }
}