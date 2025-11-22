using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Instances;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Instances;

public class ReceiveTokenInfoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var receiveTokenInfo = new ReceiveTokenInfo();

        // Assert
        receiveTokenInfo.receiveTokenCount.Should().Be(0);
    }

    [Fact]
    public void ReceiveTokenCount_ShouldBeSettable()
    {
        // Arrange
        var receiveTokenInfo = new ReceiveTokenInfo
        {
            receiveTokenCount = 5L
        };

        // Assert
        receiveTokenInfo.receiveTokenCount.Should().Be(5L);
    }
}



