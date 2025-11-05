namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

[DisplayNameAndEnName("پارامترهای ورودی عملیات اجرای تست")]
[System.ComponentModel.DataAnnotations.Schema.NotMapped]
public class RunTestOperationInput
{
    public bool GenerateException;
    public string ErrorCode;
}
