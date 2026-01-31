using System.Net;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms;

namespace Neo.Bpms.UI.MVC.Controllers;

public partial class FormController
{
    private const int MaximumPageSize = 200;

    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Api(CancellationToken cancellationToken, string NamespaceId,
        string EntityId, string Id, string sort, int page, int pageSize,
        [FromBody] Dictionary<string, string> bodyDictionary,
        [FromQuery] Dictionary<string, string> queryDictionary)
    {
        try
        {
            Form.eFormType? requiredFormType = GetFormType(Id);
            (Form form, IdentityUser user, CommonFormStructure structure, PersistenceObject po) formAndThings = await GetFormAndThings(NamespaceId, EntityId, null, requiredFormType, "RestApi");
            IdentityUser user = formAndThings.user;
            Form form = formAndThings.form;
            CommonFormStructure structure = formAndThings.structure;

            switch (requiredFormType)
            {
                case Form.eFormType.Detail:
                    (ElasticObject record, string ids) re = await GetRecord(Id, formAndThings.form, user, CultureHelper.GetCurrentNeutralCulture(),
                        structure, null, cancellationToken);
                    ElasticObject result = re.record;
                    Id = re.ids;
                    if (result.Id == 0)
                        return NotFound();
                    return Ok(GetFromFieldsFromRecord(form, result));
                case Form.eFormType.Index:
                    switch (pageSize)
                    {
                        case > MaximumPageSize:
                            return BadRequest($"Page size has a max-limit of {MaximumPageSize}");
                        case > 0:
                            _recordsPerPage = pageSize;
                            break;
                    }

                    ElasticObject filter = queryDictionary.ToElastic();
                    IndexFormData records = await GetIndexRecords(cancellationToken, form, structure,
                        CultureHelper.GetCurrentNeutralCulture(),
                        filter, sort, page, user);
                    Response.Headers["x-total-count"] = records.recordCount.ToString();
                    IEnumerable<ElasticObject> rows = records.Rows.Select(o => GetFromFieldsFromRecord(form, o));
                    return Ok(new
                    {
                        rows,
                        PageSize = _recordsPerPage,
                        TotalCount = records.recordCount
                    });
                case Form.eFormType.Edit:
                case Form.eFormType.VirtualDelete:
                    // NOTE: the following 5 lines should not be needed. Instead, apply utility should be smart enough to not update some virtually deleted record.
                    (ElasticObject record, string ids) r = await GetRecord(Id, form, user, CultureHelper.GetCurrentNeutralCulture(),
                        structure, null, cancellationToken);
                    ElasticObject o = r.record;
                    Id = r.ids;
                    if (o.Id == 0)
                        return NotFound();
                    goto case Form.eFormType.Create;
                case Form.eFormType.Create:
                    ElasticObject record = bodyDictionary?.ToElastic();
                    PostFormData submitResult = await SubmitForm(form, Id, record, user, null, null, null,
                        false, structure, cancellationToken);
                    ExceptionInfos errors = submitResult.Errors;
                    if (errors != null)
                    {
                        foreach (ExceptionInfo error in errors)
                        {
                            return Problem(error.Exception.Message, null, 400, error.ForField);
                        }
                    }

                    return SuccessStatus(requiredFormType, submitResult);
                default: throw new ArgumentOutOfRangeException($"{requiredFormType} is not supported ");
            }
        }
        catch (UnauthenticatedUserException)
        {
            return StatusCode(401);
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403);
        }
        catch (HttpException e)
        {
            Logger.LogError(e, "{message}", e.Message);
            return StatusCode(e.ErrorCode);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{message}", e.Message);
            return StatusCode(500);
        }
    }

    private ElasticObject GetFromFieldsFromRecord(Form form, ElasticObject record)
    {
        FormField.Type validFieldTypes = ValidFieldTypes(form);
        ElasticObject result = new();
        foreach (FormField field in form.formFields.Where(f => f.FieldOrControlType.In(validFieldTypes)))
        {
            if (record.GetField(field.Id, out object value))
            {
                result[field.Id] = value;
            }
        }

        return result;
    }

    private FormField.Type ValidFieldTypes(Form form)
    {
        return form.FormType switch
        {
            Form.eFormType.Index => FormField.Type.ColumnField,
            Form.eFormType.Edit or Form.eFormType.Create or Form.eFormType.VirtualDelete or Form.eFormType.Detail => FormField.Type.Field,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    private ActionResult SuccessStatus(Form.eFormType? requiredFormType, PostFormData submitResult)
    {
        return requiredFormType switch
        {
            Form.eFormType.VirtualDelete => NoContent(),
            Form.eFormType.Create => Created(
                                $"{Request.GetBaseUrl()}/api/{submitResult.Structure.NamespaceId}/{submitResult.Structure.EntityId}/{submitResult.EntityPkv}", null),
            Form.eFormType.Edit => Ok(),
            _ => throw new ArgumentOutOfRangeException($"{requiredFormType} is not supported "),
        };
    }

    private Form.eFormType? GetFormType(string Id)
    {
        return Request.Method switch
        {
            "POST" => (Form.eFormType?)Form.eFormType.Create,
            "PUT" => (Form.eFormType?)Form.eFormType.Edit,
            "DELETE" => (Form.eFormType?)Form.eFormType.VirtualDelete,
            "GET" => (Form.eFormType?)(string.IsNullOrEmpty(Id) ? Form.eFormType.Index : Form.eFormType.Detail),
            _ => throw new HttpException(HttpStatusCode.BadRequest, "Method not supported"),
        };
    }
}
