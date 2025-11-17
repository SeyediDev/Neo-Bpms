using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage;
using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;
using Neo.Bpms.Infrastructure.Features.Cmmn.ObjectStorage;
using Microsoft.Extensions.Configuration;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class FileHtmlControl(ICmmnDocument cmmnFileManager, IFormLogicHelper formLogicHelper,
    InputFieldDefinition field, ControlsRendererData controlsRendererData,
    ILogger logger, IConfiguration configuration, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    private bool _withAnotherUploader;
    public bool StreamerIsAvailable => !string.IsNullOrEmpty(configuration["MediaStreamingRootUrl"]);

    public FileHtmlControl SetAnotherUploader()
    {
        _withAnotherUploader = true;
        return this;
    }

    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();
        List<DocumentView> documents = (List<DocumentView>)ControlsRendererData.Record[Field.FieldName];
        if (documents != null)
        {
            foreach (DocumentView document in documents)
            {
                _ = RenderOneDocument(ref result, document);
            }
        }
        if (!CommonProperties.IsReadOnly && (documents == null || documents.Count == 0 || CommonProperties.IsMultiple))
        {
            _ = RenderOneDocument(ref result, null);
        }
        return result;
    }

    private NeoStringBuilder RenderOneDocument(ref NeoStringBuilder result, DocumentView? document)
    {
        NeoStringBuilder controlHtml = new();
        controlHtml += $"<div data-id=\"{Field.FieldName}\" title=\"{CommonProperties.Tooltip}\" class=\"" +
                  ControlsRendererData.ControlsClassString +
                  CalculateWidthClasses() + " " +
                  CommonProperties.ShowHideRelatedClass +
                  "\" style=\"margin-top: 8px;\" >";

        controlHtml = RenderDesignIcons(controlHtml);
        controlHtml += "<div class=\"dWrapper\">";
        controlHtml = RenderBulkEditCheckbox(controlHtml); //todo It has an issue because It has an hidden input

        if (IsEditable())
        {
            controlHtml = RenderUploadButton(controlHtml, document);
            controlHtml = RenderHiddenInputs(controlHtml, document);
        }
        if (document != null)
        {
            controlHtml = RenderFileContent(controlHtml, document);
        }

        //controlHtml += "<br/>";

        if (!_withAnotherUploader)
        {
            controlHtml += "</div></div>";
        }
        result += controlHtml;
        return result;
    }

    private bool IsEditable()
    {
        return ControlsRendererData.Structure.FormType is Form.eFormType.Create or
               Form.eFormType.Edit or
               Form.eFormType.WorkItem or
               Form.eFormType.ProcessCreate;
    }

    private NeoStringBuilder RenderUploadButton(NeoStringBuilder result, DocumentView? document)
    {
        //todo span -> label
        string label = Field.Label;
        if (document != null && document.Title != Field.Label && document.Title != Field.FieldName)
        {
            label = $"{Field.Label}-{document.Title}";
        }

        result +=
            $"<label for=\"{FileContentId(document)}\" class=\"col-form-label\" style=\"top: -18px;\">" +
            $"{label}{(CommonProperties.IsRequired ? "<span style=\"color:red;\">*</span>" : "")}" +
            $"</label>";
        if (CommonProperties.IsReadOnly || !IsEditable() || _withAnotherUploader)
        {
            return result;
        }

        string buttonLabel = "<br/>" + ViewTexts.ChooseFile;
        if (document != null)
        {
            buttonLabel = $"{ViewTexts.ChangeFile} <br/> {document.FullFileName}";
        }
        result += "<div>";
        if (!_withAnotherUploader)
        {
            result +=
                $@"<span for=""{FileContentId(document)}"" class=""upload btn btn-outline-secondary btn-block"" style=""z-index:10"" >{buttonLabel}
                        <i class=""fa fa-open""></i>
                        <input class=""fileField"" {CommonProperties.ReadOnlyRelatedAttribute} " +
                      (CommonProperties.IsRequired ? "required=\"required\" oninvalid=\"InvalidMsg(this);\" " : "") +
                      " dir=\"" + CommonProperties.Direction + 
                      "\" title=\"" + CommonProperties.Tooltip +
                      "\" type=\"file\" ownerName=\"" + Field.FieldName + "\"" +
                      " onchange=\"FormFileManager.fileValueChanged(this)\"" + LogicString + " />"+
                "</span>";
        }
        if (document != null)
        {
            result +=
                $@"<div style=""top: -7px;"">
                    <span id=""{Field.FieldName}-remove-btn"" class=""btn btn-sm btn-danger"" 
                        title=""{ViewTexts.RemoveFile}"" onclick=""FormFileManager.removeFile('{Field.FieldName}')"">
                        <i class=""fa fa-close""></i>
                    </span>
                    <span id=""{Field.FieldName}-undo-btn"" class=""btn btn-sm btn-success hidden"" 
                            title=""{ViewTexts.UndoRemoveFile}"" onclick=""FormFileManager.undoRemove('{Field.FieldName}')"">
                        <i class=""fa fa-undo"">Undo</i>
                    </span>
                </div>";
        }
        result += "</div>";
        return result;
    }

    private NeoStringBuilder RenderHiddenInputs(NeoStringBuilder result, DocumentView? document)
    {
        result += $"<input name=\"{Field.FieldName}\" type=\"hidden\" value=\"{document?.Id}\" />";
        result += $"<input name=\"{Field.FieldName}__Action\" type=\"hidden\" value=\"{FileSubmitAction.Nothing}\" />";
        return result;
    }

    private NeoStringBuilder RenderFileContent(NeoStringBuilder result, DocumentView? document)
    {
        FilePrimaryType fileType = ObtainFileType(document);
        bool showDocumentInPage = Field.PropertyBoolean(eControlPropertyId.ShowDocumentInPage);
        var href = AcquireUrl(document, fileType, false);
        switch (fileType)
        {
            case FilePrimaryType.Image:
                {
                    string imageSource = document.Dto?.Content != null && document.Dto?.Content.Length > 0
                        ? $"data:image/{document.MimeType};base64,{Encoding.UTF8.GetString(document.Dto?.Content)}"
                        : "path/to/placeholder.jpg";//todo
                    if (showDocumentInPage)
                    {
                        result += $@"<div ><img src=""{imageSource}"" id=""{FileContentId(document)}"" class=""file-content"" /></div>";
                    }
                    else
                    {
                        result = RenderDownloadBox(result, document, imageSource);
                    }
                }
                break;
            case FilePrimaryType.Video:
                result = showDocumentInPage
                    ? RenderVideo(result, document, href)
                    : RenderDownloadBox(result, document, href);
                break;
            case FilePrimaryType.Audio:
                result = showDocumentInPage
                    ? RenderAudio(result, document, href)
                    : RenderDownloadBox(result, document, href);
                break;
            case FilePrimaryType.Pdf:
                if (showDocumentInPage)
                {
                    result += $@"<div>
                                   <iframe id=""{FileContentId(document)}"" 
                                      src=""/Scripts/common-assets-includes/common-plugins/controls/pdf-viewer/viewer.html?file={AcquireUrl(document, fileType, false)}#locale=fa""
                                      class=""col-lg-12"" style=""height:600px;"">
                                   </iframe>
                                 </div>";
                }
                else
                {
                    result = RenderDownloadBox(result, document, href);
                }
                break;
            case FilePrimaryType.Text:
            case FilePrimaryType.Swf:
                {
                    //result += $"<object id=\"{FileContentId(document)}\" data=\"{AcquireUrl(document, fileType, false)}\" type=\"{document.MimeType}\" class=\"file-content\">";
                    //result += "این مرورگر، نمایش این نوع فایل را پشتیبانی نمیکند. لطفا فایل را دانلود کرده و سپس مشاهده نمایید.";
                    //result += "</object>";
                    result = RenderDownloadBox(result, document, href);
                }
                break;
            default:
                result = RenderDownloadBox(result, document, href);
                break;
        }

        return result;
    }

    private NeoStringBuilder RenderDirectDownloadLink(NeoStringBuilder result, DocumentView? document, FilePrimaryType fileType)
    {
        result += $@"<div style=""margin-bottom: 5px;"">
                           <a id=""{FileContentId(document)}"" class=""btn btn-light""
                              href=""{AcquireUrl(document, fileType, true)}""
                              value=""{document?.Title}"">
                                <span>
                                  {ViewTexts.Download}
                                </span>
                                <i class=""fa fa-download""></i>
                            </a>
                        </div>";
        return result;
    }

    private NeoStringBuilder RenderVideo(NeoStringBuilder result, DocumentView? document, string url)
    {
        if (Field.PropertyBoolean(eControlPropertyId.UseFileServer) &&
            StreamerIsAvailable)
        {
            result = RenderJwPlayer(result, document, url);
        }
        else
        {
            result += $@"<div class=""col-lg-12"">
                               <video id=""{FileContentId(document)}"" controls src=""{url}"" class=""file-content"">
                               </video>
                             </div>";
        }
        return result;
    }

    private NeoStringBuilder RenderAudio(NeoStringBuilder result, DocumentView? document, string url)
    {
        if (Field.PropertyBoolean(eControlPropertyId.UseFileServer) &&
            StreamerIsAvailable)
        {
            result = RenderJwPlayer(result, document, url);
        }
        else
        {
            result += $@"<div class=""col-lg-12"">
                                    <audio controls id=""{FileContentId(document)}"" class=""file-content"">
                                        <source src=""{url}"">
                                    </audio>
                                 </div>";
        }

        return result;
    }

    private NeoStringBuilder RenderJwPlayer(NeoStringBuilder result, DocumentView? document, string url)
    {
        ControlsRendererData.AddIncludeNeed(PluginInclude.JwPlayer);
        result += $@"<div class=""col-lg-12""><div id=""{FileContentId(document)}"" data-video-stream=""{url}""></div></div>";
        return result;
    }

    private FilePrimaryType ObtainFileType(DocumentView document)
    {
        string mimeTypeText = document.MimeType;
        if (string.IsNullOrEmpty(mimeTypeText))
        {
            logger.LogError("no .ContentType {id} {name}", ControlsRendererData.Structure?.Form_ReportId, Field.FieldName);
        }

        return mimeTypeText.GetPrimaryType();
    }

    private string AcquireUrl(DocumentView document, FilePrimaryType type, bool forDirectDownload)
    {
        if (Field.PropertyBoolean(eControlPropertyId.UseFileServer))
        {
            PaintableFileInfo pointableFile = cmmnFileManager.GetPaintableFileInfo(document);
            switch (type)
            {
                case FilePrimaryType.Video:
                case FilePrimaryType.Audio:
                    return (StreamerIsAvailable && !forDirectDownload) ? pointableFile.StreamerUrl : pointableFile.FileServerUrl;
            }
        }
        return Url.Action("DL", "Download", ControlsRendererData.Url, new { id = document.Id });
    }

    private string CalculateWidthClasses()
    {
        return _withAnotherUploader
        ? CommonProperties.WideColumnClasses
        : CommonProperties.NarrowColumnClasses;
    }

    private string FileContentId(DocumentView? document)
    {
        return $"{Field.FieldName}{(document != null ? "-" + document.Id : "")}-file-content";
    }

    private NeoStringBuilder RenderDownloadBox(NeoStringBuilder result, DocumentView? document, string href)
    {
            result += $@"<a class=""tiket-uploaded-document-a"" href=""{href}"" download=""{document.FullFileName}"">
                    <div class=""col-12 tiket-uploaded-document"">
                        <div>
                            <span class=""tiket-uploaded-document-frame"">
                                <svg width=""18"" height=""18"" viewBox=""0 0 18 18"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                                    <path d=""M10.64 2.064v2.697c0 .366.145.717.405.975.26.26.612.405.98.405h3.093"" fill=""#6366F1"" />
                                    <path d=""M10.64 2.064v2.697c0 .366.145.717.405.975.26.26.612.405.98.405h3.093"" stroke=""#6366F1"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" />
                                    <path d=""M5.735 5.735h2.449M5.735 9h6.53m-6.53 3.264h6.53m2.922-5.838v6.426a3.188 3.188 0 0 1-1.021 2.227 3.211 3.211 0 0 1-2.304.855H6.17a3.224 3.224 0 0 1-2.325-.843 3.195 3.195 0 0 1-1.033-2.24V5.147A3.188 3.188 0 0 1 3.835 2.92a3.21 3.21 0 0 1 2.304-.855h4.286a2.625 2.625 0 0 1 1.77.679l2.22 2.041a2.227 2.227 0 0 1 .774 1.642z"" stroke=""#6366F1"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" />
                                </svg>
                            </span>
                        </div>
                        <div class=""show-title"">
                            <span>{document.FullFileName}</span>
                            <span class=""size"">{document.Dto.Content.Length / 1024} kb</span>
                        </div>
                        <div>
                            <svg width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                                <path d=""M21 13.5a.5.5 0 0 1 .277.084l.076.063A.5.5 0 0 1 21.5 14v3a4.5 4.5 0 0 1-4.5 4.5H7A4.5 4.5 0 0 1 2.5 17v-3a.5.5 0 0 1 .084-.277l.062-.077a.5.5 0 0 1 .631-.062l.077.063A.5.5 0 0 1 3.5 14v3A3.5 3.5 0 0 0 7 20.5h10a3.5 3.5 0 0 0 3.5-3.5v-3a.5.5 0 0 1 .084-.277l.063-.077A.5.5 0 0 1 21 13.5z"" fill=""#000"" stroke=""#000"" />
                                <path d=""M12.5 4.5c.098 0 .194.029.274.082l.076.06 4.5 4.4.005.005a.503.503 0 0 1 .12.547.5.5 0 0 1-.744.231l-.076-.063-.004-.004-2.801-2.74-.85-.83V16a.5.5 0 1 1-1 0V6.187l-.85.832-2.8 2.739a.5.5 0 0 1-.355.142l-.098-.01a.504.504 0 0 1-.253-.14l-.006-.006a.5.5 0 0 1-.13-.252L7.5 9.395a.5.5 0 0 1 .01-.098l.03-.093a.505.505 0 0 1 .048-.086l.062-.075.001-.001 4.5-4.4A.5.5 0 0 1 12.5 4.5z"" fill=""#000"" stroke=""#000"" />
                            </svg>
                        </div>
                    </div> </a>";

        return result;
    }
}
