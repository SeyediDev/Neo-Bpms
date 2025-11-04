using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.UMLDiagram;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Diagrams;

public abstract class DiagramDefinition : BaseModelingDefinition
{
    public Diagram DefineAll()
    {
        Diagram _diagram = Identify();
        return _diagram == null ? null : !DefineDiagram() ? null : _diagram;
    }

    protected abstract Diagram Identify();
    protected abstract bool DefineDiagram();


    protected Diagram diagram;

    protected Diagram DefineDiagram(string id, string name, Bounds bounds, BaseModelClass modelElement)
    {
        diagram = new Diagram(id, name, bounds, modelElement);
        _ = ProjectDefinition.Project.AddDiagram(diagram);
        return diagram;
    }

    protected DiagramElement currentElement;
    protected DiagramElement parentElement;

    private DiagramElement GetParentElement()
    {
        return parentElement ?? diagram;
    }

    protected DiagramElement AddStyle(params Style[] styles)
    {
        foreach (Style style in styles)
        {
            currentElement.AddStyle(style);
        }

        return currentElement;
    }

    protected DiagramElement StartSubElements()
    {
        parentElement = currentElement;
        return parentElement;
    }

    protected DiagramElement EndSubElements()
    {
        parentElement = parentElement?.owningElement;
        return parentElement;
    }

    protected Diagram AddSubDiagram(Diagram subDiagram)
    {
        currentElement.AddSubElement(subDiagram);
        return subDiagram;
    }

    protected Diagram AddSubDiagram(DiagramDefinition diagramDefinition)
    {
        Diagram subDiagram = diagramDefinition.DefineAll();
        return AddSubDiagram(subDiagram);
    }

    protected DiagramElement AddActor(string id, BaseModelClass processRole, Bounds bounds, params Style[] styles)
    {
        currentElement = new Actor(id, processRole.Name, bounds, processRole, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddEntity(ClassShape.Type type, string id, Entity entity, Bounds bounds, bool bAddFields,
        params Style[] styles)
    {
        ClassShape classShape = new(type, id, entity.Name, bounds, entity, GetParentElement());
        currentElement = classShape;
        currentBaseElement = classShape;
        if (bAddFields)
        {
            foreach (EntityField field in entity.entityFields?.Values ?? Enumerable.Empty<EntityField>())
            {
                // ReSharper disable once ObjectCreationAsStatement
                _ = new ClassShapeItem(ClassShapeItem.Type.Field, field.Id, field.Name, field, classShape);
            }
        }

        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddClass(ClassShape.Type type, string id, string name, Bounds bounds,
        params Style[] styles)
    {
        currentElement = new ClassShape(type, id, name, bounds, null, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddClassItem(ClassShapeItem.Type type, string id, string name, BaseModelClass modelElement,
        params Style[] styles)
    {
        if (currentElement is not ClassShape classShape)
        {
            return null;
        }

        ClassShapeItem csi = new(type, id, name, modelElement, classShape);
        currentElement = csi;
        _ = AddStyle(styles);
        currentElement = classShape;
        return csi;
    }

    /*protected DiagramElement AddUseCase(int rank, Process process, Bounds bounds, params Style[] styles)
		{
			currentElement = new UseCase(rank, bounds, process, getParentElement());
			currentBaseElement = currentElement;
			addStyle(styles);
			return currentElement;
		}*/
    protected DiagramElement AddUseCase(int rank, string id, string name, Bounds bounds, params Style[] styles)
    {
        currentElement = new UseCase(rank, id, name, bounds, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddNote(string id, string name, Bounds bounds, params Style[] styles)
    {
        currentElement = new NoteShape(id, name, bounds, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected BusinessRuleShape currentBusinessRuleShape;

    protected DiagramElement AddBusinessRule(BusinessRuleShape.Type type, string id, string name, Bounds bounds,
        KnowledgeTerm rule, params Style[] styles)
    {
        currentElement = currentBusinessRuleShape =
            new BusinessRuleShape(type, id, name, bounds, rule, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected void AddTerm(KnowledgeTerm term)
    {
        _ = currentBusinessRuleShape.AddTerm(term);
    }

    protected void AddFact(KnowledgeTerm fact)
    {
        _ = currentBusinessRuleShape.AddFact(fact);
    }

    protected DiagramElement AddGraphicElement(string id, string name, Bounds bounds, GraphicalElement ge,
        params Style[] styles)
    {
        currentElement = new GraphicElementShape(id, name, bounds, ge, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected PackageShape currentPackage;

    protected DiagramElement AddPackage(PackageShape.eType type, string id, string name, Bounds bounds,
        params Style[] styles)
    {
        currentElement = currentPackage = new PackageShape(type, id, name, bounds, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected PackageShapeItem AddPackageItem(PackageShapeItem.eType type, string id, string name, Bounds bounds,
        Diagram linkedDiagram, params Style[] styles)
    {
        PackageShapeItem psi = new(type, id, name, bounds, linkedDiagram, currentPackage);
        currentBaseElement = psi;
        currentElement = psi;
        _ = AddStyle(styles);
        return psi;
    }

    protected DiagramElement AddRequirement(RequirementShape.eType type, Bounds bounds, BaseModelClass requirement,
        params Style[] styles)
    {
        currentElement = new RequirementShape(type, requirement.Id, requirement.Name, bounds, requirement,
            GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddTestCase(Bounds bounds, BaseModelClass testCase, params Style[] styles)
    {
        currentElement = new TestCaseShape(testCase.Id, testCase.Name, bounds, testCase, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddNode(BaseModelClass term, Bounds bounds = null, params Style[] styles)
    {
        currentElement = new NodeShape(term.Id, term.Name, bounds, term, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }

    protected DiagramElement AddRelation(Relation.eType type, string id, string name, DiagramElement source,
        DiagramElement target, BaseModelClass modelElement, params Style[] styles)
    {
        currentElement = new Relation(type, id, name, source, target, modelElement, GetParentElement());
        currentBaseElement = currentElement;
        _ = AddStyle(styles);
        return currentElement;
    }
}
