using Neo.Bpms.UI.MVC.Controls.JsControls.BindingModels;

namespace Neo.Bpms.UI.MVC.Controls.JsControls.AdvancedUpload;

public class AdvancedUpload : CabdoJsControl
{
    public AdvancedUpload(InputFieldDefinition field, ControlsRendererData controlsRenderer) : base(field,
        controlsRenderer)
    {
    }

    public override CandoStringBuilder Render()
    {
        CandoStringBuilder result = new();
        result +=
            $@"$('[id=""{Field.FieldName}""]')
                         .fineUploader({{
                            request: {{
                                endpoint: window.top.rootUrl + 'upload/upload?isConcurrent=True',
                                customHeaders: window.AddAntiForgeryToken()
                            }},
                            chunking: {{
                                enabled: true,
                                partSize: {ChunkSize}
                                ,
                                concurrent: {{
                                    enabled: true
                                }},                                
                                success: {{
                                    endpoint: window.top.rootUrl + 'upload/success'
                                }}
                            }},
                            maxConnections: {MaxConnections},
                            multiple: false
                            {ScalingOption()}
                        ";

        if (CultureHelper.GetCurrentNeutralCulture() == "fa")
        {
            result += $@",
                            messages: {MessagesOption},
                            text: {TextOption},
                            retry: {RetryOption},
                            deleteFile: {DeleteFileOption}";
        }
        result += "});";
        //            result = SetUploadedFileToRetrySubmission(result);
        RegisterControl(result, ObtainJsBindingControl(), Field.FieldName);
        return result;
    }

    //        private StringBuilder SetUploadedFileToRetrySubmission(StringBuilder result)
    //        {
    //            if (!string.IsNullOrEmpty(ControlsRenderer.Record.GetString($"{Field.FieldName}__Retry")))
    //            {
    //                result += $@"$('[id=""{Field.FieldName}""]')
    //                         .fineUploader('addInitialFiles', [{{uuid: '{ControlsRenderer.Record.GetString($"{Field.FieldName}__Retry")}' name:'نیازی به آپلود مجدد نیست.'}}]);";
    //            }
    //
    //            return result;
    //        }

    private static long _chunkSize;

    private static long ChunkSize
    {
        get
        {
            if (_chunkSize == 0)
            {
                string strChunkSize = DependencyInjectionHolder.Instance.Configuration["UploadChunkSize"];
                long.TryParse(strChunkSize, out _chunkSize);
                if (_chunkSize <= 0)
                    _chunkSize = 5000000;
            }
            return _chunkSize;
        }
    }

    private static int _maxConnections;

    private static int MaxConnections
    {
        get
        {
            if (_maxConnections == 0)
            {
                string strMaxConnections = DependencyInjectionHolder.Instance.Configuration["UploadMaxConnections"];
                int.TryParse(strMaxConnections, out _maxConnections);
                if (_maxConnections <= 0)
                    _maxConnections = 5;
            }
            return _maxConnections;
        }
    }

    private static string MessagesOption =>
        @"{
                    tooManyFilesError: 'لطفا یک فایل انتخاب کنید.',
                    unsupportedBrowser: 'مرورگر پشتیبانی نمیشود.',
                    typeError: 'نوع فایل {file} نامعتبر!',
                    sizeError: 'حجم فایل {file} بیش از اندازه مجاز است. بیشترین حجم مجاز برای بارگذاری {sizeLimit}.',
                    minSizeError: 'حجم فایل {file} کمتر از اندازه مجاز است.کمترین حجم مجاز برای بارگذاری {minSizeLimit}.',
                    emptyError: 'محتوای {file} خالیست.لطفا فایل های خود را دوباره انتخاب کنید.',
                    noFilesError: 'لطفا ابتدا یک فایل را انتخاب کنید..',
                    tooManyItemsError: 'تعداد زیادی فایل انتخاب شده است.',
                    maxHeightImageError: 'طول عکس بیش از حد مجاز است.',
                    maxWidthImageError: 'عرض عکس بیش از حد مجاز است.',
                    minHeightImageError: 'طول عکس کمتر از حد مجاز است.',
                    minWidthImageError: 'عرض عکس کمتر از حد مجاز است.',
                    retryFailTooManyItems: 'ناموفق! شما به سقف تعداد مجاز رسیده اید',
                    onLeave: 'فایل ها در حال بارگذاری هستند، در صورت بسته شدن صفحه، عملیات متوقف خواهد شد.آیا مایل به بستن صفحه هستید؟',
                    unsupportedBrowserIos8Safari: 'این مرورگر اجازه بارگذاری فایل در IOS8 را نمیدهد. لطفا تا زمان رفع شدن این مشکل، از مرورگر کروم استفاده کنید.'
              }";

    private static string TextOption =>
        @"{
                    formatProgress: '{percent}% از {total_size}',
                    failUpload: 'عملیات ناموفق!',
                    waitingForResponse: 'در حال پردازش...',
                    paused: 'متوقف'
              }";

    private static string RetryOption =>
        @"{
                    autoRetryNote: 'تلاش مجدد {retryNum}/{maxAuto}...'
              }";

    private static string DeleteFileOption =>
        @"{
                    confirmMessage: 'آیا مایل به حذف {filename} هستید؟',
                    deletingStatusText: 'در حال حذف...',
                    deletingFailedText: 'حذف ناموفق'
              }";

    private string ScalingOption()
    {
        return !Field.HasProperty(eControlPropertyId.ScaleImageSizeKb)
            ? ""
            : $@",scaling: {{
                sendOriginal: false,
                sizes: [
                    {{ name: ""medium"", maxSize: {Field.PropertyValue(eControlPropertyId.ScaleImageSizeKb)}}}
                ]
            }}";
    }

    private void Initialize()
    {
        //            throw new NotImplementedException();
    }

    private JsBindingControl ObtainJsBindingControl()
    {
        JsBindingControl jsBindingControl = new("FineUploader", Field.FieldName);

        jsBindingControl.bindingInfo.Add(new JsBindingInfo
        {
            type = JsBindingType.field,
            name = Field.FieldName,
            relatedDataField = Field.PropertyValue(eControlPropertyId.FieldMapping) ?? "uuid"
        });
        jsBindingControl.bindingInfo.Add(new JsBindingInfo
        {
            type = JsBindingType.field,
            name = $"{Field.FieldName}__Action",
            relatedDataField = "action"
        });
        return jsBindingControl;
    }
}