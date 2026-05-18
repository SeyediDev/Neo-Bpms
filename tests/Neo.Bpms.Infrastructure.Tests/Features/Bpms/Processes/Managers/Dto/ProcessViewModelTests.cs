using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class ProcessViewModelTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var process = new ProcessViewModel();

        // Assert
        process.Id.Should().BeNull();
        process.Name.Should().BeNull();
        process.EntityId.Should().BeNull();
        process.InputParams.Should().NotBeNull().And.BeEmpty();
        process.OuputParams.Should().NotBeNull().And.BeEmpty();
        process.Properties.Should().NotBeNull().And.BeEmpty();
        process.Managers.Should().NotBeNull().And.BeEmpty();
        process.Lanes.Should().NotBeNull().And.BeEmpty();
    }

    [Xunit.Fact]
    public void GetElement_WithExistingElementId_ShouldReturnElement()
    {
        // Arrange
        var element = new ElementViewModel { Id = "element-1" };
        var lane = new LaneViewModel
        {
            Elements = [element]
        };
        var process = new ProcessViewModel
        {
            Lanes = [lane]
        };

        // Act
        var result = process.GetElement("element-1");

        // Assert
        result.Should().Be(element);
    }

    [Xunit.Fact]
    public void GetElement_WithNonExistingElementId_ShouldReturnNull()
    {
        // Arrange
        var element = new ElementViewModel { Id = "element-1" };
        var lane = new LaneViewModel
        {
            Elements = [element]
        };
        var process = new ProcessViewModel
        {
            Lanes = [lane]
        };

        // Act
        var result = process.GetElement("non-existing");

        // Assert
        result.Should().BeNull();
    }

    [Xunit.Fact]
    public void GetElement_WithMultipleLanes_ShouldFindElementInAnyLane()
    {
        // Arrange
        var element1 = new ElementViewModel { Id = "element-1" };
        var element2 = new ElementViewModel { Id = "element-2" };
        var lane1 = new LaneViewModel
        {
            Elements = [element1]
        };
        var lane2 = new LaneViewModel
        {
            Elements = [element2]
        };
        var process = new ProcessViewModel
        {
            Lanes = [lane1, lane2]
        };

        // Act
        var result1 = process.GetElement("element-1");
        var result2 = process.GetElement("element-2");

        // Assert
        result1.Should().Be(element1);
        result2.Should().Be(element2);
    }

    [Xunit.Fact]
    public void GetElement_WithEmptyLanes_ShouldReturnNull()
    {
        // Arrange
        var process = new ProcessViewModel();

        // Act
        var result = process.GetElement("any-id");

        // Assert
        result.Should().BeNull();
    }
}



