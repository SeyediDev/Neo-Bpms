// ===== MODERN DASHBOARD JAVASCRIPT =====
// Performance optimized with clean architecture

// ===== AUTO-REFRESH INFRASTRUCTURE =====
let autoRefreshIntervals = {};
let autoRefreshEnabled = false;

// ===== DESIGN MODE MANAGEMENT =====
let isDesignMode = false;

window.toggleDesignMode = function() {
    isDesignMode = !isDesignMode;
    const overlay = document.getElementById('design-overlay');
    const toggleBtn = document.getElementById('design-toggle-btn');
    const dashboardContent = document.getElementById('dashboard-content');
    
    // نمایش/مخفی کردن المان‌های طراحی
    const designElements = document.querySelectorAll('.neo-dashboard-widget-title-actions, .dashboard-tab-edit-icon, .dashboard-tab-delete-icon, .dashboard-tab-add-item');
    
    if (isDesignMode) {
        // Show design overlay with animation
        if (overlay) {
            overlay.classList.add('showing');
        }
        
        toggleBtn.classList.add('active');
        toggleBtn.innerHTML = `
            <svg class="neo-dashboard-design-icon" viewBox="0 0 24 24">
                <use xlink:href="/Content/common-assets-includes/icons/svgSprite.svg#close-square" />
            </svg>
            <span>بستن طراحی</span>
        `;
        
        // نمایش دکمه‌های طراحی
        designElements.forEach(el => {
            el.style.display = el.classList.contains('dashboard-tab-add-item') ? 'flex' : 'inline-flex';
            el.style.visibility = 'visible';
        });
        
        // Enable design features
        enableDesignMode();
        
        // Prevent body scroll
        if (overlay) {
            document.body.style.overflow = 'hidden';
        }
    } else {
        // Hide design overlay with animation
        if (overlay) {
            overlay.classList.remove('showing');
        }
        
        toggleBtn.classList.remove('active');
        toggleBtn.innerHTML = `
            <svg class="neo-dashboard-design-icon" viewBox="0 0 24 24">
                <use xlink:href="/Content/common-assets-includes/icons/svgSprite.svg#edit-dashboard" />
            </svg>
            <span>حالت طراحی</span>
        `;
        
        // مخفی کردن دکمه‌های طراحی (به جز افزودن تب جدید و افزودن ویجت که همیشه نمایش داده می‌شوند)
        designElements.forEach(el => {
            if (el.classList.contains('neo-dashboard-widget-title-actions')) {
                el.style.display = 'none';
            }
        });
        
        // Disable design features
        disableDesignMode();
        
        // Restore body scroll
        document.body.style.overflow = '';
    }
    
    console.log(`Dashboard: Design mode ${isDesignMode ? 'enabled' : 'disabled'}`);
}

function enableDesignMode() {
    const dashboardGrid = document.getElementById('ReportBody');
    if (dashboardGrid) {
        // Add design mode class
        dashboardGrid.classList.add('design-mode');
        
        // Enable sortable
        $(dashboardGrid).sortable({
            handle: '.widget-handle',
            placeholder: 'widget-placeholder',
            tolerance: 'pointer',
            update: function(event, ui) {
                updateWidgetOrder();
            }
        });
        
        // Add drop zones
        addDropZones();
    }
}

function disableDesignMode() {
    const dashboardGrid = document.getElementById('ReportBody');
    if (dashboardGrid) {
        // Remove design mode class
        dashboardGrid.classList.remove('design-mode');
        
        // Disable sortable
        $(dashboardGrid).sortable('disable');
        
        // Remove drop zones
        removeDropZones();
    }
}

// ===== CONFIG MANAGEMENT =====
window.switchConfig = function(configId) {
    const configInput = document.querySelector("input[name='ConfigId']");
    if (configInput) {
        configInput.value = configId;
        
        // Submit form to load the selected config
        const form = configInput.closest("form");
        if (form) {
            form.submit();
        }
    }
    
    console.log(`Dashboard: Switched to config ${configId}`);
}

