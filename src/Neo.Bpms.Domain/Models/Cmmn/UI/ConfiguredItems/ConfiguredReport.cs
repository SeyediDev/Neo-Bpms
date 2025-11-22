using System.Text.Json.Serialization;
using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

namespace Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

/// <summary>
/// Configured Report
/// </summary>
public class ConfiguredReport : ConfiguredItem
{
    /// <summary>
    /// Configured Report
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="viewType"></param>
    /// <param name="id">id</param>
    /// <param name="name">Name</param>
    /// <returns></returns>
    public ConfiguredReport(Report report, ReportViewType viewType, string id, string name)
    {
        ViewType = viewType;
        ConfigId = id;
        Name = name;
        Report = report;
        IsPublic = true;
        IsDefault = false;
        IsMeta = false;
    }

    public ConfiguredReport()
    {
    }

    [JsonIgnore]
    public Report Report { get; set; }
    [JsonIgnore]
    public string UniqueId => $"{Report?.NamespaceId}.{Report?.EntityId}.{Report?.Id}.{ConfigId}";
    [JsonIgnore]
    public string ConfigId { get; set; }
    public ChartType ChartType { get; set; } = ChartType.Line;
    public ReportViewType ViewType { get; set; }
    public bool IsMeta { get; set; }

    public string SortFields { get; set; }
    public string HavingCondition { get; set; }
    public string WhereCondition { get; set; }
    public int RecordsPerPage { get; set; }

    public ConcurrentDictionary<string, SelectedField> Fields { get; set; } =
        new ConcurrentDictionary<string, SelectedField>();

    public ConcurrentDictionary<string, ConfiguredSubReport> SubReports =
        new();

    public ConfiguredSubReport Parent { get; set; }
    public string RangeId { get; set; }
    public ConfigRange Range { get; set; }
    public List<ConfigRange> Levels { get; set; }
    public List<ConfigProperty> Properties { get; set; }
    public List<ConditionalFormatting> Formats { get; set; }
    public List<ConfiguredScheduledReport> ScheduledReports { get; set; }

    [JsonIgnore]
    public eConfigAccessLevel AccessLevel => !IsPublic
        ? eConfigAccessLevel.Private
        : Roles != null && Roles.Any()
            ? eConfigAccessLevel.UserGroup
            : eConfigAccessLevel.Public;

    [JsonIgnore]
    public bool IsSubReport => Parent != null;
    [JsonIgnore]
    public bool IsInlineSubReport => Parent?.Type == Report.SubReportType.SubReport;

    public GroupByViewType GroupByViewType { get; set; }

