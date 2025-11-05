using Neo.Bpms.Domain.Expressions;
using File = System.IO.File;

namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public partial class ControlsRenderer
{
    protected virtual string RenderTemplate(ControlsRendererData data)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(data.Structure.NamespaceId, data.Structure.EntityId);
        Form form = entity?.GetEntityForm(data.Structure.Form_ReportId)
            ?? entity?.GetReport(data.Structure.Form_ReportId);
        string fileAddress = ProjectEntityForm.TemplateFileName(form);
        if (!File.Exists(fileAddress))
            return null;
        FileInfo fi = new(fileAddress);
        if (fi.Length == 0)
            return null;
        using (FileStream fileStream = new(fileAddress, FileMode.Open))
        {
            XHtmlTemplateMapper xHtmlTemplateMapper = new(fileStream, data.Record);
            xHtmlTemplateMapper.Render();
            return xHtmlTemplateMapper.ToString();
        }
    }
}
