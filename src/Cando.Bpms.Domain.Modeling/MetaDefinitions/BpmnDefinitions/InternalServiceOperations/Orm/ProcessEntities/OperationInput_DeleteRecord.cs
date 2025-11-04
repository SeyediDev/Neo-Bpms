namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

[DisplayNameAndEnName("پارامترهای ورودی عملیات حذف رکورد(ها)")]
[System.ComponentModel.DataAnnotations.Schema.NotMapped]
public class OperationInput_DeleteRecord
{
    public string NameSpace;
    public string Entity;
    public string WhereClause;
}
