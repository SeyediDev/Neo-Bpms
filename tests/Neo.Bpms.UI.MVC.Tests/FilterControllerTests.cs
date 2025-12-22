using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Features.Security;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Model.UI.Forms;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms;
using Neo.Bpms.Infrastructure.Features.SystemConfigs;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Neo.Bpms.UI.MVC.Tests.Helpers;
using Xunit;

#pragma warning disable CS8620 // Nullability of reference types in return type doesn't match target delegate

namespace Neo.Bpms.UI.MVC.Tests;

public class FilterControllerTests
{
    private readonly Mock<IBpmsSubjectSettingRepository> _repositoryMock;
    private readonly Mock<IAccessServices> _accessServicesMock;
    private readonly FilterController _filterController;
    private readonly IdentityUser _testUser;

    public FilterControllerTests()
    {
        _repositoryMock = new Mock<IBpmsSubjectSettingRepository>();
        _accessServicesMock = new Mock<IAccessServices>();
        _testUser = new IdentityUser { Id = "test-user", IsAdmin = true };
        
        _filterController = ControllerTestHelper.CreateFilterController(
            _repositoryMock,
            _accessServicesMock,
            _testUser);
    }

    [Fact(Skip = "Requires ProjectDefinition.Project.GetUiEntity which is static and cannot be mocked")]
    public async Task SaveFilterConfig_NewFilter_ShouldSaveAndReturnId()
    {
        // This test requires ProjectDefinition.Project.GetUiEntity which is static
        // Integration test needed for full coverage
    }

    [Fact(Skip = "Requires ProjectDefinition.Project.GetUiEntity which is static and cannot be mocked")]
    public async Task SaveFilterConfig_UpdateExistingFilter_ShouldUpdateAndReturnId()
    {
        // This test requires ProjectDefinition.Project.GetUiEntity which is static
        // Integration test needed for full coverage
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

        // Mock repository GetAsync - use As<Task<ConfiguredFilter?>>() to handle nullability
        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)existingFilter);

        // Mock repository SaveAsync
        ConfiguredFilter? savedFilter = null;
        _repositoryMock
            .Setup(x => x.SaveAsync<ConfiguredFilter>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ConfiguredFilter>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, string, ConfiguredFilter, CancellationToken>((subject, subjectId, key, config, ct) =>
            {
                savedFilter = config;
            })
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
        savedFilter.Should().NotBeNull();
        savedFilter!.Values.Should().HaveCount(2);
    }

    [Fact]
    public async Task SaveFilterValues_InvalidFilterId_ShouldReturnError()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)null);

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
        var value = jsonResult?.Value?.ToString();
        value.Should().NotBeNull().And.Contain("خطا");
    }

    [Fact]
    public async Task DeleteFilterConfig_ValidFilterId_ShouldDelete()
    {
        // Arrange
        var filter = new ConfiguredFilter(1, "Test Filter")
        {
            IsPublic = false,
            UserId = _testUser.Id
        };

        // Mock repository GetAsync - use lambda to handle nullability
        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)filter);

        // Mock repository RemoveAsync
        _repositoryMock
            .Setup(x => x.RemoveAsync(
                It.IsAny<long>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _filterController.DeleteFilterConfig(1);

        // Assert
        result.Should().NotBeNull();
        _repositoryMock.Verify(
            x => x.RemoveAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteFilterConfig_InvalidFilterId_ShouldReturnError()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)null);

        // Act
        var result = await _filterController.DeleteFilterConfig(999);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        var value = jsonResult?.Value?.ToString();
        value.Should().NotBeNull().And.Contain("خطا");
    }

    [Fact]
    public async Task DeleteFilterConfig_PublicFilterWithoutPermission_ShouldReturnError()
    {
        // Arrange
        var filter = new ConfiguredFilter(1, "Public Filter")
        {
            IsPublic = true,
            UserId = "other-user"
        };

        var nonAdminUser = new IdentityUser { Id = "non-admin", IsAdmin = false };
        var controller = ControllerTestHelper.CreateFilterController(_repositoryMock, _accessServicesMock, nonAdminUser);

        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)filter);

        // Act
        var result = await controller.DeleteFilterConfig(1);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result as JsonResult;
        jsonResult.Should().NotBeNull();
        var value = jsonResult?.Value?.ToString();
        value.Should().NotBeNull().And.Contain("خطا");
    }

    [Fact]
    public async Task ChangeParent_WithFilterId_ShouldUpdateFolderId()
    {
        // Arrange
        var filter = new ConfiguredFilter(1, "Test Filter")
        {
            FolderId = 10
        };

        _repositoryMock
            .Setup(x => x.GetAsync<ConfiguredFilter>(
                It.IsAny<long>(),
                It.IsAny<Extraction<ConfiguredFilter>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, Extraction<ConfiguredFilter> extraction, CancellationToken ct) => (ConfiguredFilter?)filter);

        ConfiguredFilter? savedFilter = null;
        _repositoryMock
            .Setup(x => x.SaveAsync<ConfiguredFilter>(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ConfiguredFilter>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, string, string, ConfiguredFilter, CancellationToken>((subject, subjectId, key, config, ct) =>
            {
                savedFilter = config;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _filterController.ChangeParent(1, null, 20);

        // Assert
        result.Should().NotBeNull();
        savedFilter.Should().NotBeNull();
        savedFilter!.FolderId.Should().Be(20);
    }
}

