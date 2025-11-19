using FluentAssertions;
using Neo.Bpms.Domain.Extensions;

namespace Neo.Bpms.UI.MVC.Tests.Extensions;

public class NumberUtillTests
{
    [Fact]
    public void StrToByteArray_ShouldConvertStringToByteArray()
    {
        // Arrange
        var input = "test";

        // Act
        var result = NumberUtill.StrToByteArray(input);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(double), true)]
    [InlineData(typeof(string), false)]
    [InlineData(typeof(bool), false)]
    public void IsNumber_ShouldReturnCorrectValue(Type type, bool expected)
    {
        // Act
        var result = NumberUtill.IsNumber(type);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123", false)]
    [InlineData(null, true)]
    [InlineData("", false)]
    public void IsNaN_ShouldReturnCorrectValue(object? input, bool expected)
    {
        // Act
        var result = NumberUtill.IsNaN(input!);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData("(123.45)", -123.45)]
    [InlineData("123/45", 123.45)]
    [InlineData("invalid", 0)]
    public void ToDouble_WithString_ShouldConvertCorrectly(string input, double expected)
    {
        // Act
        var result = NumberUtill.ToDouble(input);

        // Assert
        result.Should().BeApproximately(expected, 0.01);
    }

    [Theory]
    [InlineData(123, 123.0)]
    [InlineData("123.45", 123.45)]
    [InlineData("123'45", 12345.0)]
    public void ToDouble_WithObject_ShouldConvertCorrectly(object input, double expected)
    {
        // Act
        var result = NumberUtill.ToDouble(input);

        // Assert
        result.Should().BeApproximately(expected, 0.01);
    }

    [Theory]
    [InlineData(123, 123)]
    [InlineData(123.45, 123)]
    [InlineData("123", 123)]
    [InlineData("invalid", 0)]
    public void ToInt_ShouldConvertCorrectly(object input, int expected)
    {
        // Act
        var result = NumberUtill.ToInt(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("1234", ',', '.', true, "1,234")]
    [InlineData("1234.56", ',', '.', true, "1,234.56")]
    [InlineData("-1234", ',', '.', true, "(1,234)")]
    public void DigitGroup_ShouldFormatCorrectly(string input, char separator, char dot, bool showNegativeInParentheses, string expected)
    {
        // Act
        var result = NumberUtill.DigitGroup(input, separator, dot, showNegativeInParentheses);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Reverse_ShouldReverseString()
    {
        // Arrange
        var input = "hello";

        // Act
        var result = NumberUtill.Reverse(input);

        // Assert
        result.Should().Be("olleh");
    }

    [Theory]
    [InlineData("123", "123")]
    [InlineData("invalid", "0")]
    public void ParseNumber_ShouldParseCorrectly(string input, string expected)
    {
        // Act
        var result = NumberUtill.ParseNumber(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("1,234", false, 1234)]
    [InlineData("(1,234)", false, -1234)]
    [InlineData("(1,234)", true, 1234)]
    public void RemoveCama_ShouldRemoveCommasAndHandleNegative(string input, bool nochange, int expected)
    {
        // Act
        var result = NumberUtill.RemoveCama(input, nochange);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Max_ShouldReturnMaximumValue()
    {
        // Act
        var result = NumberUtill.Max(1, 5, 3, 2);

        // Assert
        result.Should().Be(5);
    }

    [Theory]
    [InlineData("123", true, true)]
    [InlineData("123.45", true, true)]
    [InlineData("123.45", false, false)]
    [InlineData("abc", true, false)]
    public void IsDigit_ShouldReturnCorrectValue(string text, bool validDouble, bool expected)
    {
        // Act
        var result = NumberUtill.IsDigit(text, validDouble);

        // Assert
        result.Should().Be(expected);
    }
}

