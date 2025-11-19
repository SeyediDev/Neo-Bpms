using FluentAssertions;
using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC.Tests.Extensions;

public class StringUtilsTests
{
    [Fact]
    public void NormalizeFarsi_WithArabicCharacters_ShouldConvertToPersian()
    {
        // Arrange
        var input = "ي ك";

        // Act
        var result = input.NormalizeFarsi();

        // Assert
        result.Should().Be("ی ک");
    }

    [Fact]
    public void NormalizeFarsi_WithPersianCharacters_ShouldNotChange()
    {
        // Arrange
        var input = "ی ک";

        // Act
        var result = input.NormalizeFarsi();

        // Assert
        result.Should().Be("ی ک");
    }

    [Fact]
    public void NormalizeFarsi_WithWhitespace_ShouldTrim()
    {
        // Arrange
        var input = "  test  ";

        // Act
        var result = input.NormalizeFarsi();

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void NormalizeFarsi_WithNull_ShouldReturnNull()
    {
        // Arrange
        string? input = null;

        // Act
        var result = input.NormalizeFarsi();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void NormalizeFarsi_WithEmptyString_ShouldReturnEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = input.NormalizeFarsi();

        // Assert
        result.Should().Be("");
    }
}

