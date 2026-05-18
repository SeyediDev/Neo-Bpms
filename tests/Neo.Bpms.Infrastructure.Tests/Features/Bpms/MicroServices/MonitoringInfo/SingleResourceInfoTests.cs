using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.MonitoringInfo;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.MicroServices.MonitoringInfo;

public class SingleResourceInfoTests
{
    [Xunit.Fact]
    public void DefaultConstructor_ShouldCreateEmptyInstance()
    {
        // Act
        var info = new SingleResourceInfo();

        // Assert
        info.MachineId.Should().BeNull();
        info.FunctionName.Should().BeNull();
        info.Url.Should().BeNull();
        info.CurrentRequestId.Should().Be(0);
        info.Progress.Should().Be(0);
    }

    // Note: Parameterized constructor is internal, so we can't test it directly
    // It's tested indirectly through the classes that use it (e.g., GetResourcesInfo)

    [Xunit.Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var info = new SingleResourceInfo();

        // Act
        info.MachineId = "Machine2";
        info.IsAlive = true;
        info.FunctionName = "Function2";
        info.Url = "http://test.com";
        info.CurrentRequestId = 456L;
        info.IsImmediate = false;
        info.IsWorking = true;
        info.Progress = 75;

        // Assert
        info.MachineId.Should().Be("Machine2");
        info.IsAlive.Should().BeTrue();
        info.FunctionName.Should().Be("Function2");
        info.Url.Should().Be("http://test.com");
        info.CurrentRequestId.Should().Be(456L);
        info.IsImmediate.Should().BeFalse();
        info.IsWorking.Should().BeTrue();
        info.Progress.Should().Be(75);
    }
}

