/*
 * Shared filter affordances for generated forms, reports, dashboards and work items.
 * Header buttons deliberately focus the existing filter control; they never create a
 * second query or submit a different set of values.
 */
(function (window, document) {
    'use strict';

    if (window.NeoFilterUx) {
        return;
    }

    var rootSelector = '#filterDiv, #filterTooltipPanel, #DashboardFilter';
    var groupSelector = '.modern-filter-content, .filter-main-content, .tab-pane';
    var maxVisibleControls = 8;
    var refreshTimer = null;

    function unique(items) {
        return items.filter(function (item, index) { return items.indexOf(item) === index; });
    }

    function roots() {
        return Array.prototype.slice.call(document.querySelectorAll(rootSelector)).filter(function (root) {
            return !root.parentElement || !root.parentElement.closest(rootSelector);
        });
    }

    function groups(root) {
        var candidates = Array.prototype.slice.call(root.querySelectorAll(groupSelector));
        candidates = candidates.filter(function (candidate) {
            return !candidate.parentElement || !candidate.parentElement.closest(groupSelector) ||
                !root.contains(candidate.parentElement.closest(groupSelector));
        });
        return candidates.length ? candidates : [root];
    }

    function controls(group) {
        return Array.prototype.slice.call(group.querySelectorAll('.neo-control')).filter(function (control) {
            var parent = control.parentElement && control.parentElement.closest('.neo-control');
            if (parent && group.contains(parent)) {
                return false;
            }
            return !!control.querySelector('input[name], select[name], textarea[name], .modern-multi-select-input');
        });
    }

    function isIgnoredInput(input) {
        var name = (input.getAttribute('name') || '').toLowerCase();
        return name.indexOf('filterparameter') !== -1 || input.classList.contains('modern-multi-select-search') || input.disabled || input.closest('.neo-filter-toolbar');
    }

    function hasValue(control) {
        var inputs = Array.prototype.slice.call(control.querySelectorAll('input, select, textarea'));
        var customInput = control.querySelector('.modern-multi-select-input');
        if (customInput && customInput.value && customInput.value.trim()) {
            return true;
        }
        return inputs.some(function (input) {
            if (isIgnoredInput(input)) return false;
            var type = (input.type || '').toLowerCase();
            if (type === 'checkbox' || type === 'radio') return input.checked;
            if (type === 'hidden') return !!String(input.value || '').trim();
            if (input.tagName.toLowerCase() === 'select') {
                return Array.prototype.some.call(input.options || [], function (option) {
                    return option.selected && String(option.value || '').trim() !== '';
                });
            }
            return !!String(input.value || '').trim();
        });
    }

    function labelFor(control) {
        var label = control.querySelector('label');
        var text = label ? label.textContent : control.getAttribute('data-id');
        text = (text || '').replace(/\s+/g, ' ').trim();
        return text || 'فیلتر';
    }

    function controlName(control) {
        if (!control) return '';
        var field = control.querySelector('[name]:not([name*="FilterParameter"])');
        return control.getAttribute('data-id') || (field ? field.getAttribute('name') : '') || '';
    }

    function createToolbar(root) {
        var toolbar = root.querySelector('.neo-filter-toolbar');
        if (toolbar) return toolbar;
        toolbar = document.createElement('div');
        toolbar.className = 'neo-filter-toolbar';
        toolbar.setAttribute('role', 'status');
        toolbar.innerHTML = '<div class="neo-filter-active-summary" aria-live="polite"></div>' +
            '<button type="button" class="neo-filter-more-toggle" hidden></button>';
        var anchor = root.querySelector('.filter-panel-header, .modern-filter-header, .filter-panel-content-wrapper, .modern-filter-content, .tab-pane');
        if (anchor && anchor.parentNode) {
            anchor.parentNode.insertBefore(toolbar, anchor.classList.contains('filter-panel-header') || anchor.classList.contains('modern-filter-header') ? anchor.nextSibling : anchor);
        } else {
            root.insertBefore(toolbar, root.firstChild);
        }
        return toolbar;
    }

    function updateSummary(root) {
        var toolbar = createToolbar(root);
        var summary = toolbar.querySelector('.neo-filter-active-summary');
        var active = [];
        groups(root).forEach(function (group) {
            controls(group).forEach(function (control) {
                control.classList.toggle('neo-filter-has-value', hasValue(control));
                if (hasValue(control)) active.push(control);
            });
        });
        summary.innerHTML = '';
        if (!active.length) {
            summary.textContent = '';
            toolbar.classList.remove('has-active-filters');
        } else {
            toolbar.classList.add('has-active-filters');
            var count = document.createElement('span');
            count.className = 'neo-filter-active-count';
            count.textContent = active.length + ' فیلتر فعال';
            summary.appendChild(count);
            active.forEach(function (control) {
                var chip = document.createElement('button');
                chip.type = 'button';
                chip.className = 'neo-filter-chip';
                chip.setAttribute('data-filter-chip', controlName(control));
                chip.title = 'تمرکز روی ' + labelFor(control);
                chip.innerHTML = '<span>' + labelFor(control).replace(/</g, '&lt;') + '</span><span class="neo-filter-chip-clear" aria-hidden="true">×</span>';
                summary.appendChild(chip);
            });
        }
        updateHeaderButtons();
    }

    function updateHeaderButtons() {
        Array.prototype.slice.call(document.querySelectorAll('.neo-column-filter')).forEach(function (button) {
            var control = findControl(button.getAttribute('data-filter-column'));
            button.disabled = !control;
            button.setAttribute('aria-disabled', control ? 'false' : 'true');
            if (control) button.setAttribute('aria-controls', control.closest(rootSelector).id);
            else button.removeAttribute('aria-controls');
            button.classList.toggle('is-active', !!control && hasValue(control));
            button.setAttribute('aria-pressed', !!control && hasValue(control) ? 'true' : 'false');
        });
    }

    function collapseRoot(root) {
        var advancedCount = 0;
        var advancedActive = false;
        groups(root).forEach(function (group) {
            controls(group).forEach(function (control, index) {
                var advanced = index >= maxVisibleControls;
                control.classList.toggle('neo-filter-advanced', advanced);
                if (advanced) {
                    advancedCount++;
                    advancedActive = advancedActive || hasValue(control);
                }
            });
        });
        var toggle = createToolbar(root).querySelector('.neo-filter-more-toggle');
        if (!advancedCount) {
            toggle.hidden = true;
            return;
        }
        var expanded = root.classList.contains('neo-filter-expanded') || advancedActive;
        root.classList.toggle('neo-filter-expanded', expanded);
        toggle.hidden = false;
        toggle.textContent = expanded ? 'نمایش فیلترهای کمتر' : 'نمایش ' + advancedCount + ' فیلتر بیشتر';
        toggle.setAttribute('aria-expanded', expanded ? 'true' : 'false');
    }

    function refresh() {
        roots().forEach(function (root) {
            collapseRoot(root);
            updateSummary(root);
        });
        updateHeaderButtons();
    }

    function findControl(name, scope) {
        if (!name) return null;
        var requested = String(name).replace(/\[\]$/, '').replace(/__FilterParameter$/i, '').trim();
        var requestedLower = requested.toLowerCase();
        var found = [];
        (scope ? [scope] : roots()).forEach(function (root) {
            found = found.concat(Array.prototype.slice.call(root.querySelectorAll('[data-id], [name], [id]')));
        });
        for (var i = 0; i < found.length; i++) {
            var candidate = found[i];
            var dataId = (candidate.getAttribute('data-id') || '').trim();
            var fieldName = (candidate.getAttribute('name') || '').replace(/\[\]$/, '').replace(/__FilterParameter$/i, '').trim();
            var id = (candidate.id || '').replace(/^field-/i, '').trim();
            var matches = dataId.toLowerCase() === requestedLower ||
                fieldName.toLowerCase() === requestedLower ||
                id.toLowerCase() === requestedLower;
            if (!matches) continue;
            var container = candidate.classList.contains('neo-control') ? candidate : candidate.closest('.neo-control');
            if (container && !container.classList.contains('neo-filter-toolbar')) return container;
        }
        return null;
    }

    function openPanel(control) {
        var panel = control && control.closest(rootSelector);
        if (!panel) return;
        if (panel.id === 'filterTooltipPanel') {
            if (!panel.classList.contains('show') && typeof window.toggleFilterTooltip === 'function') {
                window.toggleFilterTooltip(null, document.querySelector('[data-action="toggle-filter-tooltip"]'));
            }
        } else if (panel.id === 'filterDiv' && window.getComputedStyle(panel).display === 'none') {
            if (typeof window.toggleFilter === 'function') window.toggleFilter();
            else {
                var button = document.getElementById('showFilter');
                if (button) button.click();
            }
        }
    }

    function revealControl(control) {
        var root = control.closest(rootSelector);
        if (!root) return;
        if (control.classList.contains('neo-filter-advanced')) {
            root.classList.add('neo-filter-expanded');
            collapseRoot(root);
        }
        var panes = [];
        for (var pane = control.closest('.tab-pane'); pane && root.contains(pane);
            pane = pane.parentElement && pane.parentElement.closest('.tab-pane')) {
            panes.unshift(pane);
        }
        panes.forEach(function (pane) {
            if (pane.classList.contains('active')) return;
            var link = Array.prototype.find.call(root.querySelectorAll('[data-toggle="tab"], [data-bs-toggle="tab"]'), function (item) {
                return item.getAttribute('href') === '#' + pane.id ||
                    item.getAttribute('data-target') === '#' + pane.id ||
                    item.getAttribute('data-bs-target') === '#' + pane.id;
            });
            if (!link) return;
            if (window.bootstrap && window.bootstrap.Tab && window.bootstrap.Tab.getOrCreateInstance) {
                window.bootstrap.Tab.getOrCreateInstance(link).show();
            } else if (window.jQuery && window.jQuery.fn.tab) {
                window.jQuery(link).tab('show');
            } else link.click();
        });
    }

    function focusControl(control) {
        if (!control || !control.isConnected) return;
        revealControl(control);
        control.classList.add('neo-filter-focus');
        setTimeout(function () { control.classList.remove('neo-filter-focus'); }, 1400);
        // Position first: widgets calculate their popup coordinates when opening.
        control.scrollIntoView({ behavior: 'instant', block: 'nearest' });
        var available = function (element) {
            return !element.disabled && element.getAttribute('aria-disabled') !== 'true' &&
                element.getClientRects().length > 0 && window.getComputedStyle(element).visibility !== 'hidden';
        };
        var custom = Array.prototype.find.call(control.querySelectorAll('.modern-multi-select-input'), available);
        var select = control.querySelector('select:not([disabled]):not(.modern-multi-select-hidden)');
        var target = Array.prototype.find.call(control.querySelectorAll('input:not([type="hidden"]), select, textarea'), available);
        if (custom) {
            if (!custom.hasAttribute('tabindex')) custom.setAttribute('tabindex', '-1');
            custom.focus();
            var widget = custom.closest('.modern-multi-select, .modern-multi-select-control');
            var instance = widget && widget._modernMultiSelectInstance;
            if (instance && typeof instance.open === 'function') instance.open();
            else if (!widget || !widget.classList.contains('open')) custom.click();
        } else if (select && window.jQuery && window.jQuery.fn.select2 && window.jQuery(select).data('select2')) {
            window.jQuery(select).select2('open');
        } else if (target) {
            target.focus();
            // Existing datepicker click handlers open the initialized calendar.
            // Calling pDatepicker with a string can reinitialize it and reset values.
            if (target.classList.contains('dateField')) target.click();
        }
    }

    function showControl(control) {
        if (!control) return;
        openPanel(control);
        var panel = control.closest(rootSelector);
        if (window.jQuery && panel) {
            window.jQuery(panel).promise().done(function () { focusControl(control); });
        } else focusControl(control);
    }

    function clearControl(control) {
        if (!control) return;
        Array.prototype.slice.call(control.querySelectorAll('input, select, textarea')).forEach(function (input) {
            if (isIgnoredInput(input)) return;
            var type = (input.type || '').toLowerCase();
            if (type === 'checkbox' || type === 'radio') input.checked = false;
            else if (input.tagName.toLowerCase() === 'select') {
                if (window.jQuery && window.jQuery(input).data('select2')) window.jQuery(input).val(null).trigger('change');
                else { input.selectedIndex = -1; input.dispatchEvent(new Event('change', { bubbles: true })); }
            } else {
                input.value = '';
                input.dispatchEvent(new Event('input', { bubbles: true }));
                input.dispatchEvent(new Event('change', { bubbles: true }));
            }
            var associated = input.getAttribute('associated-hidden-name');
            if (associated) {
                var hidden = control.querySelector('[name="' + associated + '"], #' + associated.replace(/[^a-zA-Z0-9_-]/g, '\\$&'));
                if (hidden) hidden.value = '';
            }
        });
        var root = control.closest(rootSelector);
        if (root) updateSummary(root);
    }

    function bind() {
        if (window.NeoFilterUxInitialized) return;
        window.NeoFilterUxInitialized = true;
        // Capture before table sorting and document outside-click handlers.
        document.addEventListener('click', function (event) {
            var header = event.target.closest && event.target.closest('.neo-column-filter');
            if (!header) return;
            event.preventDefault();
            event.stopPropagation();
            if (!header.disabled) showControl(findControl(header.getAttribute('data-filter-column')));
        }, true);
        document.addEventListener('click', function (event) {
            var toggle = event.target.closest && event.target.closest('.neo-filter-more-toggle');
            if (toggle) {
                var root = toggle.closest(rootSelector);
                if (root) root.classList.toggle('neo-filter-expanded');
                refresh();
                return;
            }
            var chip = event.target.closest && event.target.closest('.neo-filter-chip');
            if (chip) {
                event.preventDefault();
                var control = findControl(chip.getAttribute('data-filter-chip'), chip.closest(rootSelector));
                if (event.target.closest('.neo-filter-chip-clear')) clearControl(control);
                else showControl(control);
            }
        });
        document.addEventListener('input', scheduleRefresh, true);
        document.addEventListener('change', scheduleRefresh, true);
        if (window.MutationObserver && document.body) {
            var observer = new MutationObserver(function (mutations) {
                if (mutations.some(function (mutation) {
                    return !mutation.target.closest || !mutation.target.closest('.neo-filter-toolbar');
                })) scheduleRefresh();
            });
            observer.observe(document.body, { childList: true, subtree: true });
        }
        refresh();
    }

    function scheduleRefresh() {
        clearTimeout(refreshTimer);
        refreshTimer = setTimeout(refresh, 60);
    }

    window.NeoFilterUx = { init: bind, refresh: refresh, focus: focusControl, clear: clearControl };
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', bind);
    else bind();
})(window, document);
