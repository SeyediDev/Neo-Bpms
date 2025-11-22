using FluentAssertions;
using Neo.Bpms.Domain.Model.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class WorkItemFormInfoTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var formInfo = new WorkItemFormInfo();

        // Assert
        formInfo.FormType.Should().Be(default(Form.eFormType));
        formInfo.NamespaceId.Should().BeNull();
        formInfo.EntityId.Should().BeNull();
        formInfo.FormId.Should().BeNull();
        formInfo.FormSubjectId.Should().BeNull();
        formInfo.Name.Should().BeNull();
        formInfo.TaskId.Should().BeNull();
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var formInfo = new WorkItemFormInfo
        {
            FormType = Form.eFormType.Create,
            NamespaceId = "NS1",
            EntityId = "Entity1",
            FormId = "Form1",
            FormSubjectId = "Subject1",
            Name = "Test Form",
            TaskId = "Task1"
        };

        // Assert
        formInfo.FormType.Should().Be(Form.eFormType.Create);
        formInfo.NamespaceId.Should().Be("NS1");
        formInfo.EntityId.Should().Be("Entity1");
        formInfo.FormId.Should().Be("Form1");
        formInfo.FormSubjectId.Should().Be("Subject1");
        formInfo.Name.Should().Be("Test Form");
        formInfo.TaskId.Should().Be("Task1");
    }
}



