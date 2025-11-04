using Neo.Domain.Features.Client.Dto;

namespace Neo.Bpms.Domain.Features.Security.Dto;

public class VerificationResult
{
    public bool Verified { get; set; }
    public TokenResponseDto Token { get; set; }
    public string Message { get; set; }
}
