/**
 * report-filter.js
 * Filter Functions for Report Pages
 * Extracted from _Scripts.filter.cshtml for testing
 */

// Function to collect all filter values for reports
function collectReportFilterValues() {
    const filterForm = document.getElementById('filter-form');
    if (!filterForm) return;
    
    // FIRST: Update all Modern Multi-Select controls before collecting values
    const panel = document.getElementById('filterTooltipPanel');
    if (panel) {
        const allControls = panel.querySelectorAll('.modern-multi-select-control');
        allControls.forEach(function(control) {
            const $control = $(control);
            
            // Find the hidden select element associated with this control
            // The hidden select is a sibling of the control wrapper (inserted right before it)
            let $hiddenSelect = $control.prev('select.modern-multi-select-hidden');
            
            // If not found as previous sibling, try to find it in the same parent
            if ($hiddenSelect.length === 0) {
                const $parent = $control.parent();
                const fieldName = $control.data('field-name');
                
                // Try to find by field name
                if (fieldName) {
                    $hiddenSelect = $parent.find('select.modern-multi-select-hidden[name*="' + fieldName + '"]');
                }
                
                // If still not found, try to find any hidden select in the same parent
                if ($hiddenSelect.length === 0) {
                    $hiddenSelect = $parent.find('select.modern-multi-select-hidden').first();
                }
            }
            
            if ($hiddenSelect.length > 0) {
                // Extract selected values from the control's items
                const selectedValues = [];
                
                // Get values from visible items in items container
                const $items = $control.find('.modern-multi-select-items .modern-multi-select-item');
                $items.each(function() {
                    const value = $(this).data('value');
                    if (value !== undefined && value !== null && value !== '') {
                        selectedValues.push(String(value));
                    }
                });
                
                // Also check overflow popover for additional selected items
                const $overflowItems = $control.find('.overflow-popover-content .modern-multi-select-item');
                $overflowItems.each(function() {
                    const value = $(this).data('value');
                    if (value !== undefined && value !== null && value !== '') {
                        const valueStr = String(value);
                        // Avoid duplicates
                        if (selectedValues.indexOf(valueStr) === -1) {
                            selectedValues.push(valueStr);
                        }
                    }
                });
                
                // Update the hidden select with selected values
                $hiddenSelect.val(null); // Clear first
                if (selectedValues.length > 0) {
                    $hiddenSelect.val(selectedValues);
                }
                
                // Ensure it's associated with form
                $hiddenSelect.attr('form', 'filter-form');
            }
        });
    }
    
    // SECOND: Find all filter inputs (including hidden ones)
    const allFilterInputs = document.querySelectorAll('#filterTooltipPanel input, #filterTooltipPanel select, #filterTooltipPanel textarea');
    
    allFilterInputs.forEach(function(input) {
        // Ensure input is associated with form
        input.setAttribute('form', 'filter-form');
    });
    
    // THIRD: Also check for any dynamically created inputs
    const dynamicInputs = document.querySelectorAll('input[name*="FilterParameter"], input[name*="[]"]');
    dynamicInputs.forEach(function(input) {
        if (!input.getAttribute('form')) {
            input.setAttribute('form', 'filter-form');
        }
    });
}

// Global submitFilter function for report pages
window.submitReportFilter = function() {
    // Prevent multiple submissions
    const submitBtn = document.getElementById('submit-filter');
    if (submitBtn && submitBtn.classList.contains('ladda-loading')) {
        return;
    }
    
    // Ensure all filter inputs are associated with the form
    if (typeof window.ensureFilterFormAssociation === 'function') {
        window.ensureFilterFormAssociation();
    }
    
    // Collect all filter values first
    // در کد اصلی، collectReportFilterValues به صورت function declaration است که در scope global است
    // در Jest/Node.js، باید از window.collectReportFilterValues استفاده کنیم
    if (typeof collectReportFilterValues === 'function') {
        collectReportFilterValues();
    } else if (typeof window !== 'undefined' && typeof window.collectReportFilterValues === 'function') {
        window.collectReportFilterValues();
    } else {
        console.error('collectReportFilterValues function not found!');
    }
    
    // Try to find form - check multiple possible IDs
    let form = document.getElementById('filter-form');
    if (!form) {
        form = document.querySelector('form[action*="Report"]');
    }
    if (!form) {
        form = document.querySelector('form');
    }
    
    if (form) {
        // Ensure all inputs are in the form
        const allInputs = document.querySelectorAll('#filterTooltipPanel input, #filterTooltipPanel select, #filterTooltipPanel textarea');
        allInputs.forEach(function(input) {
            if (input.getAttribute('form')) {
                // If input has form attribute, ensure it's correct
                input.setAttribute('form', form.id || 'filter-form');
            } else if (!form.contains(input)) {
                // If input is not in form, add form attribute
                input.setAttribute('form', form.id || 'filter-form');
            }
        });
        
        // Submit the form
        form.submit();
    } else {
        // Fallback: try to click hidden submit button
        const hiddenSubmit = document.getElementById('submitBtn');
        if (hiddenSubmit) {
            hiddenSubmit.click();
        } else {
            alert('خطا: فرم فیلتر یافت نشد. لطفاً صفحه را رفرش کنید.');
        }
    }
};