    /// <summary>
    /// Add Field
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="fieldId">field</param>
    /// <param name="associationName"></param>
    /// <param name="alias">Alias</param>
    /// <param name="width"></param>
    /// <param name="hAligin"></param>
    /// <param name="vAlign"></param>
    /// <param name="formula"></param>
    /// <param name="isTooltip"></param>
    /// <param name="matrixType"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public SelectedField AddField(eFieldSelectionType type, string fieldId, string entityId,
        string associationName, string alias, string formula,
        bool isTooltip, ReportMatrixType matrixType, double width = 0,
        eHAlign hAligin = eHAlign.Center,
        eVAlign vAlign = eVAlign.Top)
    {
        SelectedField fld = new()
        {
            Alias = alias,
            entityId = entityId,
            AssociationName = associationName,
            fieldId = fieldId,
            formula = formula,
            type = type,
            width = width,
            HAlign = hAligin,
            VAlign = vAlign,
            IsTooltip = isTooltip,
            MatrixType = matrixType,
            Order = (Fields.Values.Count > 0 ? Fields.Values.Max(f => f.Order) : 0) + 1
        };
        Fields.TryAdd(fld.selectedFieldId, fld);
        return fld;
    }

    /// <summary>
    /// Add Sub Report
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="subConfig"></param>
    /// <returns></returns>
    public ConfiguredSubReport AddSubReport(Report.SubReportType type, ConfiguredReport subConfig)
    {
        if (subConfig?.Report == null) return null;
        ConfiguredSubReport configuredSubReport = new()
        {
            ParentConfiguredReport = this,
            ConfiguredReport = subConfig,
            NamespaceId = subConfig.Report.NamespaceId,
            EntityId = subConfig.Report.EntityId,
            SubReportId = subConfig.Report.Id,
            Type = type
        };
        subConfig.Parent = configuredSubReport;
        SubReports ??= new ConcurrentDictionary<string, ConfiguredSubReport>();
        SubReports.TryAdd(subConfig.ConfigId, configuredSubReport);
        return configuredSubReport;
    }

    /// <summary>
    /// Add Conditional Formatting
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="className">Class Name</param>
    /// <param name="columnName">Column Name</param>
    /// <returns></returns>
    public ConditionalFormatting AddConditionalFormatting(string filter, string className, string columnName = null)
    {
        Formats ??= [];
        var format = new ConditionalFormatting
        {
            ClassName = className,
            ColumnName = columnName,
            Filter = filter,
            Type = columnName == null ? ConditionalFormatting.eType.Row : ConditionalFormatting.eType.Column
        };
        Formats.Add(format);
        return format;
    }

    public void AddProperty(ReportConfigProperty propertyId, string value)
    {
        AddProperty((long)propertyId, value);
    }
    public void AddProperty(long propertyId, string value)
    {
        Properties ??= [];

        Properties.Add(new ConfigProperty()
        {
            PropertyId = propertyId,
            Value = value
        });
    }

    public SelectedField GetSelectedFieldByName(string columnName)
    {
        SelectedField field = null;
        foreach (KeyValuePair<string, SelectedField> item in Fields.OrderBy(f => f.Value.Order))
        {
            string key = item.Key.Split('$')[1];
            if (key != columnName) continue;
            field = item.Value;
            break;
        }

        return field;
    }

    public enum eConfigAccessLevel
    {
        Private,
        Public,
        UserGroup
    }

    public enum eFieldSelectionType
    {
        asColumn = 1,
        asGroupBy = 2,
        asFilter = 5,

        asAscending = 11,
        asDescending,

        asAggregation_Sum = 21,
        asAggregation_Average,
        asAggregation_Min,
        asAggregation_Max,
        asAggregation_First,
        asAggregation_Last,
        asAggregation_SD,
        asAggregation_Mode,
        asAggregation_Trend,
        asAggregation_Count,
        asAggregation,
    }

    public enum eHAlign
    {
        Left = 1,
        Center,
        Right,
    }

    public enum eVAlign
    {
        Top = 1,
        Middle,
        Bottom,
    }

    public enum ReportMatrixType
    {
        Horizontal = 1,
        Vertical = 2,
    }

    public class SelectedField
    {
        public string entityId { get; set; }
        public string AssociationName { get; set; }
        public string fieldId { get; set; }
        public string formula { get; set; }
        public eFieldSelectionType type { get; set; }
        public object filterValue { get; set; }
        public string Alias { get; set; }
        public double width { get; set; }
        public eHAlign HAlign { get; set; }
        public eVAlign VAlign { get; set; }
        public bool ltrDirection { get; set; }
        public long Order { get; set; }
        public bool IsTooltip { get; set; }
        [JsonIgnore]
        public string selectedFieldId => type + "$" + fieldId + "$" + entityId + "$" + AssociationName;
        public ReportMatrixType? MatrixType { get; set; }
        public List<ColumnFilterDefinition> FieldFilters { get; set; } = null;
        public List<FormProperty> Properties { get; set; }

        public ColumnFilterDefinition AddFilter(ColumnFilterDefinition.eOperator Operator, string Operand)
        {
            ColumnFilterDefinition columnFilterDefinition = new()
            {
                Operator = Operator,
                Operand = Operand
            };
            FieldFilters ??= [];
            FieldFilters.Add(columnFilterDefinition);
            return columnFilterDefinition;
        }

        public void AddProperty(eControlPropertyId propertyId, object value)
        {
            Properties ??= [];
            Properties.Add(new FormProperty(propertyId, value));
        }

        public SelectedField Clone()
        {
            SelectedField sf = new()
            {
                type = type,
                entityId = entityId,
                fieldId = fieldId,
                Alias = Alias,
                AssociationName = AssociationName,
                IsTooltip = IsTooltip,
                formula = formula,
                width = width,
                ltrDirection = ltrDirection,
                HAlign = HAlign,
                MatrixType = MatrixType,
                Order = Order,
                VAlign = VAlign,
                filterValue = filterValue
            };
            if (FieldFilters != null)
            {
                foreach (ColumnFilterDefinition fieldFilter in FieldFilters)
                {
                    ColumnFilterDefinition ff = fieldFilter.Clone();
                    sf.FieldFilters.Add(ff);
                }
            }

            if (Properties != null)
            {
                sf.Properties = [];
                foreach (FormProperty property in Properties)
                {
                    sf.Properties.Add(new FormProperty(property.PropertyId, property.Value));
                }
            }

            return sf;
        }
    }

    public class ConfiguredSubReport
    {
        public ConfiguredReport ParentConfiguredReport { get; set; }
        public ConfiguredReport ConfiguredReport { get; set; }
        public Report.SubReportType Type { get; set; }
        public string NamespaceId { get; set; }
        public string EntityId { get; set; }
        public string SubReportId { get; set; }
        public string AssociationName { get; set; }
        public string DashboardId { get; set; }
        public string DashboardConfigId { get; set; }

        public ConfiguredSubReport Clone(ConfiguredReport parent, string dashboardConfigId)
        {
            ConfiguredSubReport csr = new()
            {
                Type = Type,
                NamespaceId = NamespaceId,
                EntityId = EntityId,
                DashboardId = DashboardId,
                AssociationName = AssociationName,
                SubReportId = SubReportId,
                DashboardConfigId = dashboardConfigId,

                ParentConfiguredReport = parent
            };
            csr.ConfiguredReport = ConfiguredReport.Clone(csr, null);
            return csr;
        }
    }

    public class ConfigRange
    {
        public string RangeId { get; set; }
        public ValueType MinType { get; set; }
        public ValueType MaxType { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Color { get; set; }
        public enum ValueType
        {
            Fixed = 1,
            Field
        }

        public ConfigRange Clone()
        {
            return new ConfigRange
            {
                RangeId = Guid.NewGuid().ToString(),
                MinValue = MinValue,
                MaxValue = MaxValue,
                Color = Color,
                MaxType = MaxType,
                MinType = MinType
            };
        }
    }

    public class ConfigProperty
    {
        public long PropertyId { get; set; }
        public string Value { get; set; }
    }

    public ConfiguredReport Clone(ConfiguredSubReport parent, string parentDashboardConfigId)
    {
        ConfiguredReport config = new()
        {
            ConfigId = Guid.NewGuid().ToString(),
            ChartType = ChartType,
            WhereCondition = WhereCondition,
            Formats = Formats,
            HavingCondition = HavingCondition,
            IsMeta = IsMeta,
            Report = Report,
            ViewType = ViewType,
            RecordsPerPage = RecordsPerPage,
            Name = Name,
            IsDefault = IsDefault,
            IsPublic = IsPublic,
            FolderId = FolderId,
            Roles = Roles,
            UserId = UserId,
            Parent = parent,
            SortFields = SortFields,

            ScheduledReports = null,
            Properties = Properties
        };
        if (Fields != null)
        {
            config.Fields = new ConcurrentDictionary<string, SelectedField>();
            foreach (SelectedField f in Fields.Values)
            {
                SelectedField field = f.Clone();
                config.Fields.TryAdd(field.selectedFieldId, field);
            }
        }
        if (SubReports != null)
        {
            config.SubReports = new ConcurrentDictionary<string, ConfiguredSubReport>();
            foreach (ConfiguredSubReport r in SubReports.Values)
            {
                ConfiguredSubReport sr = r.Clone(config, parentDashboardConfigId);
                config.SubReports.TryAdd(sr.ConfiguredReport.ConfigId, sr);
            }
        }
        if (Range != null)
        {
            config.Range = Range.Clone();
            RangeId = config.Range.RangeId;
        }
        if (Levels != null)
        {
            config.Levels = [];
            foreach (ConfigRange level in Levels)
            {
                config.Levels.Add(level.Clone());
            }
        }
        return config;
    }
}
