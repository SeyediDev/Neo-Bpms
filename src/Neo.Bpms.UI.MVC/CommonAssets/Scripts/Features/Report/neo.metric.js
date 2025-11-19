var NeoMetrics = {
    defaultOptions: {
        iconClass: 'fa fa-info-circle',
        value: '',
        title: '',
        backgroundColor: '#17a2b8',//If it got changed, ReportResult.cs -> GetDefaultPropertyValue should also change
        textsColor: '#ffffff'//If it got changed, ReportResult.cs -> GetDefaultPropertyValue should also change
    },
    
    // Convert English digits to Persian digits
    toPersianDigits: function(str) {
        if (!str) return str;
        var persianDigits = ['۰', '۱', '۲', '۳', '۴', '۵', '۶', '۷', '۸', '۹'];
        return str.toString().replace(/\d/g, function(w) {
            return persianDigits[parseInt(w)];
        });
    },
    
    obtainOptions: function (userOptions) {
        return {
            iconClass: userOptions.iconClass == '' ? this.defaultOptions.iconClass : userOptions.iconClass,
            value: userOptions.value,
            title: userOptions.title,
            backgroundColor: userOptions.backgroundColor == '' ? this.defaultOptions.backgroundColor : userOptions.backgroundColor,
            textsColor: userOptions.textsColor == '' ? this.defaultOptions.textsColor : userOptions.textsColor
        };
    },

    create: function (selector, userOptions) {
        var $element = $(selector);
        var options = this.obtainOptions(userOptions);

        var hasIcon = options.iconClass && options.iconClass.trim().length > 0;
        var hasTitle = options.title && options.title.trim().length > 0;
        
        // Convert value to Persian digits
        var persianValue = this.toPersianDigits(options.value);
        
        var widgetHtml = '<div class="neo-metric-box" style="background-color:' + options.backgroundColor + ';color:' + options.textsColor + ';">';

        if (hasIcon) {
            widgetHtml += '<span class="neo-metric-box__icon" aria-hidden="true"><i class="' + options.iconClass + '"></i></span>';
        }

        widgetHtml += '<div class="neo-metric-box__content">';
        widgetHtml += '<span class="neo-metric-box__value">' + persianValue + '</span>';

        if (hasTitle) {
            widgetHtml += '<span class="neo-metric-box__title">' + options.title + '</span>';
        }

        widgetHtml += '</div></div>';

        $element.html(widgetHtml);
    }
}

