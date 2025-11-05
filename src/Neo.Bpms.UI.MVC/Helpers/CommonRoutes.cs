using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Neo.Bpms.UI.MVC.Controllers;

namespace Neo.Bpms.UI.MVC.Helpers;

public class CommonRoutes
{
    public static void AddCommonRoutes(IEndpointRouteBuilder endpoints)
    {
        //It can get removed if client rest calls don't start with api.
        endpoints.MapControllerRoute(
            name: "MetaDesignApi",
            pattern: "MetaDesign/App/Api/{action}/{id?}",
            defaults: new { controller = "MetaDesign" }
        );

        endpoints.MapControllerRoute(
            name: "MetaDesign",
            pattern: "MetaDesign/App/{*clientRoute}",
            defaults: new { controller = "MetaDesign", action = "Index" }
        );

        endpoints.MapControllerRoute(
            name: "CustomPage",
            pattern: "Page/{NamespaceId}/{EntityId}/{FormId}",
            defaults: new
            {
                controller = "Form",
                action = nameof(FormController.CustomPage)
            }
        );

        endpoints.MapControllerRoute(
            name: "BpmnApi",
            pattern: "Api/Bpmn/Message/Catch",
            defaults: new
            {
                controller = "Message",
                action = nameof(MessageController.Catch)
            }
        );

        endpoints.MapControllerRoute(
            name: "DynamicApi",
            pattern: "Api/{NamespaceId}/{EntityId}/{Id?}",
            defaults: new
            {
                controller = "Form",
                action = nameof(FormController.Api)
            }
        );

        endpoints.MapControllerRoute(
            name: "Desktop",
            pattern: "Desktop/{desktopId?}",
            defaults: new
            {
                controller = "Desktop",
                action = nameof(DesktopController.Index)
            }
        );

        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

    }
}