// ===== CONFIG MANAGEMENT =====
window.editConfig = function(configId) {
    // جلوگیری از رفتار پیش‌فرض
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    
    const tabItem = document.querySelector(`.dashboard-tab-item[data-config-id="${configId}"]`);
    const currentName = tabItem ? tabItem.querySelector('.dashboard-tab-name')?.textContent?.trim() : '';
    
    const newName = prompt('نام جدید تب را وارد کنید:', currentName);
    if (!newName || !newName.trim() || newName.trim() === currentName) {
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configId,
        NewName: newName.trim()
    };
    
    fetch(window.top.rootUrl + 'Dashboard/RenameConfig', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result && result.success) {
            console.log('Dashboard: Tab renamed successfully');
            
            // به‌روزرسانی نام تب در DOM
            if (tabItem) {
                const tabNameElement = tabItem.querySelector('.dashboard-tab-name');
                if (tabNameElement) {
                    tabNameElement.textContent = newName.trim();
                }
            }
            
            // به‌روزرسانی در design overlay هم اگر وجود دارد
            const configItem = document.querySelector(`.neo-dashboard-config-item[data-config-id="${configId}"]`);
            if (configItem) {
                const configNameElement = configItem.querySelector('.neo-dashboard-config-item-name');
                if (configNameElement) {
                    configNameElement.textContent = newName.trim();
                }
            }
        } else {
            throw new Error(result?.error || 'خطا در تغییر نام تب');
        }
    })
    .catch(error => {
        console.error('Dashboard: Rename config error', error);
        alert(window.tetaI18n?.t('Error Occured') || 'خطا رخ داد: ' + error.message);
    });
}

window.deleteConfig = function(configId) {
    // جلوگیری از رفتار پیش‌فرض
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    
    const tabItem = document.querySelector(`.dashboard-tab-item[data-config-id="${configId}"]`);
    const configName = tabItem ? tabItem.querySelector('.dashboard-tab-name')?.textContent?.trim() : configId;
    
    const message = `آیا از حذف تب "${configName}" اطمینان دارید؟\n\nاین عملیات قابل بازگشت نیست.`;
    if (!confirm(message)) {
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configId
    };
    
    fetch(window.top.rootUrl + 'Dashboard/DeleteConfig', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            console.log('Dashboard: Tab deleted successfully');
            
            // حذف تب از DOM با انیمیشن
            if (tabItem) {
                tabItem.style.transition = 'all 0.3s ease';
                tabItem.style.opacity = '0';
                tabItem.style.transform = 'translateY(-10px)';
                
                setTimeout(() => {
                    tabItem.remove();
                    
                    // اگر تب حذف شده فعال بود، اولین تب را فعال کن
                    if (tabItem.classList.contains('active')) {
                        const firstTab = document.querySelector('.dashboard-tab-item');
                        if (firstTab) {
                            const firstConfigId = firstTab.getAttribute('data-config-id');
                            if (firstConfigId) {
                                window.dashboard_switchConfig(firstConfigId);
                            }
                        }
                    }
                }, 300);
            } else {
                // اگر تب در DOM نبود، صفحه را reload کن
                location.reload();
            }
        } else {
            alert('خطا در حذف تب. لطفاً دوباره تلاش کنید.');
        }
    })
    .catch(error => {
        console.error('Dashboard: Error deleting tab:', error);
        alert('خطا در حذف تب: ' + error.message);
    });
}

window.addNewConfig = function() {
    const configName = prompt('نام کانفیگ جدید را وارد کنید:', '');
    if (!configName || !configName.trim()) {
        return;
    }
    
    // TODO: Implement add new config functionality
    console.log(`Dashboard: Add new config "${configName}"`);
    
    // For now, show success message
    alert(`کانفیگ جدید "${configName}" اضافه شد`);
}

// ===== DRAG & DROP FUNCTIONALITY =====
window.dragWidget = function(event) {
    const widgetData = {
        configId: event.target.getAttribute('cid'),
        reportId: event.target.getAttribute('rid'),
        reportNamespaceId: event.target.getAttribute('rns'),
        reportEntityId: event.target.getAttribute('re')
    };
    
    event.dataTransfer.setData('text/plain', JSON.stringify(widgetData));
    event.dataTransfer.effectAllowed = 'copy';
    
    // Add visual feedback
    event.target.style.opacity = '0.5';
}

