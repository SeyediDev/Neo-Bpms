using Neo.Bpms.Domain.Entities.Cmmn.Relationship;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader.MetaEntity;

public static class ProjectEntityForm
{
    private const string Folder = "Forms";
    private const string TemplateFolder = "TemplateForms";
    private static string FolderPath(string entityPath)
    {
        return $"{entityPath}\\{Folder}";
    }

    private static string TemplateFolderPath(string entityPath)
    {
        return $"{entityPath}\\{TemplateFolder}";
    }

    private static string FileName(Form form)
    {
        return $"{form.Id}.xml";
    }

    public static string TemplateFileName(Form form)
    {
        return $"{TemplateFolderPath(ProjectEntity.EntityPath(form.Entity))}\\{form.Id}\\{form.Id}.html";//todo
    }

    public static void LoadAll(UiEntity entity, string entityPath)
    {
        string folder = FolderPath(entityPath);
        if (Directory.Exists(folder))
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folder);
            foreach (string file in files)
            {
                Load(entity, folder, file);
            }
        }

        string templateFolder = TemplateFolderPath(entityPath);
        if (Directory.Exists(templateFolder))
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(templateFolder);
            foreach (string dir in directories)
            {
                SetFormTemplate(entity, folder, dir);
            }
        }
    }

    public static void SaveAllForms(UiEntity uiEntity, string entityPath)
    {
        foreach (Form form in uiEntity.getForms() ?? [])
        {
            Save(form, entityPath);
        }
    }

    private static void Load(UiEntity entity, string folder, string fileName)
    {
        string id = ProjectMetaFile.FetchFileName(fileName);
        Form form = XMLUtill.GetObjectFromXmlFile<Form>(fileName);
        if (form == null)
        {
            _ = Directory.CreateDirectory($"{folder}\\InvalidForms");
            File.Move(fileName, $"{folder}\\InvalidForms\\{id}.xml");
            return;
        }
        ReConfig(form, entity);
        entity.AddForm(form, true);
    }

    private static void SetFormTemplate(UiEntity entity, string folder, string dir)
    {
        string id = ProjectMetaFile.LastDirectoryName(dir);
        Form form = entity.GetEntityForm(id);
        if (form == null)
        {
            _ = Directory.CreateDirectory($"{folder}\\InvalidTemplateForms");
            File.Move(dir, $"{folder}\\InvalidTemplateForms");
            return;
        }

        form.HasTemplateFile = true;
    }

    public static void Save(Form form, string entityPath = null)
    {
        string folder = FolderPath(!string.IsNullOrEmpty(entityPath) ? entityPath : ProjectEntity.EntityPath(form.entity));
        ProjectMetaFile.SaveObjectToFile(form, folder, FileName(form));
    }

    public static void Remove(string namespaceId, string entityId, string id)
    {
        UiEntity entity = ProjectDefinition.Project.GetUiEntity(namespaceId, entityId);
        Form form = entity?.getForm(id);
        if (form == null)
        {
            return;
        }

        string folder = FolderPath(ProjectEntity.EntityPath(form.entity));
        //ProjectMetaFile.DeleteItem(folder, id, form.Name, entity.DeleteForm);
        _ = entity.DeleteForm(id);
        _ = ProjectMetaFile.DeletePhysicalFile(folder, $"{id}.xml");
    }

    public static void ReConfigForm(Form form, Entity entity)
    {
        DataOperationRuntime.ReConfig(form.DataOperation);
        foreach (FormField formField in form.formFields ?? Enumerable.Empty<FormField>())
        {
            formField.Field = form.GetEntityField(formField.Id);
            if (string.IsNullOrEmpty(formField.TableEntityId))
            {
                continue;
            }

            if (GetSubTable(form, formField.TableEntityId, formField.TableAssociationId,
                    out UiEntity tableEntity, out Association tableAssociation))
            {
                formField.TableEntity = tableEntity;
                formField.TableAssociation = tableAssociation;
            }
        }
    }
    public static void ReConfig(Form form, Entity entity)
    {
        form.Parent = entity;
        foreach (EntityFormFilter filter in form.InputRecordsFilters ?? Enumerable.Empty<EntityFormFilter>())
        {
            if (filter.condition != null)
            {
                Parser.ParseTree(filter.condition);
            }

            if (filter.filter != null)
            {
                Parser.ParseTree(filter.filter);
            }
        }
        ReConfigForm(form, entity);
    }
    private static bool GetSubTable(Form form, string entityId,
        string association, out UiEntity uiEntity, out Association aso)
    {
        Entity entity = form.entity.model.GetEntity(entityId)
                          ?? ProjectDefinition.Project.GetEntityByEntityId(entityId);
        uiEntity = entity as UiEntity;
        aso = null;
        if (uiEntity == null)
        {
            return false;
        }

        if (association != null)
        {
            aso = entity.GetAssociation(association);
        }
        else
        {
            //find the first association to this entity
            foreach (Association item in entity.Associations)
            {
                if (item.DestEntityId != entity.Id)
                {
                    continue;
                }

                aso = item;
                break;
            }
        }
        return aso != null;
    }
}
