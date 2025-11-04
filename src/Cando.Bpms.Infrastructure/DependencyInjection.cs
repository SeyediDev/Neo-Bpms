using Ardalis.GuardClauses;
using Neo.Bpms.Domain.Repository;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Bpms.Infrastructure.Data.Repository.Bpms;
using Neo.Bpms.UI.MVC.ControllersMethods.PostForms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers;
using Neo.Bpms.Infrastructure.Features.Cmmn.ScheduledReports;
using Neo.Common.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaProcess;
using Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;
using Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;
using Neo.Bpms.Infrastructure.Features.MetaLoader;
namespace Neo.Bpms.Infrastructure;

public static class DependencyInjection
{
    public static void AddCandoBpmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        
        _ = services.AddScoped<ISsoIntegratorParams, ExternalLoginIntegratorParams>();
        _ = services.AddScoped<ISsoIntegrator, SsoIntegrator>();
        _ = services.AddScoped<IAccessServices, CandoAccessServices>();
        _ = services.AddScoped<ISendFormCommand, SendFormCommand>();

        _ = services.AddSingleton<IBpmsEngine, BpmsEngine>();
        _ = services.AddSingleton<MigrationSingleton>();
        _ = services.AddSingleton<IBpmsRepository, Repository>();
        _ = services.AddTransient<ILogContainer<LogContainer>, LogContainer>();
        _ = services.AddExpressionService();

        _ = services.AddSingleton<IProjectBpmn, ProjectBpmn>();
        _ = services.AddSingleton<ISpecificEntitiesLoader, SpecificEntitiesLoader>();

        _ = services.AddSingleton<IMetaDataLoader, MetaDataLoader>();
        _ = services.AddSingleton<IProjectProcess, ProjectProcess>();
        _ = services.AddSingleton<IProjectNamespace, ProjectNamespace>();
        _ = services.AddSingleton<IProjectEntity, ProjectEntity>();
        _ = services.AddSingleton<IProjectEnum, ProjectEnum>();
        _ = services.AddSingleton<IProjectMenu, ProjectMenu>();
        _ = services.AddSingleton(typeof(IEntityLoader<>), typeof(EntityLoader<>));
        
        _ = services.AddScoped<IBpmsSubjectSettingRepository, BpmsSubjectSettingRepository>();

        _ = services.AddScoped<DashboardConfigBackupRestore>();
        _ = services.AddScoped<ReportConfigBackupRestore>();
        _ = services.AddScoped<FolderConfigBackupRestore>();
        _ = services.AddScoped<FilterConfigBackupRestore>();
        _ = services.AddScoped<ScheduledReportConfigBackupRestore>();


        AddFeaturesServices(services);
        
        AddRepositories(services, configuration);
    }

    private static void AddFeaturesServices(IServiceCollection services)
    {
        _ = services.AddScoped<DashboardConfigManager>();
        _ = services.AddScoped<ReportConfigManager>();
        _ = services.AddScoped<DashboardDataRoutines>();
        _ = services.AddScoped<DashboardStructRoutines>();
        _ = services.AddScoped<ReportDataRoutines>();
        _ = services.AddScoped<ControllerMethods>();
        _ = services.AddScoped<FormStructRoutines>();
        _ = services.AddScoped<IEnrichFieldsSbvr, EnrichFieldsSbvr>();
        _ = services.AddScoped<ReportStructRoutines>();
        _ = services.AddScoped<ScheduledReportLoader>();
        _ = services.AddScoped<WorkItemManager>();
        _ = services.AddScoped<ReportFilterName>();
        _ = services.AddScoped<FormLayout>();
        _ = services.AddScoped<FormDataRoutines>();
        _ = services.AddScoped<SubReportData>();
        _ = services.AddScoped<TakingScheduledReport>();

        _ = services.AddScoped<IPostForm, PostForm>();
        _ = services.AddScoped<IApplyFormDocuments, ApplyFormDocuments>();
        _ = services.AddScoped<IApplyFormData, ApplyFormData>();
        _ = services.AddScoped<IApplyFormField, ApplyFormField>();
        _ = services.AddScoped<IJwtDecode, JwtDecode>();

        _ = services.AddScoped<IFormExcelImporter, FormExcelImporter>();
    }

    public static void UseCandoBpms(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        DependencyInjectionHolder.Instance.Configuration = scope.Inject<IConfiguration>();
        DependencyInjectionHolder.Instance.Logger = scope.Inject<ILogContainer<LogContainer>>().Logger;
        DependencyInjectionHolder.Instance.AccessServices = scope.Inject<IAccessServices>();
        DependencyInjectionHolder.Instance.BpmsEngine = scope.Inject<IBpmsEngine>();
        DependencyInjectionHolder.Instance.BuiltInFunctionFinder = scope.Inject<IBuiltInFunctionFinder>();

        DependencyInjectionHolder.Instance.BuiltInFunctionFinder
            .AddAssembly(typeof(Util.Expressions.FunctionImplementations.BuiltInFunctions).Assembly);
        DependencyInjectionHolder.Instance.BuiltInFunctionFinder
            .AddAssembly(typeof(Features.Orm.EntityExpressionFuncs.BuiltInFunctions).Assembly);
    }

    public static TService Inject<TService>(this IServiceScope scope)
    {
        TService service = scope.ServiceProvider.GetRequiredService<TService>();
        return service;
    }
    
    private static void AddRepositories(IServiceCollection services, IConfiguration configuration)
    {
        var commandConnectionString = configuration.GetConnectionString($"{nameof(DomainProvider.Domain)}CommandConnection");
        Guard.Against.Null(commandConnectionString, message: $"Connection string '{nameof(DomainProvider.Domain)}CommandConnection' not found.");
        services.AddDbContext<BpmsContextCommand>((serviceProvider, options) =>
        {
            options.UseSqlServer(commandConnectionString);
            options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
        }, ServiceLifetime.Scoped);
        services.AddScoped<IBpmsUnitOfWorkCommand>(serviceProvider => serviceProvider.GetRequiredService<BpmsContextCommand>());

        var queryConnectionString = configuration.GetConnectionString($"{nameof(DomainProvider.Domain)}QueryConnection");
        Guard.Against.Null(queryConnectionString, message: $"Connection string '{nameof(DomainProvider.Domain)}QueryConnection' not found.");
        services.AddDbContextPool<BpmsContextQuery>(options => options.UseSqlServer(queryConnectionString)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        services.AddScoped<IBpmsUnitOfWorkQuery>(serviceProvider => serviceProvider.GetRequiredService<BpmsContextQuery>());

        services.AddScoped(typeof(IBpmsCommandRepository<,>), typeof(BpmsEntityRepositoryCommand<,>));
        services.AddScoped(typeof(IBpmsQueryRepository<,>), typeof(BpmsEntityRepositoryQuery<,>));

        //services.AddScoped<ICultureTermQueryRepository, CultureTermQueryRepository>();
    }
}
