using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Processes.Managers.Dto;

public class UserViewModelTests
{
    [Xunit.Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Act
        var user = new UserViewModel();

        // Assert
        user.Id.Should().BeNull();
        user.Username.Should().BeNull();
        user.Name.Should().BeNull();
        user.Family.Should().BeNull();
        user.AvatarId.Should().BeNull();
    }

    [Xunit.Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var user1 = new UserViewModel { Id = "user-1" };
        var user2 = new UserViewModel { Id = "user-1" };

        // Act
        var result = user1.Equals(user2);

        // Assert
        result.Should().BeTrue();
    }

    [Xunit.Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var user1 = new UserViewModel { Id = "user-1" };
        var user2 = new UserViewModel { Id = "user-2" };

        // Act
        var result = user1.Equals(user2);

        // Assert
        result.Should().BeFalse();
    }

    [Xunit.Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var user = new UserViewModel { Id = "user-1" };

        // Act
        var result = user.Equals((UserViewModel)null!);

        // Assert
        result.Should().BeFalse();
    }

    [Xunit.Fact]
    public void Equals_WithSameReference_ShouldReturnTrue()
    {
        // Arrange
        var user = new UserViewModel { Id = "user-1" };

        // Act
        var result = user.Equals(user);

        // Assert
        result.Should().BeTrue();
    }

    [Xunit.Fact]
    public void Equals_WithObjectParameter_ShouldReturnTrueForSameId()
    {
        // Arrange
        var user1 = new UserViewModel { Id = "user-1" };
        object user2 = new UserViewModel { Id = "user-1" };

        // Act
        var result = user1.Equals(user2);

        // Assert
        result.Should().BeTrue();
    }

    [Xunit.Fact]
    public void Equals_WithObjectParameter_ShouldReturnFalseForDifferentType()
    {
        // Arrange
        var user = new UserViewModel { Id = "user-1" };
        object other = "not-a-user";

        // Act
        var result = user.Equals(other);

        // Assert
        result.Should().BeFalse();
    }

    [Xunit.Fact]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        // Arrange
        var user1 = new UserViewModel { Id = "user-1" };
        var user2 = new UserViewModel { Id = "user-1" };

        // Act
        var hashCode1 = user1.GetHashCode();
        var hashCode2 = user2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Xunit.Fact]
    public void GetHashCode_WithNullId_ShouldReturnZero()
    {
        // Arrange
        var user = new UserViewModel();

        // Act
        var hashCode = user.GetHashCode();

        // Assert
        hashCode.Should().Be(0);
    }
}



