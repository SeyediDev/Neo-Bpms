namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.FormModels;

public class FormViewModel
{
    public FormViewModel()
    {
    }

    public FormViewModel(Form form)
    {
        namespaceId = form.NamespaceId;
        entityId = form.EntityId;
        id = form.Id;
        name = form.Name;
        type = form.FormType;
        subject = form.FormSubjectId;
        filters = form.InputRecordsFilters?.Select(f => new FilterViewModel(f));
        autoSaves = form.DataOperation?.AutoCalcs?.Calculations?.Select(ac => new AutoSaveViewModel(ac));
        validations = form.DataOperation?.Validations?.Select(v => new ValidationViewModel(v));
        outputStateId = form.DataOperation?.outputStateId ?? 0;
    }

    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public Form.eFormType type { get; set; }
    public string subject { get; set; }
    public IEnumerable<FilterViewModel> filters { get; set; }
    public IEnumerable<AutoSaveViewModel> autoSaves { get; set; } // todo ui
    public IEnumerable<ValidationViewModel> validations { get; set; } // todo ui
    public int outputStateId { get; set; }

    public void Modify(Form entityForm)
    {
        entityForm.Id = id;
        entityForm.FormType = type;
        entityForm.Name = name;
        entityForm.FormSubjectId = subject;
        entityForm.InputRecordsFilters = filters?.Select(f => f.ToFilter()).ToList();
        entityForm.SetFormDateOperation();
        if (entityForm.DataOperation != null)
        {
            if (autoSaves != null)
            {
                entityForm.DataOperation.InitAutoCalcs();
                autoSaves.ToList().ForEach(a => entityForm.DataOperation.AutoCalcs.AddAutoCalc(a.ToAutoCalc()));
            }

            validations?.ToList().ForEach(v => entityForm.DataOperation.AddValidation(v.ToValidation()));
            entityForm.DataOperation.outputStateId = outputStateId;
        }
    }
}
