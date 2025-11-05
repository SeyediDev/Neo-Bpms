/**
 * Report Filter Modern Multi-Select Initialization
 * Neo BPMS - Report Page
 * Version: 1.0.0
 */

(function($) {
    'use strict';
    
    // Initialize Modern Multi-Select for filter fields
    // Global function to ensure all filter inputs are associated with the form
    window.ensureFilterFormAssociation = function() {
        const filterForm = document.getElementById('filter-form');
        if (filterForm) {
            const filterInputs = document.querySelectorAll('#filterTooltipPanel input, #filterTooltipPanel select, #filterTooltipPanel textarea');
            filterInputs.forEach(function(input) {
                input.setAttribute('form', 'filter-form');
                console.log('Report Modern Multi-Select: Associated input with form:', input.name || input.id);
            });
        }
    };

    function initializeModernMultiSelect() {
        console.log('Report Modern Multi-Select: Starting initialization');
        
        // Ensure all filter inputs are associated with the form
        window.ensureFilterFormAssociation();
        
        // Initialize custom modern multi-select controls
        $('.modern-multi-select-control').each(function() {
            const $control = $(this);
            const fieldName = $control.data('field-name');
            const $hiddenInput = $(`#field-${fieldName}`);
            
            console.log('Report Modern Multi-Select: Processing control for field:', fieldName);
            console.log('Report Modern Multi-Select: Hidden input found:', $hiddenInput.length > 0);
            
            if ($hiddenInput.length === 0) {
                console.warn('Report Modern Multi-Select: Hidden input not found for field:', fieldName);
                return;
            }
            
            initializeCustomModernMultiSelect($control, $hiddenInput);
        });
        
        // Fallback: Replace Select2 with Modern Multi-Select for filter fields
        $('.modern-filter-wrapper select[multiple]').each(function() {
            const $select = $(this);
            const $container = $('<div class="modern-multi-select-container"></div>');
            
            // Insert container after select
            $select.after($container);
            
            // Hide original select
            $select.hide();
            
            // Initialize Modern Multi-Select
            $container.modernMultiSelect({
                placeholder: 'انتخاب کنید...',
                searchPlaceholder: 'جستجو...',
                noResultsText: 'نتیجه‌ای یافت نشد',
                allowSearch: true,
                allowClear: true,
                data: $select.find('option').map(function() {
                    return {
                        value: $(this).val(),
                        text: $(this).text()
                    };
                }).get(),
                ajax: null
            });
            
        // Sync values with original select and ensure name[] posts arrays
        $container.on('change', function(e) {
            const values = e.detail.values || [];
            $select.prop('multiple', true);
            // Ensure name[] to bind as array on server
            if (!$select.attr('name').endsWith('[]')) {
                $select.attr('name', $select.attr('name') + '[]');
            }
            // Ensure form association for filter inputs
            $select.attr('form', 'filter-form');
            $select.val(values).trigger('change');
            
            console.log('Report Modern Multi-Select: Updated select values:', values, 'for field:', $select.attr('name'));
        });
            
            // Set initial values
            const initialValues = $select.val() || [];
            $container.modernMultiSelect('setValue', initialValues);
        });
        
        // Handle single select fields
        $('.modern-filter-wrapper select:not([multiple])').each(function() {
            const $select = $(this);
            const $container = $('<div class="modern-multi-select-container"></div>');
            
            // Insert container after select
            $select.after($container);
            
            // Hide original select
            $select.hide();
            
            // Initialize Modern Multi-Select
            $container.modernMultiSelect({
                placeholder: 'انتخاب کنید...',
                searchPlaceholder: 'جستجو...',
                noResultsText: 'نتیجه‌ای یافت نشد',
                allowSearch: true,
                allowClear: true,
                maxSelections: 1,
                data: $select.find('option').map(function() {
                    return {
                        value: $(this).val(),
                        text: $(this).text()
                    };
                }).get(),
                ajax: null
            });
            
            // Sync values with original select (single)
            $container.on('change', function(e) {
                const values = e.detail.values || [];
                $select.prop('multiple', false);
                // Ensure plain name without [] for single
                $select.attr('name', ($select.attr('name') || '').replace(/\[\]$/, ''));
                $select.attr('form', 'filter-form');
                $select.val(values[0] || '').trigger('change');
            });
            
            // Set initial values
            const initialValue = $select.val();
            if (initialValue) {
                $container.modernMultiSelect('setValue', [initialValue]);
            }
        });
    }
    
    // Initialize custom modern multi-select control
    function initializeCustomModernMultiSelect($control, $hiddenInput) {
        const $input = $control.find('.modern-multi-select-input');
        const $itemsContainer = $control.find('.modern-multi-select-items');
        const $searchInput = $control.find('.modern-multi-select-search');
        const $dropdown = $control.find('.modern-multi-select-dropdown');
        const $options = $control.find('.modern-multi-select-options');
        const $arrow = $control.find('.modern-multi-select-arrow');
        
        let isOpen = false;
        let selectedValues = [];
        let allOptions = [];
        let isLoading = false;
        
        // Parse initial values
        const initialValue = $hiddenInput.val();
        if (initialValue) {
            selectedValues = initialValue.split(',').filter(v => v.trim() !== '');
        }
        
        // Load data from API
        function loadData() {
            if (isLoading) return;
            isLoading = true;
            
            const fieldName = $control.data('field-name');
            const isRemote = $control.data('is-remote');
            const filterFormula = $control.data('filter-formula');
            
            // Show loading state
            $options.html('<div class="modern-multi-select-loading">در حال بارگذاری...</div>');
            
            // Prepare request data
            const requestData = {
                fieldName: fieldName,
                isRemote: isRemote,
                filterFormula: filterFormula,
                searchTerm: $searchInput.val() || ''
            };
            
            // Call GetComboData API
            $.ajax({
                url: window.top.rootUrl + 'Form/GetComboData',
                type: 'POST',
                data: requestData,
                headers: window.AddAntiForgeryToken(),
                success: function(response) {
                    if (response && response.data) {
                        allOptions = response.data;
                        renderOptions();
                        updateOptionStates();
                    }
                    isLoading = false;
                },
                error: function(xhr, status, error) {
                    $options.html('<div class="modern-multi-select-error">خطا در بارگذاری داده‌ها</div>');
                    isLoading = false;
                }
            });
        }
        
        // Render options
        function renderOptions() {
            $options.empty();
            allOptions.forEach(option => {
                const $option = $(`
                    <div class="modern-multi-select-option" data-value="${option.value}">
                        <div class="modern-multi-select-option-checkbox"></div>
                        <div class="modern-multi-select-option-text">${option.text}</div>
                    </div>
                `);
                $options.append($option);
            });
        }
        
        // Update hidden input
        function updateHiddenInput() {
            $hiddenInput.val(selectedValues.join(',')).trigger('change');
        }
        
        // Render selected items
        function renderSelectedItems() {
            $itemsContainer.empty();
            selectedValues.forEach(value => {
                const option = allOptions.find(opt => opt.value === value);
                if (option) {
                    const $item = $(`
                        <div class="modern-multi-select-item" data-value="${value}">
                            <span class="modern-multi-select-item-text">${option.text}</span>
                            <span class="modern-multi-select-item-remove">×</span>
                        </div>
                    `);
                    $itemsContainer.append($item);
                }
            });
        }
        
        // Update option states
        function updateOptionStates() {
            $options.find('.modern-multi-select-option').each(function() {
                const $option = $(this);
                const value = $option.data('value');
                const isSelected = selectedValues.includes(value);
                
                $option.toggleClass('selected', isSelected);
            });
        }
        
        // Toggle dropdown
        function toggleDropdown() {
            isOpen = !isOpen;
            $control.toggleClass('open', isOpen);
            $dropdown.toggle(isOpen);
            
            if (isOpen) {
                $searchInput.focus();
                if (allOptions.length === 0) {
                    loadData();
                } else {
                    updateOptionStates();
                }
            }
        }
        
        // Close dropdown
        function closeDropdown() {
            isOpen = false;
            $control.removeClass('open');
            $dropdown.hide();
            $searchInput.val('');
        }
        
        // Add item
        function addItem(value) {
            if (!selectedValues.includes(value)) {
                selectedValues.push(value);
                renderSelectedItems();
                updateOptionStates();
                updateHiddenInput();
            }
        }
        
        // Remove item
        function removeItem(value) {
            selectedValues = selectedValues.filter(v => v !== value);
            renderSelectedItems();
            updateOptionStates();
            updateHiddenInput();
        }
        
        // Filter options
        function filterOptions(searchTerm) {
            if (searchTerm && searchTerm.length > 0) {
                // Reload data with search term
                loadData();
            } else {
                // Show all options
                renderOptions();
                updateOptionStates();
            }
        }
        
        // Event handlers
        $input.on('click', function(e) {
            e.stopPropagation();
            toggleDropdown();
        });
        
        $arrow.on('click', function(e) {
            e.stopPropagation();
            toggleDropdown();
        });
        
        $searchInput.on('input', function() {
            const searchTerm = $(this).val();
            filterOptions(searchTerm);
        });
        
        $searchInput.on('keydown', function(e) {
            if (e.key === 'Escape') {
                closeDropdown();
            }
        });
        
        // Option click
        $options.on('click', '.modern-multi-select-option', function(e) {
            e.stopPropagation();
            const $option = $(this);
            const value = $option.data('value');
            
            if (selectedValues.includes(value)) {
                removeItem(value);
            } else {
                addItem(value);
            }
        });
        
        // Remove item click
        $itemsContainer.on('click', '.modern-multi-select-item-remove', function(e) {
            e.stopPropagation();
            const $item = $(this).closest('.modern-multi-select-item');
            const value = $item.data('value');
            removeItem(value);
        });
        
        // Close on outside click
        $(document).on('click', function(e) {
            if (!$control.is(e.target) && $control.has(e.target).length === 0) {
                closeDropdown();
            }
        });
        
        // Initialize
        renderSelectedItems();
    }
    
    // Initialize when document is ready
    $(document).ready(function() {
        console.log('Report Modern Multi-Select: Document ready');
        
        // Wait a bit to ensure all scripts are loaded
        setTimeout(function() {
            console.log('Report Modern Multi-Select: Checking for ModernMultiSelect class');
            console.log('ModernMultiSelect available:', typeof ModernMultiSelect !== 'undefined');
            console.log('Found controls:', $('.modern-multi-select-control').length);
            console.log('Found select elements:', $('.modern-filter-wrapper select[multiple]').length);
            
            if (typeof ModernMultiSelect !== 'undefined') {
                console.log('Report Modern Multi-Select: Initializing...');
                initializeModernMultiSelect();
            } else {
                console.log('Report Modern Multi-Select: ModernMultiSelect not found, retrying...');
                // ModernMultiSelect not found, retrying...
                setTimeout(function() {
                    console.log('Report Modern Multi-Select: Retry - ModernMultiSelect available:', typeof ModernMultiSelect !== 'undefined');
                    if (typeof ModernMultiSelect !== 'undefined') {
                        initializeModernMultiSelect();
                    } else {
                        console.error('Report Modern Multi-Select: ModernMultiSelect still not available after retry');
                    }
                }, 1000);
            }
        }, 100);
    });
    
    // Export for global access
    window.initializeReportModernMultiSelect = initializeModernMultiSelect;
    
})(jQuery);
