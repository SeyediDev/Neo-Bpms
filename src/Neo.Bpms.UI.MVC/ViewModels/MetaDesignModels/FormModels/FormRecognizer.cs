namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.FormModels;

public class FormRecognizer
{
    public FormRecognizer(Form form)
    {
        namespaceId = form.NamespaceId;
        entityId = form.EntityId;
        id = form.Id;
        name = form.Name;
        type = form.FormType;
        subject = form.FormSubjectId;
        hasAnyField = form.formFields?.Any() ?? false;
        filterKey = $"{id} {name} {subject}";
    }

    public string entityId { get; set; }
    public string namespaceId { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public Form.eFormType type { get; set; }
    public string subject { get; set; }
    public bool hasAnyField { get; set; }
    public string filterKey { get; set; }
}
