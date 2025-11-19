using FluentAssertions;
using Neo.Bpms.Infrastructure.Features.Bpms.Instances;

namespace Neo.Bpms.Infrastructure.Tests.Features.Bpms.Instances;

public class UserTaskInstanceUserFunctionsTests
{
    [Fact]
    public void EUserFunctions_ShouldHaveCorrectValues()
    {
        // Assert - System functions
        ((int)UserTaskInstance.eUserFunctions.System_Create).Should().Be(1);
        ((int)UserTaskInstance.eUserFunctions.System_OfferSingle).Should().Be(2);
        ((int)UserTaskInstance.eUserFunctions.System_OfferMultiple).Should().Be(3);
        ((int)UserTaskInstance.eUserFunctions.System_Allocate).Should().Be(4);

        // Assert - Admin functions
        ((int)UserTaskInstance.eUserFunctions.Admin_OfferSingle).Should().Be(11);
        ((int)UserTaskInstance.eUserFunctions.Admin_OfferMultiple).Should().Be(12);
        ((int)UserTaskInstance.eUserFunctions.Admin_Allocate).Should().Be(13);
        ((int)UserTaskInstance.eUserFunctions.Admin_WithdrawAndAllocate).Should().Be(14);

        // Assert - Resource functions
        ((int)UserTaskInstance.eUserFunctions.Resource_Allocate).Should().Be(21);
        ((int)UserTaskInstance.eUserFunctions.Resource_WithdrawAndAllocate).Should().Be(22);
        ((int)UserTaskInstance.eUserFunctions.Resource_StartOffered).Should().Be(23);
        ((int)UserTaskInstance.eUserFunctions.Resource_WithdrawAndStart).Should().Be(24);
        ((int)UserTaskInstance.eUserFunctions.Resource_StartAllocated).Should().Be(25);

        // Assert - Resource suspend/resume/complete/fail
        ((int)UserTaskInstance.eUserFunctions.Resource_Suspend).Should().Be(31);
        ((int)UserTaskInstance.eUserFunctions.Resource_Resume).Should().Be(32);
        ((int)UserTaskInstance.eUserFunctions.Resource_Complete).Should().Be(33);
        ((int)UserTaskInstance.eUserFunctions.Resource_Fail).Should().Be(34);
    }
}

