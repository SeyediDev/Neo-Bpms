using System.Text.Json.Serialization;
using BotDetect.Web;
using Neo.Bpms.Domain.Features.Bpms;
using Neo.Bpms.Infrastructure;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;
using Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;
using Neo.Bpms.UI.MVC.Controls;
using Neo.Bpms.UI.MVC.Features.FormLogic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Neo.Bpms.UI.MVC;

public static class DependencyInjection
{
    public static void AddBpmsMVC(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddSingleton<FormServiceOperation>();
        _ = services.AddSingleton<ServiceOperationManager>();

        _ = services.AddScoped<IControlsRenderer, ControlsRenderer>();
        _ = services.AddScoped<FormDesignHelper>();
        _ = services.AddScoped<FilterManager>();
        _ = services.AddScoped<IApplicationSetup, ApplicationSetup>();
        _ = services.AddScoped<IFormLogicHelper, FormLogicHelper>();
        _ = services.AddScoped<ISBVRRenderer, SBVRRenderer>();
        
        // User State Persistence Services
        _ = services.AddScoped<IUserStatePersistence, CacheUserStatePersistence>();
        
        // Configure ServiceProviderAccessor for static access
        services.AddSingleton(provider => provider);

        //todo It's temporary 
        _ = services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
        _ = services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);

        _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        _ = services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic));

        _ = services.AddAntiforgery(options =>
        {
            options.FormFieldName = "__RequestVerificationToken";
            options.HeaderName = "__RequestVerificationToken";
            options.Cookie.Name = "TetaAntiforgery";
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        });

        _ = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "CandoCookies";
                options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api") ? JwtBearerDefaults.AuthenticationScheme : null;
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = "returnUrl";
                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = CookieValidator.InvalidateCookie,
                    OnValidatePrincipal = CookieValidator.ValidateCookie,
                    OnSignedIn = CookieValidator.OnSignedIn,
                    OnRedirectToLogin = CookieValidator.OnRedirectToLogin,
                    OnRedirectToReturnUrl = CookieValidator.OnRedirectToReturnUrl,
                    OnSigningIn = CookieValidator.OnSigningIn,
                };
                
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration["Tokens:Issuer"],
                    ValidAudience = configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            _ = services.AddControllersWithViews(options =>
            {
                options.MaxModelBindingCollectionSize = int.MaxValue;
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddNewtonsoftJson(options => options.UseMemberCasing())
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        _ = services.AddResponseCaching();
    }

    public static void UseBpmsMVC(this IApplicationBuilder app, IConfiguration configuration,
        BpmsMVCConfigurationOptions options = null)
    {
        options ??= new BpmsMVCConfigurationOptions();
        
        _ = app.UseXXssProtection(xssProtectionOptions => xssProtectionOptions.EnabledWithBlockMode());
        _ = app.UseXContentTypeOptions();
        _ = app.UseXfo(xfoOptions => xfoOptions.SameOrigin());
        _ = app.UseCsp(cspOptions => cspOptions
                .DefaultSources(s => s.Self())
                .StyleSources(s => s.Self()
                    .UnsafeInline()
                )
                .ScriptSources(s => s.Self()
                    .UnsafeInline()
                    .UnsafeEval()
                    .CustomSources("blob:")
                )
                .FontSources(s => s.Self().CustomSources("data:"))
                .ImageSources(s => s.Self().CustomSources("data:")));
        AddPermissionPolicyHeaderMiddleware(app);

        _ = app.UseRouting();
        _ = app.UseStaticFiles();

        //app.UseMiddleware<ErrorHandlerMiddleware>();

        if (options.UseCaptcha)
        {
            // TODO: Captcha package is old (.NET Framework) and incompatible with .NET 8
            // Replace with a modern Captcha solution or update the package
            //_ = app.UseCaptcha(configuration);
        }
        _ = app.UseAuthentication();
        _ = app.UseResponseCaching();
        app.UseCandoBpms();

        _ = app.UseEndpoints(endpoints =>
        {
            CommonRoutes.AddCommonRoutes(endpoints);
            options.AddRoutesFunction?.Invoke(endpoints);
        });

        // Configure ServiceProviderAccessor for static access
        ServiceProviderAccessor.ServiceProvider = app.ApplicationServices;
        
        app.Inject<IBuiltInFunctionFinder>().AddAssembly(typeof(UserBuiltInFunctions).Assembly);
        app.Inject<IApplicationSetup>().Setup(app);
        Task.Run(() =>
        {
            app.Inject<IProjectMetaLoader>().Load(true, true, true, false);
            app.Inject<IBpmsEngine>().Load();
        });
        //MigrationToDatabase();
    }

    public static TService Inject<TService>(this IApplicationBuilder applicationBuilder)
    {
        using IServiceScope scope = applicationBuilder.ApplicationServices.CreateScope();
        TService service = scope.ServiceProvider.GetRequiredService<TService>();
        return service;
    }

    private static void AddPermissionPolicyHeaderMiddleware(IApplicationBuilder app)
    {
        _ = app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("Permissions-Policy",
                "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
            await next();
        });
    }
    //private static void MigrationToDatabase(bool syncMetaData = false)
    //{
    //    MigrationSingleton migrationManager = MigrationSingleton.Instance;
    //    migrationManager.StartMigration(new MigrationOptions
    //    {
    //        DoDropUndefinedTables = false,
    //        DropUndefinedViews = false,
    //        DropExtraIndex = false,
    //        RebuildIndex = false,
    //        DropForeignKeyIndex = false,
    //        DropUndefinedField = false,
    //        RenameUndefinedViews = false,
    //        SyncFileGroups = false,
    //        DoRenameUndefinedTables = false,
    //        SyncMetaData = syncMetaData
    //    });
    //}
}
public record BpmsMVCConfigurationOptions
{
    public bool UseHttps { get; set; } = true;
    public bool UseCaptcha { get; set; } = true;
    public Action<IEndpointRouteBuilder> AddRoutesFunction { get; set; }
}
