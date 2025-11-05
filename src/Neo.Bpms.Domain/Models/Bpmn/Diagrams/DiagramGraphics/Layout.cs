namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

/// <summary>
/// Point is used to specify a coordinate that is at a given distance (along the x and y axes) from the origin of some x-y
/// coordinate system. The point (0, 0) is considered to be at the origin of that coordinate system. Coordinates increase
/// towards the right of the x-axes and towards the bottom of the y-axis.
/// </summary>
public class Point
{
    /// <summary>
    /// a real number (<= 0 or >= 0) that represents the x-coordinate of the point.
    /// </summary>
    public double x = 0;
    /// <summary>
    /// a real number (<= 0 or >= 0) that represents the y-coordinate of the point.
    /// </summary>
    public double y = 0;
}
/// <summary>
/// Bounds specifies a rectangular area in some x-y coordinate system that is defined by a location (x and y) and a size (width and height).
/// Bounds is used to specify a rectangular area in some x-y coordinate system. The area is specified with a (x, y) location,
/// representing the distance of the area’s top-left corner from the origin, and a size (width and height) along the x-y axes.
/// </summary>
// [OCL("width >= 0 and height >=0")]
public class Bounds
{
    public Bounds()
    {
    }
    public Bounds(Bounds b)
    {
        x = b.x;
        y = b.y;
        width = b.width;
        height = b.height;
    }
    /// <summary>
    /// a real number (>=0 or <=0) that represents the x-coordinate of the bounds
    /// </summary>
    public double x = 0;
    /// <summary>
    /// a real number (>=0 or <=0) that represents the y-coordinate of the bounds
    /// </summary>
    public double y = 0;
    /// <summary>
    /// a real number (>=0) that represents the width of the bounds
    /// </summary>
    public double width;
    /// <summary>
    /// a real number (>=0) that represents the height of the bounds
    /// </summary>
    public double height;
}
/// <summary>
/// Dimension specifies two lengths (width and height) along the x and y axes in some x-y coordinate system.
/// 
/// </summary>
//[OCL("width >= 0 and height >=0")]
public class Dimension
{
    /// <summary>
    /// a real number (>=0) that represents a length along the x-axis.
    /// </summary>
    public double width;
    /// <summary>
    /// a real number (>=0) that represents a length along the y-axis.
    /// </summary>
    public double height;
}
/// <summary>
/// AlignmentKind enumerates the possible kinds for alignment for layout purposes (e.g., for text alignment within a bounding box).
/// </summary>
public enum AlignmentKind
{
    /// <summary>
    /// an alignment to the start of a given length
    /// </summary>
    start,
    /// <summary>
    /// an alignment to the end of a given length
    /// </summary>
    end,
    /// <summary>
    /// an alignment to the center of a given length
    /// </summary>
    center
}
