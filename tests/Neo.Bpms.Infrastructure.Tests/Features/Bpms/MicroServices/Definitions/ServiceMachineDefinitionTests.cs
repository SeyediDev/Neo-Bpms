using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.MicroServices.Definitions;

public class ServiceMachineDefinitionTests
{
    [Fact]
    public void Constructor_WithMachineId_ShouldSetMachineId()
    {
        // Act
        var machine = new ServiceMachineDefinition("Machine1");

        // Assert
        machine.MachineId.Should().Be("Machine1");
        machine.Functions.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void UpdateAlivedTime_ShouldUpdateLatestAliveTime()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        var initialTime = DateTime.Now;
        // Wait a bit to ensure time difference
        Thread.Sleep(10);

        // Act
        machine.UpdateAlivedTime();

        // Assert
        // The machine should be alive after UpdateAlivedTime
        machine.IsAlive().Should().BeTrue();
    }

    [Fact]
    public void YouAreConnected_ShouldResetUnsuccessfulConnection()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        machine.HadUnsuccessfulConnection();

        // Act
        machine.YouAreConnected();

        // Assert
        // After YouAreConnected, if we update alive time, it should be alive
        machine.UpdateAlivedTime();
        machine.IsAlive().Should().BeTrue();
    }

    [Fact]
    public void HadUnsuccessfulConnection_ShouldMarkAsUnsuccessful()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        machine.UpdateAlivedTime();

        // Act
        machine.HadUnsuccessfulConnection();

        // Assert
        machine.IsAlive().Should().BeFalse();
    }

    [Fact]
    public void GetFunction_WithExistingFunction_ShouldReturnFunction()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        var function = new FunctionInMachineDefinition { FuncName = "Function1" };
        machine.Functions.TryAdd("Function1", function);

        // Act
        var result = machine.GetFunction("Function1");

        // Assert
        result.Should().Be(function);
    }

    [Fact]
    public void GetFunction_WithNonExistingFunction_ShouldReturnNull()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");

        // Act
        var result = machine.GetFunction("NonExisting");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void IsAlive_WithinTimeout_ShouldReturnTrue()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        machine.UpdateAlivedTime();

        // Act
        var isAlive = machine.IsAlive();

        // Assert
        isAlive.Should().BeTrue();
    }

    [Fact]
    public void IsAlive_AfterTimeout_ShouldReturnFalse()
    {
        // Arrange
        var machine = new ServiceMachineDefinition("Machine1");
        // Use reflection to set _latestAliveTime to a time more than 125 seconds ago
        var field = typeof(ServiceMachineDefinition).GetField("_latestAliveTime", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field!.SetValue(machine, DateTime.Now.AddSeconds(-130));

        // Act
        var isAlive = machine.IsAlive();

        // Assert
        isAlive.Should().BeFalse();
    }
}

