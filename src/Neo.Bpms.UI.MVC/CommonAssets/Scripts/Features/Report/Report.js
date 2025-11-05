// Global submitFilter function for report pages
window.submitReportFilter = function() {
    console.log('Report: Submitting filter...');
    
    // Ensure all filter inputs are associated with the form
    if (typeof window.ensureFilterFormAssociation === 'function') {
        window.ensureFilterFormAssociation();
    }
    
    // Submit the form
    const form = document.getElementById('filter-form');
    if (form) {
        console.log('Report: Submitting form with filter values...');
        form.submit();
    } else {
        console.error('Report: Filter form not found');
    }
};

// Keep backward compatibility for report pages
window.submitFilter = window.submitReportFilter;

$('#showRefreshSettings').on('click',
    function() {
        $('#refreshSettingsDiv').slideToggle();
    });

var RefreshManager = function() {
    var refreshTimeout = null;
    var refreshReport = function() {
        window.location.reload(1);
    };
    var refreshTimeChanged = function() {
        if (refreshTimeout !== null)
            clearTimeout(refreshTimeout);
        if ($('#refresh-time-select').val() !== '0')
            $(this).closest('form').submit();
    };
    var setChangeHandler = function() {
        $('#refresh-time-select').on('change', refreshTimeChanged);
    };
    var initializeTimeout = function() {
        var refreshTime = $('#refresh-time-select').val();
        if ($.isNumeric(refreshTime) && refreshTime > 0) {
            refreshTimeout = setTimeout(refreshReport, refreshTime * 1000);
        }
    };

    var initialize = function() {
        initializeTimeout();
        setChangeHandler();
    };

    return {
        initialize: initialize
    };
}();
$(RefreshManager.initialize);

$("#openConfigFrame").click(function() {
	 $("#reportConfigModal .modal-body").html(
		 "<iframe src='" +
        window.top.rootUrl +
        "Report/AddNewConfig?" +
        window.PageAddressManager.getQueryParameters()
        +
        "' style='width:100%; height:100%;' frameborder='0' allowtransparency='true'></iframe>");
    $('#reportConfigModal').modal('show');
});

function deleteConfigId(configId, cb) {
    var result = confirm(window.tetaI18n.t('SureToDeleteConfig'));
    if (!result) return;
    $.ajax({
        type: 'POST',
        url: window.top.rootUrl + 'Report/DeleteConfig?ConfigId=' + configId,
        async: true,
        processData: true,
        cache: false,
        headers: AddAntiForgeryToken(),
        success: function(res) {
            if (res && cb) {
                cb();
            }
        },
        error: function(e) {
            alert(window.tetaI18n.t('Error Occured') /*+ e.responseText*/);
            Error(e);
        }
    });
}