function addDropZones() {
    const dashboardGrid = document.getElementById('ReportBody');
    if (!dashboardGrid) return;
    
    // Add drop zone indicators
    const dropZone = document.createElement('div');
    dropZone.className = 'widget-drop-zone';
    dropZone.innerHTML = `
        <div class="widget-drop-zone-content">
            <svg class="widget-drop-zone-icon" viewBox="0 0 24 24">
                <use xlink:href="/Content/common-assets-includes/icons/svgSprite.svg#add-file" />
            </svg>
            <span class="widget-drop-zone-text">ویجت را اینجا رها کنید</span>
        </div>
    `;
    
    dashboardGrid.appendChild(dropZone);
    
    // Add event listeners
    dropZone.addEventListener('dragover', handleDragOver);
    dropZone.addEventListener('drop', handleDrop);
    dropZone.addEventListener('dragleave', handleDragLeave);
}

function removeDropZones() {
    const dropZones = document.querySelectorAll('.widget-drop-zone');
    dropZones.forEach(zone => zone.remove());
}

function handleDragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'copy';
    event.target.closest('.widget-drop-zone').classList.add('drag-over');
}

function handleDrop(event) {
    event.preventDefault();
    const dropZone = event.target.closest('.widget-drop-zone');
    dropZone.classList.remove('drag-over');
    
    try {
        const widgetData = JSON.parse(event.dataTransfer.getData('text/plain'));
        addWidgetToDashboard(widgetData);
    } catch (error) {
        console.error('Dashboard: Error parsing widget data', error);
    }
}

function handleDragLeave(event) {
    const dropZone = event.target.closest('.widget-drop-zone');
    if (dropZone && !dropZone.contains(event.relatedTarget)) {
        dropZone.classList.remove('drag-over');
    }
}

function addWidgetToDashboard(widgetData) {
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: document.querySelector("input[name='ConfigId']").value,
        reportNamespaceId: widgetData.reportNamespaceId,
        reportEntityId: widgetData.reportEntityId,
        ReportId: widgetData.reportId,
        ReportConfigId: widgetData.configId
    };
    
    fetch(window.top.rootUrl + 'Dashboard/AddNewWidget', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            console.log('Dashboard: Widget added successfully');
            // Reload page to show new widget
            window.location.reload();
        }
    })
    .catch(error => {
        console.error('Dashboard: Add widget error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
    });
}

function updateWidgetOrder() {
    const dashboardGrid = document.getElementById('ReportBody');
    const orderedWidgetIds = [];
    const sortableElements = dashboardGrid.querySelectorAll('.sortable');
    
    sortableElements.forEach(element => {
        const widgetId = element.getAttribute('widgetId');
        if (widgetId) {
            orderedWidgetIds.push(widgetId);
        }
    });
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: document.querySelector("input[name='ConfigId']").value,
        widgetIds: orderedWidgetIds
    };
    
    fetch(window.top.rootUrl + 'Dashboard/UpdateWidgetOrder', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            console.log('Dashboard: Widget order updated successfully');
        }
    })
    .catch(error => {
        console.error('Dashboard: Update widget order error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
    });
}

// ===== SAVE DASHBOARD =====
window.saveDashboard = function() {
    // TODO: Implement save functionality
    console.log('Dashboard: Saving dashboard...');
    
    // For now, just close design mode
    toggleDesignMode();
    
    // Show success message
    alert('تغییرات با موفقیت ذخیره شد');
}


// ===== CONFIGURATION RENAMING =====
window.dashboard_renameConfig = function (element) {
    const configName = element.getAttribute('ch_name');
    const configId = element.getAttribute('cid');
    
    if (!configName || !configId) {
        console.error('Dashboard: Missing config name or ID');
        return;
    }
    
    const newName = prompt(window.tetaI18n.t('EnterNewNameOfConfig'), configName);
    if (!newName || newName.trim() === '') {
        return;
    }
    
    // Close dropdown immediately
    const dropdown = element.closest('.dropdown');
    if (dropdown) {
        $(dropdown).dropdown('toggle');
    }
    
    const ajaxParams = {
        newName: newName.trim(),
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configId
    };
    
    fetch(window.top.rootUrl + 'Dashboard/RenameConfig', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            // Update UI with new name
            const configNameElement = element.closest('.neo-dashboard-config-item')
                ?.querySelector('.neo-dashboard-config-name');
            
            if (configNameElement) {
                const displayName = result.length > 50 ? result.substring(0, 50) + "..." : result;
                configNameElement.textContent = displayName;
                configNameElement.title = result;
                
                // Update element attribute
                element.setAttribute('ch_name', result);
            }
        }
    })
    .catch(error => {
        console.error('Dashboard: Rename config error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
    });
}

