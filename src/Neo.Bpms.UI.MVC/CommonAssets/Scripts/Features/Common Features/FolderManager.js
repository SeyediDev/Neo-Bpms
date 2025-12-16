var FolderManager = function() {
    var constSubmitUri = 'Folder/SaveFolderConfig';
    var constDeleteUri = 'Folder/DeleteFolderConfig';

    var fillTheForm = function (data) {
        var $configFolderModal = $("#new-folder-modal");
        $configFolderModal.find('#config-folder-id').val(data.Id);
        $configFolderModal.find('#config-folder-parent-id').val(data.ParentFolderId);
        $configFolderModal.find('#folder-isForConfig').val(Boolean(data.IsForConfig));
        $configFolderModal.find('input[name="newFolderName"]').val(data.Name);
        if ($configFolderModal.find('input[name=newFolderAccess]').length > 0)
            $configFolderModal.find('input[name=newFolderAccess][value="' + data.IsPublic + '"]').prop('checked', true);
    };

    var acquireSubmitDataObj = function () {
        var result = {};
        result["NamespaceId"] = window.PageAddressManager.getNamespaceId();
        result["EntityId"] = window.PageAddressManager.getEntityId();
        var pageType = window.PageAddressManager.getPageType();
        switch (pageType) {
            case "Form":
                result["FormId"] = window.PageAddressManager.getPageId();
                break;
            case "Report":
                result["ReportId"] = window.PageAddressManager.getPageId();
                break;
            case "Dashboard":
                result["DashboardId"] = window.PageAddressManager.getPageId();
                break;
        }
        result["FolderId"] = $('#new-folder-modal #config-folder-id').val();
        result["Name"] = $('#new-folder-modal input[name="newFolderName"]').val();
        //        result.Id = $('#new-folder-modal #config-folder-id').val();
        result["ParentFolderId"] = $('#new-folder-modal #config-folder-parent-id').val();
        if (!result["ParentFolderId"]) result["ParentFolderId"] = null;
        result["IsPublic"] = $('#new-folder-modal input[name="newFolderAccess"]:checked').val() === 'true';
        result["IsForConfig"] = $('#new-folder-modal #folder-isForConfig').val();
        return result;
    };

    var showModal = function (isForConfig, parentId, folderInfo) {
        var $modal = $('#new-folder-modal');
        
        // Try to find modal in parent window if not found
        if ($modal.length === 0 && window.top && window.top.$) {
            $modal = window.top.$('#new-folder-modal');
        }

        if (folderInfo === undefined) {
            folderInfo = {
                "Id": 0,
                "Name": window.tetaI18n.t('New Folder'),
                "IsForConfig": isForConfig,
                "IsPublic": false,
                "ParentFolderId": parentId ? parentId : null
            };
        }
        fillTheForm(folderInfo);

        // Ensure modal has high z-index - higher than report-designs-modal (10000)
        // Set z-index before showing to ensure it's applied immediately
        $modal.css('z-index', '10070');
        
        // Show modal using Bootstrap
        if (typeof $modal.modal === 'function') {
            $modal.modal('show');
            // After modal is shown, ensure z-index is correct and backdrop is above report-designs-modal
            setTimeout(function() {
                $modal.css('z-index', '10070');
                // Update backdrop z-index - should be above report-designs-modal (10000) but below modal (10070)
                var $backdrops = $('.modal-backdrop');
                if ($backdrops.length > 0) {
                    $backdrops.last().css('z-index', '10069');
                }
            }, 50);
        } else {
            // Fallback if Bootstrap modal not available
            $modal.addClass('show').css({
                'display': 'block',
                'z-index': '10070'
            });
            $('body').addClass('modal-open');
            // Create backdrop if needed
            var $backdrop = $('.modal-backdrop').last();
            if ($backdrop.length === 0) {
                $backdrop = $('<div class="modal-backdrop fade show"></div>').appendTo('body');
            }
            $backdrop.css('z-index', '10069');
        }
    };

    var submissionCallback = null;

    var registerSubmissionCallback = function (cb) {
        if (submissionCallback !== null && cb !== submissionCallback)
            throw new Error('Concurrent folder submission is not supported');
        submissionCallback = cb;
    };

    var submitForm = function () {
        var saveFolderConfigObj = acquireSubmitDataObj();
        $.ajax({
            type: "POST",
            url: window.top.rootUrl + constSubmitUri,
            data: saveFolderConfigObj,
            headers: AddAntiForgeryToken(),
            success: function (res) {
                submissionCallback(res.Id, res.ParentFolderId, res.Name, saveFolderConfigObj.FolderId === res.Id);
                submissionCallback = null;
                $('#new-folder-modal').modal('hide');
            },
            error: function (res) {
                console.error(res);
                window.toast.error(window.tetaI18n.t('Error Occured'));
                $('#new-folder-modal').modal('hide');
            }
        });
    };    

    var deleteFolder = function (folderId, cb) {
        var result = confirm(window.tetaI18n.t('SureToDeleteFolder'));
        if (!result) return;
        $.ajax({
            type: "POST",
            url: window.top.rootUrl +
                constDeleteUri +
                '?folderId=' +
                folderId,
            success: function () {
                if (cb)
                    cb();
            },
            headers: AddAntiForgeryToken()
        });
    };

    return {
        showModal: showModal,
        submitForm: submitForm,
        deleteFolder: deleteFolder,
        registerSubmissionCallback: registerSubmissionCallback
    };
}();
