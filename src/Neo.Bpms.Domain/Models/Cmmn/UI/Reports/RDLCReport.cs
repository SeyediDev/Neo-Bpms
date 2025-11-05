/*using System.Collections.Generic;

namespace Neo.Bpms.Domain.Model.UI
{
	#region Report Item
	#region Base
	/// <summary>
	/// RDLCVisibility
	/// </summary>
	public class RDLCVisibility
	{
		/// <summary>
		/// expresstion: Indicates if the item should be hidden at first.
		/// 
		/// A hidden report item (where the Hidden property is the constant True) that cannot be toggled should be treated as if it is not present, 
		/// when rendering a report. This means the report layout does not change because the item is hidden (unlike hidden items that can toggle or are conditionally hidden, 
		/// thereby shifting layout to make room/remove empty space).
		/// </summary>
		public bool Hidden;
		/// <summary>
		/// The name of the text box used to hide/unhide this report item. Clicking on an instance of the ToggleItem will toggle the hidden state of every corresponding instance of this item. 
		/// If the ToggleItem becomes hidden (because either the item or an ancestor is toggled or conditionally hidden), this item should become hidden.21 Must be a text box 
		/// in the same group scope as this item or in any containing (ancestor) group scope. If omitted, no item will toggle the hidden state of this item. Not allowed on 
		/// and cannot refer to report items contained in a page header or footer. Cannot refer to a report item contained in the current report item unless current group scope has a Parent.
		/// 
		/// If the ToggleItem refers to a text box contained by and in the same group scope as the item whose visibility is being toggled and that item is a group 
		/// (or is directly contained in a group) which has a Parent element, the show/hide toggling behavior will reflect the recursive hierarchy. Specifically: 
		/// Clicking on the text box in one instance of the group will toggle the visibility of items in child instances of the group
		/// </summary>
		public string ToggleItem;
	}

	/// <summary>
	/// RDLCAction Info
	/// </summary>
	public class RDLCActionInfo
	{
		public List<RDLCAction> Actions = new List<RDLCAction>();
	}

	/// <summary>
	/// RDLCStyle
	/// </summary>
	public class RDLCStyle
	{
		public string FontStyle;
		public string FontFamily;
		public RDLCSize FontSize;
	}

	/// <summary>
	/// RDLCAction
	/// </summary>
	public class RDLCAction
	{
		/// <summary>
		/// An expression that evaluates to the URL of the hyperlink18
		/// </summary>
		public string Hyperlink;
		/// <summary>
		/// An expression that evaluates to the ID of a bookmark in the report to go to when this report item is clicked on. 
		/// (If no bookmark with this ID is found, the link will not be included in the report. If the bookmark is hidden, the link will go to the start of the page the bookmark is on. 
		/// If multiple bookmarks with this ID are found, the link will go to the first bookmark.)
		/// </summary>
		public string BookmarkLink;
		/// <summary>
		/// The drillthrough report that should be executed by clicking on the hyperlink
		/// </summary>
		public RDLCDrillthrough Drillthrough = null;
	}

	/// <summary>
	/// RDLCDrillthrough
	/// </summary>
	public class RDLCDrillthrough
	{
		/// <summary>
		/// The full folder path (for example, “/salesreports/orderdetails”), relative path (for example, “../orderdetails”) or 
		/// URL (for example, “http://reportserver/reports/sales/orderdetails”) of the drillthrough report. 
		/// Relative paths start in the same folder as the report. Note: If the current report is being used as a subreport, 
		/// the top-level report location is used as the base of the relative path.
		/// </summary>
		public string ReportName;
		//public string BookmarkLink;
		/// <summary>
		/// Parameters to the drillthrough report
		/// </summary>
		public List<RDLCDrillthroughParameter> DrillthroughParameters = new List<RDLCDrillthroughParameter>();
	}

	/// <summary>
	/// RDLCDrillthrough Parameter
	/// </summary>
	public class RDLCDrillthroughParameter
	{
		public string Name;
		public string Value;
		public bool Omit;
	}
	/// <summary>
	/// RDLCReport Item
	/// </summary>
	public abstract class RDLCReportItem : RDLCReportElement
	{
		public string Name;
		/// <summary>
		/// The distance of the item from the top of the containing object. Defaults to 0 if omitted.
		/// </summary>
		public RDLCSize Top;
		/// <summary>
		/// The distance of the item from the left of the containing object. Defaults to 0 if omitted.
		/// </summary>
		public RDLCSize Left;
		/// <summary>
		/// Height of the item. Negative sizes allowed only for lines (The height/width gives the offset of the endpoint of the line from the start point)
		/// Defaults to the height of the containing object minus Top if omitted.
		/// </summary>
		public RDLCSize Height;
		/// <summary>
		/// Width of the item. Negative sizes allowed only for lines.
		/// Defaults to the width of the containing object minus Left if omitted.
		/// 
		/// For Tablix, the default Height and Width are instead derived from the sizes of the component parts (columns, rows, cells).
		/// </summary>
		public RDLCSize Width;
		/// <summary>
		/// Drawing order of the report item within the containing object. 
		/// Items with lower indices are drawn first (appearing behind items with higher indices). Items with equal indices have an unspecified rendering order. 
		/// Default: 0 Min: 0 Max: 2147483647
		/// </summary>
		public int Zindex;
		/// <summary>
		/// A textual label for the report item. Used for such things as rendering TITLE and ALT attributes in HTML reports.
		/// </summary>
		public string ToolTip;
		/// <summary>
		/// A label to identify an instance of a report item.within the client UI (to provide a user-friendly label for searching). 
		/// Hierarchical listing of report item and group labels within the UI (the Document Map) should reflect the object containment hierarchy in the report definition. 
		/// Peer items should be listed in left-to-right top-to-bottom order. If the expression returns null, no item is added to the Document Map. Not used for report items 
		/// in the page header or footer.
		/// </summary>
		public string DocumentMapLabel;
		//public bool LinkToChild;
		/// <summary>
		/// A bookmark that can be linked to via a Bookmark action.
		/// </summary>
		public string Bookmark;
		/// <summary>
		/// The name of a data region that this report item should be repeated with if that data region spans multiple pages. 
		/// The data region must be in the same ReportItems collection as this ReportItem (Since data regions are not allowed in page headers/footers, 
		/// this means RepeatWith will be unusable in page headers/footers). Not allowed if this report item is a data region, subreport or rectangle that contains a 
		/// data region or subreport.
		/// </summary>
		public string RepeatWith;
		/// <summary>
		/// The name to use for the data element/attribute for this report item. Default: Name of the report item. Must be a CLS-compliant identifier.
		/// </summary>
		public string DataElementName;
		public enum eDataElementOutput
		{
			/// <summary>
			/// Will behave as NoOutput for any report item with Hidden set to True (not an expression) that does not have a ToggleItem, 
			/// and for any report item in a static tablix member that cannot be toggled with Hidden set to non-expression True. Otherwise, acts
			/// as NoOutput for Textboxes with constant TextRun values, as ContentsOnly for Rectangles and as Output for all other items.
			/// </summary>
			Auto,
			/// <summary>
			/// Indicates the item should appear in the output.
			/// </summary>
			Output,
			/// <summary>
			/// Indicates the item should not appear in the output.
			/// </summary>
			NoOutput,
			/// <summary>
			/// Indicates the item should not appear in the XML, but its contents should be rendered as if they were in this item’s container. Only applies to Rectangles.
			/// </summary>
			ContentsOnly
		}
		/// <summary>
		/// Indicates whether the item should appear in a data rendering.
		/// </summary>
		public eDataElementOutput DataElementOutput;
		/// <summary>
		/// Custom information to be handed to the report rendering component
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// Actions (for example, a hyperlink) associated with the ReportItem
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Indicates if the item should be hidden.
		/// </summary>
		public RDLCVisibility Visibility = null;
		//		public RDLCStyle Style = null;
	}
	#endregion Base
	#region Inherited Report items
	/// <summary>
	/// RDLCLine
	/// </summary>
	public class RDLCLine : RDLCReportItem
	{
	}
	/// <summary>
	/// RDLCPage Break
	/// </summary>
	public class RDLCPageBreak
	{
		public enum eBreakLocation
		{
			/// <summary>
			/// There should be a page break before the report item or each instance of the group.
			/// </summary>
			Start,
			/// <summary>
			/// There should be a page break after the report item or each instance of the group.
			/// </summary>
			End,
			/// <summary>
			/// There should be a page break both before and after the report item or each instance of the group.
			/// </summary>
			StartAndEnd,
			/// <summary>
			/// There should be a page break between each instance of the group (does not apply to report items).
			/// </summary>
			Between
		}
		/// <summary>
		/// Indicates where the page break should occur.
		/// </summary>
		public eBreakLocation BreakLocation;
	}
	/// <summary>
	/// RDLCRectangle
	/// </summary>
	public class RDLCRectangle : RDLCReportItem
	{
		/// <summary>
		/// Indicates all of the contents of the rectangle should be kept together on one page if possible.
		/// </summary>
		public bool KeepTogether;
		/// <summary>
		/// Indicates the borders should not appear at locations where the rectangle spans multiple pages. Also causes repeated background images to continue rather 
		/// than restart after a page break
		/// </summary>
		public bool OmitBorderOnPageBreak;
		/// <summary>
		/// Report items contained within the bounds of the rectangle
		/// </summary>
		public List<RDLCReportItem> ReportItems = null;
		/// <summary>
		/// Defines page break behavior for the rectangle.
		/// </summary>
		public RDLCPageBreak PageBreak = null;
		/// <summary>
		/// The name of a report item contained directly within this rectangle that is the target location for the Document Map label (if any). Ignored if DocumentMapLabel is not present.
		/// </summary>
		public string LinkToChild = null;
	}
	/// <summary>
	/// RDLCData Region
	/// </summary>
	public class RDLCDataRegion : RDLCReportItem
	{
		/// <summary>
		/// Expression: Message to display in the DataRegion (instead of the region layout) when no rows of data are available. 
		/// Note: Style information on the data region applies to this text.
		/// 
		/// If the data region is in a tablix cell and does not have a NoRowsMessage property, the contents of the data region will be omitted but the data region’s 
		/// background and border properties will still apply to the cell.
		/// </summary>
		public string NoRowsMessage;
		/// <summary>
		/// Indicates which data set to use for this data region. Mandatory for top level DataRegions 
		/// (not contained within another DataRegion) if there is not exactly one data set in the report. 
		/// If there is exactly one data set in the report, the data region uses that data set. (Note: If there are zero data sets in the report, data regions can not be used, 
		/// as there is no valid DataSetName to use) Ignored for DataRegions that are not top level.
		/// </summary>
		public string DataSetName;
		/// <summary>
		/// Defines the page break behavior for the data region.
		/// </summary>
		public RDLCPageBreak PageBreak = null;
		/// <summary>
		/// Filters to apply to each row of data in the data region.
		/// </summary>
		public List<RDLCFilter> Filters = null;
		/// <summary>
		/// The expressions by which to sort the rows of data in the data region.
		/// </summary>
		public List<RDLCSortExpression> SortExpressions = null;
	}
	/// <summary>
	/// RDLCImage
	/// </summary>
	public class RDLCImage : RDLCReportItem
	{
		public enum eSource
		{
			/// <summary>
			/// The Value contains a constant or expression that evaluates to the location of the image. 
			/// This can be a full folder path (for example, “/images/logo.gif”), relative path (for example, “logo.gif”) 
			/// or URL (for example, “http://reportserver/images/logo.gif”). Relative paths start in the same folder as the report.
			/// </summary>
			External,
			/// <summary>
			/// The Value contains a constant or expression that evaluates to the name of an EmbeddedImage within the report.
			/// </summary>
			Embedded,
			/// <summary>
			/// The Value contains an expression (typically a field in the database) that evaluates to the binary data for the image.
			/// </summary>
			Database
		}
		/// <summary>
		/// Identifies the source of the image.
		/// </summary>
		public eSource Source;
		/// <summary>
		/// See Source. Expected data type is string or binary, depending on Source. If the Value is null, no image is displayed
		/// </summary>
		public string Value;
		/// <summary>
		/// An expression, the value of which is the MIMEType for the image. Valid values are: image/bmp, image/jpeg, image/gif, image/png, image/x-png Required if Source = Database. 
		/// Ignored otherwise.
		/// </summary>
		public string MIMEType;
		public enum eSizing
		{
			/// <summary>
			/// The borders should grow/shrink to accommodate the image.
			/// </summary>
			AutoSize,
			/// <summary>
			/// The image is resized to exactly match the height and width of the image element.
			/// </summary>
			Fit,
			/// <summary>
			/// The image should be resized to fit, preserving aspect ratio.
			/// </summary>
			FitProportional,
			/// <summary>
			/// The image should be clipped to fit.
			/// </summary>
			Clip
		}
		/// <summary>
		/// Defines the behavior if the image does not fit in the specified size
		/// </summary>
		public eSizing Sizing = eSizing.AutoSize;
	}
	/// <summary>
	/// RDLCSub Report Parameter
	/// </summary>
	public class RDLCSubReportParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name;
		/// <summary>
		/// An expression that evaluates to the value to hand in for the parameter to the subreport or control. 
		/// For Drillthrough in Chart, this is the name of a DataField from which to obtain the value rather than an expression.
		/// </summary>
		public string Value;
		/// <summary>
		/// Expression: Indicates the parameter should be skipped. Valid only for Drillthrough parameters.
		/// </summary>
		public bool Omit;
	}
	/// <summary>
	/// RDLCSub Report
	/// </summary>
	public class RDLCSubReport : RDLCReportItem
	{
		/// <summary>
		/// The full folder path (for example, “/salesreports/orderdetails”) or relative path (for example, “orderdetails”) to a subreport on the same server. 
		/// Relative paths start in the same folder as the current report. Cannot be an empty string (ignoring whitespace).
		/// </summary>
		public string ReportName;
		/// <summary>
		/// Message to display in the Subreport (instead of the region layout) when no rows of data are available in any data set which is used in the body of the subreport. 
		/// Note: Style information on the subreport applies to this text.
		/// 
		/// If the subreport is in a tablix cell and does not have a NoRowsMessage property, the contents of the subreport will be omitted 
		/// but the subreport’s border properties will still apply to the cell.
		/// </summary>
		public string NoRowsMessage;
		/// <summary>
		/// Indicates that transactions in the subreport should be merged with transactions in the parent report (into a single transaction for the entire report) 
		/// if the data sources use the same connection.
		/// </summary>
		public bool MergeTransactions;
		/// <summary>
		/// Indicates the entire subreport should be kept together on one page if possible.
		/// </summary>
		public bool KeepTogether;
		/// <summary>
		/// Indicates the borders should not appear at locations where the subreport spans multiple pages. Also causes repeated background images to continue rather than 
		/// restart after a page break.
		/// </summary>
		public bool OmitBorderOnPageBreak;
		/// <summary>
		/// Parameters to the subreport.
		/// </summary>
		public List<RDLCSubReportParameter> Parameters = new List<RDLCSubReportParameter>();
	}
	/// <summary>
	/// RDLCText Run
	/// </summary>
	public class RDLCTextRun
	{
		/// <summary>
		/// An expression, the value of which is displayed at runtime for the TextRun.
		/// </summary>
		public string Value;
		/// <summary>
		/// Specifies the data type of the value in the event it is a constant. It may be set to any RDL data type. If omitted, constant values are assumed to be strings.
		/// </summary>
		public RDLCReportParameter.eDataType DataType;
		public enum eEvaluationMode
		{
			/// <summary>
			/// Default Evaluates as an expression if Value starts with =. Otherwise, treats the value as a constant.
			/// </summary>
			Auto,
			/// <summary>
			/// Value is evaluated as an expression.
			/// </summary>
			Expression,
			/// <summary>
			/// Value is treated as a constant.
			/// </summary>
			Constant
		}
		public eEvaluationMode EvaluationMode = eEvaluationMode.Auto;
		/// <summary>
		/// Indicates whether to preserve white space in the Value.
		/// </summary>
		public bool Space;
		/// <summary>
		/// Label for the placeholder for this TextRun. This name appears as the display placeholder in designer tools UI.
		/// </summary>
		public string Label;
		/// <summary>
		/// A textual tooltip label for the TextRun.
		/// </summary>
		public string Tooltip;
		public enum eMarkupType
		{
			/// <summary>
			/// No markup is processed. Any markup is assumed to be literal (part of the value).
			/// </summary>
			None,
			/// <summary>
			/// HTML markup appearing in the Value is processed and displayed in supporting rendering extensions.
			/// </summary>
			HTML
		}
		/// <summary>
		/// Indicates whether markup appearing in the Value should be processed.
		/// </summary>
		public eMarkupType MarkupType = eMarkupType.None;
		/// <summary>
		/// Style properties for the TextRun.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Defines the actions for this TextRun. Actions on TextRuns are ignored if an action is defined on the parent Textbox (even if the Textbox action resolves to NULL).
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
	}
	/// <summary>
	/// RDLCParagraph
	/// </summary>
	public class RDLCParagraph
	{
		/// <summary>
		/// expression: Indicates the first line indent or hanging line indent for the paragraph. 
		/// Relative to left indent; can be negative. If positive, indents just the first line (first line indent). 
		/// If negative, indents all lines but the first line (hanging indent). Default: 0
		/// </summary>
		public RDLCSize HangingIndent;
		/// <summary>
		/// expression: Indentation from the left edge of the Textbox, less left padding. Default: 0.
		/// </summary>
		public RDLCSize LeftIndent;
		/// <summary>
		/// expression: Indentation from the right edge of the Textbox, less right padding. Default: 0.
		/// </summary>
		public RDLCSize RightIndent;
		public enum eListStyles
		{
			/// <summary>
			/// Indicates that this is not a list paragraph and that there is no bullet/number for this paragraph.
			/// </summary>
			None,
			/// <summary>
			/// Indicates that this is a list paragraph with numbering
			/// </summary>
			Numbered,
			/// <summary>
			/// Indicates that this is a list paragraph with bullets.
			/// </summary>
			Bulleted
		}
		/// <summary>
		/// Indicates whether this paragraph is part of a list, and identifies the numbering type.
		/// </summary>
		public eListStyles ListStyle = eListStyles.None;
		/// <summary>
		/// Indicates the numbering style and/or indentation level. Must be >= 0 and 
		/// <= 9. For paragraphs with ListStyle=None, this property serves to indent the paragraph. When ListStyle is Bulleted or Numbered, it serves as indentation level and 
		/// bullet/number style. Default: 0.
		/// </summary>
		public int ListLevel;
		/// <summary>
		/// Collection of TextRun elements.
		/// </summary>
		public List<RDLCTextRun> TextRuns = new List<RDLCTextRun>();
		/// <summary>
		/// Style properties for the paragraph.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Spacing before the paragraph. Cannot be negative. Default: 0.
		/// </summary>
		public RDLCSize SpaceBefore;
		/// <summary>
		/// Spacing after the paragraph. Cannot be negative. Default: 0.
		/// </summary>
		public RDLCSize SpaceAfter;

	}
	/// <summary>
	/// RDLCUser Sort
	/// </summary>
	public class RDLCUserSort
	{
		/// <summary>
		/// The expression on which to sort. Has the same restrictions as a Group Filter expression. Aggregates used in the SortExpression may only use scopes which equal or 
		/// contain the SortExpressionScope. Aggregates without an explicit scope are not allowed in the SortExpression if no SortExpressionScope is specified.
		/// </summary>
		public RDLCSortExpression SortExpression;
		/// <summary>
		/// Name of the scope (data region or group) in which to evaluate the SortExpression. 
		/// If omitted, the expression will be evaluated and the sort will be performed independently in each detail scope within the SortTarget. 
		/// Must be a scope that is equal to or contained within the current scope. If the text box has no current scope (in other words, it is not contained in any data region), 
		/// SortExpressionScope must be equal to or contained within the SortTarget. Cannot be a detail scope (that is, a group with no group expressions). 
		/// The data set for the SortExpressionScope must be the same as the data set for the SortTarget. Sorting takes place within the group containing the SortExpressionScope. 
		/// For example: In a tablix with a country group and a city group with UserSort on each header and SortExpressionScope of the corresponding group, 
		/// the country sort will sort the country groups within the tablix and the city sort will sort the city groups within each country group (without rearranging the country groups).
		/// </summary>
		public string SortExpressionScope;
		/// <summary>
		/// Name of the data region, group or data set to apply the sort to. If omitted, the sort will apply to the instance of the current scope.
		/// Must be the current scope, an ancestor scope, or a peer scope which is a data region.
		/// 
		/// Tablix groupings are only valid SortTargets from within tablix grouping scopes along the same tablix axis
		/// </summary>
		public string SortTarget;
	}
	/// <summary>
	/// RDLCToggle Image
	/// </summary>
	public class RDLCToggleImage
	{
		/// <summary>
		/// A Boolean expression, the value of which determines the initial state of the toggle image. True = “expanded” (that is, a minus sign). False = “collapsed” (that is, a plus sign).
		/// </summary>
		public bool InitialState;
	}
	/// <summary>
	/// RDLCText Box
	/// </summary>
	public class RDLCTextBox : RDLCReportItem
	{
		/// <summary>
		/// Indicates the Textbox height can increase to accommodate the contents.
		/// </summary>
		public bool CanGrow;
		/// <summary>
		/// Indicates the Textbox height can decrease to match the contents.
		/// </summary>
		public bool CanShrink;
		/// <summary>
		/// Indicates the text should not be displayed when the value of the expression associated with the report item is the same as the preceding visible instance. 
		/// The value of HideDuplicates is the name of a containing group (other than the current group) or data set over which to apply the hiding. 
		/// Each time a new instance of that group is encountered, the first visible instance of this report item will not be hidden. Rows on a previous page are ignored for 
		/// the purposes of hiding duplicates. If the text box is in a tablix cell, only the text will be omitted. 
		/// The text box will remain to provide background and border for the cell. Outside of a tablix cell, the background and borders are omitted as well. 
		/// Ignored unless the text box contains only one TextRun.
		/// </summary>
		public bool HideDuplicates;
		public enum eDataElementStyle
		{
			/// <summary>
			/// Use the setting on the Report element
			/// </summary>
			Auto,
			/// <summary>
			/// Render values as attributes.
			/// </summary>
			Attribute,
			/// <summary>
			/// Render values as elements.
			/// </summary>
			Element
		}
		/// <summary>
		/// Indicates whether all TextRun values for this text box value should render as an element or attribute.
		/// </summary>
		public eDataElementStyle DataElementStyle;
		/// <summary>
		/// Indicates all of the contents of the text box should be kept together on one page if possible.
		/// </summary>
		public bool KeepTogether;
		/// <summary>
		/// Indicates the initial state of a toggling image should one be displayed as a part of the text box.
		/// 
		/// In the event of a textbox spanning multiple pages (due to KeepTogether=False or the textbox being too large for a page) the textbox is split between 
		/// text lines into multiple textboxes. Each individual line of text is always kept together.
		/// </summary>
		public RDLCToggleImage ToggleImage = null;
		/// <summary>
		/// Indicates an end-user sort control should be displayed as a part of this text box in the UI.
		/// </summary>
		public RDLCUserSort UserSort = null;
		public List<RDLCParagraph> Paragraphs = new List<RDLCParagraph>();
	}
	#endregion Inherited Report items
	#region Tablix
	//needs detail structure comparision, documentation with standard

	/// <summary>
	/// RDLCTablix Cell
	/// </summary>
	public class RDLCTablixCell
	{
		public string DataElementName;
		public string DataElementOutput;
		public RDLCCellContents CellContents = null;
	}
	/// <summary>
	/// RDLCTablix Corner Cell
	/// </summary>
	public class RDLCTablixCornerCell
	{
		public RDLCCellContents CellContents = null;
	}
	/// <summary>
	/// RDLCTablix Corner Row
	/// </summary>
	public class RDLCTablixCornerRow
	{
		public List<RDLCTablixCornerCell> Cells = new List<RDLCTablixCornerCell>();
	}
	/// <summary>
	/// RDLCTablix Corner
	/// </summary>
	public class RDLCTablixCorner
	{
		public List<RDLCTablixCornerRow> Rows = new List<RDLCTablixCornerRow>();
	}
	/// <summary>
	/// RDLCTablix Row
	/// </summary>
	public class RDLCTablixRow
	{
		public double Height;
		public List<RDLCTablixCell> Cells = new List<RDLCTablixCell>();
	}
	/// <summary>
	/// RDLCTablix Column
	/// </summary>
	public class RDLCTablixColumn
	{
		public double Width;
	}
	/// <summary>
	/// RDLCTablix Body
	/// </summary>
	public class RDLCTablixBody
	{
		public List<RDLCTablixColumn> Columns = new List<RDLCTablixColumn>();
		public List<RDLCTablixRow> Rows = new List<RDLCTablixRow>();
	}
	/// <summary>
	/// RDLCCell Contents
	/// </summary>
	public class RDLCCellContents
	{
		public int ColSpan;
		public int RowSpan;
		public RDLCReportItem ReportItem = null;
	}
	/// <summary>
	/// RDLCSize
	/// </summary>
	public class RDLCSize
	{
		public enum ePhysicalunits
		{
			Undefined,
			cm,
			mm,
			inch,
			pt,
			pc
		}
		public double Size;
		public ePhysicalunits Unit = ePhysicalunits.Undefined;
	}
	/// <summary>
	/// RDLCTablix Header
	/// </summary>
	public class RDLCTablixHeader
	{
		public RDLCSize Size;
		public RDLCCellContents CellContents = new RDLCCellContents();
	}
	/// <summary>
	/// RDLCSort Expression
	/// </summary>
	public class RDLCSortExpression
	{
		public enum eDirection
		{
			Ascending,
			Descending,
		}
		/// <summary>
		/// The value to sort the groups by. The functions RunningValue and RowNumber are not allowed in SortExpression. References to report items are not allowed.
		/// </summary>
		public string Value;
		/// <summary>
		/// Indicates the direction of the sort
		/// </summary>
		public eDirection Direction = eDirection.Ascending;
	}
	/// <summary>
	/// RDLCGroup
	/// </summary>
	public class RDLCGroup
	{
		/// <summary>
		/// Name of the Group. No two group elements may have the same name. No group element may have the same name as a data set or a data region.
		/// </summary>
		public string Name;
		/// <summary>
		/// Expression: A label to identify an instance of the group in the client UI (to provide a user-friendly label for searching). See ReportItem.Label.
		/// </summary>
		public string DocumentMapLabel;
		/// <summary>
		/// An expression that identifies the parent group in a recursive hierarchy. Only allowed if the group has exactly one group expression. Indicates the following:
		/// 1. Groups should be sorted according to the recursive hierarchy (Sort is still used to sort peer groups).
		/// 2. Labels (in the document map) should be placed/indented according to the recursive hierarchy.
		/// 3. Intra-group show/hide should toggle items according to the recursive hierarchy (see ToggleItem).
		/// If filters on the group eliminate a group instance’s parent, it is instead treated as a child of the parent’s parent. 
		/// In the event of a loop, one of the parent-child relationships will be ignored.
		/// </summary>
		public string Parent;
		/// <summary>
		/// The name to use for the data element for instances of this group. Default: Name of the group Must be a CLS-compliant identifier.
		/// </summary>
		public string DataElementName;
		public enum eDataElementOutput
		{
			/// <summary>
			/// Indicates the instances of the group should appear in the output.
			/// </summary>
			Output,
			/// <summary>
			/// Indicates the instances of the group should not appear in the output.
			/// </summary>
			NoOutput
		}
		/// <summary>
		/// Indicates whether the instances of the group should appear in a data rendering.
		/// </summary>
		public eDataElementOutput DataElementOutput = eDataElementOutput.Output;
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// The expressions by which to group the data. If omitted, this is a detail group (that is, there is one instance of the group per detail row of data).
		/// 
		/// An ordered list of expressions to group the data by. 
		/// The only aggregate function allowed in group expressions is RowNumber (RowNumber must use the immediately containing scope and cannot be used in a GroupExpression 
		/// anywhere within a Tablix Cell). References to report items are not allowed.
		/// </summary>
		public List<string> GroupExpressions = null;
		/// <summary>
		/// Filters to apply to each instance of the group.
		/// </summary>
		public List<RDLCFilter> Filters = null;
		/// <summary>
		/// A set of variables to evaluate at the group level.
		/// </summary>
		public List<RDLCVariable> Variables = null;
		/// <summary>
		/// Defines PageBreak behavior for this group.
		/// </summary>
		public RDLCPageBreak PageBreak = null;
	}
	/// <summary>
	/// RDLCTablix Member
	/// </summary>
	public class RDLCTablixMember
	{
		public string FixedData;
		public bool HideIfNoRows;
		public bool KeepWithGroup;
		public bool RepeatOnNewPage;
		public bool KeepTogether;
		public string DataElementName;
		public string DataElementOutput;
		public List<RDLCTablixMember> Menmbers = null;
		public RDLCTablixHeader Header = null;
		public RDLCVisibility Visibility = null;
		public RDLCGroup Group = null;
		public List<RDLCSortExpression> SortExpressions = null;
		public List<RDLCCustomProperty> CustomProperties = null;

	}
	/// <summary>
	/// RDLCTablix Column Hierarchy
	/// </summary>
	public class RDLCTablixColumnHierarchy
	{
		public List<RDLCTablixMember> Menmbers = new List<RDLCTablixMember>();
	}
	/// <summary>
	/// RDLCTablix Row Hierarchy
	/// </summary>
	public class RDLCTablixRowHierarchy
	{
		public List<RDLCTablixMember> Menmbers = new List<RDLCTablixMember>();
	}
	/// <summary>
	/// RDLCTablix
	/// </summary>
	public class RDLCTablix : RDLCDataRegion
	{
		public enum eLayoutDirection
		{
			RTL, LTR
		}
		public eLayoutDirection LayoutDirection;
		public string GroupsBeforeRowHeaders;
		public string RepeatColumnHeaders;
		public string RepeatRowHeaders;
		public string FixedColumnHeaders;
		public string FixedRowHeaders;
		public bool OmitBorderOnPageBreak;
		public bool KeepTogether;
		public RDLCTablixColumnHierarchy ColumnHierarchy = new RDLCTablixColumnHierarchy();
		public RDLCTablixRowHierarchy RowHierarchy = new RDLCTablixRowHierarchy();
		public RDLCTablixBody Body = new RDLCTablixBody();
		public RDLCTablixCorner Corner = null;
	}
	#endregion Tablix
	#region Chart
	/// <summary>
	/// RDLCChart Title
	/// </summary>
	public class RDLCChartTitle
	{
		/// <summary>
		/// Name of the title.
		/// </summary>
		public string Name;
		/// <summary>
		/// Caption of the title.
		/// </summary>
		public string Caption;
		/// <summary>
		/// Indicates the title should be hidden.
		/// </summary>
		public bool Hidden;
		/// <summary>
		/// Defines style properties for the title. Color, BackgroundColor and BackgroundGradientEndColor all support transparency.
		/// </summary>
		public RDLCStyle Style;
		public enum ePosition
		{
			TopCenter,//Default Position title at TopCenter
			TopLeft,//Position title at TopLeft
			TopRight,//Position title at TopRight
			LeftTop,//Position title at LeftTop
			LeftCenter,//Position title at LeftCenter
			LeftBottom,//Position title at LeftBottom
			RightTop,//Position title at RightTop
			RightCenter,//Position title at RightCenter
			RightBottom,//Position title at RightBottom
			BottomRight,//Position title at BottomRight
			BottomCenter,//Position title at BottomCenter
			BottomLeft,//Position title at BottomLeft
		}
		/// <summary>
		/// The position of the title.
		/// </summary>
		public ePosition Position = ePosition.TopCenter;
		/// <summary>
		/// Name of the chart area on which to draw the title. If omitted (or does not match any chart area name), the title is drawn relative to the chart rather than a specific chart area.
		/// </summary>
		public string DockToChartArea;
		/// <summary>
		/// expression: Indicates the title should be docked outside the chart area rather than inside the chart area. Ignored if DockToChartArea is not set.
		/// </summary>
		public bool DockOutsideChartArea;
		/// <summary>
		/// expression: Offset from the dock location, as a percentage of the chart size. Default: 0.
		/// </summary>
		public int DockOffset;
		/// <summary>
		/// Defines a custom position for the title. If omitted, automatic positioning will be used.
		/// </summary>
		public RDLCChartElementPosition ChartElementPosition = null;
		/// <summary>
		/// expression: Tool tip to display for the title
		/// </summary>
		public string ToolTip;
		/// <summary>
		/// Actions for the title.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		public enum eTextOrientation
		{
			/// <summary>
			/// Indicates the orientation will be selected automatically based on context (for example, Rotated270 for titles docked on the left).
			/// </summary>
			Auto,
			/// <summary>
			/// Horizontal text.
			/// </summary>
			Horizontal,
			/// <summary>
			/// Vertical text – Rotated 90 degrees.
			/// </summary>
			Rotated90,
			/// <summary>
			/// Vertical text – Rotated 270 degrees.
			/// </summary>
			Rotated270,
			/// <summary>
			/// Vertical text – No character rotation.
			/// </summary>
			Stacked,

		}
		/// <summary>
		/// Indicates the orientation of the text.
		/// </summary>
		public eTextOrientation TextOrientation = eTextOrientation.Auto;
	}
	/// <summary>
	/// RDLCChart No Data Message
	/// </summary>
	public class RDLCChartNoDataMessage : RDLCChartTitle
	{
	}
	/// <summary>
	/// RDLCChart Legend Title
	/// </summary>
	public class RDLCChartLegendTitle
	{
		/// <summary>
		/// Caption of the title.
		/// </summary>
		public string Caption;
		public enum eTitleSeparator
		{
			/// <summary>
			/// No separator
			/// </summary>
			None,
			Line,
			ThickLine,
			DoubleLine,
			DashLine,
			DotLine,
			GradientLine,
			ThickGradientLine
		}
		/// <summary>
		/// Indicates what type of separator to use for the legend title.
		/// </summary>
		public eTitleSeparator TitleSeparator = eTitleSeparator.None;
		public RDLCStyle Style = null;
	}
	/// <summary>
	/// RDLCChart Legend
	/// </summary>
	public class RDLCChartLegend
	{
		/// <summary>
		/// Name of the legend.
		/// </summary>
		public string Name;
		/// <summary>
		/// Indicates the legend is hidden.
		/// </summary>
		public bool Hidden;
		/// <summary>
		/// Defines style properties for the legend.
		/// </summary>
		public RDLCStyle Style;
		public enum ePosition
		{
			RightTop,//Default Position legend at RightTop
			TopLeft,//Position legend at TopLeft
			TopCenter,//Position legend at TopCenter
			TopRight,//Position legend at TopRight
			LeftTop,//Position legend at LeftTop
			LeftCenter,//Position legend at LeftCenter
			LeftBottom,//Position legend at LeftBottom
			RightCenter,//Position legend at RightCenter
			RightBottom,//Position legend at RightBottom
			BottomRight,//Position legend at BottomRight
			BottomCenter,//Position legend at BottomCenter
			BottomLeft,//Position legend at BottomLeft
		}
		/// <summary>
		/// The position of the legend.
		/// </summary>
		public ePosition Position = ePosition.RightTop;
		public enum eLayout
		{
			/// <summary>
			/// Automatically arrange labels to fit
			/// </summary>
			AutoTable,
			/// <summary>
			/// Arrange labels in a column
			/// </summary>
			Column,
			/// <summary>
			/// Arrange labels in a row
			/// </summary>
			Row,
			/// <summary>
			/// Arrange labels in a wide table
			/// </summary>
			WideTable,
			/// <summary>
			/// Arrange labels in a tall table
			/// </summary>
			TallTable,
		}
		public eLayout Layout = eLayout.AutoTable;
		/// <summary>
		/// Name of the chart area on which to draw the legend. If omitted (or does not match any chart area name), the legend is drawn relative to the chart rather than a specific chart area.
		/// </summary>
		public string DockToChartArea;
		/// <summary>
		/// expression: Indicates the legend should be docked outside the chart area rather than inside the chart area. Ignored if DockToChartArea is not set.
		/// </summary>
		public bool DockOutsideChartArea;
		/// <summary>
		/// Defines a custom position for the title. If omitted, automatic positioning will be used.
		/// </summary>
		public RDLCChartElementPosition ChartElementPosition = null;
		/// <summary>
		/// Title display in the legend.
		/// </summary>
		public RDLCChartLegendTitle ChartLegendTitle = null;
		/// <summary>
		/// Indicates text will not be autosized to fit in the legend area.
		/// </summary>
		public bool AutoFitTextDisabled;
		/// <summary>
		/// Minimum size for autosized legend text Default: 7pt.
		/// </summary>
		public RDLCSize MinFontSize;
		public enum eHeaderSeparator
		{
			/// <summary>
			/// No separator
			/// </summary>
			None,
			Line,
			ThickLine,
			DoubleLine,
			DashLine,
			DotLine,
			GradientLine,
			ThickGradientLine
		}
		/// <summary>
		/// Indicates what type of separator to use for the legend header.
		/// </summary>
		public eHeaderSeparator HeaderSeparator = eHeaderSeparator.None;
		/// <summary>
		/// Indicates what color to use for the legend header separator.
		/// </summary>
		public string HeaderSeparatorColor;
		public enum eColumnSeparator
		{
			/// <summary>
			/// No separator
			/// </summary>
			None,
			Line,
			ThickLine,
			DoubleLine,
			DashLine,
			DotLine,
			GradientLine,
			ThickGradientLine
		}
		/// <summary>
		/// Indicates what type of separator to use for the columns.
		/// </summary>
		public eColumnSeparator ColumnSeparator = eColumnSeparator.None;
		/// <summary>
		/// Indicates what color to use for the legend column separator.
		/// </summary>
		public string ColumnSeparatorColor;
		/// <summary>
		/// Spacing between columns as a percent of the font size. Default: 50
		/// </summary>
		public int ColumnSpacing;
		/// <summary>
		/// Indicates legend rows should use interlaced colors.
		/// </summary>
		public bool InterlacedRows;
		/// <summary>
		/// The background color to use for interlaced legend rows. If omitted, the chart area background color will be used.
		/// </summary>
		public string InterlacedRowsColor;
		/// <summary>
		/// Indicated legend items should be equally spaced
		/// </summary>
		public bool EquallySpacedItems;
		public enum eOrderReversed
		{
			/// <summary>
			/// Indicates the direction should be autodetected based on the series types.
			/// </summary>
			Auto,
			/// <summary>
			/// Reverse the order of items in the legend.
			/// </summary>
			True,
			/// <summary>
			/// Standard legend item ordering.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates the direction of the legend should be reversed.
		/// </summary>
		public eOrderReversed OrderReversed = eOrderReversed.Auto;
		/// <summary>
		/// Maximum size for the legend, as a percent of the chart size. Default: 50
		/// </summary>
		public int MaxAutoSize = 50;
		/// <summary>
		/// Number of characters after which to wrap the legend text Default: 25
		/// </summary>
		public int TextWrapThreshold = 25;
	}
	/// <summary>
	/// RDLCChart Axis Scale Break
	/// </summary>
	public class RDLCChartAxisScaleBreak
	{
		/// <summary>
		/// Indicates scale breaks can be automatically applied.
		/// </summary>
		public bool Enabled;
		public enum eBreakLineType
		{
			/// <summary>
			/// Display as a ragged line
			/// </summary>
			Ragged,
			/// <summary>
			/// Display as a straight line
			/// </summary>
			Straight,
			Wave,//Display as a wavy line
			None,//Do not display a line for the scale break
		}
		/// <summary>
		/// Type of line used to show the scale break.
		/// </summary>
		public eBreakLineType BreakLineType = eBreakLineType.Ragged;
		/// <summary>
		/// Percent of empty space allowed on the axis before a scale break is triggered. Must be greater than 0. Default: 25
		/// </summary>
		public int CollapsibleSpaceThreshold;
		/// <summary>
		/// Maximum number of scale breaks to apply. Default: 2
		/// </summary>
		public int MaxNumberOfBreaks;
		/// <summary>
		/// Amount of space to leave for a scale break, as a percent of the chart size. Default: 1.5
		/// </summary>
		public double Spacing;

		public enum eIncludeZero
		{
			/// <summary>
			/// Default Determine whether to allow scale breaks to span zero based on the data plotted against the axis
			/// </summary>
			Auto,
			/// <summary>
			/// Do not allow a scale break to span zero.
			/// </summary>
			True,
			/// <summary>
			/// Allow a scale break to span zero.
			/// </summary>
			False,

		}
		/// <summary>
		/// Indicates whether to prevent a scale break from spanning zero. Auto (Default) | True | False
		/// </summary>
		public eIncludeZero IncludeZero;
		/// <summary>
		/// Defines style properties for the scale break.
		/// </summary>
		public RDLCStyle Style = null;
	}
	/// <summary>
	/// RDLCChart Strip Line
	/// </summary>
	public class RDLCChartStripLine
	{
		/// <summary>
		/// Defines style properties for the strip line.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Title for the strip line.
		/// </summary>
		public string Title;
		public enum eTextOrientation
		{
			/// <summary>
			/// Indicates the orientation will be selected automatically based on context (for example,Rotated270 for titles docked on the left).
			/// </summary>
			Auto,
			Horizontal,//Horizontal text.
			Rotated90,//Vertical text – Rotated 90 degrees.
			Rotated270,//Vertical text – Rotated 270 degrees.
			Stacked,//Vertical text – No character rotation.
		}
		/// <summary>
		/// Indicates the orientation of the title text.
		/// </summary>
		public eTextOrientation TextOrientation = eTextOrientation.Auto;
		/// <summary>
		/// Actions for the strip line.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Tool tip to display for the strip line.
		/// </summary>
		public string ToolTip;
		/// <summary>
		/// Size of the strip line. Default: 0
		/// </summary>
		public double Interval;
		public enum eIntervalType
		{
			/// <summary>
			/// Interval unit is autoderived based on the data plotted against the axis.
			/// </summary>
			Auto,
			Number,//Interval is numeric
			Years,//Interval is Years
			Months,//Interval is Months
			Weeks,//Interval is Weeks
			Days,//Interval is Days
			Hours,//Interval is Hours
			Minutes,//Interval is Minutes
			Seconds,//Interval is Seconds
			Milliseconds,///Interval is Milliseconds
		}
		/// <summary>
		/// units for the Interval
		/// </summary>
		public eIntervalType IntervalType = eIntervalType.Auto;
		/// <summary>
		/// Offset from the previous strip line or axis min (for the first strip line). Default: 0
		/// </summary>
		public double IntervalOffset;
		/// <summary>
		/// Units for the IntervalOffset
		/// </summary>
		public eIntervalType IntervalOffsetType = eIntervalType.Auto;
		/// <summary>
		/// Width of the strip line.
		/// </summary>
		public double StripWidth;
		/// <summary>
		/// Units for the StripWidth
		/// </summary>
		public eIntervalType StripWidthType = eIntervalType.Auto;
	}
	/// <summary>
	/// RDLCChart Axis
	/// </summary>
	public class RDLCChartAxis
	{
		/// <summary>
		/// Name of the axis (used when there is more than one axis along a dimension).
		/// </summary>
		public string Name;
		public enum eVisible
		{
			/// <summary>
			/// Indicates the axis should be displayed if it is in use (for example, a series is plotted against it or it has a title).
			/// </summary>
			Auto,
			/// <summary>
			/// Display the axis.
			/// </summary>
			True,
			/// <summary>
			/// Hide the axis.
			/// </summary>
			False
		}
		/// <summary>
		/// Whether the axis is displayed.
		/// </summary>
		public eVisible Visible = eVisible.Auto;
		/// <summary>
		/// Defines text style properties for the axis labels and
		/// </summary>
		public RDLCStyle Style = null;
		public enum eLocation
		{
			/// <summary>
			/// Draw the axis on the default side.
			/// </summary>
			Default,
			/// <summary>
			/// Draw the axis on the opposite side.
			/// </summary>
			Opposite
		}
		/// <summary>
		/// Indicates whether the axis is drawn on the default side (for example, left for the value axis on a line chart) or on the opposite side.
		/// </summary>
		public eLocation Location = eLocation.Default;
		public enum eMargin
		{
			/// <summary>
			/// Indicates the margins are included based on the series type/subtype.
			/// </summary>
			Auto,
			/// <summary>
			/// The axis has a margin.
			/// </summary>
			True,
			/// <summary>
			/// The axis has no margin.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates whether an axis margin will be created. The size of the margin is automatically generated based on the Scale and the number of data points.
		/// </summary>
		public eMargin Margin;
		//public RLDCChartAxisTitle ChartAxisTitle = null;
		/// <summary>
		/// Default interval between gridlines, tick marks and labels. Default (0), means the axis is autodivided.
		/// </summary>
		public double Interval;
		public enum eIntervalType
		{
			/// <summary>
			/// Interval unit is autoderived based on the data plotted against the axis.
			/// </summary>
			Auto,
			Number,//Interval is numeric
			Years,//Interval is Years
			Months,//Interval is Months
			Weeks,//Interval is Weeks
			Days,//Interval is Days
			Hours,//Interval is Hours
			Minutes,//Interval is Minutes
			Seconds,//Interval is Seconds
			Milliseconds,///Interval is Milliseconds
		}
		/// <summary>
		/// Default units for the Interval
		/// </summary>
		public eIntervalType IntervalType = eIntervalType.Auto;
		/// <summary>
		/// Default offset for the first tick mark from the axis min. Default: 0
		/// </summary>
		public double IntervalOffset;
		/// <summary>
		/// Default units for the IntervalOffset
		/// </summary>
		public eIntervalType IntervalOffsetType = eIntervalType.Auto;
		/// <summary>
		/// Indicates if an automatic interval is calculated, it should be based on available size. Otherwise, the interval will be calculated based only on the data range.
		/// </summary>
		public bool VariableAutoInterval;
		/// <summary>
		/// Interval between labels. Default (0) uses ChartAxis.Interval
		/// </summary>
		public double LabelInterval;
		public enum eLabelIntervalType
		{
			/// <summary>
			/// Uses ChartAxis.IntervalType.
			/// </summary>
			Default,
			/// <summary>
			/// LabelIntervalOffset unit is autoderived based on the data plotted against the axis.
			/// </summary>
			Auto,
			Number,//Interval is numeric
			Years,//Interval is Years
			Months,//Interval is Months
			Weeks,//Interval is Weeks
			Days,//Interval is Days
			Hours,//Interval is Hours
			Minutes,//Interval is Minutes
			Seconds,//Interval is Seconds
			Milliseconds,///Interval is Milliseconds
		}
		/// <summary>
		/// Units for the LabelInterval.
		/// </summary>
		public eLabelIntervalType LabelIntervalType = eLabelIntervalType.Default;
		/// <summary>
		/// Indicates major gridlines should be displayed for this axis.
		/// </summary>
		public RDLCChartMajorGridLines ChartMajorGridLines = null;
		/// <summary>
		/// Indicates minor gridlines should be displayed for this axis.
		/// </summary>
		public RDLCChartMinorGridLines ChartMinorGridLines = null;
		/// <summary>
		/// Defines major tick marks for the axis.
		/// </summary>
		public RDLCChartMinorTickMarks ChartMinorTickMarks = null;
		/// <summary>
		/// Defines minor tick marks for the axis.
		/// </summary>
		public RDLCChartMajorTickMarks ChartMajorTickMarks = null;
		/// <summary>
		/// Indicates the marks should stay with the edge of the plot area rather than moving with the axis.
		/// </summary>
		public bool MarksAlwaysAtPlotEdge;
		/// <summary>
		/// Indicates the axis should be plotted in the reverse direction.
		/// </summary>
		public bool Reverse;
		/// <summary>
		/// Value at which to cross the other axis. If omitted (or error in expression), uses the default behavior for the chart type. Overrides Location.
		/// </summary>
		public string CrossAt;
		/// <summary>
		/// If this property is true then strip lines are drawn every other grid line interval for the axis. 
		/// If grid lines are not used for the axis then the axis’ tick marks or labels are used to determine the interlaced strip lines interval.
		/// </summary>
		public bool Interlaced;
		/// <summary>
		/// Color of the interlaced strips.
		/// </summary>
		public string InterlacedColor;
		/// <summary>
		/// Custom strip lines for the axis.
		/// </summary>
		public List<RDLCChartStripLine> ChartStripLines = null;
		public enum eArrowsType
		{
			/// <summary>
			/// No arrows
			/// </summary>
			None,
			Triangle,
			SharpTriangle,
			/// <summary>
			/// Lines Only
			/// </summary>
			Lines

		}
		public eArrowsType Arrows = eArrowsType.None;
		/// <summary>
		/// Indicates the values along this axis are scalar values (that is, numeric or date) which should be displayed on the chart in a continuous axis. 
		/// Scalar cannot be true if the axis has more than one group, if it has a static group or a group with more than one group expression. 
		/// The type of scalar (date, integer, float) is derived from the first non-null value found. All values are converted to that type. 
		/// If any non-scalar value is present, the axis will revert to non-scalar. Treated as True if this is a 
		/// ChartCategoryAxis and any ChartSeries plotted against this axis contains a ChartDataPoint with ChartDataPointValues.X defined.
		/// </summary>
		public bool Scalar;
		/// <summary>
		/// Minimum value for the axis. If omitted (or error in expression), the axis autoscales.
		/// </summary>
		public string Minimum;
		/// <summary>
		/// Maximum value for the axis. If omitted (or error in expression), the axis autoscales.
		/// </summary>
		public string Maximum;
		/// <summary>
		/// Indicates the axis is logarithmic.
		/// </summary>
		public bool LogScale;
		/// <summary>
		/// Base to use for logarithmic scale. Default: 10
		/// </summary>
		public double LogBase;
		/// <summary>
		/// Indicates the axis labels are hidden.
		/// </summary>
		public bool HideLabels;
		/// <summary>
		/// The angle at which to display axis labels. Must be an integer between –90 and 90Default: 0
		/// </summary>
		public double Angle;
		/// <summary>
		/// Indicates the axis label font size will not be reduced to fit within the chart.
		/// </summary>
		public bool PreventFontShrink;
		/// <summary>
		/// Indicates the axis label font size will not be increased to fit within the chart.
		/// </summary>
		public bool PreventFontGrow;
		/// <summary>
		/// Indicates the axis labels will not be staggered to fit within the chart.
		/// </summary>
		public bool PreventLabelOffset;
		/// <summary>
		/// Indicates the axis labels will not be word-wrapped to fit within the chart.
		/// </summary>
		public bool PreventWordWrap;
		public enum eAllowLabelRotation
		{
			/// <summary>
			/// Default Rotate in 90 degree increments
			/// </summary>
			Rotate90,
			/// <summary>
			/// Rotate in 30 degree increments
			/// </summary>
			Rotate30,
			/// <summary>
			/// Rotate in 45 degree increments
			/// </summary>
			Rotate45,
			/// <summary>
			/// Rotation is not allowed
			/// </summary>
			None,

		}
		/// <summary>
		/// Indicates the “step” by which axis labels can be incrementally rotated to fit within the chart. 
		/// </summary>
		public eAllowLabelRotation AllowLabelRotation;

		/// <summary>
		/// Indicates the axis should always include zero. Ignored if Minimum is set.
		/// </summary>
		public bool IncludeZero;
		/// <summary>
		/// Indicates axis labels should not be automatically adjusted to fit.
		/// </summary>
		public bool LabelsAutoFitDisabled;
		/// <summary>
		/// Minimum font size when autofitting labels.
		/// </summary>
		public RDLCSize MinFontSize;
		/// <summary>
		/// Maximum font size when autofitting labels.
		/// </summary>
		public RDLCSize MaxFontSize;
		/// <summary>
		/// Indicates the labels should be offset.
		/// </summary>
		public string OffsetLabels;
		/// <summary>
		/// Indicates labels should be hidden at axis ends.
		/// </summary>
		public bool HideEndLabels;
		/// <summary>
		/// Defines scale break behavior for the axis.
		/// </summary>
		public RDLCChartAxisScaleBreak ChartAxisScaleBreak;
		/// <summary>
		/// Custom properties for the axis.
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
	}
	/// <summary>
	/// RDLCChart Category Axes
	/// </summary>
	public class RDLCChartCategoryAxes
	{
		public List<RDLCChartAxis> Axis = new List<RDLCChartAxis>();
	}
	/// <summary>
	/// RDLCChart Value Axes
	/// </summary>
	public class RDLCChartValueAxes
	{
		public List<RDLCChartAxis> Axis = new List<RDLCChartAxis>();
	}
	/// <summary>
	/// RDLCChart Element Position
	/// </summary>
	public class RDLCChartElementPosition
	{
		/// <summary>
		/// The distance of the item from the top of the containing object, as a percentage of the container. Defaults to 0 if omitted.
		/// </summary>
		public double Top;
		/// <summary>
		/// The distance of the item from the left of the containing object, as a percentage of the container. Defaults to 0 if omitted.
		/// </summary>
		public double Left;
		/// <summary>
		/// Height of the item as a percentage of its containing object. Defaults to 100 minus Top if omitted.
		/// </summary>
		public double Height;
		/// <summary>
		/// Width of the item as a percentage of its containing object. Defaults to 100 minus Left if omitted.
		/// </summary>
		public double Width;
	}
	/// <summary>
	/// RDLCChart Inner Plot Position
	/// </summary>
	public class RDLCChartInnerPlotPosition : RDLCChartElementPosition
	{
	}
	/// <summary>
	/// RDLCChart Align Type
	/// </summary>
	public class RDLCChartAlignType
	{
		/// <summary>
		/// expression: Indicates the chart areas should align on axes views.
		/// </summary>
		public bool AxesView;
		/// <summary>
		/// expression: Indicates the chart areas should align on cursors
		/// </summary>
		public bool Cursor;
		/// <summary>
		/// expression: Indicates the chart areas should align on chart area positions.
		/// </summary>
		public bool Position;
		/// <summary>
		/// expression: Indicates the chart areas should align on inner plot positions.
		/// </summary>
		public bool InnerPlotPosition;
	}
	/// <summary>
	/// RDLCChart Area
	/// </summary>
	public class RDLCChartArea
	{
		public string Name;
		/// <summary>
		/// expression: Indicates the chart area should be hidden.
		/// </summary>
		public bool Hidden;
		/// <summary>
		/// Defines the category axes.
		/// </summary>
		public RDLCChartCategoryAxes CategoryAxes = null;
		/// <summary>
		/// Defines the value axes
		/// </summary>
		public RDLCChartValueAxes ValueAxes = null;
		/// <summary>
		/// Properties for a 3D chart layout
		/// </summary>
		public RDLCChartThreeDProperties ChartThreeDProperties = null;
		/// <summary>
		/// Defines style properties for the chart area. Each of the properties of type Color support transparency.
		/// </summary>
		public RDLCStyle Style = null;
		public enum eAlignOrientation
		{
			/// <summary>
			/// No alignment.
			/// </summary>
			None,
			Vertical,
			Horizontal,
			/// <summary>
			/// Both vertical and horizontal alignment.
			/// </summary>
			All
		}
		/// <summary>
		/// Indicates in which directions the chart area should be aligned with the target chart area.
		/// </summary>
		public eAlignOrientation AlignOrientation = eAlignOrientation.None;
		/// <summary>
		/// Indicates which aspects of the chart area should be aligned with the target chart area. Ignored if AlignWithChartArea is not set.
		/// </summary>
		public RDLCChartAlignType ChartAlignType = null;
		/// <summary>
		/// Name of a chart area with which to align this chart area
		/// </summary>
		public string AlignWithChartArea = null;
		/// <summary>
		/// Defines a custom position for the chart area. If omitted, automatic positioning will be used
		/// </summary>
		public RDLCChartElementPosition ChartElementPosition = null;
		/// <summary>
		/// Defines a custom position for the inner plot area. If omitted, automatic positioning will be used.
		/// </summary>
		public RDLCChartInnerPlotPosition ChartInnerPlotPosition = null;
		/// <summary>
		/// expression: Indicates the same font size should be used for all axes (if the font size is automatic).
		/// </summary>
		public bool EquallySizedAxesFont;

	}
	/// <summary>
	/// RDLCChart Border Skin
	/// </summary>
	public class RDLCChartBorderSkin
	{
		public enum eChartBorderSkinType
		{
			/// <summary>
			/// No border skin
			/// </summary>
			None,
			/// <summary>
			/// Use Emboss border skin
			/// </summary>
			Emboss,
			/// <summary>
			/// Use Raised border skin
			/// </summary>
			Raised,
			/// <summary>
			/// Use Sunken border skin
			/// </summary>

			Sunken,
			/// <summary>
			/// Use FrameThin1 border skin
			/// </summary>
			FrameThin1,
			/// <summary>
			/// Use FrameThin2 border skin
			/// </summary>
			FrameThin2,
			/// <summary>
			/// Use FrameThin3 border skin
			/// </summary>
			FrameThin3,
			/// <summary>
			/// Use FrameThin4 border skin
			/// </summary>
			FrameThin4,
			/// <summary>
			/// Use FrameThin5 border skin
			/// </summary>
			FrameThin5,
			/// <summary>
			/// Use FrameThin6 border skin
			/// </summary>
			FrameThin6,
			/// <summary>
			/// Use FrameTitle1 border skin
			/// </summary>
			FrameTitle1,
			/// <summary>
			/// Use FrameTitle2 border skin
			/// </summary>
			FrameTitle2,
			/// <summary>
			/// Use FrameTitle3 border skin
			/// </summary>
			FrameTitle3,
			/// <summary>
			/// Use FrameTitle4 border skin
			/// </summary>
			FrameTitle4,
			/// <summary>
			/// Use FrameTitle5 border skin
			/// </summary>
			FrameTitle5,
			/// <summary>
			/// Use FrameTitle6 border skin
			/// </summary>
			FrameTitle6,
			/// <summary>
			/// Use FrameTitle7 border skin
			/// </summary>
			FrameTitle7,
			/// <summary>
			/// Use FrameTitle8 border skin
			/// </summary>
			FrameTitle8,
		}
		/// <summary>
		/// Border skin type for the chart
		/// </summary>
		public eChartBorderSkinType ChartBorderSkinType = eChartBorderSkinType.None;
		/// <summary>
		/// Style properties for the border skin. Each of the properties of type Color support transparency.
		/// </summary>
		public RDLCStyle Style;

	}
	/// <summary>
	/// RDLCChart Member
	/// </summary>
	public class RDLCChartMember
	{
		/// <summary>
		/// expression: The label displayed on the legend (for series members and category members when ChartSeries.Type = Shape)
		/// </summary>
		public string Label;
		/// <summary>
		/// Submembers contained within this member.
		/// </summary>
		public List<RDLCChartMember> Menmbers = null;
		/// <summary>
		/// The expressions by which to group the data. If omitted, this is a static member (otherwise, this is a dynamic member). 
		/// Not allowed if any ancestor member is a detail group. Page breaks in the group are not allowed..
		/// </summary>
		public RDLCGroup Group = null;
		/// <summary>
		/// The expressions by which to sort the member instances. Not allowed if Group is omitted.
		/// </summary>
		public List<RDLCSortExpression> SortExpressions = null;
		/// <summary>
		/// Custom properties for the member.
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// The name to use for the data element for this member. Must be a CLS-compliant identifier. Default for dynamic members: [Group.Name] Collection Default for static members: [Label]
		/// 
		/// Since Label is an expression, this is the one case where the DataElementName technically may vary per instance. 
		/// In the event the Label property evaluates to a string which is not a CLS-compliant identifier, the value provided to the renderer will be Null.
		/// </summary>
		public string DataElementName;
		public enum eDataElementOutput
		{
			/// <summary>
			/// Behaves as Output for dynamic members. Behaves as ContentsOnly for static members.
			/// </summary>
			Auto,
			/// <summary>
			/// Indicates the member should appear in the output.
			/// </summary>
			Output,
			/// <summary>
			/// Indicates the member should not appear in the output.
			/// </summary>
			NoOutput,
		}
		/// <summary>
		/// Indicates whether the member should appear in a data rendering
		/// </summary>
		public eDataElementOutput DataElementOutput = eDataElementOutput.Auto;
	}
	/// <summary>
	/// RDLCChart Category Hierarchy
	/// </summary>
	public class RDLCChartCategoryHierarchy
	{
		public List<RDLCChartMember> Menmbers = new List<RDLCChartMember>();
	}
	/// <summary>
	/// RDLCChart Series Hierarchy
	/// </summary>
	public class RDLCChartSeriesHierarchy
	{
		public List<RDLCChartMember> Menmbers = new List<RDLCChartMember>();
	}
	/// <summary>
	/// RDLCChart Three DProperties
	/// </summary>
	public class RDLCChartThreeDProperties
	{
		//Enabled
		//0-1
		//Expression (Boolean)
		//Whether or not a chart is displayed in 3D. Default is False (2D).
		//ProjectionMode
		//0-1
		//Expression (Enum)
		//The projection mode used for the 3D rendering.
		//Value
		//Description
		//Oblique
		//Default Use an oblique projection
		//Perspective
		//Use a perspective projection
		//Perspective
		//0-1
		//Expression (Integer)
		//Represents the percent of perspective. Applies only for Perspective projection. Default: 0
		//Rotation
		//0-1
		//Expression (Integer)
		//Rotation angle Default: 30
		//Inclination
		//0-1
		//Expression (Integer)
		//Inclination angle Default: 30
		//DepthRatio
		//0-1
		//Expression (Integer)
		//Ratio (in percent)between depth and width. Default: 100
		//Shading
		//0-1
		//Expression (Enum)
		//Type of 3D shading.
		//Value
		//Description
		//Real
		//Default Realistic shading
		//Simple
		//Simplified shading
		//None
		//No shading
		//GapDepth
		//0-1
		//Expression (Integer)
		//Percent depth gap between 3D bars and columns. Default: 100
		//WallThickness
		//0-1
		//Expression (Integer)
		//Percent thickness of outer walls. Default: 7
		//Clustered
		//0-1
		//Expression (Boolean)
		//Determines if data series are clustered (displayed along distinct rows). Only applies to bar and column chart types. Defaults to false.
	}
	/// <summary>
	/// RDLCChart Grid Lines
	/// </summary>
	public abstract class RDLCChartGridLines
	{
		//Enabled
		//0-1
		//Expression (Enum)
		//Indicates the gridlines should be shown.
		//Value
		//Description
		//Auto
		//Default True for major grid lines and false for minor grid lines.
		//True
		//Show the grid lines.
		//False
		//Hide the grid lines.
		//Style
		//0-1
		//Element
		//Line style properties for the grid lines.
		//Interval
		//0-1
		//Expression (Float)
		//Interval between gridlines. Default (0) uses ChartAxis.Interval.
		//IntervalType
		//0-1
		//Expression (Enum)
		//Units for the Interval.
		//Value
		//Description
		//Default
		//Default Uses ChartAxis.IntervalType
		//Auto
		//Interval unit is autoderived based on the data plotted against the axis.
		//Number
		//Interval is numeric
		//Years
		//Interval is Years
		//Months
		//Interval is Months
		//Weeks
		//Interval is Weeks
		//Days
		//Interval is Days
		//Hours
		//Interval is Hours
		//Minutes
		//Interval is Minutes
		//Seconds
		//Interval is Seconds
		//Milliseconds
		//Interval is Milliseconds
		//IntervalOffset
		//0-1
		//Expression (Float)
		//Offset for the first gridline from the axis min. Default (0) uses ChartAxis.IntervalOffset.

		//IntervalOffsetType
		//0-1
		//Expression (Enum)
		//Units for the IntervalOffset.
		//Value
		//Description
		//Default
		//Default Uses ChartAxis.IntervalOffsetType.
		//Auto
		//IntervalOffset unit is autoderived based on the data plotted against the axis.
		//Number
		//IntervalOffset is numeric
		//Years
		//IntervalOffset is Years
		//Months
		//IntervalOffset is Months
		//Weeks
		//IntervalOffset is Weeks
		//Days
		//IntervalOffset is Days
		//Hours
		//IntervalOffset is Hours
		//Minutes
		//IntervalOffset is Minutes
		//Seconds
		//IntervalOffset is Seconds
		//Milliseconds
		//IntervalOffset is Milliseconds
	}
	/// <summary>
	/// RDLCChart Major Grid Lines
	/// </summary>
	public class RDLCChartMajorGridLines : RDLCChartGridLines
	{

	}
	/// <summary>
	/// RDLCChart Minor Grid Lines
	/// </summary>
	public class RDLCChartMinorGridLines : RDLCChartGridLines
	{

	}
	/// <summary>
	/// RDLCChart Tick Marks
	/// </summary>
	public abstract class RDLCChartTickMarks
	{
		//Enabled
		//0-1
		//Expression (Enum)
		//Indicates the tick marks should be shown. Auto (Default) | True | False
		//Value
		//Description
		//Auto
		//Default True for major tick marks and false for minor tick marks.
		//True
		//Show the tick marks.
		//False
		//Hide the tick marks.
		//Type
		//0-1
		//Expression (Enum)
		//Type of the tick mark None | Inside | Outside (Default) | Cross
		//Value
		//Description
		//Outside
		//Default Tick mark outside the axis.
		//Inside
		//Tick mark inside the axis.
		//Cross
		//Tick mark across the axis.
		//None
		//No tick mark.
		//Style
		//0-1
		//Element
		//Line style properties for the tick marks.
		//Length
		//0-1
		//Expression (Float)
		//Length of the tick mark, as a percentage of the chart size. Default: 1
		//Interval
		//0-1
		//Expression (Float)
		//Interval between tick marks. Default (0) uses ChartAxis.Interval.
		//IntervalType
		//0-1
		//Expression (Enum)
		//Units for the Interval.
		//Value
		//Description
		//Default
		//Default Uses ChartAxis.IntervalType.
		//Auto
		//Interval unit is autoderived based on the data plotted against the axis.
		//Number
		//Interval is numeric
		//Years
		//Interval is Years
		//Months
		//Interval is Months
		//Weeks
		//Interval is Weeks
		//Days
		//Interval is Days
		//Hours
		//Interval is Hours
		//Minutes
		//Interval is Minutes
		//Seconds
		//Interval is Seconds
		//Milliseconds
		//Interval is Milliseconds
		//IntervalOffset
		//0-1
		//Expression (Float)
		//Offset for the first tick mark from the axis min. Default (0) uses ChartAxis.IntervalOffset.

		//IntervalOffsetType
		//0-1
		//Expression (Enum)
		//Units for the IntervalOffset.
		//Value
		//Description
		//Default
		//Default Uses ChartAxis.IntervalOffsetType.
		//Auto
		//IntervalOffset unit is autoderived based on the data plotted against the axis.
		//Number
		//IntervalOffset is numeric
		//Years
		//IntervalOffset is Years
		//Months
		//IntervalOffset is Months
		//Weeks
		//IntervalOffset is Weeks
		//Days
		//IntervalOffset is Days
		//Hours
		//IntervalOffset is Hours
		//Minutes
		//IntervalOffset is Minutes
		//Seconds
		//IntervalOffset is Seconds
		//Milliseconds
		//IntervalOffset is Milliseconds
	}
	/// <summary>
	/// RDLCChart Major Tick Marks
	/// </summary>
	public class RDLCChartMajorTickMarks : RDLCChartTickMarks
	{

	}
	/// <summary>
	/// RDLCChart Minor Tick Marks
	/// </summary>
	public class RDLCChartMinorTickMarks : RDLCChartTickMarks
	{

	}
	/// <summary>
	/// RDLCChart Custom Palette Colors
	/// </summary>
	public class RDLCChartCustomPaletteColors
	{
		public List<string> ChartCustomPaletteColors = new List<string>();
	}
	/// <summary>
	/// RDLCChart Data Point Values
	/// </summary>
	public class RDLCChartDataPointValues
	{
		public RDLCSize X, Y, Size;
	}
	/// <summary>
	/// RDLCChart Data Point
	/// </summary>
	public class RDLCChartDataPoint
	{
		/// <summary>
		/// The name to use for the data element for this data point. Default: Name of corresponding static series or category. 
		/// If there is no static series or categories, “Value” Must be a CLS-compliant identifier.
		/// </summary>
		public string DataElementName;
		public enum eDataElementOutput
		{
			ContentsOnly,//Default Indicates the data point should not appear in the output, but its values should be rendered as if they were in the cell’s container.
			Output,//Indicates the data point should appear in the output.
			NoOutput,//Indicates the data point should not appear in the output.

		}
		/// <summary>
		/// Indicates whether the data point should appear in a data rendering.
		/// </summary>
		public eDataElementOutput DataElementOutput = eDataElementOutput.ContentsOnly;
		/// <summary>
		/// Actions associated with this data point.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Defines appearance of the data point marker.
		/// </summary>
		public RDLCChartMarker Marker = null;
		/// <summary>
		/// Indicates the values should be marked with data labels.
		/// </summary>
		public RDLCChartDataLabel DataLabel = null;
		/// <summary>
		/// Data values for the point.
		/// </summary>
		public RDLCChartDataPointValues Values = null;
		/// <summary>
		/// Label to use on the axis for the data point.
		/// </summary>
		public string AxisLabel;
		/// <summary>
		/// Tool tip to display for the data point.
		/// </summary>
		public string ToolTip;
		/// <summary>
		/// Defines style properties for the data point.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Defines how the data point appears when displayed in a legend (when Series.Type = Shape).
		/// </summary>
		public RDLCChartItemInLegend ChartItemInLegend = null;
		/// <summary>
		/// Custom properties for the data point. This includes all custom chart attributes.
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
	}
	/// <summary>
	/// RDLCChart Data Label
	/// </summary>
	public class RDLCChartDataLabel
	{
		public double Value;
		public enum ePosition
		{
			Auto,//Default
			Top,//Position label at Top of data point
			TopLeft,//Position label at TopLeft of data point
			TopRight,//Position label at TopRight of data point
			Left,//Position label at Left of data point
			Center,//Position label at Center of data point
			Right,//Position label at Right of data point
			BottomRight,//Position label at BottomRight of data point
			Bottom,//Position label at Bottom of data point
			BottomLeft,//Position label at BottomLeft of data point
			Outside,//Position label Outside of data point For non-Pie charts, Outside is treated as Top.
		}
		/// <summary>
		/// Position of the label.
		/// </summary>
		public ePosition Position = ePosition.Auto;
		/// <summary>
		/// Actions associated with this data label.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Defines style properties for the labels. Supplied styles override Series styles.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Indicates the Y value of the data point should be used as the label.
		/// </summary>
		public bool UseValueAsLabel;
		/// <summary>
		/// Label for the data point. Not used if UseValueAsLabel = True
		/// </summary>
		public string Label = null;
		/// <summary>
		/// Whether the data label is displayed on the chart. Defaults to False.
		/// </summary>
		public bool Visible;
		/// <summary>
		/// Angle of rotation of the label text.
		/// </summary>
		public int Rotation;
		/// <summary>
		/// Tool tip to display for the data label.
		/// </summary>
		public string ToolTip = null;
	}
	/// <summary>
	/// RDLCChart Smart Label
	/// </summary>
	public class RDLCChartSmartLabel
	{
		/// <summary>
		/// Indicates smart labels should be turned off.
		/// </summary>
		public bool Disabled;


		//AllowOutSidePlotArea
		//0-1
		//Expression (Enum)
		//Indicates whether datapoint labels can be drawn outside of the plot area. True | False | Partial (Default)
		//Value
		//Description
		//Partial
		//Default Labels can be partially outside the plot area.
		//True
		//Labels can be entirely outside the plot area.
		//False
		//Labels must be entirely inside the plot area.
		//CalloutBackColor
		//0-1
		//Expression (Color)
		//Fill color of the box around the point label text when the CalloutStyle = Box
		//CalloutLineAnchor
		//0-1
		//Expression (Enum)
		//Shape that should be drawn on the point end of the callout line. None | Arrow (Default) | Diamond | Square | Round
		//CalloutLineColor
		//0-1
		//Expression (Color)
		//Color of the callout line. Default: Black

		//CalloutLineStyle
		//0-1
		//Expression (Enum)
		//Style of the callout line.
		//Value
		//Description
		//Solid
		//Default Solid line
		//None
		//No line
		//Dotted
		//Dotted line
		//Dashed
		//Dashed line
		//Double
		//Double solid line
		//DashDot
		//Dash-dot line
		//DashDotDot
		//Dash-dot-dot line
		//CalloutLineWidth
		//0-1
		//Expression (Size)
		//Width of the callout line. Default: 0.75pt
		//CalloutStyle
		//0-1
		//Expression (Enum)
		//Style to use when drawing the callout lines. None | Underline (Default) | Box
		//Value
		//Description
		//Underline
		//Default Attach the callout line to an underline on the label
		//Box
		//Attach the callout line to an box around the label
		//None
		//No additional label style for the callout line

		//ShowOverlapped
		//0-1
		//Expression (Boolean)
		//Indicates labels should be displayed even when overlapping issues cannot be resolved.

		//MarkerOverlapping
		//0-1
		//Expression (Boolean)
		//Indicates point labels are allowed to overlap point markers.
		//MaxMovingDistance
		//0-1
		//Expression (Size)
		//The maximum distance from the data point that data point labels can be moved to prevent overlapping. Default: 23 pt.
		//MinMovingDistance
		//0-1
		//Expression (Size)
		//The minimum distance from the data point that data point labels can be moved to prevent overlapping.
		//ChartNoMoveDirections
		//0-1
		//Element
		//Indicates which directions the label is not allowed to move
	}
	/// <summary>
	/// RDLCChart No Move Directions
	/// </summary>
	public class RDLCChartNoMoveDirections
	{
		//Up
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move directly up.
		//Left
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move directly left.
		//Right
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move directly right.
		//Down
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move directly down.
		//UpLeft
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move up-left.
		//UpRight
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move up-right.
		//DownLeft
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move down-left.
		//DownRight
		//0-1
		//Expression (Boolean)
		//Indicates the smart label will not move down-right.
	}
	/// <summary>
	/// RDLCChart Marker
	/// </summary>
	public class RDLCChartMarker
	{
		public enum eType
		{
			None,//Default No marker
			Square,//Square marker
			Circle,//Circle marker
			Diamond,//Diamond marker
			Triangle,//Triangle marker
			Cross,//Cross marker
			Star4,//Star (4 points) marker
			Star5,//Star (5 points) marker
			Star6,//Star (6 points) marker
			Star10,//Star (10 points) marker
			Auto,//Automatically cycle through marker types for each series
		}
		/// <summary>
		/// Defines the marker type for values.
		/// </summary>
		public eType Type = eType.None;
		/// <summary>
		/// Represents the height and width of the plotting area of marker(s). Default: 3.75pt.
		/// </summary>
		public RDLCSize Size;
		public RDLCStyle Style;
	}
	/// <summary>
	/// RDLCChart Item In Legend
	/// </summary>
	public class RDLCChartItemInLegend
	{
		/// <summary>
		/// Label to use in the legend for the item For ChartDataPoint, if LegendText is omitted, the Label properties from the ChartCategoriesHierarchy are used as 
		/// the legend text (concatenated with “ – “ between each pair).
		/// </summary>
		public string LegendText = null;
		/// <summary>
		/// Tool tip to display for the item in the legend.
		/// </summary>
		public string ToolTip = null;
		/// <summary>
		/// Actions associated with the item in the legend.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Indicates the item should not be shown in the legend.
		/// </summary>
		public bool Hidden;
	}
	/// <summary>
	/// RDLCChart Empty Points
	/// </summary>
	public class RDLCChartEmptyPoints
	{
		/// <summary>
		/// Defines style properties for the data point.
		/// </summary>
		public RDLCStyle Style = null;
		/// <summary>
		/// Custom properties for the data point. This includes all custom series type attributes.
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// Actions associated with the data point.
		/// </summary>
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Defines appearance of the data point marker.
		/// </summary>
		public RDLCChartMarker Marker = null;
		/// <summary>
		/// Indicates the values should be marked with data labels.
		/// </summary>
		public RDLCChartDataLabel DataLabel = null;
		/// <summary>
		/// Label to use on the axis for empty data points.
		/// </summary>
		public string AxisLabel = null;
		/// <summary>
		/// Tool tip to display for the data point.
		/// </summary>
		public string ToolTip = null;
	}
	/// <summary>
	/// RDLCChart Series
	/// </summary>
	public class RDLCChartSeries
	{
		/// <summary>
		/// Name of the series.
		/// </summary>
		public string Name;
		/// <summary>
		/// Indicates the series should be hidden.
		/// </summary>
		public bool Hidden;
		public enum eType
		{
			Column,//Default Column chart
			Bar,//Bar chart
			Line,//Line chart
			Shape,//Shape chart
			Scatter,//Scatter chart
			Area,//Area chart
			Range,//Range chart
			Polar,//Polar chart
		}
		/// <summary>
		/// Visualization type for the series
		/// </summary>
		public eType Type = eType.Column;
		public enum eSubType
		{
			Plain,//Default for all Types except Shape
			Stacked,//For Column, Bar and Area
			PercentStacked,//For Column, Bar and Area
			Smooth,//For Line, Area and Range
			Stepped,//For Line only
			Pie,//Default for Shape
			ExplodedPie,//For Shape only
			Doughnut,//For Shape only
			ExplodedDoughnut,//For Shape only
			Funnel,//For Shape only
			Pyramid,//For Shape only
			Bubble,//For Scatter only
			StackedForArea,//For Area only
			PercentStackedForArea,//For Area only
			Candlestick,//For Range only
			Stock,//For Range only
			Bar,//For Range only
			Column,//For Range only
			BoxPlot,//For Range only
			ErrorBar,//For Range only
			Radar,//For Polar only
		}
		/// <summary>
		/// Visualization subtype for the series. Available subtypes (and default subtype) depends on Type.
		/// If an invalid Subtype is specified, the default Subtype for the specified Type is used.
		/// </summary>
		public eSubType Subtype;
		/// <summary>
		/// Name of the legend in which this series should appear.
		/// </summary>
		public string LegendName = null;
		/// <summary>
		/// Name of the chart area in which to plot the series. Defaults to the first chart area in the chart.
		/// </summary>
		public string ChartAreaName = null;
		/// <summary>
		/// Name of the value axis against which to plot this series. If omitted, the series should be plotted against the first value axis.
		/// </summary>
		public string ValueAxisName = null;
		/// <summary>
		/// Name of the category axis against which to plot this series. If omitted, the series should be plotted against the first category axis.
		/// </summary>
		public string CategoryAxisName = null;
		/// <summary>
		/// Defines behavior of empty points in the series.
		/// </summary>
		public RDLCChartEmptyPoints ChartEmptyPoints = null;
		public RDLCActionInfo ActionInfo = null;
		/// <summary>
		/// Data points within the series. Mandatory for ChartSeries within ChartSeriesCollection. Must be omitted for ChartSeries within DerivedChartSeriesCollection.
		/// </summary>
		public List<RDLCChartDataPoint> ChartDataPoints = new List<RDLCChartDataPoint>();
		/// <summary>
		/// Defines style properties for the series.
		/// </summary>
		public RDLCStyle Style = null;
		public RDLCChartAxis ReferenceAxis;
		public RDLCChartArea ReferenceArea;
		public RDLCChartLegend ReferenceLegend;
		/// <summary>
		/// Indicates the values should be marked with data labels. Applies only within DerivedSeries.
		/// </summary>
		public RDLCChartDataLabel ChartDataLabel = null;
		/// <summary>
		/// Defines appearance of the data point marker. Applies only within DerivedSeries.
		/// </summary>
		public RDLCChartMarker ChartMarker = null;
		/// <summary>
		/// Custom properties for the series. This includes all custom chart attributes for series.
		/// 
		/// ChartSeries and ChartDataPoint support a set of custom attributes which modify the visualization behavior of certain series types and subtypes.
		/// See http://support.dundas.com/OnlineDocumentation/WinChart2003/CustomAttributes_All.html Upgrade note: This includes PointWidth and DrawingStyle, which were previously RDL elements.
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// Defines how the series appears when displayed in a legend.
		/// </summary>
		public RDLCChartItemInLegend ChartItemInLegend;
		/// <summary>
		/// Smart label properties.
		/// </summary>
		public RDLCChartSmartLabel ChartSmartLabel;

	}
	/// <summary>
	/// RDLCChart Derived Series
	/// </summary>
	public class RDLCChartDerivedSeries
	{
		/// <summary>
		/// Name of the series from which to derive.
		/// </summary>
		public string SourceChartSeriesName;
		/// <summary>
		/// Formula to apply to the data values from the source series.
		/// See http://support.dundas.com/OnlineDocumentation/WinChart2003/FormulasOverview.html
		/// </summary>
		public string DerivedSeriesFormula;
		/// <summary>
		/// Parameters to the formula.
		/// </summary>
		public List<RDLCChartFormulaParameter> FormulaParameters = null;
		/// <summary>
		/// Series properties for the derived series.
		/// </summary>
		public RDLCChartSeries ChartSeries;
	}
	/// <summary>
	/// RDLCChart Formula Parameter
	/// </summary>
	public class RDLCChartFormulaParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name;
		/// <summary>
		/// Value of the parameter if the value does not depend on the actual data points.
		/// </summary>
		public string Value;
		/// <summary>
		/// Name of the ChartDataPointValue property to use as the value of this parameter.
		/// </summary>
		public string Source;
	}
	/// <summary>
	/// RDLCChart Code Parameter
	/// </summary>
	public class RDLCChartCodeParameter
	{
		//Name
		//1
		//Name
		//Name of the parameter.
		//Value
		//1
		//Expression (Variant)
		//Value of the parameter.
	}
	/// <summary>
	/// RDLCChart Data
	/// </summary>
	public class RDLCChartData
	{
		public List<RDLCChartSeries> Series = new List<RDLCChartSeries>();
		public List<RDLCChartDerivedSeries> DerivedSeries = null;
	}
	/// <summary>
	/// RDLCChart
	/// </summary>
	public class RDLCChart : RDLCDataRegion
	{
		public enum ePalette
		{
			/// <summary>
			/// 
			/// </summary>
			Default,
			EarthTones,
			Excel,
			GrayScale,
			Light,
			Pastel,
			SemiTransparent,
			Berry,
			Chocolate,
			Fire,
			SeaGreen,
			BrightPastel,
			Custom
		}
		/// <summary>
		/// Determines the color palette for the chart items.
		/// </summary>
		public ePalette Palette = ePalette.Default;
		public enum ePaletteHatchBehavior
		{
			/// <summary>
			/// Treated as None.
			/// </summary>
			Default,
			/// <summary>
			/// No hatching will be added to the data points
			/// </summary>
			None,
			/// <summary>
			/// Automatic hatching will be applied to all data points (unless BackgroundHatchType is specified as non-Default).
			/// </summary>
			Always
		}
		public ePaletteHatchBehavior PaletteHatchBehavior = ePaletteHatchBehavior.Default;
		/// <summary>
		/// expression: The height to which the chart should grow/shrink. Height is used as the initial height for relative layout changes due to resizing.
		/// </summary>
		public RDLCSize DynamicHeight;
		/// <summary>
		/// expression: The width to which the chart should grow/shrink. Width is used as the initial width for relative layout changes due to resizing.
		/// </summary>
		public RDLCSize DynamicWidth;
		/// <summary>
		/// The hierarchy of category members for the chart.
		/// </summary>
		public RDLCChartCategoryHierarchy CategoryHierarchy = null;
		/// <summary>
		/// The hierarchy of series members for the chart.
		/// </summary>
		public RDLCChartSeriesHierarchy SeriesHierarchy = null;
		public List<RDLCChartTitle> ChartTitles = null;
		public List<RDLCChartLegend> ChartLegends = null;
		public List<RDLCChartArea> ChartAreas = null;
		/// <summary>
		/// Defines the data values for the chart.
		/// </summary>
		public RDLCChartData ChartData = null;
		/// <summary>
		/// Defines a border skin for the chart.
		/// </summary>
		public RDLCChartBorderSkin ChartBorderSkin = null;
		/// <summary>
		/// Title to display if the chart contains no data.
		/// </summary>
		public RDLCChartNoDataMessage ChartNoDataMessage = null;
	}
	#endregion Chart
	#region Custom Report Item
	//needs detail structure comparision, documentation with standard
	/// <summary>
	/// RDLCData Member
	/// </summary>
	public class RDLCDataMember
	{
		public string SubTotal;
		public List<RDLCDataMember> Menmbers = null;
		public RDLCGroup Group = null;
		public List<RDLCSortExpression> SortExpressions = null;
		public List<RDLCCustomProperty> CustomProperties = null;
	}
	/// <summary>
	/// RDLCData Column Hierarchy
	/// </summary>
	public class RDLCDataColumnHierarchy
	{
	}
	/// <summary>
	/// RDLCData Row Hierarchy
	/// </summary>
	public class RDLCDataRowHierarchy
	{
	}
	/// <summary>
	/// RDLCData Cell
	/// </summary>
	public class RDLCDataCell
	{
		public List<RDLCValue> Values = new List<RDLCValue>();
	}
	/// <summary>
	/// RDLCData Row
	/// </summary>
	public class RDLCDataRow
	{
		public List<RDLCDataCell> Cells = new List<RDLCDataCell>();
	}
	/// <summary>
	/// RDLCCustom Data
	/// </summary>
	public class RDLCCustomData
	{
		public string DataSetName;
		public List<RDLCFilter> Filters = null;
		public List<RDLCDataRow> Rows = null;
		public RDLCDataRowHierarchy RowHierarchy = null;
		public RDLCDataColumnHierarchy ColumnHierarchy = null;
	}
	/// <summary>
	/// RDLCAlt Report Item
	/// </summary>
	public class RDLCAltReportItem
	{
		public RDLCReportItem ReportItem;
	}
	/// <summary>
	/// RDLCCustom Report Item
	/// </summary>
	public class RDLCCustomReportItem : RDLCReportItem
	{
		public RDLCAltReportItem AltReportItem = null;
		public RDLCCustomData Data = null;
	}
	#endregion Custom Report Item
	#region Gauge Panel
	//needs detail structure comparision, documentation with standard
	/// <summary>
	/// RDLCGauge Scale Range
	/// </summary>
	public class RDLCGaugeScaleRange
	{
	}
	/// <summary>
	/// RDLCRadial Gauge Pointer
	/// </summary>
	public class RDLCRadialGaugePointer : RDLCGaugePointer
	{
	}
	/// <summary>
	/// RDLCLinear Gauge Pointer
	/// </summary>
	public class RDLCLinearGaugePointer : RDLCGaugePointer
	{
	}
	/// <summary>
	/// RDLCGauge Pointer
	/// </summary>
	public class RDLCGaugePointer
	{
	}
	/// <summary>
	/// RDLCCustom Label
	/// </summary>
	public class RDLCCustomLabel
	{
	}
	/// <summary>
	/// RDLCRadial Gauge Scale
	/// </summary>
	public class RDLCRadialGaugeScale : RDLCGaugeScale
	{
		public List<RDLCRadialGaugePointer> Pointers = null;
		public List<RDLCGaugeScaleRange> Ranges = null;
	}
	/// <summary>
	/// RDLCLinear Gauge Scale
	/// </summary>
	public class RDLCLinearGaugeScale : RDLCGaugeScale
	{
		public List<RDLCLinearGaugePointer> Pointers = null;
		public List<RDLCGaugeScaleRange> Ranges = null;
	}
	/// <summary>
	/// RDLCGauge Scale
	/// </summary>
	public class RDLCGaugeScale
	{
		public List<RDLCCustomLabel> CustomLabels = null;
	}
	/// <summary>
	/// RDLCLinear Gauge
	/// </summary>
	public class RDLCLinearGauge : RDLCGauge
	{
		public List<RDLCLinearGaugeScale> Scales = null;
	}
	/// <summary>
	/// RDLCRadial Gauge
	/// </summary>
	public class RDLCRadialGauge : RDLCGauge
	{
		public List<RDLCRadialGaugeScale> Scales = null;
	}
	/// <summary>
	/// RDLCGauge Member
	/// </summary>
	public class RDLCGaugeMember
	{
		public List<RDLCGaugeMember> Members = null;
		public RDLCGroup Group = null;
		public List<RDLCSortExpression> SortExpressions = null;
	}
	/// <summary>
	/// RDLCGauge Label
	/// </summary>
	public class RDLCGaugeLabel
	{
	}
	/// <summary>
	/// RDLCGauge
	/// </summary>
	public class RDLCGauge
	{
	}
	/// <summary>
	/// RDLCGauge Panel
	/// </summary>
	public class RDLCGaugePanel
	{
		public RDLCGaugeMember Member = null;
		public List<RDLCLinearGauge> LinearGauges = null;
		public List<RDLCRadialGauge> RadialGauges = null;
		public List<RDLCGaugeLabel> Labels = null;
	}
	#endregion Gauge Panel
	#endregion Report Item
	#region Report Layout
	/// <summary>
	/// RDLCReport Element
	/// </summary>
	public abstract class RDLCReportElement
	{
		public RDLCStyle Style = null;
	}
	/// <summary>
	/// RDLCBody
	/// </summary>
	public class RDLCBody : RDLCReportElement
	{
		/// <summary>
		/// Height of the body.
		/// </summary>
		public RDLCSize Height;
		/// <summary>
		/// The region that contains the elements of the report body.
		/// </summary>
		public List<RDLCReportItem> ReportItems = null;
		/// <summary>
		/// add Report Item
		/// </summary>
		/// <param name="ReportItem">Report Item</param>
		/// <returns></returns>
		public RDLCReportItem addReportItem(RDLCReportItem ReportItem)
		{
			if (ReportItems == null)
				ReportItems = new List<RDLCReportItem>();
			ReportItems.Add(ReportItem);
			return ReportItem;
		}
	}
	/// <summary>
	/// RDLCPage Section
	/// </summary>
	public abstract class RDLCPageSection : RDLCReportElement
	{
		/// <summary>
		/// Height of the page section.
		/// </summary>
		public RDLCSize Height;
		/// <summary>
		/// Indicates if the page section should be shown on the first page of the report. Not used in single-page reports if this is a PageFooter.
		/// </summary>
		public bool PrintOnFirstPage;
		/// <summary>
		/// Indicates if the page section should be shown on the last page of the report. Not used in single-page reports if this is a PageHeader.
		/// </summary>
		public bool PrintOnLastPage;
		/// <summary>
		/// The region that contains the elements of the page section layout
		/// No data regions or subreports are allowed in the page section. 
		/// All page breaks are ignored in the page section.
		/// </summary>
		public List<RDLCReportItem> ReportItems = null;
		/// <summary>
		/// add Report Item
		/// </summary>
		/// <param name="ReportItem">Report Item</param>
		/// <returns></returns>
		public RDLCReportItem addReportItem(RDLCReportItem ReportItem)
		{
			if (ReportItems == null)
				ReportItems = new List<RDLCReportItem>();
			ReportItems.Add(ReportItem);
			return ReportItem;
		}
	}
	/// <summary>
	/// RDLCPage Header
	/// </summary>
	public class RDLCPageHeader : RDLCPageSection
	{
	}
	/// <summary>
	/// RDLCPage Footer
	/// </summary>
	public class RDLCPageFooter : RDLCPageSection
	{
	}
	/// <summary>
	/// RDLCPage
	/// </summary>
	public class RDLCPage
	{
		/// <summary>
		/// Default height for rendering the report in a physical-page oriented renderer. Default: 11 in. Must be greater than 0 in.
		/// </summary>
		public RDLCSize PageHeight;
		/// <summary>
		///	Default width for rendering the report in a physical-page oriented renderer. Default: 8.5 in. Must be greater than 0 in.
		/// </summary>
		public RDLCSize PageWidth;
		/// <summary>
		/// Default height for rendering the report when in an interactive renderer. There is no maximum size. A value of 0 (with any unit) indicates height should be unlimited. 
		/// Defaults to PageHeight.
		/// </summary>
		public RDLCSize InteractiveHeight;
		/// <summary>
		/// Default width for rendering the report when in an interactive renderer. There is no maximum size.
		/// A value of 0 (with any unit) indicates width should be unlimited. Defaults to PageWidth.
		/// </summary>
		public RDLCSize InteractiveWidth;
		/// <summary>
		/// Width of the left margin. Default: 0 in.
		/// </summary>
		public RDLCSize LeftMargin;
		/// <summary>
		/// Width of the right margin. Default: 0 in.
		/// </summary>
		public RDLCSize RightMargin;
		/// <summary>
		/// Width of the top margin. Default: 0 in.
		/// </summary>
		public RDLCSize TopMargin;
		/// <summary>
		/// Width of the bottom margin. Default: 0 in.
		/// </summary>
		public RDLCSize BottomMargin;
		/// <summary>
		/// Default number of columns for rendering the report Default: 1. Min: 1. Max: 1000
		/// </summary>
		public int Columns;
		/// <summary>
		/// Spacing between each column in multi-column renderings. Default: 0.5 in.
		/// </summary>
		public RDLCSize ColumnSpacing;
		/// <summary>
		/// The header that is rendered at the top of each page of the report.
		/// </summary>
		public RDLCPageHeader PageHeader = null;
		/// <summary>
		/// The footer that is rendered at the bottom of each page of the report.
		/// </summary>
		public RDLCPageFooter PageFooter = null;
		/// <summary>
		/// Style information for the page.
		/// </summary>
		public RDLCStyle Style = null;
	}
	/// <summary>
	/// RDLCEmbedded Image
	/// </summary>
	public class RDLCEmbeddedImage
	{
		public string Name;
		/// <summary>
		/// The MIMEType for the image. Valid values are as follows: image/bmp, image/jpeg, image/gif, image/png, image/x-png
		/// </summary>
		public string MimeType;
		/// <summary>
		/// Base-64 encoded image data.
		/// </summary>
		public string ImageData;
	}
	/// <summary>
	/// RDLCClass
	/// </summary>
	public class RDLCClass
	{
		/// <summary>
		/// The name of the class
		/// </summary>
		public string ClassName;
		/// <summary>
		/// The name of the member variable of Class to assign the class to. This member variable can be used in expressions throughout the report.
		/// </summary>
		public string InstanceName;
	}
	/// <summary>
	/// RDLCCode Module
	/// </summary>
	public class RDLCCodeModule
	{
		/// <summary>
		/// Name of the code module to load
		/// </summary>
		public string Name;
	}
	/// <summary>
	/// RDLCCustom Property
	/// </summary>
	public class RDLCCustomProperty
	{
		/// <summary>
		/// Name of the property. Properties with null or duplicate names are not allowed.
		/// </summary>
		public string Name;
		/// <summary>
		/// Value of the property.
		/// </summary>
		public string Value;
	}
	/// <summary>
	/// RDLCVariable
	/// </summary>
	public class RDLCVariable
	{
		/// <summary>
		/// The Variable element defines a named expression to be evaluated within the group or report.
		/// </summary>
		public string Name;
		/// <summary>
		/// Expression to evaluate globally for the report or for each group instance. Unlike expressions evaluated in visual elements of the report, 
		/// each instance of this expression is calculated only once when the report is executed and never recalculated during subsequent renderings. 
		/// This is necessary for time-dependent calculations.
		/// </summary>
		public string Value;
	}
	#endregion Report Layout
	#region Report Data
	#region data source
	/// <summary>
	/// RDLCConnection Properties
	/// </summary>
	public class RDLCConnectionProperties
	{
		/// <summary>
		/// The type of the data source. (for example “SQL”, “OLEDB”, “OLEDB-MD”) This is the name of a registered data provider.
		/// </summary>
		public string DataProvider;
		/// <summary>
		/// The connection string for the data source.
		/// </summary>
		public string ConnectString;
		/// <summary>
		/// Indicates that this data source should be connected to using integrated security.
		/// </summary>
		public bool IntegratedSecurity;
		/// <summary>
		/// The prompt displayed to the user when prompting for database credentials for this data source.
		/// </summary>
		public string Prompt;

	}
	/// <summary>
	/// RDLCData Source
	/// </summary>
	public class RDLCDataSource
	{
		/// <summary>
		/// The name of the data source. Must be unique in the report.
		/// </summary>
		public string Name;
		/// <summary>
		/// Indicates the data sets that use this data source should be executed in a single transaction
		/// </summary>
		public bool Transaction;
		/// <summary>
		/// The full folder path (for example, “/salesreports/salesdatabase”) or relative path (for example, “salesdatabase”) to a data source on the same server. 
		/// Relative paths start in the same folder as the report. The data source uses the connection properties from the DataSourceReference.
		/// </summary>
		public string DataSourceReference;
		/// <summary>
		/// Information about how to connect to the data source.
		/// </summary>
		public RDLCConnectionProperties ConnectionProperties = null;
	}
	#endregion data source
	#region data set
	/// <summary>
	/// RDLCQuery Parameter
	/// </summary>
	public class RDLCQueryParameter
	{
		/// <summary>
		/// Name of the parameter
		/// </summary>
		public string Name;
		/// <summary>
		/// An expression that evaluates to the value to hand to the data source. 
		/// The expression can refer to report parameters but cannot contain references to report elements, fields in the data model or aggregate functions. 
		/// In the case of a parameter to a Values or DefaultValue query, the expression can only refer to report parameters that occur earlier in the parameters list. 
		/// The value for this query parameter is then taken from the user selection for that earlier report parameter. 
		/// The Value element has an optional DataType attribute which specifies the data type of the value in the event it is a constant. 
		/// It may be set to any RDL data type (see ReportParameter.DataType). If omitted, constant values are assumed to be strings.
		/// </summary>
		public string Value;
	}
	/// <summary>
	/// RDLCQuery
	/// </summary>
	public class RDLCQuery
	{
		/// <summary>
		/// Name of the data source to execute the query against.
		/// </summary>
		public string DataSourceName;
		public enum eCommandType
		{
			/// <summary>
			/// The CommandText contains a query command to execute.
			/// </summary>
			Text,
			/// <summary>
			/// The CommandText contains the name of a stored procedure to execute.
			/// </summary>
			StoredProcedure,
			/// <summary>
			/// The CommandText contains the name of a table from which to retrieve rows.
			/// </summary>
			TableDirect
		}
		/// <summary>
		/// Indicates what type of query is contained in the CommandText.
		/// </summary>
		public eCommandType CommandType;
		/// <summary>
		/// The query to execute to obtain the data for the report.
		/// </summary>
		public string CommandText;
		/// <summary>
		/// Number of seconds to allow for the query to run before timing out. Must be nonnegative. If omitted or zero, the query should not time out. Max: 2147483647
		/// </summary>
		public int TimeOut;
		/// <summary>
		/// A list of parameters that are passed to the data source as part of the query.
		/// The QueryParameters element contains parameters that are passed to the data source as part of the query.
		/// </summary>
		public List<RDLCQueryParameter> QueryParameters = null;
	}
	/// <summary>
	/// RDLCFilter Value
	/// </summary>
	public class RDLCFilterValue
	{
		/// <summary>
		/// A value to use for comparison (via the Operation) to the value of the FilterExpression. 
		/// See Filter Expression Restrictions later in this document. 
		/// The FilterValue element has an optional DataType attribute which specifies the data type of the value in the event it is a constant. 
		/// It may be set to any RDL data type (see ReportParameter.DataType). 
		/// If omitted, constant values are assumed to be strings.
		/// </summary>
		public string Value;
	}
	/// <summary>
	/// RDLCFilter
	/// </summary>
	public class RDLCFilter
	{
		public enum eOperator
		{
			/// <summary>
			/// Equality comparison.
			/// </summary>
			Equal,
			/// <summary>
			/// Like comparison. Uses the same special characters as the Visual Basic LIKE operator (for example “?” to represent a single character and
			/// “*” to represent any series of characters)
			/// </summary>
			Like,
			NotEqual,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			/// <summary>
			/// Check if FilterExpression is in top N (as defined by the FilterValue) values.
			/// </summary>
			TopN,
			/// <summary>
			/// Check if FilterExpression is in top N (as defined by the FilterValue) values.
			/// </summary>
			BottomN,
			/// <summary>
			/// Check if FilterExpression is in top N percent (as defined by the FilterValue) values.
			/// </summary>
			TopPercent,
			/// <summary>
			/// Check if FilterExpression is in bottom N percent (as defined by the FilterValue) values.
			/// </summary>
			BottomPercent,
			/// <summary>
			/// Check if FilterExpression is equal to any FilterValue.
			/// </summary>
			In,
			/// <summary>
			/// Check if FilterExpression is between the two FilterValues.
			/// </summary>
			Between,
		}
		/// <summary>
		/// An expression that is evaluated for each instance within the group or each row of the data set or data region and compared (via the Operator) 
		/// to the FilterValues. Failed comparisons result in the row/instance being filtered out of the data set, data region or group. 
		/// See Filter Expression Restrictions later in this document.
		/// 
		/// Filter expressions/values cannot contain references to report items. 
		/// Data Set and Data Region filter expressions/values cannot contain aggregate functions. 
		/// Group filter expressions/values cannot contain RunningValue or RowNumber. 
		/// Group filter expressions/values cannot use the First or Last aggregate with anything other than the default (current) scope. 
		/// Failure when evaluating any filter expression or filter value causes the report to immediately return an error.
		/// </summary>
		public string FilterExpression;
		/// <summary>
		/// The operator used to compare the FilterExpression and FilterValues.
		/// 
		/// Notes: Top and Bottom operators include ties in the resulting data. 
		/// String comparisons are locale-dependent. Null equals Null. 
		/// TopPercent and BottomPercent round up and down respectively, 
		/// if the percentage would result in a partial item being included (for example Top 25% of 13 items is 4 items whereas Bottom 75% is 9 items).
		/// </summary>
		public eOperator Operator;
		/// <summary>
		/// The values to compare to the FilterExpression15. 
		/// For Equal, Like, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual, TopN, BottomN, TopPercent and BottomPercent, there must be exactly one FilterValue. 
		/// For TopN and BottomN, the FilterValue expression must evaluate to an integer. 
		/// For TopPercent and BottomPercent, the FilterValue expression must evaluate to an integer or float. 
		/// For Between, there must be exactly two FilterValue elements. 
		/// For In, the FilterValues are treated as a set (if the FilterExpression value appears anywhere in the set of FilterValues, the instance is not filtered out.)
		/// </summary>
		public List<RDLCFilterValue> FilterValues = new List<RDLCFilterValue>();
	}
	/// <summary>
	/// RDLCField
	/// </summary>
	public class RDLCField
	{
		/// <summary>
		/// Name to use for the field in the report. Note: Field names need only be unique within the containing Fields collection.
		/// </summary>
		public string Name;
		/// <summary>
		/// Name of the field in the query. Note: Data field names do not have to be unique. 
		/// Multiple fields can refer to the same data field name (although a warning will be generated during publishing).
		/// </summary>
		public string DataField;
		/// <summary>
		/// An expression that evaluates to the value of this field. 
		/// For example: =Fields!Price.Value+Fields!Tax.Value 
		/// The expression cannot contain aggregates or references to report items. 
		/// The Value element has an optional DataType attribute which specifies the data type of the value in the event it is a constant. 
		/// It may be set to any RDL data type (see ReportParameter.DataType). If omitted, constant values are assumed to be strings.
		/// </summary>
		public string Value;
	}
	/// <summary>
	/// RDLCData Set
	/// </summary>
	public class RDLCDataSet
	{
		/// <summary>
		/// Name of the data set. Cannot be the same name as any data region or group.
		/// </summary>
		public string Name;
		public enum eCaseSensitivity
		{
			/// <summary>
			/// The case sensitivity setting should be autoderived by querying the data provider. Defaults to False if the data provider does not support that method
			/// </summary>
			Auto,
			/// <summary>
			/// Data in this data set is case sensitive.
			/// </summary>
			True,
			/// <summary>
			/// Data in this data set is case insensitive
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates if the data is case sensitive.
		/// </summary>
		public eCaseSensitivity CaseSensitivity = eCaseSensitivity.Auto;
		public enum eAccentSensitivity
		{
			/// <summary>
			/// The accent sensitivity setting should be autoderived by querying the data provider. Defaults to False if the data provider does not support that method.
			/// </summary>
			Auto,
			/// <summary>
			/// Data in this data set is accent sensitive.
			/// </summary>
			True,
			/// <summary>
			/// Data in this data set is accent insensitive.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates if the data is accent sensitive.
		/// </summary>
		public eAccentSensitivity AccentSensitivity = eAccentSensitivity.Auto;
		public enum eKanatypeSensitivity
		{
			/// <summary>
			/// The Kanatype sensitivity setting should be autoderived by querying the data provider. Defaults to False if the data provider does not support that method.
			/// </summary>
			Auto,
			/// <summary>
			/// Data in this data set is Kanatype sensitive.
			/// </summary>
			True,
			/// <summary>
			/// Data in this data set is Kanatype insensitive.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates if the data is Kanatype sensitive.
		/// </summary>
		public eKanatypeSensitivity KanatypeSensitivity = eKanatypeSensitivity.Auto;
		public enum eWidthSensitivity
		{
			/// <summary>
			/// The Width sensitivity setting should be autoderived by querying the data provider. Defaults to False if the data provider does not support that method.
			/// </summary>
			Auto,
			/// <summary>
			/// Data in this data set is Width sensitive.
			/// </summary>
			True,
			/// <summary>
			/// Data in this data set is Width insensitive.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates if the data is Width sensitive.
		/// </summary>
		public eWidthSensitivity WidthSensitivity = eWidthSensitivity.Auto;
		public enum eInterpretSubtotalsAsDetails
		{
			/// <summary>
			/// Subtotal rows will be treated as details if the report does not use the Aggregate() function to access any fields in this data set.
			/// </summary>
			Auto,
			/// <summary>
			/// Subtotal rows should be interpreted as detail rows.
			/// </summary>
			True,
			/// <summary>
			/// Subtotals rows are retrieved only via the Aggregate function.
			/// </summary>
			False
		}
		/// <summary>
		/// Indicates whether subtotal rows returned from a data provider that supports server subtotals should be interpreted as detail rows instead.
		/// </summary>
		public eInterpretSubtotalsAsDetails InterpretSubtotalsAsDetails = eInterpretSubtotalsAsDetails.Auto;
		/// <summary>
		/// The locale to use for the collation sequence for sorting data.
		/// Uses the standard Microsoft SQL Server collation names. If no Collation is specified, the collation setting should be autoderived by querying the data provider. 
		/// Defaults to the collation corresponding to the report’s Language property if the data provider does not support that method or returns an unsupported or invalid value.
		/// </summary>
		public string Collation;

		/// <summary>
		/// Information about the data source, including connection information, query, and so on, required to get the data from the data source
		/// </summary>
		public RDLCQuery Query;
		/// <summary>
		/// The fields in the data set. (the fields in the data model.)
		/// The data model maps to the fields in SQL and OLE-DB queries based on name. Each field in the data model corresponds to the field in the OLE-DB rowset of the same name.
		/// Multi-dimensional data rowsets (OLE-DB for OLAP) also map to the data model based on name. Each level and measure in the multi-dimensional cube corresponds to a field in the data model.
		/// </summary>
		public List<RDLCField> Fields = null;
		public List<RDLCFilter> Filters = null;
	}
	#endregion data set
	#region Report Parameter
	/// <summary>
	/// RDLCData Set Reference
	/// </summary>
	public class RDLCDataSetReference
	{
		/// <summary>
		/// Name of the data set to use.
		/// </summary>
		public string DataSetName;
		/// <summary>
		/// Name of the field to use for the values/defaults for the parameter.
		/// </summary>
		public string ValueField;
		/// <summary>
		/// Name of the field to use for the value to display to the user for the selection. If not supplied or the returned value is null, 
		/// the value in the ValueField is used. Not used for DefaultValue.
		/// </summary>
		public string LabelField;
	}
	#region default value
	/// <summary>
	/// RDLCValue
	/// </summary>
	public class RDLCValue
	{
		/// <summary>
		/// A value used as a default for a parameter. 
		/// Cannot refer to Fields or ReportItems or any parameters that occur after the current parameter. 
		/// If the Value expression returns an array, each item in the array is treated as a single value. Items in the array must not be arrays. 
		/// For single-value parameters, only the first item in the array is used. This element is nullable.
		/// </summary>
		public string value;
	}
	/// <summary>
	/// RDLCDefault Value
	/// </summary>
	public class RDLCDefaultValue
	{
		/// <summary>
		/// The default values for the parameter
		/// 
		/// A set of values (used as defaults for a parameter). For single-value parameters, only a single Value is allowed.
		/// </summary>
		public List<RDLCValue> Values = null;
		/// <summary>
		/// The query to execute to obtain the default value(s) for the parameter. 
		/// For single-value parameters, the default is the first value of the ValueField. 
		/// For multivalue parameters, the default is all values of the ValueField.
		/// </summary>
		public RDLCDataSetReference DataSetReference = null;
	}
	#endregion default value
	#region valid values
	/// <summary>
	/// RDLCParameter Value
	/// </summary>
	public class RDLCParameterValue
	{
		/// <summary>
		/// Expression:
		/// Possible value for the parameter. 
		/// For Boolean parameters, use “true” and “false” 
		/// For DateTime parameters, use ISO 8601.
		/// For Float parameters, use “.” As the optional decimal separator. 
		/// If the Value expression returns an array, each item in the array is treated as a single value. The items in the array must not be arrays.
		/// </summary>
		public string Value;
		/// <summary>
		/// Expression:
		/// Label for the value to display in the UI. If not supplied, the Value is used as the label (if Value is not supplied, Label is the empty string). 
		/// If the Value expression returns an array, the Label expression must return an array with the same number of items. If the Value expression does not return an array, 
		/// the Label expression must not return an array.
		/// </summary>
		public string Label;
	}
	/// <summary>
	/// RDLCValid Values
	/// </summary>
	public class RDLCValidValues
	{
		/// <summary>
		/// Hardcoded values for the parameter.
		/// </summary>
		public List<RDLCParameterValue> ParameterValues = null;
		/// <summary>
		/// The query to execute to obtain a list of possible values for the parameter.
		/// </summary>
		public RDLCDataSetReference DataSetReference = null;
	}
	#endregion valid values
	/// <summary>
	/// RDLCReport Parameter
	/// </summary>
	public class RDLCReportParameter
	{
		/// <summary>
		/// Name of the parameter. (This is the name used when expressions refer to the parameter.) Note: Parameter names need only be unique within the containing Parameters collection.
		/// </summary>
		public string Name;
		public enum eDataType
		{
			Boolean,
			DateTime,
			Integer,
			Float,
			String
		}
		/// <summary>
		/// The data type of the parameter
		/// </summary>
		public eDataType DataType;
		/// <summary>
		/// Indicates the value for this parameter can be Null. Cannot be true if this is a multivalue parameter. (Not currently supported by any data extensions that support multivalue parameters.)
		/// </summary>
		public bool Nullable;
		/// <summary>
		/// Indicates the value for this parameter can be the empty string. Ignored if DataType is not String.
		/// </summary>
		public bool AllowBlank;
		/// <summary>
		/// The user prompt to display when asking for parameter values (Expression).
		/// If omitted, the user should not be prompted for or allowed to otherwise provide a value for this parameter.
		/// </summary>
		public string Prompt;
		/// <summary>
		/// Indicates the parameter should not be displayed to the user (however, it will still be available for programmatic use with subreports, drillthrough reports etc.)
		/// </summary>
		public bool Hidden;
		/// <summary>
		/// Indicates this is a multivalue parameter (a parameter that can take a set of values). Multivalue parameters are accessed in expressions as zero-based arrays in the 
		/// Value and Label properties (for example, Parameters!Cities.Value(0) and Parameters!Cities.Label(0)). Ignored for Boolean parameters.
		/// </summary>
		public bool MultiValue;
		public enum eUsedInQuery
		{
			/// <summary>
			/// True if any query parameter value expression is a simple reference to this parameter or there are any subreports in the report or there exists any query parameter 
			/// value expression that is anything other than a constant or a simple parameter reference.
			/// </summary>
			Auto,
			/// <summary>
			/// The parameter is used in a query in the report.
			/// </summary>
			True,
			/// <summary>
			/// The parameter is not used in a query in the report.
			/// </summary>
			False

		}
		/// <summary>
		/// Indicates whether the parameter is used in a query in the report. This is necessary to determine if the queries must be rerun if the parameter changes.
		/// </summary>
		public eUsedInQuery UsedInQuery = eUsedInQuery.Auto;
		/// <summary>
		/// Default value to use for the parameter (if not provided by the user). If no value is provided as a part of the definition or by the user, the value is null. Required if there is no Prompt and either Nullable is False or a ValidValues list is provided that does not contain Null (an omitted Value).
		/// </summary>
		public RDLCDefaultValue DefaultValue = null;
		/// <summary>
		/// Possible values for the parameter (for the end-user UI).
		/// </summary>
		public RDLCValidValues ValidValues = null;
	}
	#endregion Report Parameter
	#endregion Report Data
	/// <summary>
	/// RDLCReport
	/// </summary>
	public class RDLCReport
	{
		#region Report Layout
		/// <summary>
		/// Description of the report
		/// </summary>
		public string? Description;
		/// <summary>
		/// Author of the report
		/// </summary>
		public string Author;
		/// <summary>
		/// Rate, in seconds, at which the report page  (when rendered as HTML) automatically  refreshes. Must be nonnegative.
		/// If omitted or zero, the report page should not  automatically refresh. 
		/// Max: 2147483647
		/// </summary>
		public int AutoRefresh;
		/// <summary>
		/// Definitions for custom functions to be used in expressions in the report. Custom functions must be instance methods. If a function OnInit() is defined within Code, 
		/// it is called during parameter, report and page header/footer initialization. The function must be defined as Protected and Overrides
		/// </summary>
		public string Code;
		/// <summary>
		/// Width of the report.
		/// </summary>
		public RDLCSize Width;
		/// <summary>
		/// The primary language of the text. Default is server language. Used as the default for all language-dependent expressions in the report.
		/// </summary>
		public string Language;
		/// <summary>
		/// Describes how the body of the report is structured and rendered.
		/// </summary>
		public RDLCBody Body;
		/// <summary>
		/// Contains page layout information about the report
		/// </summary>
		public RDLCPage Page;
		/// <summary>
		/// Images embedded in the report
		/// </summary>
		public List<RDLCEmbeddedImage> EmbeddedImages = null;
		/// <summary>
		/// Classes to instantiate during report initialization.
		/// </summary>
		public List<RDLCClass> Classes = null;
		/// <summary>
		/// Code modules to make available to the report for use in expressions.
		/// </summary>
		public List<RDLCCodeModule> CodeModules = null;
		/// <summary>
		/// Custom information to be handed to the report rendering component
		/// </summary>
		public List<RDLCCustomProperty> CustomProperties = null;
		/// <summary>
		/// Variables defined for the report as a whole.
		/// </summary>
		public List<RDLCVariable> Variables = null;
		/// <summary>
		/// Indicates that Variables throughout the report are not required to be pre-evaluated at the start of report processing and may be evaluated on-demand based on usage.
		/// Deferred variable evaluation can improve performance but should not be used if any variables are time-dependent
		/// </summary>
		public bool DeferVariableEvaluation;
		/// <summary>
		/// Indicates that all whitespace in containers (such as Body and Rectangle) should be consumed when contents grow rather than preserving the minimum whitespace 
		/// between the contents and the container
		/// </summary>
		public bool ConsumeContainerWhitespace;
		/// <summary>
		/// The location to a transformation to apply to  a report data rendering. This can be a full folder path (for example, “/xsl/xfrm.xsl”) or relative path (for example “xfrm.xsl”).
		/// Relative paths start in the same folder as the report.
		/// </summary>
		public string DataTransform;
		/// <summary>
		/// The schema or namespace to use for a report data rendering.
		/// </summary>
		public string DataSchema;
		/// <summary>
		/// Name of a top level element that represents the report data. Default: Report. Must be a CLS-compliant identifier.
		/// </summary>
		public string DataElementName;
		public enum eDataElementStyle
		{
			/// <summary>
			/// Default : Render values as attributes
			/// </summary>
			Attribute,
			/// <summary>
			/// Render values as elements
			/// </summary>
			Element
		}
		/// <summary>
		/// Indicates whether leaf-level values (for example, text box values and chart data values) should render as elements or attributes.
		/// </summary>
		public eDataElementStyle DataElementStyle = eDataElementStyle.Attribute;
		/// <summary>
		/// add Embedded Image
		/// </summary>
		/// <param name="EmbeddedImage">Embedded Image</param>
		/// <returns></returns>
		public RDLCEmbeddedImage addEmbeddedImage(RDLCEmbeddedImage EmbeddedImage)
		{
			if (EmbeddedImages == null)
				EmbeddedImages = new List<RDLCEmbeddedImage>();
			EmbeddedImages.Add(EmbeddedImage);
			return EmbeddedImage;
		}
		/// <summary>
		/// add Class
		/// </summary>
		/// <param name="Class">Class</param>
		/// <returns></returns>
		public RDLCClass addClass(RDLCClass Class)
		{
			if (Classes == null)
				Classes = new List<RDLCClass>();
			Classes.Add(Class);
			return Class;
		}
		/// <summary>
		/// add Code Module
		/// </summary>
		/// <param name="CodeModule">Code Module</param>
		/// <returns></returns>
		public RDLCCodeModule addCodeModule(RDLCCodeModule CodeModule)
		{
			if (CodeModules == null)
				CodeModules = new List<RDLCCodeModule>();
			CodeModules.Add(CodeModule);
			return CodeModule;
		}
		/// <summary>
		/// add Custom Property
		/// </summary>
		/// <param name="CustomProperty">Custom Property</param>
		/// <returns></returns>
		public RDLCCustomProperty addCustomProperty(RDLCCustomProperty CustomProperty)
		{
			if (CustomProperties == null)
				CustomProperties = new List<RDLCCustomProperty>();
			CustomProperties.Add(CustomProperty);
			return CustomProperty;
		}
		/// <summary>
		/// add Variable
		/// </summary>
		/// <param name="Variable">Variable</param>
		/// <returns></returns>
		public RDLCVariable addVariable(RDLCVariable Variable)
		{
			if (Variables == null)
				Variables = new List<RDLCVariable>();
			Variables.Add(Variable);
			return Variable;
		}
		#endregion Report Layout

		#region Report Data
		/// <summary>
		/// Describes the data sources from which data  sets are taken for this report.
		/// The DataSources element contains information about how to connect to the sources of data for the various DataSets.
		/// </summary>
		public List<RDLCDataSource> DataSources = null;
		/// <summary>
		/// Describes the data that is displayed as part of the report. 
		/// 
		/// The DataSets element contains information about the sets of data to display as a part of the report.
		/// </summary>
		public List<RDLCDataSet> DataSets = null;
		/// <summary>
		/// Parameters for the report.
		/// </summary>
		public List<RDLCReportParameter> ReportParameters = null;
		#endregion Report Data
	}
}
*/