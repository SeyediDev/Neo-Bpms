using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports;
using static Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ConfiguredDashboard;

namespace Neo.Bpms.Infrastructure.Tests.Features.Cmmn.Dashboards;

public class DashboardDataRoutinesCacheTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IConfiguration> _mockConfiguration;
    
    public DashboardDataRoutinesCacheTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Xunit.Fact]
    public void GetWidgetCacheTimeMinutes_ShouldReturnDefaultValue_WhenNoPropertySet()
    {
        // Arrange
        var widget = new ConfigWidget
        {
            Id = "TestWidget",
            ReportNamespaceId = "TestNamespace",
            ReportEntityId = "TestEntity",
            ReportId = "TestReport",
            ReportConfigId = "TestConfig"
        };
        
        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act
        var result = GetPrivateMethodResult<int>(dashboardDataRoutines, "GetWidgetCacheTimeMinutes", widget);

        // Assert
        result.Should().Be(10); // پیش‌فرض 10 دقیقه
    }
    
    private DashboardDataRoutines CreateDashboardDataRoutines()
    {
        var mockReportDataRoutines = new Mock<ReportDataRoutines>(MockBehavior.Loose);
        var mockLogger = new Mock<ILogger<SlowQueryLogger>>();
        var slowQueryLogger = new SlowQueryLogger(mockLogger.Object);
        
        return new DashboardDataRoutines(
            mockReportDataRoutines.Object,
            slowQueryLogger,
            _mockConfiguration.Object,
            _memoryCache);
    }

    [Xunit.Fact]
    public void GetWidgetCacheTimeMinutes_ShouldReturnPropertyValue_WhenPropertySet()
    {
        // Arrange
        var widget = new ConfigWidget
        {
            Id = "TestWidget",
            ReportNamespaceId = "TestNamespace",
            ReportEntityId = "TestEntity",
            ReportId = "TestReport",
            ReportConfigId = "TestConfig"
        };
        widget.AddProperty(eControlPropertyId.WidgetCacheTimeMinutes, "15");
        
        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act
        var result = GetPrivateMethodResult<int>(dashboardDataRoutines, "GetWidgetCacheTimeMinutes", widget);

        // Assert
        result.Should().Be(15);
    }

    [Xunit.Fact]
    public void GetWidgetCacheTimeMinutes_ShouldReturnAppSettingsValue_WhenPropertyNotSet()
    {
        // Arrange
        var widget = new ConfigWidget
        {
            Id = "TestWidget",
            ReportNamespaceId = "TestNamespace",
            ReportEntityId = "TestEntity",
            ReportId = "TestReport",
            ReportConfigId = "TestConfig"
        };

        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(x => x.Value).Returns("20");
        _mockConfiguration.Setup(x => x.GetSection("Dashboard:WidgetCacheTimeMinutes"))
            .Returns(configurationSection.Object);
        
        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act
        var result = GetPrivateMethodResult<int>(dashboardDataRoutines, "GetWidgetCacheTimeMinutes", widget);

        // Assert
        result.Should().Be(20);
    }

    [Xunit.Fact]
    public void InvalidateWidgetCache_ShouldRemoveCacheEntry_WhenCalled()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var filterValues = new ElasticObject();
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };
        var maxRecord = 10;
        var culture = "fa";
        var forPrint = false;

        // Set cache first
        var cacheKey = GenerateCacheKey(widget, reportConfig, filterValues, user, maxRecord, culture, forPrint);
        _memoryCache.Set(cacheKey, new ReportData(reportConfig, null, filterValues, false));

        // Verify cache exists
        _memoryCache.TryGetValue(cacheKey, out _).Should().BeTrue();

        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act
        dashboardDataRoutines.InvalidateWidgetCache(widget, reportConfig, filterValues, user, maxRecord, culture, forPrint);

        // Assert
        _memoryCache.TryGetValue(cacheKey, out _).Should().BeFalse();
    }

    [Xunit.Fact]
    public void InvalidateWidgetCache_ShouldNotThrow_WhenWidgetIsNull()
    {
        // Arrange
        var reportConfig = CreateTestReportConfig();
        var filterValues = new ElasticObject();
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };

        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act & Assert
        var act = () => dashboardDataRoutines.InvalidateWidgetCache(
            null!, reportConfig, filterValues, user, 10, "fa", false);
        act.Should().NotThrow();
    }

    [Xunit.Fact]
    public void InvalidateWidgetCache_ShouldNotThrow_WhenReportConfigIsNull()
    {
        // Arrange
        var widget = CreateTestWidget();
        var filterValues = new ElasticObject();
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };

        var dashboardDataRoutines = CreateDashboardDataRoutines();

        // Act & Assert
        var act = () => dashboardDataRoutines.InvalidateWidgetCache(
            widget, null!, filterValues, user, 10, "fa", false);
        act.Should().NotThrow();
    }

    [Xunit.Fact]
    public void GenerateCacheKey_ShouldIncludeAllParameters()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var filterValues = new ElasticObject();
        filterValues.SetField("TestField", "TestValue");
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };
        var maxRecord = 15;
        var culture = "en";
        var forPrint = true;

        // Act
        var cacheKey = GenerateCacheKey(widget, reportConfig, filterValues, user, maxRecord, culture, forPrint);

        // Assert
        cacheKey.Should().Contain("DashboardWidget");
        cacheKey.Should().Contain(widget.Id);
        cacheKey.Should().Contain(reportConfig.ConfigId);
        cacheKey.Should().Contain(widget.ReportNamespaceId);
        cacheKey.Should().Contain(widget.ReportEntityId);
        cacheKey.Should().Contain(widget.ReportId);
        cacheKey.Should().Contain(maxRecord.ToString());
        cacheKey.Should().Contain(culture);
        cacheKey.Should().Contain(forPrint.ToString());
        cacheKey.Should().Contain(user.Id);
        cacheKey.Should().Contain("Filters:");
    }

    [Xunit.Fact]
    public void GenerateCacheKey_ShouldGenerateDifferentKeys_ForDifferentUsers()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var filterValues = new ElasticObject();
        var user1 = new IdentityUser { Id = "user1", UserName = "testuser1" };
        var user2 = new IdentityUser { Id = "user2", UserName = "testuser2" };

        // Act
        var key1 = GenerateCacheKey(widget, reportConfig, filterValues, user1, 10, "fa", false);
        var key2 = GenerateCacheKey(widget, reportConfig, filterValues, user2, 10, "fa", false);

        // Assert
        key1.Should().NotBe(key2);
    }

    [Xunit.Fact]
    public void GenerateCacheKey_ShouldGenerateDifferentKeys_ForDifferentFilters()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var filterValues1 = new ElasticObject();
        filterValues1.SetField("Field1", "Value1");
        var filterValues2 = new ElasticObject();
        filterValues2.SetField("Field2", "Value2");
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };

        // Act
        var key1 = GenerateCacheKey(widget, reportConfig, filterValues1, user, 10, "fa", false);
        var key2 = GenerateCacheKey(widget, reportConfig, filterValues2, user, 10, "fa", false);

        // Assert
        key1.Should().NotBe(key2);
    }

    [Xunit.Fact]
    public void GenerateCacheKey_ShouldNotIncludeFilters_WhenFilterValuesIsNull()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };

        // Act
        var cacheKey = GenerateCacheKey(widget, reportConfig, null, user, 10, "fa", false);

        // Assert
        cacheKey.Should().NotContain("Filters:");
    }

    [Xunit.Fact]
    public void GenerateCacheKey_ShouldNotIncludeFilters_WhenFilterValuesIsEmpty()
    {
        // Arrange
        var widget = CreateTestWidget();
        var reportConfig = CreateTestReportConfig();
        var filterValues = new ElasticObject();
        var user = new IdentityUser { Id = "user1", UserName = "testuser" };

        // Act
        var cacheKey = GenerateCacheKey(widget, reportConfig, filterValues, user, 10, "fa", false);

        // Assert
        cacheKey.Should().NotContain("Filters:");
    }

    // Helper methods
    private ConfigWidget CreateTestWidget()
    {
        return new ConfigWidget
        {
            Id = "TestWidget",
            ReportNamespaceId = "TestNamespace",
            ReportEntityId = "TestEntity",
            ReportId = "TestReport",
            ReportConfigId = "TestConfig"
        };
    }

    private ConfiguredReport CreateTestReportConfig()
    {
        return new ConfiguredReport
        {
            ConfigId = "TestConfigId"
        };
    }

    private string GenerateCacheKey(
        ConfigWidget widget,
        ConfiguredReport reportConfig,
        ElasticObject? filterValues,
        IdentityUser user,
        int maxRecord,
        string culture,
        bool forPrint)
    {
        // Use reflection to call private method
        var method = typeof(DashboardDataRoutines).GetMethod(
            "GenerateCacheKey",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        
        return (string)method!.Invoke(null,
        [
            widget, reportConfig, filterValues!, user, maxRecord, culture, forPrint 
        ])!;
    }

    private T GetPrivateMethodResult<T>(DashboardDataRoutines instance, string methodName, params object[] parameters)
    {
        var method = typeof(DashboardDataRoutines).GetMethod(
            methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        return (T)method!.Invoke(instance, parameters)!;
    }
}

