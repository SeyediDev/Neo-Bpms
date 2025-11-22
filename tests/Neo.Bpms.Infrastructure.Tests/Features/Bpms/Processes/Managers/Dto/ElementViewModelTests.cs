using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class ElementViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var element = new ElementViewModel();

        // Assert
        element.Id.Should().BeNull();
        element.Name.Should().BeNull();
        element.Type.Should().BeNull();
        element.ProcessLink.Should().BeNull();
        element.Inputs.Should().NotBeNull().And.BeEmpty();
        element.Outputs.Should().NotBeNull().And.BeEmpty();
        element.ColorIndex.Should().Be(0);
    }

    [Fact]
    public void Colspan_WithNoInputsOrOutputs_ShouldReturnZero()
    {
        // Arrange
        var element = new ElementViewModel();

        // Act
        var colspan = element.Colspan;

        // Assert
        colspan.Should().Be(0);
    }

    [Fact]
    public void Colspan_WithMoreInputsThanOutputs_ShouldReturnInputsCount()
    {
        // Arrange
        var element = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel>
            {
                new(),
                new(),
                new()
            },
            Outputs = new List<FlowElementViewModel>
            {
                new()
            }
        };

        // Act
        var colspan = element.Colspan;

        // Assert
        colspan.Should().Be(3);
    }

    [Fact]
    public void Colspan_WithMoreOutputsThanInputs_ShouldReturnOutputsCount()
    {
        // Arrange
        var element = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel>
            {
                new()
            },
            Outputs = new List<FlowElementViewModel>
            {
                new(),
                new(),
                new(),
                new()
            }
        };

        // Act
        var colspan = element.Colspan;

        // Assert
        colspan.Should().Be(4);
    }

    [Fact]
    public void Colspan_WithEqualInputsAndOutputs_ShouldReturnThatCount()
    {
        // Arrange
        var element = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel>
            {
                new(),
                new()
            },
            Outputs = new List<FlowElementViewModel>
            {
                new(),
                new()
            }
        };

        // Act
        var colspan = element.Colspan;

        // Assert
        colspan.Should().Be(2);
    }
}



