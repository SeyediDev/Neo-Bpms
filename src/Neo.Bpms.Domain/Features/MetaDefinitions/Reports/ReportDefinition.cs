namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

/// <summary>
/// Base class to define reports and their details. All report definitions in the business and meta models are sub classes of this object.
/// </summary>
public abstract class ReportDefinition : FormDefinition
{
    protected Report report { get; set; }
    protected Report.ReportFields reportFields { get; set; }

    /// <summary>
    /// Define All
    /// </summary>
    /// <param name="uiEntity">entity</param>
    /// <param name="definition"></param>
    /// <returns></returns>
    public Report DefineReport(UiEntity uiEntity, EntityDefinition definition)
    {
        entityDefinition = definition;
        entity = uiEntity;
        report = IdentifyReport();
        if (report == null) return null;
        entity.AddReport(report);
        report.Roles = Roles;

        DefineFilters();
        Filters();
        DefineDataSources();
        DataSources();
        DefineGroupBy();
        DefineUIRules();
        UIRules();
        DefineReportLayouts();
        ReportLayouts();
        DefineConfigs();
        PossibleSubReports();
        return report;
    }

    /// <summary>
    /// Define Report
    /// </summary>
    /// <param name="enName">enName</param>
    /// <param name="name">name</param>
    /// <returns></returns>
    protected Report DefineReport(string enName, string name
        /*, eReportPosition position = eReportPosition.PageContent , Report.eFlags Flags = Report.eFlags.vfPortrait*/)
    {
        report = entity.GetReport(GetType().Name) ??
            new Report(entity, GetType().Name, name, enName /*, position, Flags*/); //,TViewStyle Style
        form = report;
        reportFields = report.reportFields;
        return report;
    }

    /// <summary>
    /// Define Report
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected Report DefineReport(string name
        /*, eReportPosition position = eReportPosition.PageContent, Report.eFlags Flags = Report.eFlags.vfPortrait*/)
    {
        return DefineReport(GetType().GetDisplayableName(), name/*, position , Flags */);
    }

    protected Report SelectReport<TReport>()
    {
        report = entity.GetReport(typeof(TReport).Name);
        form = report;
        reportFields = report.reportFields;
        return report;
    }

    private Report.Field _currentField;

