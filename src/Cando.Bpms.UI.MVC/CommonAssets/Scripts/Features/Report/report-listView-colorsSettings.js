function submitColorsSettings(modalKey) {
        // Debug: Check if modalObjects exists
    
    // Try both window.top and window
    var modalObj = (window.top.modalObjects && window.top.modalObjects[modalKey]) || 
                   (window.modalObjects && window.modalObjects[modalKey]);
    
    if (!modalObj) {
        alert('خطا: اطلاعات مدال یافت نشد. لطفاً صفحه را مجدداً بارگذاری کنید.');
        return;
    }
    
	var ajaxParams = {
        NamespaceId: modalObj.NamespaceId,
        EntityId: modalObj.EntityId,
        ReportId: modalObj.ReportId,
        ConfigId: modalObj.ConfigId,
        success: $("#_color_success-" + modalKey).val(),
        danger: $("#_color_danger-" + modalKey).val(),
        info: $("#_color_info-" + modalKey).val(),
        warning: $("#_color_warning-" + modalKey).val(),
        active: $("#_color_active-" + modalKey).val()
    };
    
        // Ajax params prepared
    
    var obj = JSON.stringify(ajaxParams);
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Report/ApplyColorsSettings',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        processData: true,
        cache: false,
        data: obj,
        headers: window.AddAntiForgeryToken(),
        success: function (res) {
                // Colors settings applied successfully
            location.reload();
        },
        error: function (e) {
            // Error applying colors settings
            alert(window.tetaI18n.t('Error Occured') + e.responseText);
        }
    });
}
