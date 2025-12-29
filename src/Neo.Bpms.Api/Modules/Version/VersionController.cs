namespace Neo.Bpms.Api.Modules.Version;

/// <summary>
/// API controller for version information
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    private readonly IVersionService _versionService;

    public VersionController(IVersionService versionService)
    {
        _versionService = versionService;
    }

    /// <summary>
    /// Get application version information
    /// </summary>
    [HttpGet]
    public ActionResult<VersionInfo> GetVersion()
    {
        return Ok(_versionService.GetVersionInfo());
    }

    /// <summary>
    /// Get short version string
    /// </summary>
    [HttpGet("short")]
    public ActionResult<string> GetShortVersion()
    {
        return Ok(_versionService.GetVersionInfo().ShortVersion);
    }
}

