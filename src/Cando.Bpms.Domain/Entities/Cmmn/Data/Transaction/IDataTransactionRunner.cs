namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Transaction;

public interface IDataTransactionRunner
{
    void BeginTransaction();
    void EndTransaction();
    void Commit();

    void Rollback();
    void Release();
}