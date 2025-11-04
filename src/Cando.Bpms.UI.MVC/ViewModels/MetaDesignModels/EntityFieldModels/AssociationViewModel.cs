namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

//public class AssociationViewModel
//{
//	public AssociationViewModel()
//	{

//	}
//	public AssociationViewModel(EntityField field)
//	{
//		var association = field.AssociationEntity;
//		namespaceId = association.DestNamespaceId;
//		entityId = association.DestEntityId;
//		constraint = association.Constraint;
//		dbConstraintNameMap = association.DbConstraintNameMap;
//		isBitMask = association.IsBitMask;
//		deleteBehaivior = association.OnDeleteBehaviour;
//		updateBehaivior = association.OnUpdateBehaviour;
//		isParent = association.GetType() == typeof(WeakEntityAssociation);
//		maps = association.Maps?.Select(m => new AssociationMapViewModel(m)).ToList();
//		if (association.File != null)
//			fileFields = new FileFieldViewModel(association.File);
//	}

//	public string namespaceId { get; set; }
//	public string entityId { get; set; }
//	public string constraint { get; set; }
//	public string dbConstraintNameMap { get; set; }
//	public bool isBitMask { get; set; }
//	public RelationDeleteUpdateBehaviour deleteBehaivior { get; set; }
//	public RelationDeleteUpdateBehaviour updateBehaivior { get; set; }
//	public List<AssociationMapViewModel> maps { get; set; }
//	public FileFieldViewModel fileFields { get; set; }
//	public bool isParent { get; set; }

//	public Association ToAssociation(Model.Entities.Entity entity, FieldViewModel fieldViewModel)
//	{
//		var fieldFlags = (fieldViewModel.notNull ? eEntityFieldFlags.NotNull : eEntityFieldFlags.None) |
//							  (fieldViewModel.notMap ? eEntityFieldFlags.NotMap : eEntityFieldFlags.None) |
//							  (isBitMask ? eEntityFieldFlags.IsBitMask : eEntityFieldFlags.None);
//		var associationEntity = ProjectDefinition.Project.GetEntity(namespaceId, entityId)
//			?? ProjectDefinition.Project.GetEntityId(entityId);

//		Association association;
//		if (isParent)
//		{
//			association = new WeakEntityAssociation(entity, fieldViewModel.id, fieldViewModel.persianName,
//				fieldViewModel.englishName, associationEntity, constraint, dbConstraintNameMap, fieldFlags)
//			{
//				Maps = maps?.Select(map => map.ToMap()).ToList()
//			};
//		}
//		else
//		{
//			association = new Association(entity, fieldViewModel.id, fieldViewModel.persianName,
//				fieldViewModel.englishName, associationEntity, constraint, dbConstraintNameMap,
//				deleteBehaivior, updateBehaivior, fieldFlags)
//			{
//				Maps = maps?.Select(map => map.ToMap()).ToList()
//			};
//		}

//		if (IsReferToFile(namespaceId, entityId) || IsReferToFile(associationEntity?.NamespaceId, associationEntity?.Id))
//			association.File = fileFields?.ToFile();
//		return association;
//	}

//	private static bool IsReferToFile(string namespaceId, string entityId)
//	{
//		return namespaceId == "SystemConfigs" && entityId == "FileInfo";
//	}
//}