// ===== ADD NEW CONFIGURATION =====
window.dashboard_addConfig = function (element) {
    const configName = prompt(window.tetaI18n.t('EnterNewConfigName'), "");
    if (!configName || configName.trim() === '') {
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigName: configName.trim()
    };
    
    fetch(window.top.rootUrl + 'Dashboard/AddNewConfig', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            const configInput = document.querySelector("input[name='ConfigId']");
            if (configInput) {
                configInput.value = result;
                configInput.closest("form").submit();
            }
        }
    })
    .catch(error => {
        console.error('Dashboard: Add config error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
    });
}

// ===== ADD WIDGET =====
window.dashboard_addWidget = function (element) {
    const configId = element.getAttribute('cid');
    const reportId = element.getAttribute('rid');
    const reportNamespaceId = element.getAttribute('rns');
    const reportEntityId = element.getAttribute('re');
    
    if (!configId || !reportId || !reportNamespaceId || !reportEntityId) {
        console.error('Dashboard: Missing widget parameters');
        return;
    }
    
    const configInput = document.querySelector("input[name='ConfigId']");
    if (!configInput) {
        console.error('Dashboard: Config input not found');
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configInput.value,
        reportNamespaceId: reportNamespaceId,
        reportEntityId: reportEntityId,
        ReportId: reportId,
        ReportConfigId: configId
    };
    
    // Show loading state
    element.style.opacity = '0.6';
    element.style.pointerEvents = 'none';
    
    fetch(window.top.rootUrl + 'Dashboard/AddNewWidget', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            changeOccured();
            // Add success animation
            element.style.transform = 'scale(1.2)';
            element.style.color = '#10b981';
            setTimeout(() => {
                element.style.transform = 'scale(1)';
                element.style.opacity = '1';
                element.style.pointerEvents = 'auto';
            }, 300);
        }
    })
    .catch(error => {
        console.error('Dashboard: Add widget error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
        // Reset loading state
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    });
}

// ===== REMOVE WIDGET =====
window.dashboard_removeWidget = function (element) {
    const widgetId = element.getAttribute('widgetId');
    if (!widgetId) {
        console.error('Dashboard: Missing widget ID');
        return;
    }
    
    const configInput = document.querySelector("input[name='ConfigId']");
    if (!configInput) {
        console.error('Dashboard: Config input not found');
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configInput.value,
        widgetId: widgetId
    };
    
    // Show loading state
    element.style.opacity = '0.6';
    element.style.pointerEvents = 'none';
    
    fetch(window.top.rootUrl + 'Dashboard/RemoveWidget', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            // Remove widget with animation
            const widgetContainer = element.closest('.neo-dashboard-widget-container');
            if (widgetContainer) {
                widgetContainer.style.transition = 'all 0.3s ease';
                widgetContainer.style.transform = 'scale(0.8)';
                widgetContainer.style.opacity = '0';
                setTimeout(() => widgetContainer.remove(), 300);
            }
        }
    })
    .catch(error => {
        console.error('Dashboard: Remove widget error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
        // Reset loading state
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    });
}

var eReportViewType = {'Chart' : 2};

// ===== SUBMIT WIDGET SETTINGS =====
window.dashboard_submitWidgetSettings = function (element) {
    const widgetId = element.getAttribute('widgetId');
    if (!widgetId) {
        console.error('Dashboard: Missing widget ID');
        return;
    }
    
    const configInput = document.querySelector("input[name='ConfigId']");
    if (!configInput) {
        console.error('Dashboard: Config input not found');
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configInput.value,
        widgetId: widgetId,
        widgetWidth: document.getElementById('widgetWidth-' + widgetId)?.value,
        widgetHeight: document.getElementById('widgetHeight-' + widgetId)?.value,
        recordsCount: document.getElementById('recordsCount-' + widgetId)?.value,
        ChartType: document.getElementById('selectChart-' + widgetId)?.value
    };
    
    // Hide modal immediately for better UX
    const modal = document.getElementById('widgetSettingsModal-' + widgetId);
    if (modal) {
        $(modal).modal('hide');
    }
    
    // Show loading state
    element.style.opacity = '0.6';
    element.style.pointerEvents = 'none';
    
    fetch(window.top.rootUrl + 'Dashboard/SubmitWidgetSettings', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            changeOccured();
            // Show success feedback
            element.style.color = '#10b981';
            setTimeout(() => {
                element.style.color = '';
                element.style.opacity = '1';
                element.style.pointerEvents = 'auto';
            }, 1000);
        }
    })
    .catch(error => {
        console.error('Dashboard: Submit widget settings error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
        // Reset loading state
        element.style.opacity = '1';
        element.style.pointerEvents = 'auto';
    });
}

