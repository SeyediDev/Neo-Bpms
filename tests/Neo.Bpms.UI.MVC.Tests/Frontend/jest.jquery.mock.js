// jQuery mock for Jest
// This provides a basic jQuery implementation for testing

module.exports = function(selector) {
    const elements = typeof selector === 'string' 
        ? document.querySelectorAll(selector)
        : Array.isArray(selector) 
            ? selector 
            : [selector].filter(Boolean);

    return {
        length: elements.length,
        val: function(value) {
            if (value !== undefined) {
                elements.forEach(el => {
                    if (el.tagName === 'SELECT' && el.multiple) {
                        Array.from(el.options).forEach(opt => {
                            opt.selected = Array.isArray(value) ? value.includes(opt.value) : value === opt.value;
                        });
                    } else {
                        el.value = value;
                    }
                });
                return this;
            }
            return elements[0]?.value || null;
        },
        attr: function(name, value) {
            if (value !== undefined) {
                elements.forEach(el => el.setAttribute(name, value));
                return this;
            }
            return elements[0]?.getAttribute(name);
        },
        find: function(sel) {
            const found = [];
            elements.forEach(el => {
                found.push(...el.querySelectorAll(sel));
            });
            return module.exports(found);
        },
        each: function(callback) {
            elements.forEach((el, index) => callback.call(el, index, el));
            return this;
        },
        empty: function() {
            elements.forEach(el => el.innerHTML = '');
            return this;
        },
        html: function(html) {
            if (html !== undefined) {
                elements.forEach(el => el.innerHTML = html);
                return this;
            }
            return elements[0]?.innerHTML;
        },
        addClass: function(cls) {
            elements.forEach(el => el.classList.add(cls));
            return this;
        },
        removeClass: function(cls) {
            elements.forEach(el => el.classList.remove(cls));
            return this;
        },
        hasClass: function(cls) {
            return elements[0]?.classList.contains(cls) || false;
        },
        is: function(selector) {
            const el = elements[0];
            if (!el) return false;
            if (selector.startsWith('[')) {
                const [attr, value] = selector.slice(1, -1).split('=');
                return el.hasAttribute(attr) && (!value || el.getAttribute(attr) === value);
            }
            return el.matches(selector);
        },
        closest: function(sel) {
            let parent = elements[0]?.parentElement;
            while (parent) {
                if (parent.matches(sel)) return module.exports([parent]);
                parent = parent.parentElement;
            }
            return module.exports([]);
        },
        prev: function(sel) {
            const prev = elements[0]?.previousElementSibling;
            if (prev && (!sel || prev.matches(sel))) {
                return module.exports([prev]);
            }
            return module.exports([]);
        },
        parent: function() {
            return elements[0]?.parentElement ? module.exports([elements[0].parentElement]) : module.exports([]);
        },
        data: function(name) {
            return elements[0]?.dataset[name];
        },
        trigger: function(event) {
            elements.forEach(el => {
                const evt = new Event(event.replace('.external', ''));
                el.dispatchEvent(evt);
            });
            return this;
        },
        prop: function(name, value) {
            if (value !== undefined) {
                elements.forEach(el => el[name] = value);
                return this;
            }
            return elements[0]?.[name];
        },
        css: function() {
            return this;
        },
        appendTo: function() {
            return this;
        },
        off: function() {
            return this;
        },
        on: function() {
            return this;
        }
    };
};

// Make it work as a constructor too
module.exports.fn = module.exports.prototype = {};

