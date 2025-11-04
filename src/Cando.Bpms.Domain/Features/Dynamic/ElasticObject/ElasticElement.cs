namespace Neo.Bpms.Domain.Features.Dynamic;

public partial class ElasticObject
{
    public IEnumerable<ElasticObject> Elements => _elasticProvider.Elements;
    public Dictionary<string, List<ElasticObject>> ElementCollections => _elasticProvider.ElementCollections;
    public ElasticObject Element(string name)
    {
        return _elasticProvider.Element(name);
    }
    public List<ElasticObject> ElementList(string listName)
    {
        return _elasticProvider.ElementList(listName);
    }
    public void AddElement(ElasticObject element)
    {
        //			element._nodeType = NodeType.Element;
        element.InternalParent = this;
        _elasticProvider.AddElement(element);
    }

    public void RemoveElement(ElasticObject element)
    {
        _elasticProvider.RemoveElement(element);
    }
    public List<ElasticObject> GetElements(string id)
    {
        if (ElementCollections == null)
            return null;
        ElementCollections.TryGetValue(id, out List<ElasticObject> elements);
        return elements;
    }
    public ElasticObject GetElement(string id)
    {
        return GetElements(id)?.FirstOrDefault();
    }

    public IEnumerable<string> GetElementsRef(string id)
    {
        return GetElements(id)?.Select(e => e.InternalValue?.ToString());
    }

    public string GetElementRef(string id)
    {
        return GetElement(id)?.InternalValue?.ToString();
    }
}
