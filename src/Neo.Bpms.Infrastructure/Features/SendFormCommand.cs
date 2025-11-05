namespace Neo.Bpms.Infrastructure.Features;
public class SendFormCommand(/*IMediator mediator*/) : ISendFormCommand
{
    public async Task<(bool result, string message)> Send(ElasticObject record, Type commandType)
    {
        //CQRSRequestBase<CQRSResponseBase<bool>> request =
        //    (CQRSRequestBase<CQRSResponseBase<bool>>)record.To(commandType);
        //CQRSResponseBase<bool> result = await mediator.Send(request);
        //return (result.Result, result.Messages?.Count>0?string.Join(",", result.Messages) : "Error In SendFormCommand");
        await Task.CompletedTask;
        return (false, "Error In SendFormCommand");
    }
}
