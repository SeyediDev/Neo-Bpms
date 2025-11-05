using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Neo.Bpms.UI.MVC.Attributes;

public class DynamicActionBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ElasticObject temp = new();
        foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> field in bindingContext.HttpContext.Request.Form)
        {
            try
            {
                if (field.Key == "__RequestVerificationToken")
                    continue;
                string s = field.Value.ToString();
                if (Check.CheckParams(s))
                {
                    temp.SetField(field.Key, s);
                }
                else
                {
                    throw new HttpException(
                        "داده وارد شده از طرف کاربر خطر ناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
                }
            }
            catch //(Exception e)
            {
                throw new HttpException(
                    "داده وارد شده از طرف کاربر خطر ناک بوده به همین علت ادامه عملیات مجاز نمی باشد.");
            }
        }
        bindingContext.Result = ModelBindingResult.Success(temp);
        return Task.CompletedTask;
    }
}
