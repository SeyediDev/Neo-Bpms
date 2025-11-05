using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Neo.Bpms.UI.MVC.Attributes;

public class DynamicActionGetBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ElasticObject temp = new();
        foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> parameter in bindingContext.HttpContext.Request.Query)
        {
            try
            {
                if (parameter.Key == "__RequestVerificationToken")
                    continue;
                string s = parameter.Value.ToString();
                if (Check.CheckParams(s))
                {
                    temp.SetField(parameter.Key, s);
                }
                else
                {
                    throw new HttpException(
                        "داده وارد شده از طرف کاربر خطر ناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
                }
            }
            catch (Exception)
            {
                throw new HttpException(
                    "داده وارد شده از طرف کاربر خطر ناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
            }
        }
        bindingContext.Result = ModelBindingResult.Success(temp);
        return Task.CompletedTask;
    }
}
