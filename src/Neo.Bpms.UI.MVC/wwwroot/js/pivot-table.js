/**
 * PivotTable - یک کامپوننت ساده و سبک برای ساخت Pivot Table سمت کلاینت
 * بدون وابستگی به کتابخانه‌های خارجی
 */
class PivotTable {
    constructor(containerId, options = {}) {
        this.container = document.getElementById(containerId);
        if (!this.container) {
            throw new Error(`Container with id "${containerId}" not found`);
        }

        this.options = {
            data: options.data || [],
            rowFields: options.rowFields || [],
            columnFields: options.columnFields || [],
            valueFields: options.valueFields || [],
            aggregator: options.aggregator || 'sum',
            locale: options.locale || 'fa-IR',
            direction: options.direction || 'rtl',
            ...options
        };

        this.pivotData = null;
        this.init();
    }

    init() {
        if (this.options.data.length === 0) {
            this.container.innerHTML = '<div class="pivot-empty">داده‌ای برای نمایش موجود نیست</div>';
            return;
        }

        if (this.options.rowFields.length === 0 && this.options.columnFields.length === 0) {
            this.container.innerHTML = '<div class="pivot-empty">لطفاً حداقل یک فیلد سطر یا ستون انتخاب کنید</div>';
            return;
        }

        if (this.options.valueFields.length === 0) {
            this.container.innerHTML = '<div class="pivot-empty">لطفاً حداقل یک فیلد مقدار انتخاب کنید</div>';
            return;
        }

        this.processData();
        this.render();
    }

    /**
     * پردازش داده‌ها و ساخت ساختار Pivot
     */
    processData() {
        const { data, rowFields, columnFields, valueFields, aggregator } = this.options;

        // ساختار Pivot: { rowKey: { colKey: { valueField: aggregatedValue } } }
        const pivot = {};

        if (!data || !Array.isArray(data) || data.length === 0) {
            this.pivotData = {};
            return;
        }

        data.forEach(row => {
            if (!row || typeof row !== 'object') return;
            
            const rowKey = this.buildKey(row, rowFields);
            const colKey = this.buildKey(row, columnFields);

            if (!pivot[rowKey]) {
                pivot[rowKey] = {};
            }
            if (!pivot[rowKey][colKey]) {
                pivot[rowKey][colKey] = {};
                valueFields.forEach(field => {
                    if (field) {
                        pivot[rowKey][colKey][field] = {
                            values: [],
                            count: 0
                        };
                    }
                });
            }

            valueFields.forEach(field => {
                if (field && pivot[rowKey][colKey][field]) {
                    const value = this.parseValue(row[field]);
                    if (value !== null && !isNaN(value)) {
                        pivot[rowKey][colKey][field].values.push(value);
                        pivot[rowKey][colKey][field].count++;
                    }
                }
            });
        });

        this.pivotData = this.aggregateData(pivot, aggregator);
    }

    buildKey(row, fields) {
        if (fields.length === 0) return 'Total';
        return fields.map(field => {
            const value = row[field];
            return value !== null && value !== undefined ? String(value) : '(خالی)';
        }).join('|');
    }

    parseValue(value) {
        if (value === null || value === undefined || value === '') return null;
        if (typeof value === 'number') return value;
        const parsed = parseFloat(String(value).replace(/,/g, ''));
        return isNaN(parsed) ? null : parsed;
    }

    aggregateData(pivot, aggregator) {
        const aggregated = {};
        
        Object.keys(pivot).forEach(rowKey => {
            aggregated[rowKey] = {};
            Object.keys(pivot[rowKey]).forEach(colKey => {
                aggregated[rowKey][colKey] = {};
                Object.keys(pivot[rowKey][colKey]).forEach(field => {
                    const data = pivot[rowKey][colKey][field];
                    // بررسی وجود data و values
                    if (data && data.values && Array.isArray(data.values)) {
                        aggregated[rowKey][colKey][field] = this.calculateAggregate(data.values, aggregator);
                    } else {
                        // اگر data یا values وجود نداشت، null برگردان
                        aggregated[rowKey][colKey][field] = aggregator.toLowerCase() === 'count' ? 0 : null;
                    }
                });
            });
        });

        return aggregated;
    }

