namespace Neo.Bpms.Domain.Models.Bpmn.Diagrams.DiagramGraphics;

/// <summary>
/// Style contains formatting properties that affect the appearance or style of diagram elements, including diagram themselves.
/// 
/// A Style is a set of properties (e.g., fontName, fillColor or strokeWidth) that affect the appearance or style of diagram
/// elements rather than their intrinsic semantics. Style is defined as an abstract class without prescribing any style properties
/// to leave it up to language-specific DI extensions to define concrete style classes with their own properties that are
/// applicable to their diagram element types.
/// 
/// A style element can either be local to (owned by) a diagram element or shared between (referenced by) several diagram
/// elements, in which case it is owned elsewhere (e.g., by packaging elements in the language incorporating diagram
/// interchange). A value set to a local style property in a diagram element overrides one that is set to the same property on
/// a shared style referenced by the same diagram element.
/// Style properties are typically defined as optional to allow the state of “unset” to be legal. This is needed to implement
/// cascading style, where an unset style property in one diagram element gets its value from the closest diagram element in
/// its owning element chain that has a value set for that property.
/// The above semantics effectively specify that a value for a style property is based on the following mechanisms (in order
/// of precedence):
/// • if there is a cascading value set on a local style, use it.
/// • Otherwise, if there is a cascading value set on a shared style, use it.
/// • Otherwise, if a cascading value is available from a diagram element in the owning element chain, use it from the closest
/// owning element.
/// • Otherwise, use the style property’s default value.
/// </summary>
public abstract class Style(string name)
{
    //		public List<PropertyValue> propertyValues = new List<PropertyValue>();
    public string name = name;


    public ColorItem fillColor;
    public double fillOpacity;
    public double strokeWidth;
    public double strokeOpacity;
    public ColorItem strokeColor;
    public List<double> strokeDashLength;
    public double fontSize;
    public string fontName;
    public ColorItem fontColor;
    public bool fontItalic;
    public bool fontBold;
    public bool fontUnderline;
    public bool fontStrikeThrough;
    public Fill fill;
}
//public class PropertyValue
//{
//	public string property;
//	public string value;
//}
