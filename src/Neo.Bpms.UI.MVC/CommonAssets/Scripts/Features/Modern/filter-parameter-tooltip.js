/**
 * Filter Parameter Tooltip Implementation
 * Neo BPMS - Modern Filter Parameters
 * Version: 1.0.0
 */

(function($) {
    'use strict';
    
    // Global variables to track open tooltips
    let openTooltip = null;
    
    // Initialize filter parameter tooltips
    function initializeFilterParameterTooltips() {
        console.log('Filter Parameter Tooltip: Starting initialization');
        
        // Close tooltip when clicking outside
        $(document).on('click', function(e) {
            if (openTooltip && !$(e.target).closest('.filter-parameter-tooltip').length) {
                closeTooltip(openTooltip);
            }
        });
        
        // Handle escape key
        $(document).on('keydown', function(e) {
            if (e.key === 'Escape' && openTooltip) {
                closeTooltip(openTooltip);
            }
        });
        
        console.log('Filter Parameter Tooltip: Initialization complete');
    }
    
    // Toggle tooltip visibility - Global function
    window.toggleFilterParameterTooltip = function(uniqueFieldId) {
        console.log('Filter Parameter Tooltip: toggleFilterParameterTooltip called for', uniqueFieldId);
        const tooltipId = `#${uniqueFieldId}-fp-tooltip`;
        const $tooltip = $(tooltipId);
        const $icon = $(`#${uniqueFieldId}-fp-icon`).closest('.filter-parameter-icon');
        
        if (openTooltip === tooltipId) {
            closeTooltip(tooltipId);
        } else {
            // Close any other open tooltip
            if (openTooltip) {
                closeTooltip(openTooltip);
            }
            
            // Position tooltip relative to icon
            const iconOffset = $icon.offset();
            const iconWidth = $icon.outerWidth();
            const iconHeight = $icon.outerHeight();
            
            // Check if offset is available (fallback for older browsers)
            if (iconOffset && iconOffset.top !== undefined) {
                $tooltip.css({
                    'display': 'block',
                    'position': 'absolute',
                    'top': (iconHeight + 8) + 'px',
                    'left': '0px',
                    'z-index': '2002' // Higher than modern-filter-wrapper (2000)
                });
            } else {
                // Fallback positioning
                $tooltip.css({
                    'display': 'block',
                    'position': 'absolute',
                    'top': '32px',
                    'left': '0px',
                    'z-index': '2002' // Higher than modern-filter-wrapper (2000)
                });
            }
            
            openTooltip = tooltipId;
            
            // Add hover effects
            $tooltip.find('.filter-parameter-item').hover(
                function() {
                    if (!$(this).hasClass('selected')) {
                        $(this).css('background', 'linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%)');
                    }
                },
                function() {
                    if (!$(this).hasClass('selected')) {
                        $(this).css('background', '');
                    }
                }
            );
        }
    };
    
    // Close tooltip
    function closeTooltip(tooltipId) {
        if (tooltipId) {
            $(tooltipId).css('display', 'none');
            openTooltip = null;
        }
    }
    
    // Select filter parameter - Global function
    window.selectFilterParameter = function(parameter, uniqueFieldId) {
        console.log('Filter Parameter Tooltip: Selecting parameter', parameter, 'for field', uniqueFieldId);
        
        // Extract field name from unique ID (remove the GUID part)
        const fieldName = uniqueFieldId.split('_')[0];
        
        // Update hidden input
        const hiddenInputName = `${fieldName}_filterParameter`;
        $(`input[name="${hiddenInputName}"]`).val(parameter);
        
        // Update icon
        const iconId = `#${uniqueFieldId}-fp-icon`;
        const $icon = $(iconId);
        
        // Get the SVG for the selected parameter
        const svg = getFilterParameterIconSvg(parameter);
        $icon.html(svg);
        
        // Update visual state
        const $tooltip = $(`#${uniqueFieldId}-fp-tooltip`);
        $tooltip.find('.filter-parameter-item').removeClass('selected').css('background', '');
        $tooltip.find(`[onclick*="${parameter}"]`).addClass('selected').css('background', 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)');
        
        // Close tooltip
        closeTooltip(`#${uniqueFieldId}-fp-tooltip`);
        
        // Trigger change event for any dependent logic
        $(`input[name="${hiddenInputName}"]`).trigger('change');
        
        console.log('Filter Parameter Tooltip: Parameter selected successfully');
    };
    
    // Get SVG icon for filter parameter
    function getFilterParameterIconSvg(parameter) {
        const icons = {
            'IsEqualTo': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M11.9199 22.75C5.99992 22.75 1.16992 17.93 1.16992 12C1.16992 6.07 5.99992 1.25 11.9199 1.25C17.8399 1.25 22.6699 6.07 22.6699 12C22.6699 17.93 17.8499 22.75 11.9199 22.75ZM11.9199 2.75C6.81992 2.75 2.66992 6.9 2.66992 12C2.66992 17.1 6.81992 21.25 11.9199 21.25C17.0199 21.25 21.1699 17.1 21.1699 12C21.1699 6.9 17.0199 2.75 11.9199 2.75Z" fill="#2563EB"/><path d="M16.5455 14.9999H7.81818C7.37091 14.9999 7 14.629 7 14.1817C7 13.7344 7.37091 13.3635 7.81818 13.3635H16.5455C16.9927 13.3635 17.3636 13.7344 17.3636 14.1817C17.3636 14.629 17.0036 14.9999 16.5455 14.9999Z" fill="#2563EB"/><path d="M16.5455 10.6364H7.81818C7.37091 10.6364 7 10.2655 7 9.81818C7 9.37091 7.37091 9 7.81818 9H16.5455C16.9927 9 17.3636 9.37091 17.3636 9.81818C17.3636 10.2655 17.0036 10.6364 16.5455 10.6364Z" fill="#2563EB"/></svg>',
            'IsNotEqualTo': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M11.9199 22.75C5.99992 22.75 1.16992 17.93 1.16992 12C1.16992 6.07 5.99992 1.25 11.9199 1.25C17.8399 1.25 22.6699 6.07 22.6699 12C22.6699 17.93 17.8499 22.75 11.9199 22.75ZM11.9199 2.75C6.81992 2.75 2.66992 6.9 2.66992 12C2.66992 17.1 6.81992 21.25 11.9199 21.25C17.0199 21.25 21.1699 17.1 21.1699 12C21.1699 6.9 17.0199 2.75 11.9199 2.75Z" fill="#2563EB"/><path d="M16.5455 14.9999H7.81818C7.37091 14.9999 7 14.629 7 14.1817C7 13.7344 7.37091 13.3635 7.81818 13.3635H16.5455C16.9927 13.3635 17.3636 13.7344 17.3636 14.1817C17.3636 14.629 17.0036 14.9999 16.5455 14.9999Z" fill="#2563EB"/><path d="M16.5455 10.6364H7.81818C7.37091 10.6364 7 10.2655 7 9.81818C7 9.37091 7.37091 9 7.81818 9H16.5455C16.9927 9 17.3636 9.37091 17.3636 9.81818C17.3636 10.2655 17.0036 10.6364 16.5455 10.6364Z" fill="#2563EB"/><path d="M9.58632 18.0239C9.4028 18.0731 9.20115 18.0546 9.01744 17.9486C8.66227 17.7435 8.53805 17.2799 8.74309 16.9247L14.1413 6.46555C14.3464 6.11034 14.8101 5.9861 15.1652 6.1912C15.5204 6.39629 15.6446 6.85988 15.4396 7.21508L10.0413 17.6742C9.93526 17.8579 9.76984 17.9747 9.58632 18.0239Z" fill="#2563EB"/></svg>',
            'Contains': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M10.5804 15.58C10.3804 15.58 10.1904 15.5 10.0504 15.36L7.22043 12.53C6.93043 12.24 6.93043 11.76 7.22043 11.47C7.51043 11.18 7.99043 11.18 8.28043 11.47L10.5804 13.77L15.7204 8.62998C16.0104 8.33998 16.4904 8.33998 16.7804 8.62998C17.0704 8.91998 17.0704 9.39998 16.7804 9.68998L11.1104 15.36C10.9704 15.5 10.7804 15.58 10.5804 15.58Z" fill="#2563EB"/></svg>',
            'DoesNotContain': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M9.16986 15.5801C8.97986 15.5801 8.78986 15.5101 8.63986 15.3601C8.34986 15.0701 8.34986 14.5901 8.63986 14.3001L14.2999 8.64011C14.5899 8.35011 15.0699 8.35011 15.3599 8.64011C15.6499 8.93011 15.6499 9.41011 15.3599 9.70011L9.69986 15.3601C9.55986 15.5101 9.35986 15.5801 9.16986 15.5801Z" fill="#2563EB"/><path d="M14.8299 15.5801C14.6399 15.5801 14.4499 15.5101 14.2999 15.3601L8.63986 9.70011C8.34986 9.41011 8.34986 8.93011 8.63986 8.64011C8.92986 8.35011 9.40986 8.35011 9.69986 8.64011L15.3599 14.3001C15.6499 14.5901 15.6499 15.0701 15.3599 15.3601C15.2099 15.5101 15.0199 15.5801 14.8299 15.5801Z" fill="#2563EB"/></svg>',
            'StartsWith': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M11.9697 22.75C6.04973 22.75 1.21973 17.93 1.21973 12C1.21973 6.07 6.04973 1.25 11.9697 1.25C17.8897 1.25 22.7197 6.07 22.7197 12C22.7197 17.93 17.8997 22.75 11.9697 22.75ZM11.9697 2.75C6.86973 2.75 2.71973 6.9 2.71973 12C2.71973 17.1 6.86973 21.25 11.9697 21.25C17.0697 21.25 21.2197 17.1 21.2197 12C21.2197 6.9 17.0697 2.75 11.9697 2.75Z" fill="#2563EB"/><path d="M10.5602 16.99C10.1202 16.99 9.70023 16.88 9.33023 16.67C8.47023 16.17 7.99023 15.19 7.99023 13.91V10.56C7.99023 9.27999 8.46023 8.29999 9.32023 7.79999C10.1802 7.29999 11.2702 7.37999 12.3802 8.01999L15.2802 9.68999C16.3902 10.33 17.0002 11.23 17.0002 12.23C17.0002 13.22 16.3902 14.13 15.2802 14.77L12.3802 16.44C11.7602 16.81 11.1302 16.99 10.5602 16.99ZM10.5602 8.96999C10.3802 8.96999 10.2102 9.00999 10.0802 9.08999C9.70023 9.30999 9.49023 9.83999 9.49023 10.56V13.91C9.49023 14.62 9.70023 15.16 10.0802 15.37C10.4502 15.59 11.0202 15.5 11.6402 15.15L14.5402 13.48C15.1602 13.12 15.5102 12.67 15.5102 12.24C15.5102 11.81 15.1502 11.36 14.5402 11L11.6402 9.32999C11.2402 9.08999 10.8702 8.96999 10.5602 8.96999Z" fill="#2563EB"/></svg>',
            'EndsWith': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M11.9697 22.75C6.04973 22.75 1.21973 17.93 1.21973 12C1.21973 6.07 6.04973 1.25 11.9697 1.25C17.8897 1.25 22.7197 6.07 22.7197 12C22.7197 17.93 17.8997 22.75 11.9697 22.75ZM11.9697 2.75C6.86973 2.75 2.71973 6.9 2.71973 12C2.71973 17.1 6.86973 21.25 11.9697 21.25C17.0697 21.25 21.2197 17.1 21.2197 12C21.2197 6.9 17.0697 2.75 11.9697 2.75Z" fill="#2563EB"/><path d="M13.2695 16.98H10.7295C8.19953 16.98 7.01953 15.8 7.01953 13.27V10.73C7.01953 8.20002 8.19953 7.02002 10.7295 7.02002H13.2695C15.7995 7.02002 16.9795 8.20002 16.9795 10.73V13.27C16.9795 15.8 15.7995 16.98 13.2695 16.98ZM10.7295 8.52002C9.03953 8.52002 8.51953 9.04002 8.51953 10.73V13.27C8.51953 14.96 9.03953 15.48 10.7295 15.48H13.2695C14.9595 15.48 15.4795 14.96 15.4795 13.27V10.73C15.4795 9.04002 14.9595 8.52002 13.2695 8.52002H10.7295Z" fill="#2563EB"/></svg>',
            'GreaterThan': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M15.5 12.75H8.5C8.09 12.75 7.75 12.41 7.75 12C7.75 11.59 8.09 11.25 8.5 11.25H15.5C15.91 11.25 16.25 11.59 16.25 12C16.25 12.41 15.91 12.75 15.5 12.75Z" fill="#2563EB"/><path d="M12 16.25C11.59 16.25 11.25 15.91 11.25 15.5V8.5C11.25 8.09 11.59 7.75 12 7.75C12.41 7.75 12.75 8.09 12.75 8.5V15.5C12.75 15.91 12.41 16.25 12 16.25Z" fill="#2563EB"/></svg>',
            'GreaterThanOrEqual': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M15.5 12.75H8.5C8.09 12.75 7.75 12.41 7.75 12C7.75 11.59 8.09 11.25 8.5 11.25H15.5C15.91 11.25 16.25 11.59 16.25 12C16.25 12.41 15.91 12.75 15.5 12.75Z" fill="#2563EB"/><path d="M12 16.25C11.59 16.25 11.25 15.91 11.25 15.5V8.5C11.25 8.09 11.59 7.75 12 7.75C12.41 7.75 12.75 8.09 12.75 8.5V15.5C12.75 15.91 12.41 16.25 12 16.25Z" fill="#2563EB"/><path d="M8.5 8.5C8.09 8.5 7.75 8.16 7.75 7.75C7.75 7.34 8.09 7 8.5 7C8.91 7 9.25 7.34 9.25 7.75C9.25 8.16 8.91 8.5 8.5 8.5Z" fill="#2563EB"/><path d="M15.5 8.5C15.09 8.5 14.75 8.16 14.75 7.75C14.75 7.34 15.09 7 15.5 7C15.91 7 16.25 7.34 16.25 7.75C16.25 8.16 15.91 8.5 15.5 8.5Z" fill="#2563EB"/></svg>',
            'LessThan': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M15.5 12.75H8.5C8.09 12.75 7.75 12.41 7.75 12C7.75 11.59 8.09 11.25 8.5 11.25H15.5C15.91 11.25 16.25 11.59 16.25 12C16.25 12.41 15.91 12.75 15.5 12.75Z" fill="#2563EB"/><path d="M12 7.75C12.41 7.75 12.75 8.09 12.75 8.5V15.5C12.75 15.91 12.41 16.25 12 16.25C11.59 16.25 11.25 15.91 11.25 15.5V8.5C11.25 8.09 11.59 7.75 12 7.75Z" fill="#2563EB"/></svg>',
            'LessThanOrEqual': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M12 22.75C6.07 22.75 1.25 17.93 1.25 12C1.25 6.07 6.07 1.25 12 1.25C17.93 1.25 22.75 6.07 22.75 12C22.75 17.93 17.93 22.75 12 22.75ZM12 2.75C6.9 2.75 2.75 6.9 2.75 12C2.75 17.1 6.9 21.25 12 21.25C17.1 21.25 21.25 17.1 21.25 12C21.25 6.9 17.1 2.75 12 2.75Z" fill="#2563EB"/><path d="M15.5 12.75H8.5C8.09 12.75 7.75 12.41 7.75 12C7.75 11.59 8.09 11.25 8.5 11.25H15.5C15.91 11.25 16.25 11.59 16.25 12C16.25 12.41 15.91 12.75 15.5 12.75Z" fill="#2563EB"/><path d="M12 7.75C12.41 7.75 12.75 8.09 12.75 8.5V15.5C12.75 15.91 12.41 16.25 12 16.25C11.59 16.25 11.25 15.91 11.25 15.5V8.5C11.25 8.09 11.59 7.75 12 7.75Z" fill="#2563EB"/><path d="M8.5 8.5C8.09 8.5 7.75 8.16 7.75 7.75C7.75 7.34 8.09 7 8.5 7C8.91 7 9.25 7.34 9.25 7.75C9.25 8.16 8.91 8.5 8.5 8.5Z" fill="#2563EB"/><path d="M15.5 8.5C15.09 8.5 14.75 8.16 14.75 7.75C14.75 7.34 15.09 7 15.5 7C15.91 7 16.25 7.34 16.25 7.75C16.25 8.16 15.91 8.5 15.5 8.5Z" fill="#2563EB"/></svg>',
            'IsNull': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M14 6.75H10C9.04 6.75 7.25 6.75 7.25 4C7.25 1.25 9.04 1.25 10 1.25H14C14.96 1.25 16.75 1.25 16.75 4C16.75 4.96 16.75 6.75 14 6.75ZM10 2.75C9.01 2.75 8.75 2.75 8.75 4C8.75 5.25 9.01 5.25 10 5.25H14C15.25 5.25 15.25 4.99 15.25 4C15.25 2.75 14.99 2.75 14 2.75H10Z" fill="#2563EB"/><path d="M15 22.75H9C3.38 22.75 2.25 20.17 2.25 16V9.99999C2.25 5.43999 3.9 3.48999 7.96 3.27999C8.37 3.25999 8.73 3.56999 8.75 3.98999C8.77 4.40999 8.45 4.74999 8.04 4.76999C5.2 4.92999 3.75 5.77999 3.75 9.99999V16C3.75 19.7 4.48 21.25 9 21.25H15C19.52 21.25 20.25 19.7 20.25 16V9.99999C20.25 5.77999 18.8 4.92999 15.96 4.76999C15.55 4.74999 15.23 4.38999 15.25 3.97999C15.27 3.56999 15.63 3.24999 16.04 3.26999C20.1 3.48999 21.75 5.43999 21.75 9.98999V15.99C21.75 20.17 20.62 22.75 15 22.75Z" fill="#2563EB"/></svg>',
            'IsNotNull': '<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M15 12.95H8C7.59 12.95 7.25 12.61 7.25 12.2C7.25 11.79 7.59 11.45 8 11.45H15C15.41 11.45 15.75 11.79 15.75 12.2C15.75 12.61 15.41 12.95 15 12.95Z" fill="#2563EB"/><path d="M12.38 16.95H8C7.59 16.95 7.25 16.61 7.25 16.2C7.25 15.79 7.59 15.45 8 15.45H12.38C12.79 15.45 13.13 15.79 13.13 16.2C13.13 16.61 12.79 16.95 12.38 16.95Z" fill="#2563EB"/><path d="M14 6.75H10C9.04 6.75 7.25 6.75 7.25 4C7.25 1.25 9.04 1.25 10 1.25H14C14.96 1.25 16.75 1.25 16.75 4C16.75 4.96 16.75 6.75 14 6.75ZM10 2.75C9.01 2.75 8.75 2.75 8.75 4C8.75 5.25 9.01 5.25 10 5.25H14C15.25 5.25 15.25 4.99 15.25 4C15.25 2.75 14.99 2.75 14 2.75H10Z" fill="#2563EB"/><path d="M15 22.75H9C3.38 22.75 2.25 20.17 2.25 16V9.99999C2.25 5.43999 3.9 3.48999 7.96 3.27999C8.36 3.25999 8.73 3.56999 8.75 3.98999C8.77 4.40999 8.45 4.74999 8.04 4.76999C5.2 4.92999 3.75 5.77999 3.75 9.99999V16C3.75 19.7 4.48 21.25 9 21.25H15C19.52 21.25 20.25 19.7 20.25 16V9.99999C20.25 5.77999 18.8 4.92999 15.96 4.76999C15.55 4.74999 15.23 4.38999 15.25 3.97999C15.27 3.56999 15.63 3.24999 16.04 3.26999C20.1 3.48999 21.75 5.43999 21.75 9.98999V15.99C21.75 20.17 20.62 22.75 15 22.75Z" fill="#2563EB"/></svg>'
        };
        
        return icons[parameter] || icons['IsEqualTo'];
    }
    
    // Initialize when document is ready
    $(document).ready(function() {
        console.log('Filter Parameter Tooltip: Document ready, initializing...');
        
        // Check if jQuery is available
        if (typeof $ === 'undefined') {
            console.error('Filter Parameter Tooltip: jQuery is not available');
            return;
        }
        
        // Check if required functions exist
        if (typeof window.toggleFilterParameterTooltip === 'undefined') {
            console.error('Filter Parameter Tooltip: toggleFilterParameterTooltip function is not defined');
            return;
        }
        
        initializeFilterParameterTooltips();
        console.log('Filter Parameter Tooltip: Initialization complete');
    });
    
})(jQuery);
