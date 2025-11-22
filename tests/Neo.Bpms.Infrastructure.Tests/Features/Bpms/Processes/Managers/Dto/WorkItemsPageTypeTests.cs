using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class WorkItemsPageTypeTests
{
    [Fact]
    public void WorkItemsPageType_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)WorkItemsPageType.MyWorkItems).Should().Be(1);
        ((int)WorkItemsPageType.WorkItems).Should().Be(2);
    }
}



