namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.Orm.ProcessEntities;

[DisplayNameAndEnName("پارامترهای ورودی عملیات ایجاد رکورد")]
[System.ComponentModel.DataAnnotations.Schema.NotMapped]
public class OperationInput_InsertRecord
{
    public string NameSpace;
    public string Entity;
    public object UpdatingFields;
}