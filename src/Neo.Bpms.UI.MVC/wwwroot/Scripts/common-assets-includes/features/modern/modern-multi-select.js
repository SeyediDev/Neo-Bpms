/**
 * Modern Multi-Select Control - Neo BPMS
 * Version: 1.0.0
 * A beautiful, modern multi-select control for reference fields
 */

class ModernMultiSelect {
    constructor(element, options = {}) {
        this.element = element;
        this.options = {
            placeholder: 'انتخاب کنید...',
            searchPlaceholder: 'جستجو...',
            noResultsText: 'نتیجه‌ای یافت نشد',
            loadingText: 'در حال بارگذاری...',
            allowSearch: true,
            allowClear: true,
            maxSelections: null,
            ajax: null,
            data: [],
            valueField: 'value',
            textField: 'text',
            ...options
        };
        
        this.selectedItems = [];
        this.isOpen = false;
        this.isLoading = false;
        this.searchTerm = '';
        
        this.init();
    }
    
    init() {
        this.createHTML();
        this.bindEvents();
        this.loadData();
    }
    
    createHTML() {
        // Create the main container
        this.element.innerHTML = `
            <div class="modern-multi-select">
                <div class="modern-multi-select-input" tabindex="0">
                    <div class="modern-multi-select-items"></div>
                    <input type="text" class="modern-multi-select-search" 
                           placeholder="${this.options.searchPlaceholder}" 
                           autocomplete="off">
                    <div class="modern-multi-select-arrow">
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                            <path d="M8 11L3 6h10l-5 5z"/>
                        </svg>
                    </div>
                </div>
                <div class="modern-multi-select-dropdown">
                    <div class="modern-multi-select-options"></div>
                </div>
            </div>
        `;
        
        this.container = this.element.querySelector('.modern-multi-select');
        this.input = this.element.querySelector('.modern-multi-select-input');
        this.searchInput = this.element.querySelector('.modern-multi-select-search');
        this.itemsContainer = this.element.querySelector('.modern-multi-select-items');
        this.dropdown = this.element.querySelector('.modern-multi-select-dropdown');
        this.optionsContainer = this.element.querySelector('.modern-multi-select-options');
        this.arrow = this.element.querySelector('.modern-multi-select-arrow');
    }
    
