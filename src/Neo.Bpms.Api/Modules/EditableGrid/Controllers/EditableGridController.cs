using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.Api.Modules.EditableGrid.Models;
using Neo.Bpms.Api.Modules.EditableGrid.Services;

namespace Neo.Bpms.Api.Modules.EditableGrid.Controllers;

[ApiController]
[Route("api/grid")]
public class EditableGridController : ControllerBase
{
    private readonly IEditableGridService _gridService;
    private readonly ILogger<EditableGridController> _logger;

    public EditableGridController(
        IEditableGridService gridService,
        ILogger<EditableGridController> logger)
    {
        _gridService = gridService;
        _logger = logger;
    }

    /// <summary>
    /// Get grid data
    /// </summary>
    [HttpGet("{endpoint}")]
    public async Task<ActionResult<GridDataResponse>> GetData(
        [FromRoute] string endpoint,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = "asc",
        [FromQuery] string? filter = null)
    {
        try
        {
            var request = new GridDataRequest
            {
                Endpoint = endpoint,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Filter = filter,
            };

            var result = await _gridService.GetGridDataAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grid data for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "Failed to get grid data", message = ex.Message });
        }
    }

    /// <summary>
    /// Batch update cells
    /// </summary>
    [HttpPost("{endpoint}/batch-update")]
    public async Task<ActionResult<BatchUpdateResponse>> BatchUpdate(
        [FromRoute] string endpoint,
        [FromBody] BatchUpdateRequest request)
    {
        try
        {
            var result = await _gridService.BatchUpdateAsync(endpoint, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch updating grid data for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "Failed to update grid data", message = ex.Message });
        }
    }

    /// <summary>
    /// Get grid configuration (columns, types, etc.)
    /// </summary>
    [HttpGet("{endpoint}/config")]
    public async Task<ActionResult<GridConfigResponse>> GetConfig([FromRoute] string endpoint)
    {
        try
        {
            var config = await _gridService.GetGridConfigAsync(endpoint);
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grid config for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "Failed to get grid config", message = ex.Message });
        }
    }
}

