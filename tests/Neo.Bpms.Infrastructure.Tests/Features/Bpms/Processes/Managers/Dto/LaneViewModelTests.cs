using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class LaneViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var lane = new LaneViewModel();

        // Assert
        lane.Id.Should().BeNull();
        lane.Name.Should().BeNull();
        lane.ParentId.Should().BeNull();
        lane.Performer.Should().NotBeNull();
        lane.Elements.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Colspan_WithNoElements_ShouldReturnZero()
    {
        // Arrange
        var lane = new LaneViewModel();

        // Act
        var colspan = lane.Colspan;

        // Assert
        colspan.Should().Be(0);
    }

    [Fact]
    public void Colspan_WithSingleElement_ShouldReturnOnePlusElementColspan()
    {
        // Arrange
        var element = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel> { new(), new() },
            Outputs = new List<FlowElementViewModel> { new() }
        };
        var lane = new LaneViewModel
        {
            Elements = new List<ElementViewModel> { element }
        };

        // Act
        var colspan = lane.Colspan;

        // Assert
        // Colspan = 1 (for the element itself) + 2 (element's colspan which is max(2, 1) = 2) = 3
        colspan.Should().Be(3);
    }

    [Fact]
    public void Colspan_WithMultipleElements_ShouldReturnSumOfOnePlusEachElementColspan()
    {
        // Arrange
        var element1 = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel> { new() },
            Outputs = new List<FlowElementViewModel> { new(), new() }
        };
        var element2 = new ElementViewModel
        {
            Inputs = new List<FlowElementViewModel> { new(), new(), new() },
            Outputs = new List<FlowElementViewModel> { new() }
        };
        var lane = new LaneViewModel
        {
            Elements = new List<ElementViewModel> { element1, element2 }
        };

        // Act
        var colspan = lane.Colspan;

        // Assert
        // element1: 1 + max(1, 2) = 1 + 2 = 3
        // element2: 1 + max(3, 1) = 1 + 3 = 4
        // Total: 3 + 4 = 7
        colspan.Should().Be(7);
    }
}



