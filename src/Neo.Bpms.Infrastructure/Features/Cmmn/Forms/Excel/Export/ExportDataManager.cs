namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Export;

public class ExportDataManager(ElasticObject filterValues, IdentityUser user,
    CancellationToken cancellationToken, string culture, Dictionary<string, FormSheet> formSheets)
{
    public Dictionary<string, FormExportData> Data { get; set; }
    private readonly FormSheet _mainFormSheet = formSheets.Values.FirstOrDefault(s => !s.IsChild);

    private CommonFormStructure Structure => _mainFormSheet.Structure;
    private Form Form => _mainFormSheet.Form;

    public void QueryData(FormDataRoutines formDataRoutines)
    {
        CreateDataObject();
        LocalParameters lp = GetLocalParameters(user, null);
        IndexFormData mainFormData = GetMainFormData(formDataRoutines, lp);
        GetSubFormsData(formDataRoutines, mainFormData, lp);
    }

    IList<ElasticObject> GetPage(IList<ElasticObject> list, int page)
    {
        int pageSize = 1000;
        return [.. list.Skip(page * pageSize).Take(pageSize)];
    }

    private void GetSubFormsData(FormDataRoutines formDataRoutines, IndexFormData mainFormData, LocalParameters lp)
    {
        foreach (KeyValuePair<string, FormExportData> tableData in Data)
        {
            if (tableData.Value.IsMainForm) continue;
            TableDefinition table = Structure.Tables.FirstOrDefault(t => t.FieldName == tableData.Key);
            if (table == null) continue;
            List<string> parentIds = [];
            int page = 0;
            while (GetPage(mainFormData.Rows, page).Any())
            {
                foreach (ElasticObject record in GetPage(mainFormData.Rows, page))
                {
                    LocalParameters localParam = lp.Clone();
                    localParam.AddOrUpdate("q", record);
                    string parentId = record.GetString("Id");
                    if (!string.IsNullOrEmpty(table.TableDef.AssociationId))
                    {
                        EntityField associationField = ProjectDefinition.Project.GetEntity(table.NamespaceId, table.EntityId)
                            ?.GetField(table.TableDef.AssociationId);
                        parentId = associationField?.AssociationEntity?.Maps != null &&
                                   associationField.AssociationEntity.Maps.Count > 0
                            ? record[associationField.AssociationEntity.Maps[0].SourceField]?.ToString()
                            : record[table.TableDef.AssociationId]?.ToString();
                    }

                    parentIds.Add(parentId);
                }
                List<ElasticObject> tableRecords = formDataRoutines.GetSubTableRecords(parentIds, culture, lp, table, user, true, true, cancellationToken);
                if (tableRecords != null && tableRecords.Count != 0)
                {
                    tableData.Value.Result.Rows.AddRange(tableRecords);
                }
                page++;
            }

            tableData.Value.Result.recordCount = tableData.Value.Result.Rows.Count;
        }
    }

    private IndexFormData GetMainFormData(FormDataRoutines formDataRoutines, LocalParameters lp)
    {
        IndexFormData records = FormDataRoutines.GetRecords(Structure, Form.entity, Form, culture, filterValues,
            null, lp, null, user, true, null, null, cancellationToken);
        FormExportData mainForm = Data.Values.FirstOrDefault(d => d.IsMainForm) ?? throw new Exception();
        mainForm.Result = records;
        return mainForm.Result;
    }

    private void CreateDataObject()
    {
        Data = [];
        foreach (KeyValuePair<string, FormSheet> formSheet in formSheets)
        {
            FormExportData data = new(!formSheet.Value.IsChild) { Result = new IndexFormData() };
            Data.Add(formSheet.Key, data);
        }
    }

    private static LocalParameters GetLocalParameters(IdentityUser user, ElasticObject record)
    {
        LocalParameters lp = new() { { "user", user }, { "userId", user?.Id } };
        if (record != null)
            lp.Add("q", record);
        return lp;
    }
}
