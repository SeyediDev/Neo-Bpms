using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.MicroServices.Definitions;

public class ServiceFunctionDefinitionTests
{
    [Fact]
    public void Constructor_WithFunctionName_ShouldSetFuncName()
    {
        // Act
        var function = new ServiceFunctionDefinition("TestFunction");

        // Assert
        function.FuncName.Should().Be("TestFunction");
        function.ProviderMachines.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void UpdateOrAddProviderMachineFunction_WithNewMachine_ShouldAdd()
    {
        // Arrange
        var function = new ServiceFunctionDefinition("TestFunction");
        var machineFunction = new FunctionInMachineDefinition { FuncName = "TestFunction" };

        // Act
        function.UpdateOrAddProviderMachineFunction("Machine1", machineFunction);

        // Assert
        function.ProviderMachines.Should().HaveCount(1);
        function.ProviderMachines["Machine1"].Should().Be(machineFunction);
    }

    [Fact]
    public void UpdateOrAddProviderMachineFunction_WithExistingMachine_ShouldUpdate()
    {
        // Arrange
        var function = new ServiceFunctionDefinition("TestFunction");
        var machineFunction1 = new FunctionInMachineDefinition { FuncName = "TestFunction", Url = "Url1" };
        var machineFunction2 = new FunctionInMachineDefinition { FuncName = "TestFunction", Url = "Url2" };
        function.UpdateOrAddProviderMachineFunction("Machine1", machineFunction1);

        // Act
        function.UpdateOrAddProviderMachineFunction("Machine1", machineFunction2);

        // Assert
        function.ProviderMachines.Should().HaveCount(1);
        function.ProviderMachines["Machine1"].Should().Be(machineFunction2);
        function.ProviderMachines["Machine1"].Url.Should().Be("Url2");
    }

    [Fact]
    public void UpdateOrAddProviderMachineFunction_WithMultipleMachines_ShouldAddAll()
    {
        // Arrange
        var function = new ServiceFunctionDefinition("TestFunction");
        var machineFunction1 = new FunctionInMachineDefinition { FuncName = "TestFunction" };
        var machineFunction2 = new FunctionInMachineDefinition { FuncName = "TestFunction" };

        // Act
        function.UpdateOrAddProviderMachineFunction("Machine1", machineFunction1);
        function.UpdateOrAddProviderMachineFunction("Machine2", machineFunction2);

        // Assert
        function.ProviderMachines.Should().HaveCount(2);
        function.ProviderMachines["Machine1"].Should().Be(machineFunction1);
        function.ProviderMachines["Machine2"].Should().Be(machineFunction2);
    }
}



