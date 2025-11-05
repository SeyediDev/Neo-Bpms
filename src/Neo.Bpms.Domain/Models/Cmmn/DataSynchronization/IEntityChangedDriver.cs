namespace Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;
public interface IEntityChangedDriver : IEntityReference
{
    void InsertDBReport<T>(T record);
    void UpdateDBReport<T>(T record);
    void UpdateGroupDBReport(List<ExpressionNode> filters);
    void DeleteDBReport<T>(T record);
    void DeleteGroupDBReport(ExpressionNode deleteFilter);
    void ReloadDBReport();
}