using FluentAssertions;
using Moq;
using Neo.Bpms.Domain.Models.JobScheduling;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.JobScheduler;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Jobs.JobScheduler;

public class JobTests
{
    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var job = new Job();
        var jobSchedule = Mock.Of<IJobSchedule>();
        var jobItem = new object();

        // Act
        job.JobSchedule = jobSchedule;
        job.JobItem = jobItem;

        // Assert
        job.JobSchedule.Should().Be(jobSchedule);
        job.JobItem.Should().Be(jobItem);
    }

    [Fact]
    public void Properties_ShouldBeGettable()
    {
        // Arrange
        var jobSchedule = Mock.Of<IJobSchedule>();
        var jobItem = new { Id = 1, Name = "Test" };
        var job = new Job
        {
            JobSchedule = jobSchedule,
            JobItem = jobItem
        };

        // Act & Assert
        job.JobSchedule.Should().Be(jobSchedule);
        job.JobItem.Should().Be(jobItem);
    }
}

