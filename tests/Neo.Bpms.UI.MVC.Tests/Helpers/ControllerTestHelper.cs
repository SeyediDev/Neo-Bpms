using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Neo.Bpms.Domain.Features.Security;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms;
using Neo.Bpms.Infrastructure.Features.SystemConfigs;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Neo.Domain.Features.Client;

namespace Neo.Bpms.UI.MVC.Tests.Helpers;

public static class ControllerTestHelper
{
    public static FilterController CreateFilterController(
        Mock<IBpmsSubjectSettingRepository>? repositoryMock = null,
        Mock<IAccessServices>? accessServicesMock = null,
        IdentityUser? user = null)
    {
        repositoryMock ??= new Mock<IBpmsSubjectSettingRepository>();
        accessServicesMock ??= new Mock<IAccessServices>();
        user ??= new IdentityUser { Id = "test-user", IsAdmin = true };

        var filterConfigBackupRestore = new FilterConfigBackupRestore(repositoryMock.Object);
        var folderConfigBackupRestore = new FolderConfigBackupRestore(repositoryMock.Object);
        var controllerMethods = new ControllerMethods(accessServicesMock.Object, folderConfigBackupRestore);

        var controller = new FilterController(controllerMethods, filterConfigBackupRestore, folderConfigBackupRestore);

        // Setup HttpContext and User
        var httpContext = new Mock<HttpContext>();
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Id)
        }, "Test");
        claimsIdentity.AddClaim(new Claim("IsAuthenticated", "true"));
        
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Mock IRequesterUser
        var requesterUserMock = new Mock<IRequesterUser>();
        requesterUserMock.Setup(x => x.GetProperty(nameof(IdentityUser))).Returns(user);

        // Mock IServiceProvider
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(x => x.GetService(typeof(IRequesterUser))).Returns(requesterUserMock.Object);

        // Mock Request and Cookies (needed for FetchCulture)
        var requestMock = new Mock<HttpRequest>();
        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(x => x["_culture"]).Returns((string?)null);
        requestMock.Setup(x => x.Cookies).Returns(cookiesMock.Object);
        httpContext.Setup(x => x.Request).Returns(requestMock.Object);

        httpContext.Setup(x => x.User).Returns(claimsPrincipal);
        httpContext.Setup(x => x.RequestServices).Returns(serviceProviderMock.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext.Object
        };

        return controller;
    }
}

