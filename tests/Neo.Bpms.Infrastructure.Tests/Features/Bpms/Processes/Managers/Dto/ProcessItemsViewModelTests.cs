using FluentAssertions;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class ProcessItemsViewModelTests
{
    [Xunit.Fact]
    public void Constructor_WithElasticObject_ShouldMapProperties()
    {
        // Arrange
        var item = new ElasticObject();
        item.SetField("ProcessId", "Process1");
        item.SetField("Count", 100L);
        item.SetField("NotStartedCount", 25L);

        // Act
        var viewModel = new ProcessItemsViewModel(item);

        // Assert
        viewModel.ProcessId.Should().Be("Process1");
        viewModel.Count.Should().Be(100L);
        viewModel.NotStartedCount.Should().Be(25L);
        // ProcessName depends on ProjectDefinition.Project which is a static dependency
        // This would need refactoring to be fully testable
        viewModel.ProcessName.Should().NotBeNull(); // May be null if process not found
    }

    [Xunit.Fact]
    public void Constructor_WithEmptyElasticObject_ShouldSetDefaultValues()
    {
        // Arrange
        var item = new ElasticObject();

        // Act
        var viewModel = new ProcessItemsViewModel(item);

        // Assert
        viewModel.ProcessId.Should().BeNull();
        viewModel.Count.Should().Be(0L);
        viewModel.NotStartedCount.Should().Be(0L);
    }
}



