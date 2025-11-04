using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Artifacts;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class ArtifactXmlConvertor
{
    internal static void Export(dynamic containerElement, IArtifactContainer artifactContainer)
    {
        foreach (var artifact in artifactContainer.artifacts ?? Enumerable.Empty<Artifact>())
        {
            dynamic element = null;
            switch (artifact)
            {
                case TextAnnotation annotation:
                    {
                        element = containerElement.textAnnotation();
                        var textAnnotation = annotation;
                        var text = element.text();
                        text.InternalValue = textAnnotation.text;
                        element.textFormat = textAnnotation.textFormat ?? "text/plain";
                        break;
                    }

                case Association a:
                    {
                        element = containerElement.association();
                        var association = a;
                        element.sourceRef = association.sourceRef;
                        element.targetRef = association.targetRef;
                        element.associationDirection = association.associationDirection;
                        break;
                    }

                case Group _:
                    element = containerElement.@group();
                    //todo
                    break;
            }

            if (element != null)
                BaseElementXmlConvertor.Export(element, artifact);
        }
    }

    internal static void Import(ElasticObject containerElement, IArtifactContainer artifactContainer)
    {
        artifactContainer.artifacts = null;
        foreach (var elements in containerElement.ElementCollections ??
            Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
        {
            foreach (var element in elements.Value)
            {
                Artifact artifact = null;
                switch (elements.Key)
                {
                    case "textAnnotation":
                        artifact = new TextAnnotation(artifactContainer, "",
                            element.GetElement("text")?.InternalValue?.ToString(),
                            element.GetString("textFormat") ?? "text/plain");
                        break;
                    case "association":
                        artifact = new Association(artifactContainer, "",
                                            element.GetString("sourceRef"),
                                            element.GetString("targetRef"))
                        {
                            associationDirection = element.GetEnumText("associationDirection",
                                                Association.AssociationDirection.One)
                        };
                        break;
                    case "group":
                        artifact = new Group(artifactContainer, "", null, null);
                        //todo
                        break;
                }

                if (artifact == null) continue;
                BaseElementXmlConvertor.Import(element, artifact);
                artifactContainer.artifacts ??= [];
                artifactContainer.artifacts.Add(artifact);
            }
        }
    }
}