    calculateAggregate(values, aggregator) {
        if (!values || !Array.isArray(values) || values.length === 0) {
            // برای count، حتی اگر values خالی باشد، باید 0 برگردانیم
            if (aggregator.toLowerCase() === 'count') {
                return 0;
            }
            return null;
        }

        switch (aggregator.toLowerCase()) {
            case 'sum':
                return values.reduce((a, b) => a + b, 0);
            case 'count':
                return values.length;
            case 'avg':
            case 'average':
                const sum = values.reduce((a, b) => a + b, 0);
                return sum / values.length;
            case 'min':
                return Math.min(...values);
            case 'max':
                return Math.max(...values);
            default:
                return values.reduce((a, b) => a + b, 0);
        }
    }

    /**
     * رندر جدول - بازنویسی کامل برای نمایش صحیح
     */
    render() {
        const { rowFields, columnFields, valueFields } = this.options;
        
        const rowKeys = [...new Set(Object.keys(this.pivotData))].sort();
        const colKeys = this.getAllColumnKeys();

        if (rowKeys.length === 0) {
            this.container.innerHTML = '<div class="pivot-empty">داده‌ای برای نمایش موجود نیست</div>';
            return;
        }

        // اگر ستون نداریم، یک ستون خالی ایجاد می‌کنیم
        if (colKeys.length === 0) {
            colKeys.push('Total');
        }

        let html = '<table class="pivot-table" dir="' + this.options.direction + '">';
        
        // ========== HEADER ==========
        html += '<thead>';
        
        // محاسبه تعداد سطرهای هدر
        // ساختار: corner + (columnFields row) + (valueFields row if > 1) + (colKeys row)
        let headerRowCount = 1; // corner همیشه وجود دارد
        let hasColumnFieldsRow = columnFields.length > 0;
        let hasValueFieldsRow = valueFields.length > 1;
        let hasColKeysRow = colKeys.length > 0 && (hasColumnFieldsRow || hasValueFieldsRow || colKeys.length > 1);
        
        if (hasColumnFieldsRow) headerRowCount++;
        if (hasValueFieldsRow) headerRowCount++;
        if (hasColKeysRow) headerRowCount++;
        
        // سطر اول: corner + column fields (اگر وجود داشته باشند)
        html += '<tr>';
        html += `<th class="pivot-corner" rowspan="${headerRowCount}"></th>`;
        
        if (columnFields.length > 0) {
            // برای هر column field، یک هدر با colspan مناسب
            const totalCols = colKeys.length * valueFields.length;
            let usedCols = 0;
            columnFields.forEach((field, index) => {
                let colspan;
                if (index === columnFields.length - 1) {
                    // آخرین فیلد: بقیه ستون‌ها
                    colspan = totalCols - usedCols;
                } else {
                    colspan = Math.floor(totalCols / columnFields.length);
                    usedCols += colspan;
                }
                html += `<th class="pivot-header" colspan="${colspan}">${this.getFieldLabel(field)}</th>`;
            });
        } else if (valueFields.length > 1) {
            // اگر column field نداریم اما value fields بیش از یک است
            html += `<th class="pivot-header" colspan="${colKeys.length * valueFields.length}">مقدار</th>`;
        } else {
            // فقط یک هدر کلی
            html += `<th class="pivot-header" colspan="${colKeys.length}">مقدار</th>`;
        }
        
        html += '</tr>';
        
        // سطر دوم: value fields (اگر بیش از یک باشد)
        if (valueFields.length > 1) {
            html += '<tr>';
            valueFields.forEach(field => {
                html += `<th class="pivot-value-header" colspan="${colKeys.length}">${this.getFieldLabel(field)}</th>`;
            });
            html += '</tr>';
        }
        
        // سطر آخر: colKeys (مقادیر ستون‌ها)
        if (hasColKeysRow) {
            html += '<tr>';
            valueFields.forEach(() => {
                colKeys.forEach(colKey => {
                    html += `<th class="pivot-col-value">${this.formatKey(colKey)}</th>`;
                });
            });
            html += '</tr>';
        } else if (colKeys.length > 0 && !hasColumnFieldsRow && !hasValueFieldsRow) {
            // اگر فقط colKeys داریم و column/value field نداریم
            html += '<tr>';
            colKeys.forEach(colKey => {
                html += `<th class="pivot-col-value">${this.formatKey(colKey)}</th>`;
            });
            html += '</tr>';
        }
        
        html += '</thead>';
        
        // ========== BODY ==========
        html += '<tbody>';
        
        rowKeys.forEach(rowKey => {
            const rowSpan = valueFields.length;
            
            valueFields.forEach((valueField, vfIndex) => {
                html += '<tr>';
                
                // هدر سطر (فقط در اولین value field)
                if (vfIndex === 0) {
                    html += `<th class="pivot-row-header" rowspan="${rowSpan}">${this.formatKey(rowKey)}</th>`;
                }
                
                // هدر value field (اگر بیش از یک باشد)
                if (valueFields.length > 1) {
                    html += `<th class="pivot-value-label">${this.getFieldLabel(valueField)}</th>`;
                }
                
                // مقادیر برای هر ستون
                colKeys.forEach(colKey => {
                    const value = this.pivotData[rowKey]?.[colKey]?.[valueField] ?? null;
                    html += `<td class="pivot-value">${this.formatValue(value)}</td>`;
                });
                
                html += '</tr>';
            });
        });
        
        html += '</tbody>';
        html += '</table>';

        this.container.innerHTML = html;
        this.applyStyles();
    }

