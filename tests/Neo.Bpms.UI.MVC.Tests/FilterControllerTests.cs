using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Features.Security;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms;
using Neo.Bpms.Infrastructure.Features.SystemConfigs;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Xunit;

namespace Neo.Bpms.UI.MVC.Tests;

public class FilterControllerTests
{
    private readonly Mock<ControllerMethods> _controllerMethodsMock;
    private readonly Mock<FilterConfigBackupRestore> _filterConfigBackupRestoreMock;
    private readonly Mock<FolderConfigBackupRestore> _folderConfigBackupRestoreMock;
    private readonly FilterController _filterController;

    public FilterControllerTests()
    {
        _controllerMethodsMock = new Mock<ControllerMethods>();
        _filterConfigBackupRestoreMock = new Mock<FilterConfigBackupRestore>();
        _folderConfigBackupRestoreMock = new Mock<FolderConfigBackupRestore>();
        _filterController = new FilterController(
            _controllerMethodsMock.Object,
            _filterConfigBackupRestoreMock.Object,
            _folderConfigBackupRestoreMock.Object
        );
    }

    [Fact]
    public async Task SaveFilterConfig_NewFilter_ShouldSaveAndReturnId()
    {
        // Arrange
        var filterValues = new List<ConfiguredFilterValue>
        {
            new() { FieldId = "Field1", Value = "Value1" },
            new() { FieldId = "Field2", Value = "Value2" }
        };

        var savedFilter = new ConfiguredFilter(1, "Test Filter")
        {
            Values = filterValues
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.Save(It.IsAny<ConfiguredFilter>(), It.IsAny<CancellationToken>()))
            .Callback<ConfiguredFilter, CancellationToken>((filter, ct) =>
            {
                filter.Id = 1;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _filterController.SaveFilterConfig(
            "Namespace1", "Entity1", "Form1", null, null,
            null, "Test Filter", false, null, "Config1",
            null, false, filterValues, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveFilterConfig_UpdateExistingFilter_ShouldUpdateAndReturnId()
    {
        // Arrange
        var existingFilter = new ConfiguredFilter(1, "Existing Filter")
        {
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "OldValue" }
            }
        };

        var newFilterValues = new List<ConfiguredFilterValue>
        {
            new() { FieldId = "Field1", Value = "NewValue" },
            new() { FieldId = "Field2", Value = "Value2" }
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFilter);

        _filterConfigBackupRestoreMock
            .Setup(x => x.Save(It.IsAny<ConfiguredFilter>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _filterController.SaveFilterConfig(
            "Namespace1", "Entity1", "Form1", null, null,
            1, "Updated Filter", false, null, "Config1",
            null, false, newFilterValues, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _filterConfigBackupRestoreMock.Verify(
            x => x.Save(It.Is<ConfiguredFilter>(f => f.Values.Count == 2), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveFilterValues_ValidFilterId_ShouldUpdateValues()
    {
        // Arrange
        var existingFilter = new ConfiguredFilter(1, "Test Filter")
        {
            Values = new List<ConfiguredFilterValue>
            {
                new() { FieldId = "Field1", Value = "OldValue" }
            }
        };

        var newFilterValues = new List<ConfiguredFilterValue>
        {
            new() { FieldId = "Field1", Value = "NewValue" },
            new() { FieldId = "Field2", Value = "Value2" }
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(1))
            .ReturnsAsync(existingFilter);

        _filterConfigBackupRestoreMock
            .Setup(x => x.Save(It.IsAny<ConfiguredFilter>()))
            .Returns(Task.CompletedTask);

        var model = new SaveFilterValuesModel
        {
            FilterId = 1,
            FilterValues = newFilterValues
        };

        // Act
        var result = await _filterController.SaveFilterValues(model);

        // Assert
        result.Should().NotBeNull();
        _filterConfigBackupRestoreMock.Verify(
            x => x.Save(It.Is<ConfiguredFilter>(f => f.Values.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task SaveFilterValues_InvalidFilterId_ShouldReturnError()
    {
        // Arrange
        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(999))
            .ReturnsAsync((ConfiguredFilter?)null);

        var model = new SaveFilterValuesModel
        {
            FilterId = 999,
            FilterValues = new List<ConfiguredFilterValue>()
        };

        // Act
        var result = await _filterController.SaveFilterValues(model);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteFilterConfig_ValidFilterId_ShouldDelete()
    {
        // Arrange
        var filter = new ConfiguredFilter(1, "Test Filter")
        {
            IsPublic = false
        };

        _filterConfigBackupRestoreMock
            .Setup(x => x.GetConfig(1))
            .ReturnsAsync(filter);

        _filterConfigBackupRestoreMock
            .Setup(x => x.RemoveConfig(It.IsAny<ConfiguredFilter>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _filterController.DeleteFilterConfig(1);

        // Assert
        result.Should().NotBeNull();
        _filterConfigBackupRestoreMock.Verify(
            x => x.RemoveConfig(It.IsAny<ConfiguredFilter>()),
            Times.Once);
    }
}

