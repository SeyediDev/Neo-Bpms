using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Instances;
using System.Reflection;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Instances;

public class GatewaySyncWaitingInstanceDeserializeTests
{
    [Xunit.Fact]
    public void Deserialize_WithValidString_ShouldReturnDictionary()
    {
        // Arrange
        string receivedTokens = "path1:5,path2:10,path3:3";
        var method = typeof(GatewaySyncWaitingInstance).GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Dictionary<string, ReceiveTokenInfo>)method!.Invoke(null, [receivedTokens])!;

        // Assert
        result.Should().HaveCount(3);
        result["path1"].receiveTokenCount.Should().Be(5L);
        result["path2"].receiveTokenCount.Should().Be(10L);
        result["path3"].receiveTokenCount.Should().Be(3L);
    }

    [Xunit.Fact]
    public void Deserialize_WithNull_ShouldReturnEmptyDictionary()
    {
        // Arrange
        string? receivedTokens = null;
        var method = typeof(GatewaySyncWaitingInstance).GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Dictionary<string, ReceiveTokenInfo>)method!.Invoke(null, [receivedTokens!])!;

        // Assert
        result.Should().BeEmpty();
    }

    [Xunit.Fact]
    public void Deserialize_WithEmptyString_ShouldReturnEmptyDictionary()
    {
        // Arrange
        string receivedTokens = "";
        var method = typeof(GatewaySyncWaitingInstance).GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Dictionary<string, ReceiveTokenInfo>)method!.Invoke(null, [receivedTokens])!;

        // Assert
        result.Should().BeEmpty();
    }

    [Xunit.Fact]
    public void Deserialize_WithInvalidFormat_ShouldSkipInvalidEntries()
    {
        // Arrange
        string receivedTokens = "path1:5,invalid,path2:10";
        var method = typeof(GatewaySyncWaitingInstance).GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Dictionary<string, ReceiveTokenInfo>)method!.Invoke(null, [receivedTokens])!;

        // Assert
        result.Should().HaveCount(2);
        result["path1"].receiveTokenCount.Should().Be(5L);
        result["path2"].receiveTokenCount.Should().Be(10L);
    }

    [Xunit.Fact]
    public void Deserialize_WithNonNumericCount_ShouldSkipInvalidEntries()
    {
        // Arrange
        string receivedTokens = "path1:5,path2:abc,path3:10";
        var method = typeof(GatewaySyncWaitingInstance).GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var result = (Dictionary<string, ReceiveTokenInfo>)method!.Invoke(null, [receivedTokens])!;

        // Assert
        result.Should().HaveCount(2);
        result["path1"].receiveTokenCount.Should().Be(5L);
        result["path3"].receiveTokenCount.Should().Be(10L);
    }
}