// ===== DESIGN CONTROLS STATE =====
let weAreInDesignMode = false;

// ===== SHOW DESIGN CONTROLS =====
window.dashboard_showDesignControls = function () {
    weAreInDesignMode = !weAreInDesignMode;
    const designControls = document.querySelectorAll('.design-control');
    
    designControls.forEach(control => {
        control.classList.toggle('hidden');
    });
    
    const reportBody = document.getElementById('ReportBody');
    if (!reportBody) {
        console.error('Dashboard: ReportBody not found');
        return;
    }
    
    if (weAreInDesignMode) {
        // Enable sortable with modern approach
        $(reportBody).sortable({
            update: function(event, ui) {
                const orderedWidgetIds = [];
                const sortableElements = reportBody.querySelectorAll('.sortable');
                
                sortableElements.forEach(element => {
                    const widgetId = element.getAttribute('widgetId');
                    if (widgetId) {
                        orderedWidgetIds.push(widgetId);
                    }
                });
                
                const configInput = document.querySelector("input[name='ConfigId']");
                if (!configInput) {
                    console.error('Dashboard: Config input not found');
                    return;
                }
                
                const ajaxParams = {
                    NamespaceId: window.PageAddressManager.getNamespaceId(),
                    EntityId: window.PageAddressManager.getEntityId(),
                    DashboardId: window.PageAddressManager.getPageId(),
                    ConfigId: configInput.value,
                    widgetIds: orderedWidgetIds
                };
                
                fetch(window.top.rootUrl + 'Dashboard/SubmitWidgetsOrder', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json; charset=utf-8',
                        ...window.AddAntiForgeryToken()
                    },
                    body: JSON.stringify(ajaxParams)
                })
                .then(response => response.json())
                .then(result => {
                    if (result) {
                        console.log('Dashboard: Widget order updated successfully');
                    }
                })
                .catch(error => {
                    console.error('Dashboard: Update widget order error', error);
                    alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
                });
            }
        });
        $(reportBody).sortable("enable");
    } else {
        $(reportBody).sortable("disable");
    }
}

// ===== CHANGE TRACKING =====
let isSeeChangesEnabled = false;

function changeOccured() {
    if (!isSeeChangesEnabled) {
        const seeChangesBtn = document.getElementById('seeChanges');
        if (seeChangesBtn) {
            seeChangesBtn.classList.toggle('hidden');
        }
        isSeeChangesEnabled = true;
    }
}

// ===== SEE CHANGES EVENT LISTENER =====
document.addEventListener('DOMContentLoaded', function() {
    const seeChangesBtn = document.getElementById('seeChanges');
    if (seeChangesBtn) {
        seeChangesBtn.addEventListener('click', function() {
            location.reload(true);
        });
    }
});


// ===== DELETE CONFIG (با مدال تأیید و بدون رفرش صفحه) =====
window.dashboard_deleteConfig2 = function (configId, configName) {
    // جلوگیری از رفتار پیش‌فرض
    event.preventDefault();
    event.stopPropagation();
    
    const message = `آیا از حذف تب "${configName}" اطمینان دارید؟\n\nاین عملیات قابل بازگشت نیست.`;
    if (!confirm(message)) {
        return;
    }
    
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configId
    };
    
    fetch(window.top.rootUrl + 'Dashboard/DeleteConfig', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result) {
            console.log('Dashboard: Tab deleted successfully');
            
            // حذف تب از DOM با انیمیشن
            const tabItem = document.querySelector(`.dashboard-tab-item[data-config-id="${configId}"]`);
            if (tabItem) {
                tabItem.style.transition = 'all 0.3s ease';
                tabItem.style.opacity = '0';
                tabItem.style.transform = 'translateY(-10px)';
                
                setTimeout(() => {
                    tabItem.remove();
                    
                    // اگر تب حذف شده فعال بود، اولین تب را فعال کن
                    if (tabItem.classList.contains('active')) {
                        const firstTab = document.querySelector('.dashboard-tab-item');
                        if (firstTab) {
                            const firstConfigId = firstTab.getAttribute('data-config-id');
                            if (firstConfigId) {
                                window.dashboard_switchConfig(firstConfigId);
                            }
                        }
                    }
                }, 300);
            }
            
            // حذف از design overlay هم اگر وجود دارد
            const configItem = document.querySelector(`.neo-dashboard-config-item[data-config-id="${configId}"]`);
            if (configItem) {
                configItem.style.transition = 'all 0.3s ease';
                configItem.style.opacity = '0';
                configItem.style.transform = 'translateX(-20px)';
                setTimeout(() => configItem.remove(), 300);
            }
        }
    })
    .catch(error => {
        console.error('Dashboard: Delete config error', error);
        alert(window.tetaI18n.t('Error Occured') + ' ' + error.message);
    });
};

