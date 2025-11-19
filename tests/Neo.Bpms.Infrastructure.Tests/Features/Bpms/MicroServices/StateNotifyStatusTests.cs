using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.MicroServices;

public class StateNotifyStatusTests
{
    [Theory]
    [InlineData(StateNotifyStatus.cancel)]
    [InlineData(StateNotifyStatus.done)]
    [InlineData(StateNotifyStatus.error)]
    [InlineData(StateNotifyStatus.fresh)]
    [InlineData(StateNotifyStatus.info)]
    [InlineData(StateNotifyStatus.progress)]
    [InlineData(StateNotifyStatus.pause)]
    [InlineData(StateNotifyStatus.resume)]
    [InlineData(StateNotifyStatus.warning)]
    public void Enum_ShouldHaveAllExpectedValues(StateNotifyStatus status)
    {
        // Act
        var value = (int)status;

        // Assert
        value.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Enum_ShouldHaveAllExpectedMembers()
    {
        // Act
        var values = Enum.GetValues<StateNotifyStatus>();

        // Assert
        values.Should().HaveCount(10);
        values.Should().Contain(StateNotifyStatus.cancel);
        values.Should().Contain(StateNotifyStatus.done);
        values.Should().Contain(StateNotifyStatus.error);
        values.Should().Contain(StateNotifyStatus.fresh);
        values.Should().Contain(StateNotifyStatus.info);
        values.Should().Contain(StateNotifyStatus.progress);
        values.Should().Contain(StateNotifyStatus.pause);
        values.Should().Contain(StateNotifyStatus.resume);
        values.Should().Contain(StateNotifyStatus.warning);
    }
}

