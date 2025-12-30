using Neo.Bpms.Api.Modules.PowerBIDashboard.Models;

namespace Neo.Bpms.Api.Modules.PowerBIDashboard.Services;

/// <summary>
/// Service for managing data models and transformations (PowerBI-like data layer)
/// </summary>
public interface IDataModelService
{
    /// <summary>
    /// Get data from a data source
    /// </summary>
    Task<DataTable> GetDataAsync(DataSourceRequest request);

    /// <summary>
    /// Apply transformations to data
    /// </summary>
    Task<DataTable> TransformDataAsync(DataTable source, TransformationRequest transformation);

    /// <summary>
    /// Aggregate data
    /// </summary>
    Task<DataTable> AggregateDataAsync(DataTable source, AggregationRequest aggregation);

    /// <summary>
    /// Create calculated field
    /// </summary>
    Task<DataTable> AddCalculatedFieldAsync(DataTable source, CalculatedField calculatedField);

    /// <summary>
    /// Filter data
    /// </summary>
    Task<DataTable> FilterDataAsync(DataTable source, FilterRequest filter);

    /// <summary>
    /// Group data
    /// </summary>
    Task<DataTable> GroupDataAsync(DataTable source, GroupRequest group);

    /// <summary>
    /// Join multiple data sources
    /// </summary>
    Task<DataTable> JoinDataAsync(JoinRequest join);
}