    bindEvents() {
        // Toggle dropdown
        this.input.addEventListener('click', (e) => {
            e.stopPropagation();
            this.toggle();
        });
        
        // Search functionality
        this.searchInput.addEventListener('input', (e) => {
            this.searchTerm = e.target.value;
            this.filterOptions();
        });
        
        // Close dropdown when clicking outside
        document.addEventListener('click', (e) => {
            if (!this.container.contains(e.target)) {
                this.close();
            }
        });
        
        // Keyboard navigation
        this.input.addEventListener('keydown', (e) => {
            this.handleKeydown(e);
        });
        
        // Prevent form submission on Enter
        this.searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
            }
        });
    }
    
    handleKeydown(e) {
        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                this.open();
                this.focusFirstOption();
                break;
            case 'ArrowUp':
                e.preventDefault();
                this.open();
                this.focusLastOption();
                break;
            case 'Escape':
                this.close();
                this.input.focus();
                break;
            case 'Tab':
                this.close();
                break;
        }
    }
    
    toggle() {
        if (this.isOpen) {
            this.close();
        } else {
            this.open();
        }
    }
    
    open() {
        if (this.isOpen) return;
        
        this.isOpen = true;
        this.container.classList.add('open');
        this.searchInput.focus();
        this.renderOptions();
    }
    
    close() {
        if (!this.isOpen) return;
        
        this.isOpen = false;
        this.container.classList.remove('open');
        this.searchTerm = '';
        this.searchInput.value = '';
    }
    
    async loadData() {
        if (this.options.ajax) {
            this.setLoading(true);
            try {
                const response = await this.options.ajax();
                this.options.data = response;
                this.renderOptions();
            } catch (error) {
                console.error('Error loading data:', error);
                this.showError('خطا در بارگذاری داده‌ها');
            } finally {
                this.setLoading(false);
            }
        } else {
            this.renderOptions();
        }
    }
    
    setLoading(loading) {
        this.isLoading = loading;
        if (loading) {
            this.optionsContainer.innerHTML = `
                <div class="modern-multi-select-loading">
                    ${this.options.loadingText}
                </div>
            `;
        }
    }
    
    showError(message) {
        this.optionsContainer.innerHTML = `
            <div class="modern-multi-select-empty">
                ${message}
            </div>
        `;
    }
    
    filterOptions() {
        if (!this.searchTerm) {
            this.renderOptions();
            return;
        }
        
        const filtered = this.options.data.filter(item => {
            const text = item[this.options.textField].toLowerCase();
            return text.includes(this.searchTerm.toLowerCase());
        });
        
        this.renderOptions(filtered);
    }
    
    renderOptions(data = this.options.data) {
        if (this.isLoading) return;
        
        if (!data || data.length === 0) {
            this.optionsContainer.innerHTML = `
                <div class="modern-multi-select-empty">
                    ${this.searchTerm ? this.options.noResultsText : this.options.placeholder}
                </div>
            `;
            return;
        }
        
        const html = data.map(item => {
            const isSelected = this.selectedItems.some(selected => 
                selected[this.options.valueField] === item[this.options.valueField]
            );
            
            return `
                <div class="modern-multi-select-option ${isSelected ? 'selected' : ''}" 
                     data-value="${item[this.options.valueField]}">
                    <div class="modern-multi-select-option-checkbox"></div>
                    <div class="modern-multi-select-option-text">${item[this.options.textField]}</div>
                </div>
            `;
        }).join('');
        
        this.optionsContainer.innerHTML = html;
        
        // Bind option click events
        this.optionsContainer.querySelectorAll('.modern-multi-select-option').forEach(option => {
            option.addEventListener('click', (e) => {
                e.stopPropagation();
                this.toggleOption(option);
            });
        });
    }
    
    toggleOption(optionElement) {
        const value = optionElement.dataset.value;
        const item = this.options.data.find(item => 
            item[this.options.valueField] == value
        );
        
        if (!item) return;
        
        const isSelected = this.selectedItems.some(selected => 
            selected[this.options.valueField] === item[this.options.valueField]
        );
        
        if (isSelected) {
            this.removeItem(item);
        } else {
            this.addItem(item);
        }
    }
    
    addItem(item) {
        if (this.options.maxSelections && this.selectedItems.length >= this.options.maxSelections) {
            return;
        }
        
        this.selectedItems.push(item);
        this.renderSelectedItems();
        this.renderOptions();
        this.triggerChange();
    }
    
    removeItem(item) {
        this.selectedItems = this.selectedItems.filter(selected => 
            selected[this.options.valueField] !== item[this.options.valueField]
        );
        this.renderSelectedItems();
        this.renderOptions();
        this.triggerChange();
    }
    
    renderSelectedItems() {
        const html = this.selectedItems.map(item => `
            <div class="modern-multi-select-item">
                <span class="modern-multi-select-item-text">${item[this.options.textField]}</span>
                <span class="modern-multi-select-item-remove" data-value="${item[this.options.valueField]}">×</span>
            </div>
        `).join('');
        
        this.itemsContainer.innerHTML = html;
        
        // Bind remove events
        this.itemsContainer.querySelectorAll('.modern-multi-select-item-remove').forEach(removeBtn => {
            removeBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                const value = removeBtn.dataset.value;
                const item = this.selectedItems.find(item => 
                    item[this.options.valueField] == value
                );
                if (item) {
                    this.removeItem(item);
                }
            });
        });
    }
    
    focusFirstOption() {
        const firstOption = this.optionsContainer.querySelector('.modern-multi-select-option');
        if (firstOption) {
            firstOption.focus();
        }
    }
    
    focusLastOption() {
        const options = this.optionsContainer.querySelectorAll('.modern-multi-select-option');
        const lastOption = options[options.length - 1];
        if (lastOption) {
            lastOption.focus();
        }
    }
    
    triggerChange() {
        const event = new CustomEvent('change', {
            detail: {
                selectedItems: this.selectedItems,
                values: this.selectedItems.map(item => item[this.options.valueField])
            }
        });
        this.element.dispatchEvent(event);
    }
    
    // Public methods
    getValue() {
        return this.selectedItems.map(item => item[this.options.valueField]);
    }
    
    getSelectedItems() {
        return [...this.selectedItems];
    }
    
    setValue(values) {
        this.selectedItems = this.options.data.filter(item => 
            values.includes(item[this.options.valueField])
        );
        this.renderSelectedItems();
        this.renderOptions();
    }
    
    clear() {
        this.selectedItems = [];
        this.renderSelectedItems();
        this.renderOptions();
        this.triggerChange();
    }
    
    destroy() {
        this.element.innerHTML = '';
        this.element.removeAttribute('data-modern-multi-select');
    }
}

// jQuery plugin wrapper
if (typeof jQuery !== 'undefined') {
    jQuery.fn.modernMultiSelect = function(options) {
        return this.each(function() {
            if (!this.dataset.modernMultiSelect) {
                this.dataset.modernMultiSelect = 'true';
                new ModernMultiSelect(this, options);
            }
        });
    };
}

// Auto-initialize elements with data-modern-multi-select attribute
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('[data-modern-multi-select]').forEach(element => {
        const options = JSON.parse(element.dataset.modernMultiSelectOptions || '{}');
        new ModernMultiSelect(element, options);
    });
});

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ModernMultiSelect;
}
