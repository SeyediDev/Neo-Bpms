using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls;

/// <summary>
/// برایِ تعریفِ یک کنترلِ خاص در پروژه‌ها یک کلاس که این
/// اینترفیس را پیاده‌سازی می‌کند ایجاد می‌کنیم و یک نمونه از آن را با
/// <see cref="ControlsRenderer.RegisterControl"/>
/// ثبت می‌کنیم.
/// </summary>
public interface IBusinessDefinedControl
{
    /// <summary>
    /// شناسه‌یِ نوعِ کنترل
    /// BusinessControlId مطابقِ آنچه که در مدل در پراپرتیِ
    /// وارد می‌شود
    /// </summary>
    string Id { get; }
    /// <summary>
    /// عنوانِ کنترل.
    ///  قابلِ استفاده مثلاً در پنلِ ابزارهایِ طراحِ فرم
    /// </summary>
    string DesignLabel { get; }
    /// <summary>
    /// فهرستِ پراپرتی‌هایی که برایِ کنترل استفاده و موضوعیت دارد.
    /// قابلِ استفاده در طراحیِ فرم
    /// </summary>
    IList<eControlPropertyId> SupportingProperties();
    /// <summary>
    /// آنچه در قسمتِ مارک‌آپِ فرم برایِ این کنترل تولید می‌شود 
    /// </summary>
    NeoStringBuilder RenderHtml(InputFieldDefinition fieldDefinition,
        ControlsRendererData controlsRendererData);
    /// <summary>
    /// آنچه در قسمتِ اسکریپت‌ها برایِ این کنترل تولید می‌شود 
    /// </summary>
    NeoStringBuilder RenderScript(InputFieldDefinition fieldDefinition,
        ControlsRenderer controlsRenderer);
    /// <summary>
    /// اینکلود‌هایِ موردِ نیازِ کنترل
    /// معمولاً فایل‌هایِ
    /// css و js
    /// </summary>
    HtmlString WebAssetIncludes();
}