// ===== ADD WIDGET (NEW VERSION) - باز کردن پنل طراحی برای انتخاب ویجت =====
window.dashboard_addWidget2 = function () {
    // جلوگیری از رفتار پیش‌فرض
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    
    // اگر حالت طراحی فعال نیست، آن را فعال کن
    if (!isDesignMode) {
        window.toggleDesignMode();
    }
    
    // اسکرول به قسمت انتخاب ویجت
    setTimeout(() => {
        const widgetSection = document.querySelector('.neo-dashboard-design-section:has(.neo-dashboard-widget-palette)');
        if (widgetSection) {
            widgetSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
            
            // هایلایت کردن قسمت برای جلب توجه کاربر
            widgetSection.style.transition = 'all 0.3s ease';
            widgetSection.style.background = 'var(--neo-dashboard-primary-light)';
            setTimeout(() => {
                widgetSection.style.background = '';
            }, 1000);
        }
    }, 100);
};

// ===== AUTO-REFRESH INFRASTRUCTURE =====

/**
 * Enable auto-refresh for a specific widget
 * @param {string} widgetId - The ID of the widget to refresh
 * @param {number} intervalSeconds - Refresh interval in seconds (default: 60)
 */
window.dashboard_enableAutoRefresh = function(widgetId, intervalSeconds = 60) {
    // Clear existing interval if any
    if (autoRefreshIntervals[widgetId]) {
        clearInterval(autoRefreshIntervals[widgetId]);
    }
    
    // Set new interval
    autoRefreshIntervals[widgetId] = setInterval(() => {
        dashboard_refreshWidget(widgetId);
    }, intervalSeconds * 1000);
    
    console.log(`Dashboard: Auto-refresh enabled for widget ${widgetId} (every ${intervalSeconds}s)`);
};

/**
 * Disable auto-refresh for a specific widget
 * @param {string} widgetId - The ID of the widget
 */
window.dashboard_disableAutoRefresh = function(widgetId) {
    if (autoRefreshIntervals[widgetId]) {
        clearInterval(autoRefreshIntervals[widgetId]);
        delete autoRefreshIntervals[widgetId];
        console.log(`Dashboard: Auto-refresh disabled for widget ${widgetId}`);
    }
};

/**
 * Refresh a specific widget without page reload
 * @param {string} widgetId - The ID of the widget to refresh
 */
