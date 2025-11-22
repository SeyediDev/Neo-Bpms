using FluentAssertions;
using Moq;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Infrastructure.Features.Bpms.Instances;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class ProcessInstanceViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var viewModel = new ProcessInstanceViewModel();

        // Assert
        viewModel.Id.Should().Be(0);
        viewModel.ClosedTime.Should().BeNull();
        viewModel.State.Should().BeNull();
        viewModel.Entity.Should().BeNull();
        viewModel.EntityPkv.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithProcessInstance_ShouldMapProperties()
    {
        // Arrange
        var closeTime = new DateTime(2024, 1, 15, 10, 30, 0);
        var processInstance = Mock.Of<ProcessInstance>(pi =>
            pi.Id == 123L &&
            pi.CloseTime == closeTime &&
            pi.state == ProcessInstanceStateId.Completed &&
            pi.EntityId == "TestEntity" &&
            pi.EntityPkv == "PKV123");

        // Act
        var viewModel = new ProcessInstanceViewModel(processInstance);

        // Assert
        viewModel.Id.Should().Be(123L);
        viewModel.ClosedTime.Should().Be(closeTime.ToString("g"));
        viewModel.State.Should().Be(ProcessInstanceStateId.Completed.ToString());
        viewModel.Entity.Should().Be("TestEntity");
        viewModel.EntityPkv.Should().Be("PKV123");
    }

    [Fact]
    public void Constructor_WithProcessInstanceWithNullCloseTime_ShouldSetEmptyString()
    {
        // Arrange
        var processInstance = Mock.Of<ProcessInstance>(pi =>
            pi.Id == 456L &&
            pi.CloseTime == null &&
            pi.state == ProcessInstanceStateId.Active &&
            pi.EntityId == "TestEntity2" &&
            pi.EntityPkv == "PKV456");

        // Act
        var viewModel = new ProcessInstanceViewModel(processInstance);

        // Assert
        viewModel.Id.Should().Be(456L);
        viewModel.ClosedTime.Should().Be("");
        viewModel.State.Should().Be(ProcessInstanceStateId.Active.ToString());
        viewModel.Entity.Should().Be("TestEntity2");
        viewModel.EntityPkv.Should().Be("PKV456");
    }
}



