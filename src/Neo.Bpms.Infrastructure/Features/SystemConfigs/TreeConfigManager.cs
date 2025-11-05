namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public static class TreeConfigManager
{
    public static TreeTypes TreeTypes
    {
        get
        {
            if (_treeTypes != null) return _treeTypes;
            _treeTypes = [];
            var trees = QueryUtility.New<TreeConfig>().OrderBy("NodeTypeId").ToList<TreeConfig>();
            TreeTypeItem lastTreeTypeItem = null;
            foreach (var treeConfig in trees)
                lastTreeTypeItem = InsertToMemory(lastTreeTypeItem, treeConfig);
            return _treeTypes;
        }
    }

    private static TreeTypes _treeTypes;
    public static double LastTreeOrderId = 1;

    public static void Save(TreeConfig treeConfig, IdentityUser user)
    {
        var apply = ApplyUtility<TreeConfig>.New(user);
        if (treeConfig.Id <= 0)
        {
            treeConfig.Id = 0;
            if (apply.Insert(treeConfig))
                InsertToMemory(null, treeConfig);
            else
                throw new Exception("اطلاعات درخت قابل ذخیره سازی نیست");
        }
        else if (!apply.Update(treeConfig))
            throw new Exception("اطلاعات درخت قابل ذخیره سازی نیست");
    }

    public static TreeConfig Delete(long id, TreeNodeTypeId? nodeTypeId, IdentityUser user)
    {
        var treeConfig = DeleteFromMemory(id, nodeTypeId);
        if (treeConfig == null || id <= 0)
            return treeConfig;

        foreach (var treeType in TreeTypes.Values)
        {
            var children = treeType.Items.Values.Where(t => t.ParentId == treeConfig.Id).ToList();
            foreach (var child in children)
                Delete(child.Id, treeType.NodeTypeId, user);
        }

        var apply = ApplyUtility<TreeConfig>.New(user);
        apply.DeleteWithFilter("Id=\"" + id + "\"");
        return treeConfig;
    }

    public static TreeConfig Fetch(TreeNodeTypeId nodeTypeId, long id)
    {
        return FetchTreeTypeItem(null, nodeTypeId)?.Fetch(id);
    }

    public static TreeConfig Fetch(TreeNodeTypeId nodeTypeId, string standardCode,
        long id, long? parentId, string name)
    {
        var treeTypeItem = FetchTreeTypeItem(null, nodeTypeId);
        if (treeTypeItem == null) return null;
        var treeConfig = treeTypeItem.Fetch(standardCode, parentId);
        if (treeConfig != null) return treeConfig;
        treeConfig = new TreeConfig
        {
            Id = id,
            Name = name,
            NodeTypeId = nodeTypeId,
            StandardCode = standardCode,
            ParentId = parentId,
            OrderId = LastTreeOrderId++,
        };
        Save(treeConfig, null);
        treeTypeItem.Add(treeConfig);
        return treeConfig;
    }

    private static TreeConfig DeleteFromMemory(long id, TreeNodeTypeId? nodeTypeId)
    {
        TreeConfig treeConfig = null;
        if (nodeTypeId != null)
        {
            var treeTypeItem = FetchTreeTypeItem(null, nodeTypeId.Value);
            treeConfig = DeleteFromMemory(id, treeTypeItem);
        }
        else
        {
            foreach (var treeType in TreeTypes.Values)
            {
                treeConfig = DeleteFromMemory(id, treeType);
                if (treeConfig != null)
                    break;
            }
        }
        return treeConfig;
    }

    private static TreeConfig DeleteFromMemory(long id, TreeTypeItem treeTypeItem)
    {
        var treeConfig = treeTypeItem.Delete(id);
        return treeConfig;
    }

    private static TreeTypeItem InsertToMemory(TreeTypeItem treeTypeItem, TreeConfig treeConfig)
    {
        treeTypeItem = FetchTreeTypeItem(treeTypeItem, treeConfig.NodeTypeId);
        treeTypeItem.Add(treeConfig);
        return treeTypeItem;
    }

    private static TreeTypeItem FetchTreeTypeItem(TreeTypeItem treeTypeItem, TreeNodeTypeId nodeTypeId)
    {
        if (treeTypeItem?.NodeTypeId != nodeTypeId)
            TreeTypes.TryGetValue(nodeTypeId, out treeTypeItem);
        if (treeTypeItem != null) return treeTypeItem;
        treeTypeItem = new TreeTypeItem { NodeTypeId = nodeTypeId };
        TreeTypes.Add(treeTypeItem.NodeTypeId, treeTypeItem);
        return treeTypeItem;
    }
}

public class TreeTypeItem
{
    public TreeNodeTypeId NodeTypeId { get; set; }
    public ConcurrentDictionary<long, TreeConfig> Items { get; } = new ConcurrentDictionary<long, TreeConfig>();
    public bool Add(TreeConfig treeConfig)
    {
        return Items.TryAdd(treeConfig.Id, treeConfig);
    }

    public TreeConfig Delete(long id)
    {
        Items.TryRemove(id, out var treeConfig);
        return treeConfig;
    }

    public TreeConfig Fetch(long id)
    {
        Items.TryGetValue(id, out var treeConfig);
        return treeConfig;
    }

    public TreeConfig Fetch(string standardCode, long? parentId)
    {
        return Items.Values.FirstOrDefault(item =>
            item.StandardCode == standardCode && (parentId == null || item.ParentId == parentId));
    }
}

public class TreeTypes : Dictionary<TreeNodeTypeId, TreeTypeItem>
{

}