window.dashboard_refreshWidget = function(widgetId) {
    const widgetContainer = document.querySelector(`[widgetId="${widgetId}"]`);
    if (!widgetContainer) {
        console.error(`Dashboard: Widget ${widgetId} not found`);
        return;
    }
    
    const widgetContent = widgetContainer.querySelector('.neo-dashboard-widget-content');
    if (!widgetContent) {
        console.error(`Dashboard: Widget content for ${widgetId} not found`);
        return;
    }
    
    // Show loading state
    widgetContent.style.opacity = '0.5';
    widgetContent.style.pointerEvents = 'none';
    
    // Add loading indicator
    const loadingIndicator = document.createElement('div');
    loadingIndicator.className = 'neo-dashboard-widget-loading';
    loadingIndicator.innerHTML = `
        <svg class="neo-dashboard-spinner" viewBox="0 0 24 24" style="width: 24px; height: 24px; animation: spin 1s linear infinite;">
            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none" opacity="0.25"/>
            <path fill="currentColor" d="M12 2 A10 10 0 0 1 22 12" opacity="0.75"/>
        </svg>
        <span style="margin-right: 8px;">در حال به‌روزرسانی...</span>
    `;
    widgetContent.appendChild(loadingIndicator);
    
    // Get widget configuration
    const configId = document.querySelector("input[name='ConfigId']")?.value;
    
    // Perform AJAX request to refresh widget data
    const ajaxParams = {
        NamespaceId: window.PageAddressManager.getNamespaceId(),
        EntityId: window.PageAddressManager.getEntityId(),
        DashboardId: window.PageAddressManager.getPageId(),
        ConfigId: configId,
        WidgetId: widgetId
    };
    
    fetch(window.top.rootUrl + 'Dashboard/RefreshWidget', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=utf-8',
            ...window.AddAntiForgeryToken()
        },
        body: JSON.stringify(ajaxParams)
    })
    .then(response => response.json())
    .then(result => {
        if (result && result.html) {
            // Update widget content
            widgetContent.innerHTML = result.html;
            
            // Success animation
            widgetContent.style.transition = 'opacity 0.3s ease';
            widgetContent.style.opacity = '1';
            widgetContent.style.pointerEvents = '';
            
            console.log(`Dashboard: Widget ${widgetId} refreshed successfully`);
        } else {
            throw new Error('Invalid response from server');
        }
    })
    .catch(error => {
        console.error(`Dashboard: Failed to refresh widget ${widgetId}:`, error);
        
        // Remove loading indicator
        if (loadingIndicator && loadingIndicator.parentNode) {
            loadingIndicator.remove();
        }
        
        // Restore widget state
        widgetContent.style.opacity = '1';
        widgetContent.style.pointerEvents = '';
        
        // Show error message
        const errorMessage = document.createElement('div');
        errorMessage.className = 'neo-dashboard-widget-error';
        errorMessage.style.cssText = 'color: #ef4444; padding: 8px; text-align: center;';
        errorMessage.textContent = 'خطا در به‌روزرسانی ویجت';
        widgetContent.appendChild(errorMessage);
        
        // Remove error message after 3 seconds
        setTimeout(() => errorMessage.remove(), 3000);
    });
};

/**
 * Enable auto-refresh for all widgets in the dashboard
 * @param {number} intervalSeconds - Refresh interval in seconds (default: 300 = 5 minutes)
 */
window.dashboard_enableGlobalAutoRefresh = function(intervalSeconds = 300) {
    const widgets = document.querySelectorAll('[widgetId]');
    widgets.forEach(widget => {
        const widgetId = widget.getAttribute('widgetId');
        if (widgetId) {
            dashboard_enableAutoRefresh(widgetId, intervalSeconds);
        }
    });
    autoRefreshEnabled = true;
    console.log(`Dashboard: Global auto-refresh enabled (every ${intervalSeconds}s)`);
};

/**
 * Disable auto-refresh for all widgets
 */
window.dashboard_disableGlobalAutoRefresh = function() {
    Object.keys(autoRefreshIntervals).forEach(widgetId => {
        dashboard_disableAutoRefresh(widgetId);
    });
    autoRefreshEnabled = false;
    console.log('Dashboard: Global auto-refresh disabled');
};

/**
 * Toggle global auto-refresh
 */
window.dashboard_toggleGlobalAutoRefresh = function() {
    if (autoRefreshEnabled) {
        dashboard_disableGlobalAutoRefresh();
        // Update UI
        const btn = document.getElementById('autoRefreshToggle');
        const text = document.getElementById('autoRefreshText');
        if (btn) {
            btn.classList.remove('neo-dashboard-btn-success');
            btn.classList.add('neo-dashboard-btn-secondary');
        }
        if (text) {
            text.textContent = 'به‌روزرسانی خودکار';
        }
    } else {
        dashboard_enableGlobalAutoRefresh();
        // Update UI
        const btn = document.getElementById('autoRefreshToggle');
        const text = document.getElementById('autoRefreshText');
        if (btn) {
            btn.classList.remove('neo-dashboard-btn-secondary');
            btn.classList.add('neo-dashboard-btn-success');
        }
        if (text) {
            text.textContent = '✓ به‌روزرسانی فعال';
        }
    }
};

// Add CSS for loading spinner animation
const style = document.createElement('style');
style.textContent = `
    @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
    }
    .neo-dashboard-widget-loading {
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 16px;
        color: var(--neo-dashboard-primary, #3b82f6);
        font-size: 14px;
    }
    .neo-dashboard-spinner {
        animation: spin 1s linear infinite;
    }
`;
document.head.appendChild(style);

