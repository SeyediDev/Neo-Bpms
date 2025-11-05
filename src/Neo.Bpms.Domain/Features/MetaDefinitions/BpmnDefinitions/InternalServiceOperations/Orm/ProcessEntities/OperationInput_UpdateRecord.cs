namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

[DisplayNameAndEnName("پارامترهای ورودی عملیات اصلاح رکورد(ها)")]
[System.ComponentModel.DataAnnotations.Schema.NotMapped]
public class OperationInput_UpdateRecord
{
    public string NameSpace;
    public string Entity;
    public string WhereClause;
    public object UpdatingFields;
}