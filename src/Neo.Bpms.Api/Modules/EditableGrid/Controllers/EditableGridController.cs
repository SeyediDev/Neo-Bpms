using Microsoft.AspNetCore.Authorization;
using Neo.Bpms.Api.Modules.EditableGrid.Models;
using Neo.Bpms.Api.Modules.EditableGrid.Services;
using Neo.Bpms.Domain.Models.Security.Authentication;

namespace Neo.Bpms.Api.Modules.EditableGrid.Controllers;

[ApiController]
[Route("api/grid")]
[Authorize]
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

    private IdentityUser? GetCurrentUser()
    {
        // Get user from HttpContext
        if (User?.Identity?.IsAuthenticated == true)
        {
            // TODO: Implement user retrieval from HttpContext
            // This should use the same mechanism as BpmsController.GetUser()
        }
        return null;
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

    /// <summary>
    /// Export grid data to Excel
    /// </summary>
    [HttpPost("{endpoint}/export-excel")]
    public async Task<IActionResult> ExportExcel(
        [FromRoute] string endpoint,
        [FromBody] ExcelExportRequest request)
    {
        try
        {
            var fileBytes = await _gridService.ExportToExcelAsync(endpoint, request);
            return File(fileBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"grid-export-{DateTime.UtcNow:yyyy-MM-dd}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting Excel for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "Failed to export Excel", message = ex.Message });
        }
    }

    /// <summary>
    /// Import data from Excel
    /// </summary>
    [HttpPost("{endpoint}/import-excel")]
    public async Task<ActionResult<ExcelImportResponse>> ImportExcel(
        [FromRoute] string endpoint,
        [FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "File is required" });
            }

            var result = await _gridService.ImportFromExcelAsync(endpoint, file);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel for endpoint: {Endpoint}", endpoint);
            return StatusCode(500, new { error = "Failed to import Excel", message = ex.Message });
        }
    }
}

