using Neo.Bpms.Api.Modules.EditableGrid.Models;

namespace Neo.Bpms.Api.Modules.EditableGrid.Services;

public interface IEditableGridService
{
    Task<GridDataResponse> GetGridDataAsync(GridDataRequest request);
    Task<BatchUpdateResponse> BatchUpdateAsync(string endpoint, BatchUpdateRequest request);
    Task<GridConfigResponse> GetGridConfigAsync(string endpoint);
    Task<byte[]> ExportToExcelAsync(string endpoint, ExcelExportRequest request);
    Task<ExcelImportResponse> ImportFromExcelAsync(string endpoint, IFormFile file);
}

