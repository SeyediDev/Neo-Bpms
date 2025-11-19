using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class WorkItemsQueryTypeTests
{
    [Fact]
    public void WorkItemsQueryType_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)WorkItemsQueryType.MyWorkItems).Should().Be(1);
        ((int)WorkItemsQueryType.Process).Should().Be(2);
        ((int)WorkItemsQueryType.MyProcess).Should().Be(3);
    }
}

