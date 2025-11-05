using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

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
        foreach (Domain.Entities.Cmmn.Data.Provider.IDataProvider dataProvider in DataSourceProviderManager.ProviderContainer)
        {
            dataProvider.Dispose();
        }
    }
}
