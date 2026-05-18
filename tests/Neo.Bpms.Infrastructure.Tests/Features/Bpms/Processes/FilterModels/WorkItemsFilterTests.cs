using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.FilterModels;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.FilterModels;

public class WorkItemsFilterTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var filter = new WorkItemsFilter();

        // Assert
        filter.__Created.Should().BeTrue();
        filter.__JustRestorableTasks.Should().BeFalse();
        filter.Page.Should().Be(1);
        filter.__PageType.Should().Be(WorkItemsPageType.MyWorkItems);
    }

    [Xunit.Fact]
    public void SetDefaultValues_ShouldSetOfferedAllocatedAndSuspendedToTrue()
    {
        // Arrange
        var filter = new WorkItemsFilter
        {
            __Offered = false,
            __Allocated = false,
            __Suspended = false
        };

        // Act
        filter.SetDefaultValues();

        // Assert
        filter.__Offered.Should().BeTrue();
        filter.__Allocated.Should().BeTrue();
        filter.__Suspended.Should().BeTrue();
    }

    [Xunit.Fact]
    public void GetDefaultWorkItemsFilter_ShouldCreateFilterWithCorrectValues()
    {
        // Arrange
        var processId = "Process1";
        var processVersionId = "Version1";
        var userId = "User1";
        var page = 2;

        // Act
        var filter = WorkItemsFilter.GetDefaultWorkItemsFilter(processId, processVersionId, userId, page);

        // Assert
        filter.__ProcessId.Should().Be(processId);
        filter.ProcessId.Should().Be(processId);
        filter.__ProcessVersionId.Should().Be(processVersionId);
        filter.ProcessVersionId.Should().Be(processVersionId);
        filter.UserId.Should().Be(userId);
        filter.Page.Should().Be(page);
        filter.__Offered.Should().BeTrue();
        filter.__Allocated.Should().BeTrue();
        filter.__Suspended.Should().BeTrue();
        filter.__Created.Should().BeTrue();
    }

    [Xunit.Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Arrange
        var filter = new WorkItemsFilter
        {
            __ProcessId = "P1",
            __ProcessVersionId = "V1",
            __Activity = "Activity1",
            __Description = "Description1",
            __NamespaceId = "NS1",
            __EntityId = "E1",
            __EntityPkv = "PKV1",
            __FromCreationDateText = "2024-01-01",
            __ToCreationDateText = "2024-12-31",
            UserId = "User1",
            UserGroupId = 100,
            __Offered = true,
            __Allocated = false,
            __Suspended = true,
            __Created = true,
            __Finished = false
        };

        // Assert
        filter.ProcessId.Should().Be("P1");
        filter.ProcessVersionId.Should().Be("V1");
        filter.Activity.Should().Be("Activity1");
        filter.Description.Should().Be("Description1");
        filter.NamespaceId.Should().Be("NS1");
        filter.EntityId.Should().Be("E1");
        filter.EntityPkv.Should().Be("PKV1");
        filter.FromCreationDateText.Should().Be("2024-01-01");
        filter.ToCreationDateText.Should().Be("2024-12-31");
        filter.Offered.Should().BeTrue();
        filter.Allocated.Should().BeFalse();
        filter.Suspended.Should().BeTrue();
        filter.Created.Should().BeTrue();
        filter.Finished.Should().BeFalse();
    }
}

