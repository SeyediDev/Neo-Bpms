using Neo.Bpms.Domain.Features.Security;
using MediatR;

namespace Neo.Bpms.UI.MVC.Controllers.Public;

public class ControllerBaseInjection : Controller
{
    private ILogger _logger;
    public ILogger Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger>();

    private ISender _sender;
    public ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    private IAccessServices _accessServices;
    protected IAccessServices AccessServices =>
        _accessServices ??= HttpContext.RequestServices.GetRequiredService<IAccessServices>();
}
