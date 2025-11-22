using FluentAssertions;
using Neo.Bpms.Domain.Features.Dynamic;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class WorkItemViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var viewModel = new WorkItemViewModel();

        // Assert
        viewModel.ProcessInstanceId.Should().Be(0);
        viewModel.ActivityInstanceId.Should().Be(0);
        viewModel.UserTaskState.Should().Be(default(UserTaskInstanceStateId));
        viewModel.ProcessId.Should().BeNull();
        viewModel.ProcessVersion.Should().BeNull();
        viewModel.TaskId.Should().BeNull();
        viewModel.ProcessName.Should().BeNull();
        viewModel.ActivityName.Should().BeNull();
        viewModel.Description.Should().BeNull();
        viewModel.CreationTime.Should().Be(default(DateTime));
        viewModel.StartTime.Should().BeNull();
        viewModel.CompletionTime.Should().BeNull();
        viewModel.ActualOwnerId.Should().BeNull();
        viewModel.UserGroupId.Should().Be(0);
        viewModel.ActualOwnerName.Should().BeNull();
        viewModel.UserGroupName.Should().BeNull();
        viewModel.Forms.Should().BeNull();
        viewModel.NamespaceId.Should().BeNull();
        viewModel.EntityId.Should().BeNull();
        viewModel.EntityPkv.Should().BeNull();
        viewModel.EntityDescription.Should().BeNull();
        viewModel.FlowNodeType.Should().BeNull();
        viewModel.ActivityType.Should().BeNull();
        viewModel.EventType.Should().BeNull();
        viewModel.DisplayFields.Should().BeNull();
        viewModel.Record.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithElasticObject_ShouldMapProperties()
    {
        // Arrange
        var creationTime = new DateTime(2024, 1, 15, 10, 0, 0);
        var startTime = new DateTime(2024, 1, 15, 11, 0, 0);
        var completionTime = new DateTime(2024, 1, 15, 12, 0, 0);
        var record = new ElasticObject();
        record.SetField("__WorkflowId", "Process1");
        record.SetField("__WFVersion", "1.0");
        record.SetField("__ActivityInstanceId", 123L);
        record.SetField("__ProcessInstanceId", 456L);
        record.SetField("__FlowNodeId", "Task1");
        record.SetField("__ActualOwnerId", "User1");
        record.SetField("__UserGroupId", 789L);
        record.SetField("__CreationTime", creationTime);
        record.SetField("__StartTime", startTime);
        record.SetField("__CompletionTime", completionTime);
        record.SetField("__userTaskStateId", (long)UserTaskInstanceStateId.Started);
        record.SetField("__Description", "Test Description");
        record.SetField("__EntityPKV", "PKV123");
        record.SetField("__UserName", "John Doe");
        record.SetField("__UserGroupName", "Admin Group");

        // Act
        var viewModel = new WorkItemViewModel(record);

        // Assert
        viewModel.ProcessId.Should().Be("Process1");
        viewModel.ProcessVersion.Should().Be("1.0");
        viewModel.ActivityInstanceId.Should().Be(123L);
        viewModel.ProcessInstanceId.Should().Be(456L);
        viewModel.TaskId.Should().Be("Task1");
        viewModel.ActivityName.Should().Be("");
        viewModel.ActualOwnerId.Should().Be("User1");
        viewModel.UserGroupId.Should().Be(789L);
        viewModel.CreationTime.Should().Be(creationTime);
        viewModel.StartTime.Should().Be(startTime);
        viewModel.CompletionTime.Should().Be(completionTime);
        viewModel.UserTaskState.Should().Be(UserTaskInstanceStateId.Started);
        viewModel.Description.Should().Be("Test Description");
        viewModel.EntityPkv.Should().Be("PKV123");
        viewModel.ActualOwnerName.Should().Be("John Doe");
        viewModel.UserGroupName.Should().Be("Admin Group");
        viewModel.Record.Should().Be(record);
        viewModel.Forms.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void WorkItemClass_WithNoneStateAndNoEventType_ShouldReturnDanger()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.None,
            EventType = null,
            FlowNodeType = null
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("danger");
    }

    [Fact]
    public void WorkItemClass_WithNoneStateAndEventType_ShouldReturnEmpty()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.None,
            EventType = Event.eEventType.Message
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("");
    }

    [Fact]
    public void WorkItemClass_WithNoneStateAndParallelGateway_ShouldReturnEmpty()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.None,
            FlowNodeType = FlowNodeTypeId.ParallelGateway
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("");
    }

    [Fact]
    public void WorkItemClass_WithCreatedStateAndCallActivitySubProcess_ShouldReturnInfo()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Created,
            ActivityType = Activity.eActivityType.CallActivitySubProcess
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("info");
    }

    [Fact]
    public void WorkItemClass_WithCreatedStateAndUserTask_ShouldReturnWarning()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Created,
            ActivityType = Activity.eActivityType.UserTask
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("warning");
    }

    [Fact]
    public void WorkItemClass_WithStartedState_ShouldReturnInfo()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Started
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("info");
    }

    [Fact]
    public void WorkItemClass_WithSuspendedState_ShouldReturnWarning()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Suspended
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("warning");
    }

    [Fact]
    public void WorkItemClass_WithCompletedState_ShouldReturnSuccess()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Completed
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("success");
    }

    [Fact]
    public void WorkItemClass_WithFailedState_ShouldReturnDanger()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            UserTaskState = UserTaskInstanceStateId.Failed
        };

        // Act
        var workItemClass = viewModel.WorkItemClass;

        // Assert
        workItemClass.Should().Be("danger");
    }

    [Fact]
    public void GetAttributes_ShouldReturnCorrectHtmlAttributes()
    {
        // Arrange
        var viewModel = new WorkItemViewModel
        {
            ActivityInstanceId = 123L,
            ProcessId = "Process1",
            TaskId = "Task1",
            ProcessInstanceId = 456L,
            ProcessVersion = "1.0",
            Description = "Test Description",
            UserTaskState = UserTaskInstanceStateId.Started
        };

        // Act
        var attributes = viewModel.GetAttributes();

        // Assert
        attributes.Should().Contain("title=\"Test Description\"");
        attributes.Should().Contain("id=\"ai-row-123\"");
        attributes.Should().Contain("data-processid=\"Process1\"");
        attributes.Should().Contain("data-activityid=\"Task1\"");
        attributes.Should().Contain("data-piid=\"456\"");
        attributes.Should().Contain("data-version=\"1.0\"");
        attributes.Should().Contain("data-aiid=\"123\"");
        attributes.Should().Contain("class=\"selectable-table-row table-info\"");
    }
}



