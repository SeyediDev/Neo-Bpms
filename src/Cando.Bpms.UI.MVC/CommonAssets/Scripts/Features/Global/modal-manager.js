window.ModalManager = function ($) {
    var defaults = {
        $body: '',
        $bodyClass: '',
        $footer: '',
        $headerClass: '',
        title: '',
        $title: null,
        id: 'myModal',
        modalPlaceSelector: '#modal-place',
        size: 'lg',
        dontShowHeader: false,
        backdrop: true,
        keyboard: true,
        shownCallback: null
    };

    var getModalHeader = function (options) {
        var modalHeader = $('<div/>',
            {
                'class': 'modal-header ' + options.$headerClass
            });
        var closeButton = $('<button/>',
            {
                'class': 'close',
                'type': 'button',
                'data-dismiss': 'modal',
                'aria-label': 'Close'
            });
        var timesIcon = $('<span/>',
            {
                'aria-hidden': true,
                'text': '×'
            });
        closeButton.append(timesIcon);
        var modalTitle = $('<h4/>',
            {
                'id': options.id + 'Label',
                'class': 'modal-title',
            });
        if (options.$title) {
            modalTitle.append(options.$title);
        } else {
            modalTitle.text(options.title);
        }
        modalHeader.append(modalTitle);
        modalHeader.append(closeButton);
        return modalHeader;
    };
    var getModal = function (options) {
        var modalWrapper = $('<div/>',
            {
                'class': 'modal fade',
                'id': options.id,
                'data-backdrop': options.backdrop,
                'data-keyboard': options.keyboard,
                'tabindex': '-1',
                'role': 'dialog',
                'aria-labelledby': options.id + 'Label'
            });
        var modalDialog = $('<div/>',
            {
                'class': 'modal-dialog modal-' + options.size,
                'role': 'document'
            });
        var modalContent = $('<div/>',
            {
                'class': 'modal-content'
            });
        if (!options.dontShowHeader) {
            var modalHeader = getModalHeader(options);
            modalContent.append(modalHeader);
        }
        if (options.$body) {
            var modalBody = $('<div/>',
                {
                    'class': 'modal-body ' + options.$bodyClass
                }).append(options.$body);
            modalContent.append(modalBody);
        }

        if (options.$footer) {
            var modalFooter = $('<div/>',
                {
                    'class': 'modal-footer'
                }).append(options.$footer);
            modalContent.append(modalFooter);
        }
        modalDialog.append(modalContent);
        modalWrapper.append(modalDialog);
        modalWrapper.on('hidden.bs.modal', function () {
            clearPreviousModal(options.modalPlaceSelector);
        });
        if (typeof options.shownCallback === 'function') {
            modalWrapper.on('shown.bs.modal', function () {
                options.shownCallback();
            });
        }
        return modalWrapper;
    };

    var clearPreviousModal = function (modalPlaceSelector) {
        $(modalPlaceSelector).html('');
        $('.modal-backdrop').remove();
    }

    var openModal = function (options) {
        var modalOptions = $.extend({}, defaults, options);

        clearPreviousModal(modalOptions.modalPlaceSelector);
        $(modalOptions.modalPlaceSelector).html(getModal(modalOptions));
        $('#' + modalOptions.id).modal('show');
    }

    var closeModal = function (id) {
        id = id || 'myModal';
        $('#' + id).modal('hide');
    }

    var confirm = function (confirmOptions) {
        if (typeof confirmOptions.callback !== 'function')
            throw new Error('Callback function must be provided.');
        if (!confirmOptions.message)
            throw new Error('message must be provided.');
        confirmOptions.yesLabel = confirmOptions.yesLabel || window.tetaI18n.t('Yes');
        confirmOptions.noLabel = confirmOptions.noLabel || window.tetaI18n.t('No');
        var modalOptions = {
            backdrop: 'static',
            keyboard: false,
            dontShowHeader: true,
            size: 'md',
            $body: $('<div/>', {text: confirmOptions.message}),
            $footer: $('<div/>')
                .append($('<button/>',
                    {
                        'class': 'btn btn-primary',
                        'id': 'yes_button',
                        'type': 'button',
                        'text': confirmOptions.yesLabel,
                        on: {
                            click: function () {
                                confirmOptions.callback(true);
                                closeModal();
                            }

                        }
                    })
                    .append($('<i/>',
                        {
                            'class': 'fa fa-check mx-1'
                        }))
                )
                .append($('<button/>',
                    {
                        'class': 'btn btn-secondary mx-1',
                        'id': 'no_button',
                        'type': 'button',
                        'text': confirmOptions.noLabel,
                        one: {
                            click: function () {
                                confirmOptions.callback(false);
                                closeModal();

                            }
                        }
                    })
                    .append($('<i/>',
                        {
                            'class': 'fa fa-times mx-1'
                        }))
                )
        };
        openModal(modalOptions);
    }
    return {
        openModal: openModal,
        closeModal: closeModal,
        confirm: confirm
    };
}(window.jQuery);