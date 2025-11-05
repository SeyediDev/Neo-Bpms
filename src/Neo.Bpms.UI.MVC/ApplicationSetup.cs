using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC;
internal interface IApplicationSetup
{
    void Setup(IApplicationBuilder app);
}
internal class ApplicationSetup(IHostApplicationLifetime lifetime) : IApplicationSetup
{
    public void Setup(IApplicationBuilder app)
    {
        lifetime.ApplicationStopping.Register(() => Shutdown(app));
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };
    }

    private static void Shutdown(IApplicationBuilder app)
    {
        ILogger<LogContainer> logger = app.Inject<ILogContainer<LogContainer>>().Logger;
        logger.LogTrace("Application Stopping");
        foreach (IDataProvider dataProvider in DataSourceProviderManager.ProviderContainer)
        {
            dataProvider.Dispose();
        }
    }
}