// Keep backward compatibility for report pages
window.submitFilter = window.submitReportFilter;

// Clear filter function for report pages - Handles Modern Multi-Select
window.clearReportFilter = function() {
    const panel = document.getElementById('filterTooltipPanel');
    if (!panel) {
        return;
    }
    
    // FIRST: Clear all hidden selects (these are the actual form inputs)
    const hiddenSelects = panel.querySelectorAll('select.modern-multi-select-hidden');
    hiddenSelects.forEach(function(select) {
        const $select = $(select);
        if ($select.is('[multiple]')) {
            $select.val(null);
        } else {
            $select.val('');
        }
        // Trigger change.external event to notify the custom control
        $select.trigger('change.external');
        $select.trigger('change');
    });
    
    // SECOND: Clear all Modern Multi-Select controls visually
    const allControls = panel.querySelectorAll('.modern-multi-select-control');
    allControls.forEach(function(control) {
        const $control = $(control);
        
        // Manually update the visual control
        const $itemsContainer = $control.find('.modern-multi-select-items');
        const $overflowPopover = $control.find('.modern-multi-select-overflow-popover');
        const $overflowPopoverContent = $overflowPopover.find('.overflow-popover-content');
        const $overflowBadge = $control.find('.modern-multi-select-overflow-badge');
        const $dropdown = $control.find('.modern-multi-select-dropdown');
        
        // Clear visual items
        if ($itemsContainer.length > 0) {
            $itemsContainer.empty();
            $itemsContainer.html('<span style="color: #94a3b8;">--- انتخاب کنید ---</span>');
        }
        
        // Clear popover content
        if ($overflowPopoverContent.length > 0) {
            $overflowPopoverContent.empty();
        }
        
        // Hide and clear badge
        if ($overflowBadge.length > 0) {
            $overflowBadge.empty();
            $overflowBadge.attr('data-visible', 'false');
            $overflowBadge.removeClass('active');
        }
        
        // Close dropdown
        $control.removeClass('open');
        if ($dropdown.length > 0 && $dropdown[0]) {
            $dropdown[0].style.setProperty('display', 'none', 'important');
            $dropdown[0].style.setProperty('visibility', 'hidden', 'important');
            $dropdown[0].style.setProperty('opacity', '0', 'important');
        }
        
        // Close popover
        if ($overflowPopover.length > 0 && $overflowPopover[0]) {
            $overflowPopover[0].style.setProperty('display', 'none', 'important');
            $overflowPopover[0].style.setProperty('visibility', 'hidden', 'important');
            $overflowPopover[0].style.setProperty('opacity', '0', 'important');
            $overflowPopover[0].style.setProperty('pointer-events', 'none', 'important');
        }
    });
    
    // THIRD: Clear all regular form fields (inputs, selects, textareas) that are NOT Modern Multi-Select
    const regularFields = panel.querySelectorAll('input:not(.modern-multi-select-hidden):not([type="hidden"]), select:not(.modern-multi-select-hidden), textarea');
    regularFields.forEach(function(field) {
        const $field = $(field);
        
        // Skip if this is a Modern Multi-Select control element
        if ($field.closest('.modern-multi-select-control').length > 0) {
            return;
        }
        
        if ($field.is('select')) {
            if ($field.is('[multiple]')) {
                $field.val(null);
            } else {
                // Try to set empty value
                if ($field.find('option[value=""]').length > 0) {
                    $field.val('');
                } else if ($field.find('option[value="0"]').length > 0) {
                    $field.val('0');
                } else {
                    $field.prop('selectedIndex', -1);
                }
            }
            $field.trigger('change');
        } else if ($field.is('input[type="checkbox"], input[type="radio"]')) {
            $field.prop('checked', false);
            $field.trigger('change');
        } else {
            $field.val('');
            $field.trigger('change');
        }
    });
};

// Make collectReportFilterValues available globally
// در کد اصلی (_Scripts.filter.cshtml) این function در scope global است
// پس باید در window قرار گیرد تا window.submitReportFilter بتواند آن را فراخوانی کند
if (typeof window !== 'undefined') {
    window.collectReportFilterValues = collectReportFilterValues;
}
// همچنین در global برای تست
if (typeof global !== 'undefined') {
    global.collectReportFilterValues = collectReportFilterValues;
}

// Export for testing (if using modules)
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        collectReportFilterValues,
        submitReportFilter: window.submitReportFilter,
        clearReportFilter: window.clearReportFilter
    };
}

