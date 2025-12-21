using FluentAssertions;
using Moq;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.User;
using Neo.Bpms.Infrastructure.Features.SystemConfigs;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Xunit;

namespace Neo.Bpms.UI.MVC.Tests;

public class FilterManagerTests
{
    private readonly Mock<FilterConfigBackupRestore> _filterConfigBackupRestoreMock;
    private readonly FilterManager _filterManager;

    public FilterManagerTests()
    {
        _filterConfigBackupRestoreMock = new Mock<FilterConfigBackupRestore>();
        _filterManager = new FilterManager(_filterConfigBackupRestoreMock.Object);
    }

    [Fact]
    public async Task GetConfiguredFilterValues_WithFilterId_ShouldReturnFilterValues()
    {
        // Arrange
        var filterId = 1L;
        var configuredFilter = new ConfiguredFilter(filterId, "Test Filter")
        {
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "Value1" },
                new() { FieldId = "Field2", Value = "Value2" }
            }
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(filterId))
            .ReturnsAsync(configuredFilter);

        var user = new IdentityUser { Id = "user1" };
        var values = new ElasticObject();
        var configuredFilters = new List<ConfiguredFilter>();

        // Act
        (ElasticObject result, ConfiguredFilter? resultFilter) = await _filterManager.GetConfiguredFilterValues(
            filterId, configuredFilters, user, null, values);

        // Assert
        result.Should().NotBeNull();
        resultFilter.Should().NotBeNull();
        resultFilter!.Id.Should().Be(filterId);
        result.GetString("Field1").Should().Be("Value1");
        result.GetString("Field2").Should().Be("Value2");
    }

    [Fact]
    public async Task GetConfiguredFilterValues_WithFilterIdAndExistingValues_ShouldMergeValues()
    {
        // Arrange
        var filterId = 1L;
        var configuredFilter = new ConfiguredFilter(filterId, "Test Filter")
        {
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "FilterValue1" },
                new() { FieldId = "Field2", Value = "FilterValue2" }
            }
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(filterId))
            .ReturnsAsync(configuredFilter);

        var user = new IdentityUser { Id = "user1" };
        var existingValues = new ElasticObject();
        existingValues.SetField("Field3", "ExistingValue3");
        var configuredFilters = new List<ConfiguredFilter>();

        // Act
        (ElasticObject result, ConfiguredFilter? resultFilter) = await _filterManager.GetConfiguredFilterValues(
            filterId, configuredFilters, user, null, existingValues);

        // Assert
        result.Should().NotBeNull();
        result.GetString("Field1").Should().Be("FilterValue1");
        result.GetString("Field2").Should().Be("FilterValue2");
        result.GetString("Field3").Should().Be("ExistingValue3");
    }

    [Fact]
    public async Task GetConfiguredFilterValues_WithoutFilterId_ShouldReturnDefaultFilter()
    {
        // Arrange
        var defaultFilter = new ConfiguredFilter(2, "Default Filter")
        {
            IsDefault = true,
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "DefaultValue1" }
            }
        };

        var configuredFilters = new List<ConfiguredFilter> { defaultFilter };
        var user = new IdentityUser { Id = "user1" };
        var values = new ElasticObject();

        // Act
        (ElasticObject result, ConfiguredFilter? resultFilter) = await _filterManager.GetConfiguredFilterValues(
            null, configuredFilters, user, null, values);

        // Assert
        result.Should().NotBeNull();
        resultFilter.Should().NotBeNull();
        resultFilter!.IsDefault.Should().BeTrue();
        result.GetString("Field1").Should().Be("DefaultValue1");
    }

    [Fact]
    public async Task GetConfiguredFilterValues_WithPersistenceObject_ShouldUsePersistenceValues()
    {
        // Arrange
        var persistenceObject = new PersistenceObject
        {
            FilterValues = new ElasticObject()
        };
        persistenceObject.FilterValues.SetField("Field1", "PersistenceValue1");

        var user = new IdentityUser { Id = "user1" };
        var values = new ElasticObject();
        var configuredFilters = new List<ConfiguredFilter>();

        // Act
        (ElasticObject result, ConfiguredFilter? resultFilter) = await _filterManager.GetConfiguredFilterValues(
            null, configuredFilters, user, persistenceObject, values);

        // Assert
        result.Should().NotBeNull();
        result.GetString("Field1").Should().Be("PersistenceValue1");
    }

    [Fact]
    public async Task GetConfiguredFilterValues_WithMultipleValueSources_ShouldMergeCorrectly()
    {
        // Arrange
        var filterId = 1L;
        var configuredFilter = new ConfiguredFilter(filterId, "Test Filter")
        {
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "FilterValue1" },
                new() { FieldId = "Field2", Value = "FilterValue2" }
            }
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(filterId))
            .ReturnsAsync(configuredFilter);

        var persistenceObject = new PersistenceObject
        {
            FilterValues = new ElasticObject()
        };
        persistenceObject.FilterValues.SetField("Field3", "PersistenceValue3");

        var existingValues = new ElasticObject();
        existingValues.SetField("Field4", "ExistingValue4");

        var user = new IdentityUser { Id = "user1" };
        var configuredFilters = new List<ConfiguredFilter>();

        // Act
        (ElasticObject result, ConfiguredFilter? resultFilter) = await _filterManager.GetConfiguredFilterValues(
            filterId, configuredFilters, user, persistenceObject, existingValues);

        // Assert
        result.Should().NotBeNull();
        // Filter values should take precedence
        result.GetString("Field1").Should().Be("FilterValue1");
        result.GetString("Field2").Should().Be("FilterValue2");
        // Existing values should be merged
        result.GetString("Field4").Should().Be("ExistingValue4");
    }
}