    getAllColumnKeys() {
        if (!this.pivotData || Object.keys(this.pivotData).length === 0) {
            return ['Total'];
        }
        
        const colKeys = new Set();
        Object.values(this.pivotData).forEach(row => {
            if (row && typeof row === 'object') {
                Object.keys(row).forEach(colKey => {
                    if (colKey) colKeys.add(colKey);
                });
            }
        });
        
        const result = [...colKeys].sort();
        return result.length > 0 ? result : ['Total'];
    }

    formatKey(key) {
        if (key === 'Total') return 'جمع کل';
        return key.split('|').join(' - ');
    }

    formatValue(value) {
        if (value === null || value === undefined) return '—';
        if (typeof value === 'number') {
            return value.toLocaleString(this.options.locale, {
                minimumFractionDigits: 0,
                maximumFractionDigits: 2
            });
        }
        return String(value);
    }

    getFieldLabel(field) {
        if (typeof field === 'object' && field.label) {
            return field.label;
        }
        if (typeof field === 'object' && field.name) {
            return field.name;
        }
        return String(field);
    }

    applyStyles() {
        if (!document.getElementById('pivot-table-styles')) {
            const style = document.createElement('style');
            style.id = 'pivot-table-styles';
            style.textContent = `
                .pivot-table {
                    width: 100%;
                    border-collapse: collapse;
                    font-size: 0.9rem;
                    direction: rtl;
                    background: #ffffff;
                }
                .pivot-table th,
                .pivot-table td {
                    border: 1px solid #e2e8f0;
                    padding: 10px 12px;
                    text-align: center;
                    vertical-align: middle;
                }
                .pivot-table th {
                    background-color: #f8fafc;
                    font-weight: 600;
                    position: sticky;
                }
                .pivot-corner {
                    background-color: #e0e0e0 !important;
                    z-index: 10;
                }
                .pivot-header {
                    background-color: #3b82f6 !important;
                    color: #ffffff !important;
                    font-weight: 700;
                    position: sticky;
                    top: 0;
                    z-index: 5;
                }
                .pivot-value-header {
                    background-color: #10b981 !important;
                    color: #ffffff !important;
                    font-weight: 700;
                    position: sticky;
                    top: 0;
                    z-index: 5;
                }
                .pivot-row-header {
                    background-color: #f59e0b !important;
                    color: #ffffff !important;
                    font-weight: 700;
                    text-align: right;
                    position: sticky;
                    right: 0;
                    z-index: 4;
                }
                .pivot-value-label {
                    background-color: #6b7280 !important;
                    color: #ffffff !important;
                    font-weight: 600;
                }
                .pivot-col-value {
                    background-color: #f1f5f9 !important;
                    font-weight: 500;
                }
                .pivot-value {
                    background-color: #ffffff;
                    font-weight: 500;
                }
                .pivot-value:hover {
                    background-color: #f0f9ff;
                }
                .pivot-empty {
                    text-align: center;
                    padding: 40px;
                    color: #94a3b8;
                    font-size: 1rem;
                }
            `;
            document.head.appendChild(style);
        }
    }

    updateData(data) {
        this.options.data = data;
        this.init();
    }

    setDimensions(rowFields, columnFields, valueFields) {
        this.options.rowFields = rowFields;
        this.options.columnFields = columnFields;
        this.options.valueFields = valueFields;
        this.init();
    }

    setAggregator(aggregator) {
        this.options.aggregator = aggregator;
        this.init();
    }
}

if (typeof module !== 'undefined' && module.exports) {
    module.exports = PivotTable;
}
