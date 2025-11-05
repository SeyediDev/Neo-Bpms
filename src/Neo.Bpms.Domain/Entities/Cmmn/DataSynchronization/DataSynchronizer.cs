namespace Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;

public class DataSynchronizer
{
    public ConcurrentBag<IDataSynchronizerItem> Items { get; set; }

    public DataSynchronizer()
    {
        Items = [];
    }


    public void Add(IDataSynchronizerItem driver)
    {
        Items.Add(driver);
    }
}