/**
 * TableModalLinks - Opens IndexTable form links in modal dialogs
 * 
 * This module intercepts clicks on table form links with data-open-modal="true"
 * and opens them in a Bootstrap Dialog modal with an iframe.
 * 
 * Supports both BootstrapDialog (if available) and standard Bootstrap modals.
 */
window.TableModalLinks = (function ($) {
    'use strict';

    var defaults = {
        dialogWidth: '90%',
        dialogHeightRatio: 0.75,
        modalId: 'tableFormModal'
    };

    var currentModal = null;

    /**
     * Creates the modal HTML if it doesn't exist
     */
    function ensureModalExists() {
        if ($('#' + defaults.modalId).length === 0) {
            var modalHtml = 
                '<div class="modal fade" id="' + defaults.modalId + '" tabindex="-1" role="dialog" aria-labelledby="' + defaults.modalId + 'Label">' +
                    '<div class="modal-dialog modal-xl" role="document" style="width: ' + defaults.dialogWidth + '; max-width: ' + defaults.dialogWidth + ';">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<h5 class="modal-title" id="' + defaults.modalId + 'Label"></h5>' +
                                '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
                                    '<span aria-hidden="true">&times;</span>' +
                                '</button>' +
                            '</div>' +
                            '<div class="modal-body" style="padding: 0; height: ' + (screen.availHeight * defaults.dialogHeightRatio) + 'px;">' +
                                '<iframe id="' + defaults.modalId + 'Iframe" style="width: 100%; height: 100%; border: none;" frameborder="0" allowTransparency="true"></iframe>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>';
            $('body').append(modalHtml);
            
            // Handle modal close - cleanup iframe
            $('#' + defaults.modalId).on('hidden.bs.modal', function () {
                $('#' + defaults.modalId + 'Iframe').attr('src', 'about:blank');
            });
        }
    }

    /**
     * Opens a form URL in a modal dialog with an iframe
     * @param {string} url - The URL to open
     * @param {string} title - The modal title
     */
    function openFormModal(url, title) {
        // Build iframe URL - convert to IframeForm if needed
        var iframeUrl = convertToIframeUrl(url);
        
        // Try BootstrapDialog first (if available from open-drilldown.js)
        var targetWindow = (window.top && window.top !== window) ? window.top : window;
        var TargetBootstrapDialog = targetWindow.BootstrapDialog || window.BootstrapDialog;

        if (TargetBootstrapDialog) {
            openWithBootstrapDialog(iframeUrl, title, TargetBootstrapDialog);
        } else {
            // Fallback to standard Bootstrap modal
            openWithBootstrapModal(iframeUrl, title);
        }
    }

    /**
     * Opens form using BootstrapDialog (from open-drilldown.js)
     */
    function openWithBootstrapDialog(iframeUrl, title, TargetBootstrapDialog) {
        // Create iframe container
        var $container = $('<div style="width:100%;height:100%;"></div>');
        var $iframe = $('<iframe></iframe>')
            .attr('src', iframeUrl)
            .attr('frameborder', '0')
            .attr('allowTransparency', 'true')
            .css({
                'width': '100%',
                'height': '100%',
                'border': 'none'
            });
        $container.append($iframe);

        var dialog = new TargetBootstrapDialog({
            title: title || window.tetaI18n?.t('Form') || 'Form',
            message: $container
        });

        dialog.realize();
        
        // Apply modal sizing
        var dialogHeight = screen.availHeight * defaults.dialogHeightRatio;
        dialog.getModalBody().css({
            'width': '100%',
            'height': dialogHeight + 'px',
            'padding': '0'
        });
        dialog.getModalDialog().css('width', defaults.dialogWidth);
        dialog.getModalBody().find('.bootstrap-dialog-body').css({
            'width': '100%',
            'height': '100%'
        });
        dialog.getModalBody().find('.bootstrap-dialog-message').css({
            'width': '100%',
            'height': '100%'
        });

        currentModal = dialog;

        // Handle iframe load for auto-close on success
        $iframe.on('load', function() {
            handleIframeLoad(this, function() {
                dialog.close();
            });
        });

        dialog.open();
    }

    /**
     * Opens form using standard Bootstrap modal
     */
    function openWithBootstrapModal(iframeUrl, title) {
        ensureModalExists();
        
        var $modal = $('#' + defaults.modalId);
        var $iframe = $('#' + defaults.modalId + 'Iframe');
        
        // Set title and iframe source
        $('#' + defaults.modalId + 'Label').text(title || window.tetaI18n?.t('Form') || 'Form');
        $iframe.attr('src', iframeUrl);
        
        // Handle iframe load for auto-close on success
        $iframe.off('load').on('load', function() {
            handleIframeLoad(this, function() {
                $modal.modal('hide');
            });
        });
        
        // Show modal
        $modal.modal('show');
        currentModal = $modal;
    }

    /**
     * Handles iframe load event to detect form submission success
     */
    function handleIframeLoad(iframe, closeCallback) {
        try {
            var iframeWindow = iframe.contentWindow;
            if (iframeWindow && iframeWindow.modalFormSubmitted) {
                closeCallback();
                // Refresh parent page or table
                if (window.location && window.location.reload) {
                    window.location.reload();
                }
            }
        } catch (e) {
            // Cross-origin restriction, ignore
        }
    }

    /**
     * Converts a regular form URL to IframeForm URL
     * @param {string} url - Original URL
     * @returns {string} - IframeForm URL
     */
    function convertToIframeUrl(url) {
        // Check if already an iframe URL
        if (url.indexOf('IframeForm') > -1 || url.indexOf('IframeIndex') > -1) {
            return url;
        }

        // Convert Form actions to IframeForm
        var formActions = ['Edit', 'Details', 'Delete', 'Create'];
        for (var i = 0; i < formActions.length; i++) {
            var action = formActions[i];
            var pattern = '/Form/' + action;
            if (url.indexOf(pattern) > -1) {
                return url.replace(pattern, '/Form/IframeForm');
            }
        }

        return url;
    }

    /**
     * Closes the currently open modal
     */
    function closeModal() {
        if (currentModal) {
            if (currentModal.close) {
                currentModal.close();
            } else if (currentModal.modal) {
                currentModal.modal('hide');
            }
            currentModal = null;
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
     */
    function init() {
        // Use event delegation for dynamically added links
        $(document).on('click', '[data-open-modal="true"]', function (e) {
            e.preventDefault();
            e.stopPropagation();

            var $link = $(this);
            var url = $link.attr('href');
            var title = $link.attr('data-modal-title') || $link.attr('title') || '';

            if (url && url !== '#') {
                openFormModal(url, title);
            }
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