    /// <summary>
    /// Add Column field(s) to the report
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <returns></returns>
    public new bool AddColumns(params string[] fieldId)
    {
        foreach (var fldId in fieldId)
        {
            var field = report.GetEntityField(fldId);
            bool asGroupBy = false, asAggregation = false;
            if (field != null)
            {
                if (field.IsDoubleParam() || (field.IsLongParam() && !field.IsEnum) || field.IsTimeSpan() || field.IsDateTime() )
                {
                    asAggregation = true;
                }
                if (field.IsBoolParam() || field.IsTextParam() || field.IsEnum || field.IsForeignParam() )
                {
                    asGroupBy = true;
                }
            }
            _currentField = Report.AddColumn(reportFields, fldId, asGroupBy, asAggregation);
            if (_currentField == null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Adds Column Field To The Report
    /// </summary>
    /// <param name="fieldId">field Id</param>
    /// <returns></returns>
    protected bool AddColumn(string fieldId, string alias = null)
    {
        var field = report.GetEntityField(fieldId);
        return AddColumn(field, alias);
    }

    private bool AddColumn(EntityField field, string alias)
    {
        if (field == null) return false;
        bool asGroupBy = false, asAggregation = false;
        if (field.Id!="Id" && (field.IsDoubleParam() || field.IsLongParam() || field.IsTimeSpan() || field.IsDateTime()))
        {
            asAggregation = true;
        }
        if (field.Id != "Id" && (field.IsBoolParam() || field.IsTextParam() || field.IsForeignParam() || field.IsEnum ))
        {
            asGroupBy = true;
        }
        _currentField = Report.AddColumn(reportFields, field.Id, asGroupBy, asAggregation);
        if (string.IsNullOrEmpty(alias))
        {
            _currentField.alias = alias;
        }
        return _currentField != null;
    }

    protected override void AddColumnInAddAll(EntityField field, eControlTypeId control)
    {
        AddColumn(field.Id, control);
        if (field.Required ||
            (field.AssociationEntity?.Maps?.Any(m => entity.GetField(m.SourceField)?.Required ?? false) ?? false))
            AddFieldProperty(eControlPropertyId.Required, true);
    }

    /// <summary>
    /// Adds Group By Field To The Report
    /// </summary>
    /// <param name="fieldId">field Id</param>
    /// <param name="controlProperty"></param>
    /// <param name="addInFilter"></param>
    /// <returns></returns>
    protected bool AddGroupByField(string fieldId, eControlPropertyId? controlProperty = null,
        bool addInFilter = true)
    {
        if (string.IsNullOrEmpty(fieldId)) return false;
        if (addInFilter)
        {
            var id = (_currentIncludeEntity != null ? _currentIncludeEntity.AssociationId + "." : "") + fieldId;
            var filterField = form?.formFields.FirstOrDefault(f =>
                f.Id == id && string.IsNullOrEmpty(f.TableEntityId) &&
                f.FieldOrControlType == FormField.Type.FilterField);
            if (filterField == null)
            {
                var field = entity.GetField(id);
                if (id.Contains(".") ||
                    field != null && field.Formula == null && field.FieldType != TVariableTypes.Date)
                {
                    AddFilterField(id);
                }
            }
        }

        _currentField = Report.AddGroupBy(reportFields, fieldId);
        if (_currentField == null)
            return false;
        if (controlProperty != null)
            AddFieldProperty(controlProperty.Value, true);
        return true;
    }

    /// <summary>
    /// Add GroupBy Field(s) to the report
    /// </summary>
    /// <param name="fieldsIds">field</param>
    /// <returns></returns>
    public bool AddGroupByFields(params string[] fieldsIds)
    {
        foreach (var fieldId in fieldsIds)
            AddGroupByField(fieldId);
        return true;
    }

    /// <summary>
    /// Add Aggregation field(s) to the report
    /// </summary>
    /// <param name="fieldId">field</param>
    /// <returns></returns>
    protected bool AddAggregation(params string[] fieldId)
    {
        foreach (var fldId in fieldId)
        {
            _currentField = Report.AddAggregation(reportFields, fldId);
            if (_currentField == null)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="controlProperty">The control property.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    protected bool AddFieldProperty(eControlPropertyId controlProperty, object value)
    {
        if (_currentField == null) return false;
        _currentField.addProperty(new FormProperty(controlProperty, value));
        return true;
    }

    /// <summary>
    /// Adds the input records having.
    /// </summary>
    /// <param name="having">The having.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <returns></returns>
    protected ReportHaving AddHaving(string having, string condition = null, string entityId = null)
    {
        var irf = new ReportHaving
        {
            entityId = entityId,
            filter = Parser.ParseTree(having),
            condition = Parser.ParseTree(condition)
        };
        report.AddInputRecordsHaving(irf);
        return irf;
    }

    private IncludeEntity _currentIncludeEntity;

    /// <summary>
    /// Include Entity
    /// </summary>
    /// <param name="associationId">association Name</param>
    /// <returns></returns>
    protected bool IncludeEntity(string associationId)
    {
        if (report == null) return false;
        _currentIncludeEntity = report.AddIncludeEntity(associationId);
        reportFields = _currentIncludeEntity.reportFields;
        return true;
    }

    /// <summary>
    /// Add Possible Sub Report to be used in report configurations setup
    /// </summary>
    /// <param name="entityName">entity Name</param>
    /// <param name="associationName">association Name</param>
    /// <param name="subReportId">sub Report</param>
    /// <returns></returns>
    protected bool AddPossibleSubReport(string entityName, string associationName, string subReportId)
    {
        return report?.AddPossibleSubReport(entityName, associationName, subReportId) != null;
    }

    /// <summary>
    /// Add Possible Sub Report to be used in report configurations setup
    /// </summary>
    /// <param name="associationName">association Name</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public bool AddPossibleSubReport<TEntity, TReport>(string associationName)
    {
        return report?.AddPossibleSubReport(typeof(TEntity).Name, associationName, typeof(TReport).Name) != null;
    }

    /// <summary>
    /// Add Possible Sub Report to be used in report configurations setup
    /// </summary>
    /// <returns></returns>
    protected bool AddPossibleSubReport<TEntity, TReport, TAssociation>()
    {
        return report?.AddPossibleSubReport(typeof(TEntity).Name, typeof(TAssociation).Name, typeof(TReport).Name) !=
               null;
    }

    /// <summary>
    /// Add All Association Fields To GroupBy
    /// </summary>

    public void AddAllAssociationToGroupBy()
    {
        foreach (var field in report.reportFields.Values.OrderBy(f => f.Id))
        {
            var f = entity.GetField(field.fieldId);
            if (f?.AssociationEntity != null)
            {
                AddGroupByField(f.Id);
            }
        }
    }

    /// <summary>
    /// IdentifyReport
    /// </summary>
    /// 
    /// <returns></returns>
    //protected abstract Report IdentifyReport();
    protected Report IdentifyReport()
    {
        return DefineReport(EnName, Name);
    }

    /// <summary>
    /// Define Report Data Sources
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineDataSources()
    {
        return true;
    }

    /// <summary>
    /// Define Report Data Sources
    /// </summary>
    /// <returns></returns>
    protected virtual void DataSources()
    {
    }

    /// <summary>
    /// Define Report Data Sources
    /// </summary>
    /// <returns></returns>
    protected virtual void PossibleSubReports()
    {
    }

    /// <summary>
    /// Define Report Layouts
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineReportLayouts()
    {
        return true;
    }

    /// <summary>
    /// Define Report Layouts
    /// </summary>
    /// <returns></returns>
    protected virtual void ReportLayouts()
    {
    }

    protected virtual void DefineGroupBy()
    {
    }

    protected sealed override Form Identify()
    {
        return IdentifyReport();
    }

    protected sealed override bool DefineViewModel()
    {
        if (!DefineFilters()) return false;
        Filters();
        if (!DefineDataSources()) return false;
        DataSources();
        return true;
    }

    /// <summary>
    /// Define Configs
    /// </summary>
    /// <returns></returns>
    private void DefineConfigs()
    {
        List<ReportConfigDefinition> configDefinitions = [];
        foreach (var configDefinition in ExtractSubsInstances<ReportConfigDefinition>())
        {
            configDefinition?.DefineAll(report);
            configDefinitions.Add(configDefinition);
        }
        foreach (var configDefinition in configDefinitions)
        {
            configDefinition?.SetSubReports();
        }
    }
}
