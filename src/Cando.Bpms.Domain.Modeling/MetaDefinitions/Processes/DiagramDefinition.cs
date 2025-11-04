using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramGraphics;
using Neo.Bpms.Domain.Entities.Bpmn.Diagrams.DiagramInterchange;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    protected virtual void Diagrams()
    {
    }

    private BPMNDiagram _currentDiagram;
    private DiagramElement _currentDiagramElement;
    private BPMNPlane _currentPlane;
    private BPMNLabel _currentLabel;

    /// <summary>
    /// Sets the process graphic information.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <returns></returns>
    protected bool SetProcessGraphicInfo(Bounds bounds)
    {
        if (process == null)
        {
            return false;
        }

        _currentDiagram = new BPMNDiagram(bounds, process, process.Name);
        _currentPlane = new BPMNPlane(process, process.Name, _currentDiagram);
        //currentBaseElement = currentPlane;
        return true;
    }

    /// <summary>
    /// Sets the lane graphic information.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <param name="laneId">The lane identifier.</param>
    /// <param name="alignment">The alignment.</param>
    /// <param name="width">The width.</param>
    /// <returns></returns>
    protected bool SetLaneGraphicInfo(Bounds bounds, string laneId, AlignmentKind alignment = AlignmentKind.center,
        int width = 30)
    {
        Lane lane = FindLane(laneId);
        if (lane == null)
        {
            return false;
        }

        Bounds labelBounds = new(bounds) { width = width };
        _currentLabel = new BPMNLabel(lane.Name, labelBounds, alignment);
        _currentDiagramElement = new BPMNShape(bounds, lane, lane.Name, _currentPlane, _currentLabel);
        return true;
    }

    /// <summary>
    /// Sets the element graphic information.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <param name="elementId">The element identifier.</param>
    /// <param name="alignment">The alignment.</param>
    /// <returns></returns>
    protected bool SetElementGraphicInfo(Bounds bounds, int elementId, AlignmentKind alignment = AlignmentKind.center)
    {
        FlowElement element = FindElement(elementId.ToString());
        if (element == null)
        {
            return false;
        }

        Bounds labelBounds = new(bounds);
        _currentLabel = new BPMNLabel(element.Name, labelBounds, alignment);
        _currentDiagramElement = new BPMNShape(bounds, element, element.Name, _currentPlane, _currentLabel);
        return true;
    }

    /// <summary>
    /// Sets the flow graphic information.
    /// </summary>
    /// <param name="bounds">The bounds.</param>
    /// <param name="sourceElementId">The source element identifier.</param>
    /// <param name="targetElementId">The target element identifier.</param>
    /// <param name="alignment">The alignment.</param>
    /// <param name="RelationX"></param>
    /// <param name="RelationY">The relY.</param>
    /// <returns></returns>
    protected bool SetFlowGraphicInfo(Bounds bounds, string sourceElementId, string targetElementId,
        AlignmentKind alignment = AlignmentKind.start, int RelationX = 0, int RelationY = 0)
    {
        if (_currentPlane == null)
        {
            return false;
        }

        DiagramElement source = FindDiagramElement(_currentPlane, sourceElementId);
        DiagramElement target = FindDiagramElement(_currentPlane, targetElementId);
        FlowElement flow = FindElement(sourceElementId + "_" + targetElementId);
        if (source == null || target == null || flow == null)
        {
            return false;
        }

        Bounds labelBounds = new(bounds);
        labelBounds.x += RelationX;
        labelBounds.y += RelationY;
        _currentLabel = new BPMNLabel(flow.Name, labelBounds, alignment);
        _currentDiagramElement = new BPMNEdge(source, target, flow, flow.Name, _currentPlane, _currentLabel);
        return true;
    }

    /// <summary>
    /// Sets the style.
    /// </summary>
    /// <param name="style">The style.</param>
    /// <returns></returns>
    protected bool SetStyle(Style style)
    {
        if (_currentDiagramElement != null)
        {
            _currentDiagramElement.AddStyle(style);
        }
        else if (_currentPlane != null)
        {
            _currentPlane.AddStyle(style);
        }
        else if (_currentDiagram != null)
        {
            _currentDiagram.AddStyle(style);
        }
        else
        {
            return false;
        }

        return true;
    }

    #region private functions

    /// <summary>
    /// Finds the diagram element.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    private static DiagramElement FindDiagramElement(DiagramElement parent, string id)
    {
        if (parent.ownedElements == null)
        {
            return null;
        }

        foreach (DiagramElement item in parent.ownedElements)
        {
            if (item.Id == id)
            {
                return item;
            }

            DiagramElement el = FindDiagramElement(item, id);
            if (el != null)
            {
                return el;
            }
        }

        return null;
    }

    #endregion
}
