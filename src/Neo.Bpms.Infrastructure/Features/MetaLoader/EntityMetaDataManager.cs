using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.MetaLoader;

public class EntityMetaDataManager : MetaDataManager<EntityMetaData>
{
    protected override void SyncMetaData()
    {
        foreach (var model in ProjectDefinition.Project.Namespaces.Values)
        {
            var modelNamespace = FetchModel(model);
            var metaEntities = FetchEntities(modelNamespace, true);
            var backupMetaEntities = FetchEntities(modelNamespace, false);
            foreach (var entity in model.GetEntities().Values)
            {
                var metaEntity =
                    FetchMetaEntity(metaEntities, backupMetaEntities, entity, modelNamespace);
                ProjectDefinition.Entities.TryAdd(entity.DbId, entity);
                var uiEntity = (UiEntity)entity;
                SyncForms(metaEntity, uiEntity);
                SyncReports(metaEntity, uiEntity);
                SyncDashboards(metaEntity, uiEntity);
                SyncEntityFields(metaEntity, uiEntity);
            }
        }
    }

    private void SyncForms(MetaModelEntity metaEntity, UiEntity uiEntity)
    {
        if (uiEntity.getForms() == null)
            return;
        var metaForms = _metaData.Forms.AddOrGetItem(metaEntity.Id);
        var backupMetaForms = _backupMetaData.Forms.AddOrGetItem(metaEntity.Id);
        foreach (var form in uiEntity.getForms())
        {
            FetchPage(metaForms, backupMetaForms, form, MetaModelPageTypeId.Form);
            ProjectDefinition.Forms.TryAdd(form.DbId, form);
        }
    }
    private void SyncReports(MetaModelEntity metaEntity, UiEntity uiEntity)
    {
        if (uiEntity.GetReports() == null)
            return;
        var metaReports = _metaData.Reports.AddOrGetItem(metaEntity.Id);
        var backupMetaReports = _backupMetaData.Reports.AddOrGetItem(metaEntity.Id);
        foreach (var report in uiEntity.GetReports())
        {
            FetchPage(metaReports, backupMetaReports, report, MetaModelPageTypeId.Report);
            ProjectDefinition.Reports.TryAdd(report.DbId, report);
        }
    }
    private void SyncDashboards(MetaModelEntity metaEntity, UiEntity uiEntity)
    {
        if (uiEntity.GetDashboards() == null)
            return;
        var metaDashboards = _metaData.Dashboards.AddOrGetItem(metaEntity.Id);
        var backupMetaDashboards = _backupMetaData.Dashboards.AddOrGetItem(metaEntity.Id);
        foreach (var dashboard in uiEntity.GetDashboards().Values)
        {
            FetchPage(metaDashboards, backupMetaDashboards, dashboard, MetaModelPageTypeId.Dashboard);
            ProjectDefinition.Dashboards.TryAdd(dashboard.DbId, dashboard);
        }
    }
    
    private void SyncEntityFields(MetaModelEntity metaEntity, UiEntity uiEntity)
    {
        if (uiEntity.entityFields == null)
            return;
        var metaFields = _metaData.Fields.AddOrGetItem(metaEntity.Id);
        var backupMetaFields = _backupMetaData.Fields.AddOrGetItem(metaEntity.Id);
        foreach (var field in uiEntity.entityFields.Values)
        {
            FetchField(metaFields, backupMetaFields, field);
            ProjectDefinition.Fields.TryAdd(field.DbId, field);
        }
    }

    protected override void BackupUnusedItems()
    {
        BackupUnusedItems(_metaData.Namespaces.Values);
        BackupUnusedItems(_metaData.Entities.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.Forms.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.Reports.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.Dashboards.Values.SelectMany(f => f.Values));
        BackupUnusedItems(_metaData.Fields.Values.SelectMany(f => f.Values));
    }

    private MetaModelNamespace FetchModel(ModelNamespace model)
    {
        var fetchItem = FetchItem(
            new MetaModelNamespace
            {
                MetaNamespaceId = model.Id,
                Name = model.Name
            }, t => t.MetaNamespaceId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.MetaNamespaceId != item2.MetaNamespaceId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.MetaNamespaceId = item2.MetaNamespaceId;
                return true;
            }, _metaData.Namespaces, _backupMetaData.Namespaces, model.Id);
        model.DbId = fetchItem.Id;
        return fetchItem;
    }

    private Dictionary<string, MetaModelEntity> FetchEntities(MetaModelNamespace modelNamespace, bool bActive)
    {
        return (bActive ? _metaData : _backupMetaData).Entities.AddOrGetItem(modelNamespace.Id);
    }

    private MetaModelEntity FetchMetaEntity(IDictionary<string, MetaModelEntity> metaEntities,
        IDictionary<string, MetaModelEntity> backupMetaEntities, Entity entity, MetaModelNamespace metaModel)
    {
        var fetchItem = FetchItem(
            new MetaModelEntity
            {
                MetaNamespaceId = metaModel.Id,
                MetaEntityId = entity.Id,
                Name = entity.Name
            }, t => t.MetaEntityId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.MetaEntityId != item2.MetaEntityId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.MetaEntityId = item2.MetaEntityId;
                return true;
            }, metaEntities, backupMetaEntities, entity.Id);
        entity.DbId = fetchItem.Id;
        return fetchItem;
    }

    private void FetchPage(IDictionary<string, MetaModelPage> metaForms,
        IDictionary<string, MetaModelPage> backupMetaForms, IEntityPage form, MetaModelPageTypeId pageType)
    {
        var fetchItem = FetchItem(
            new MetaModelPage
            {
                MetaEntityId = form.Entity.DbId,
                PageTypeId = pageType,
                PageId = form.Id,
                Name = form.Name
            }, t => t.PageId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.PageType != item2.PageType ||
                item.MetaEntityId != item2.MetaEntityId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.PageId = item2.PageId;
                item.PageType = item2.PageType;
                item.MetaEntityId = item2.MetaEntityId;
                return true;
            }, metaForms, backupMetaForms, form.Id);
        form.DbId = fetchItem.Id;
    }
    private void FetchField(IDictionary<string, MetaModelField> metaFields,
        IDictionary<string, MetaModelField> backupMetaFields, EntityField field)
    {
        var fetchItem = FetchItem(
            new MetaModelField
            {
                MetaEntityId = field.Entity.DbId,
                MetaFieldId = field.Id,
                Name = field.Name
            }, t => t.MetaFieldId,
            (item, item2) =>
                item.Name != item2.Name ||
                item.MetaEntityId != item2.MetaEntityId,
            (item, item2) =>
            {
                item.Name = item2.Name;
                item.MetaEntityId = item2.MetaEntityId;
                return true;
            }, metaFields, backupMetaFields, field.Id);
        field.DbId = fetchItem.Id;
    }
}
