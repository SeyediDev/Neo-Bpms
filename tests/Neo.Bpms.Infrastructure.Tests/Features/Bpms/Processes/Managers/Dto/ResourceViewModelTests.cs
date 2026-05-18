using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class ResourceViewModelTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var resource = new ResourceViewModel();

        // Assert
        resource.Type.Should().BeNull();
        resource.Code.Should().BeNull();
        resource.Name.Should().BeNull();
        resource.Claim.Should().BeNull();
    }

    [Xunit.Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var resource = new ResourceViewModel
        {
            Type = "User",
            Code = "USER001",
            Name = "Test User",
            Claim = "test-claim"
        };

        // Assert
        resource.Type.Should().Be("User");
        resource.Code.Should().Be("USER001");
        resource.Name.Should().Be("Test User");
        resource.Claim.Should().Be("test-claim");
    }
}



