using FluentAssertions;
using Moq;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Interfaces.Operation;

public class RunOperationCallBackParamsTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeAllProperties()
    {
        // Arrange
        var operationImplementationRef = "Operation1";
        var id = 123L;
        var machineId = "Machine1";
        var auditTrail = Mock.Of<AuditTrail>();

        // Act
        var paramsObj = new RunOperationCallBackParams(operationImplementationRef, id, machineId, auditTrail);

        // Assert
        paramsObj.OperationName.Should().Be(operationImplementationRef);
        paramsObj.Id.Should().Be(id);
        paramsObj.PreferredMachineId.Should().Be(machineId);
        paramsObj.AuditTrail.Should().Be(auditTrail);
    }

    [Xunit.Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var operationImplementationRef = "TestOperation";
        var id = 456L;
        var machineId = "Machine2";
        var auditTrail = Mock.Of<AuditTrail>();
        var paramsObj = new RunOperationCallBackParams(operationImplementationRef, id, machineId, auditTrail);

        // Act
        var result = paramsObj.ToString();

        // Assert
        result.Should().Be($"Operation {operationImplementationRef}, Id {id}");
    }

    [Xunit.Fact]
    public void ShouldImplementIOperationUserParams()
    {
        // Arrange
        var auditTrail = Mock.Of<AuditTrail>();
        var paramsObj = new RunOperationCallBackParams("Op", 1, "Machine", auditTrail);

        // Assert
        paramsObj.Should().BeAssignableTo<IOperationUserParams>();
    }
}

