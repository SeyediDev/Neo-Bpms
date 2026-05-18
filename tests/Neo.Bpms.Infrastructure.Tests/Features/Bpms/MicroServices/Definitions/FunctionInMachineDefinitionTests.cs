using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions;
using System.Reflection;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.MicroServices.Definitions;

public class FunctionInMachineDefinitionTests
{
    private static void SetResourceState(ServiceResource resource, ResourceStates state)
    {
        var field = typeof(ServiceResource).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(resource, state);
    }
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var function = new FunctionInMachineDefinition();

        // Assert
        function.FuncName.Should().BeNull();
        function.ResourceCount.Should().Be(0);
        function.Url.Should().BeNull();
        function.RefreshTime.Should().Be(0);
        function.PollingAddress.Should().BeNull();
        function.Resources.Should().NotBeNull().And.BeEmpty();
        function.Immediate.Should().BeFalse();
        // Machine is internal, cannot test directly
    }

    [Xunit.Fact]
    public void InstantiateStateForEachResource_WithResourceCount_ShouldCreateResources()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 3
        };

        // Act
        function.InstantiateStateForEachResource();

        // Assert
        function.Resources.Should().HaveCount(3);
        function.Resources[0].ResourceIndex.Should().Be(0);
        function.Resources[1].ResourceIndex.Should().Be(1);
        function.Resources[2].ResourceIndex.Should().Be(2);
    }

    [Xunit.Fact]
    public void InstantiateStateForEachResource_WithZeroResourceCount_ShouldNotCreateResources()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 0
        };

        // Act
        function.InstantiateStateForEachResource();

        // Assert
        function.Resources.Should().BeEmpty();
    }

    [Xunit.Fact]
    public void GetResource_WithValidIndex_ShouldReturnResource()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 3
        };
        function.InstantiateStateForEachResource();

        // Act
        var resource1 = function.GetResource(1);
        var resource2 = function.GetResource(2);
        var resource3 = function.GetResource(3);

        // Assert
        resource1.Should().Be(function.Resources[0]);
        resource2.Should().Be(function.Resources[1]);
        resource3.Should().Be(function.Resources[2]);
    }

    [Xunit.Fact]
    public void GetResource_WithInvalidIndex_ShouldReturnNull()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 3
        };
        function.InstantiateStateForEachResource();

        // Act
        var resource0 = function.GetResource(0);
        var resource4 = function.GetResource(4);

        // Assert
        resource0.Should().BeNull();
        resource4.Should().BeNull();
    }

    [Xunit.Fact]
    public void GetFirstReadyResource_WithIdleResource_ShouldReturnFirstIdle()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 3
        };
        function.InstantiateStateForEachResource();
        // Set first resource to Idle (default state)
        // Set second resource to Working
        SetResourceState(function.Resources[1], ResourceStates.Working);

        // Act
        var readyResource = function.GetFirstReadyResource();

        // Assert
        readyResource.Should().Be(function.Resources[0]);
    }

    [Xunit.Fact]
    public void GetFirstReadyResource_WithNoIdleResource_ShouldReturnNull()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 2
        };
        function.InstantiateStateForEachResource();
        SetResourceState(function.Resources[0], ResourceStates.Working);
        SetResourceState(function.Resources[1], ResourceStates.Working);

        // Act
        var readyResource = function.GetFirstReadyResource();

        // Assert
        readyResource.Should().BeNull();
    }

    [Xunit.Fact]
    public void GetWorkingResources_ShouldReturnOnlyWorkingResources()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 3
        };
        function.InstantiateStateForEachResource();
        SetResourceState(function.Resources[0], ResourceStates.Idle);
        SetResourceState(function.Resources[1], ResourceStates.Working);
        SetResourceState(function.Resources[2], ResourceStates.Working);

        // Act
        var workingResources = function.GetWorkingResources().ToList();

        // Assert
        workingResources.Should().HaveCount(2);
        workingResources.Should().Contain(function.Resources[1]);
        workingResources.Should().Contain(function.Resources[2]);
        workingResources.Should().NotContain(function.Resources[0]);
    }

    [Xunit.Fact]
    public void IdleResourcesCount_ShouldReturnCountOfNonWorkingResources()
    {
        // Arrange
        var function = new FunctionInMachineDefinition
        {
            ResourceCount = 4
        };
        function.InstantiateStateForEachResource();
        SetResourceState(function.Resources[0], ResourceStates.Idle);
        SetResourceState(function.Resources[1], ResourceStates.Working);
        SetResourceState(function.Resources[2], ResourceStates.Idle);
        SetResourceState(function.Resources[3], ResourceStates.Working);

        // Act
        var idleCount = function.IdleResourcesCount();

        // Assert
        idleCount.Should().Be(2);
    }
}



