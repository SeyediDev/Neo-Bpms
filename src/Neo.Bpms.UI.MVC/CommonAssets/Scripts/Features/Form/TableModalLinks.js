/**
 * TableModalLinks - Opens IndexTable form links in modal dialogs
 * 
 * Alt+Click opens in modal, regular click opens in new tab (default behavior)
 * 
 * Uses BootstrapDialog (same as drilldown modals) for consistent UI
 */
window.TableModalLinks = (function ($) {
    'use strict';

    var defaults = {
        dialogWidth: '90%',
        dialogHeightRatio: 0.7
    };

    var currentDialog = null;

    /**
     * Opens a form URL in a modal dialog with an iframe
     * @param {string} url - The URL to open
     * @param {string} title - The modal title
     */
    function openFormModal(url, title) {
        // Get BootstrapDialog from window.top (where it's defined in open-drilldown.js)
        var targetWindow = window.top || window;
        var TargetBootstrapDialog = targetWindow.BootstrapDialog || window.BootstrapDialog;

        if (!TargetBootstrapDialog) {
            // Fallback: just open in new tab if BootstrapDialog is not available
            console.warn('TableModalLinks: BootstrapDialog not found, opening in new tab');
            window.open(url, '_blank');
            return;
        }

        // Create iframe container - same approach as OpenDrillDownModal
        var $container = $('<div style="width:99%;height:100%;"></div>');
        $container.append(
            "<iframe src='" + url + "' style='width:100%; height:100%;' frameborder='0' allowTransparency='true'></iframe>"
        );

        var dialogTitle = title || (window.tetaI18n && window.tetaI18n.t ? window.tetaI18n.t('Form') : 'Form');

        var dialog = new TargetBootstrapDialog({
            title: dialogTitle,
            message: $container
        });

        dialog.realize();

        // Apply modal sizing - same as OpenDrillDownModal
        var dialogHeight = screen.availHeight * defaults.dialogHeightRatio;
        dialog.getModalBody().css("width", "100%");
        dialog.getModalBody().css("height", dialogHeight + "px");
        dialog.getModalBody().css("padding", "0");
        dialog.getModalDialog().css("width", defaults.dialogWidth);
        dialog.getModalBody().find(".bootstrap-dialog-body").css("width", "100%");
        dialog.getModalBody().find(".bootstrap-dialog-body").css("height", "100%");
        dialog.getModalBody().find(".bootstrap-dialog-message").css("width", "100%");
        dialog.getModalBody().find(".bootstrap-dialog-message").css("height", "100%");

        currentDialog = dialog;

        dialog.open();
    }

    /**
     * Closes the currently open modal
     */
    function closeModal() {
        if (currentDialog && currentDialog.close) {
            currentDialog.close();
            currentDialog = null;
        }
    }

    /**
     * Refreshes the parent page - called from iframe on successful form submission
     */
    function refreshAndClose() {
        closeModal();
        if (window.location && window.location.reload) {
            window.location.reload();
        }
    }

    /**
     * Initializes click handlers for modal links
     * Alt+Click opens in modal, regular click opens in new tab (default behavior)
     */
    function init() {
        // Use event delegation for dynamically added links
        $(document).on('click', '[data-open-modal="true"]', function (e) {
            var $link = $(this);
            var url = $link.attr('href');
            var title = $link.attr('data-modal-title') || $link.attr('title') || '';

            // Alt+Click: Open in modal
            if (e.altKey && url && url !== '#') {
                e.preventDefault();
                e.stopPropagation();
                openFormModal(url, title);
                return false;
            }
            
            // Regular click: Let default behavior happen (open in new tab)
            // Don't prevent default - the link will work normally
        });
    }

    // Public API
    return {
        init: init,
        openFormModal: openFormModal,
        closeModal: closeModal,
        refreshAndClose: refreshAndClose
    };

})(window.jQuery);

// Auto-initialize when document is ready
$(document).ready(function () {
    if (window.TableModalLinks) {
        window.TableModalLinks.init();
    }
});

