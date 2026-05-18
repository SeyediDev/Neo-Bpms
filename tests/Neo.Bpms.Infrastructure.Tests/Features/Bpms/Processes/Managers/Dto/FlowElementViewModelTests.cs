using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class FlowElementViewModelTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var flowElement = new FlowElementViewModel();

        // Assert
        flowElement.Element.Should().BeNull();
        flowElement.Condition.Should().BeNull();
        flowElement.StateId.Should().Be(0);
    }

    [Xunit.Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var element = new ElementViewModel { Id = "test-id" };
        var flowElement = new FlowElementViewModel
        {
            Element = element,
            Condition = "test-condition",
            StateId = 5
        };

        // Assert
        flowElement.Element.Should().Be(element);
        flowElement.Condition.Should().Be("test-condition");
        flowElement.StateId.Should().Be(5);
    }
}



