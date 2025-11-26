/**
 * Object SideNavi
 * public methods : init
 * init param : Object css data
 */

var SideNavi = (function () {

    var container = {},
		config = {},
		posStep = 30,
		posStart = null,
		posEnd = null,
		isSlideing = false,
		isVisible = false,
		activeIndex = -1,
		changeVisibility = false;

    function getPosStart() {
        if (posStart === null) {
            // posStart = 0 (no translation, container at initial position)
            posStart = 0;
        }
        return posStart;
    }
    function getPosEnd() {
        if (posEnd === null) {
            // posEnd = width of side-navi-data (translate to the left to show data panel)
            var dataWidth = $(config.data, container).width() * 1;
            posEnd = dataWidth || 400;
        }
        return posEnd;
    }
    function getPos() {
        // Get current translateX value
        var transform = container.css('transform');
        if (transform && transform !== 'none') {
            var matrix = transform.match(/matrix\(([^)]+)\)/);
            if (matrix && matrix[1]) {
                var values = matrix[1].split(',');
                // translateX is the 4th value in matrix
                return parseFloat(values[4]) || 0;
            }
        }
        return 0;
    }
    function toggleIsVisible() {
        isVisible = !(isVisible);
    }
    function isActiveItem(item) {
        return item.hasClass('active');
    }
    function setActiveTab() {
        $(config.tab + config.active, container).removeClass(config.active.replace('.', ''));
        $(config.tab + ':eq(' + activeIndex + ')', container).addClass(config.active.replace('.', ''));
    }
    function removeActiveItem() {
        $(config.item + config.active, container).removeClass('active');
    }
    function setActiveItem(item) {
        removeActiveItem();
        item.addClass('active');
    }
    function slideEvent() {

        var pos = getPos() * 1;
        var posStart = getPosStart();
        var posEnd = getPosEnd();
        
        console.log('SideNavi: slideEvent', {
            currentPos: pos,
            posStart: posStart,
            posEnd: posEnd,
            isVisible: isVisible,
            isSlideing: isSlideing
        });
        
        // Show side-navi-data when sliding to visible, hide when sliding to hidden
        // Always show during slide animation for smooth transition
        if (isVisible) {
            // Sliding to visible - show data panel
            $(config.data, container).css({
                'display': 'block',
                'z-index': '30001',
                'visibility': 'visible',
                'opacity': '1'
            });
        } else {
            // Sliding to hidden - hide data panel after animation completes
            // But keep it visible during animation for smooth transition
            var currentPos = getPos();
            var posStart = getPosStart();
            if (currentPos <= posStart) {
                // Animation completed - hide it
                $(config.data, container).css({
                    'display': 'none',
                    'visibility': 'hidden',
                    'opacity': '0'
                });
            } else {
                // Still animating - keep visible
                $(config.data, container).css({
                    'display': 'block',
                    'z-index': '30001',
                    'visibility': 'visible',
                    'opacity': '1'
                });
            }
        }

        if (isVisible && pos < posEnd || !isVisible && pos > posStart) {

            pos = (isVisible) ? pos + posStep : pos - posStep;

            if (isVisible && pos + posStep >= posEnd || !isVisible && pos - posStep <= posStart) {

                pos = (isVisible) ? posEnd : posStart;
                container.css('transform', 'translateX(-' + pos + 'px)');
                isSlideing = false;
                
                // Show/hide side-navi-data based on visibility
                if (isVisible) {
                    $(config.data, container).css({
                        'display': 'block',
                        'visibility': 'visible',
                        'opacity': '1'
                    });
                } else {
                    $(config.data, container).css({
                        'display': 'none',
                        'visibility': 'hidden',
                        'opacity': '0'
                    });
                }
                
                console.log('SideNavi: Slide completed', {
                    finalPos: pos,
                    isVisible: isVisible,
                    dataDisplay: $(config.data, container).css('display')
                });

            } else {
                container.css('transform', 'translateX(-' + pos + 'px)');
                setTimeout(function () { slideEvent() }, 20);
            }

        } else {
            isSlideing = false;
            console.log('SideNavi: Slide not needed', {
                pos: pos,
                posStart: posStart,
                posEnd: posEnd,
                isVisible: isVisible
            });
        }

    }
    function slide() {
        if (!isSlideing) {
            isSlideing = true;
            slideEvent();
        }
    }
    function setEventParam(item) {

        activeIndex = $(config.item, container).index(item);

        if (isActiveItem(item)) {

            toggleIsVisible();
            removeActiveItem();
            changeVisibility = true;

        } else {

            setActiveItem(item);

            if (!isVisible) {
                toggleIsVisible();
                changeVisibility = true;
            }
        }
    }
    function eventListener() {

        // Use event delegation to ensure clicks work even if elements are dynamically added
        $(container).on('click', config.item, function (event) {

            event.preventDefault();
            event.stopPropagation();
            
            var $item = $(this);
            console.log('SideNavi: Tab clicked', {
                item: $item.text().trim(),
                isVisible: isVisible,
                activeIndex: activeIndex,
                itemExists: $item.length > 0
            });
            
            setEventParam($item);
            
            console.log('SideNavi: After setEventParam', {
                isVisible: isVisible,
                changeVisibility: changeVisibility,
                activeIndex: activeIndex
            });

            if (isVisible) { 
                setActiveTab();
                // Ensure side-navi-data is visible
                $(config.data, container).css({
                    'display': 'block',
                    'z-index': '30001',
                    'pointer-events': 'auto',
                    'visibility': 'visible',
                    'opacity': '1'
                });
            }

            if (changeVisibility) {
                // Ensure side-navi-data is visible before sliding
                $(config.data, container).css({
                    'display': 'block',
                    'z-index': '30001',
                    'pointer-events': 'auto',
                    'visibility': 'visible',
                    'opacity': '1'
                });
                
                console.log('SideNavi: Starting slide', {
                    currentRight: container.css('right'),
                    posStart: getPosStart(),
                    posEnd: getPosEnd()
                });

                slide();
            }

            return false;
        });
        $(config.container + ', .keep-side-navi').on('click', function (event) {
            event.stopPropagation();
        });
        $(document).on('click', function () {
            if (isVisible) {
                var $activeTab = $(config.item + config.active);
                if ($activeTab.length) {
                    setEventParam($activeTab);
                    if (isVisible) { setActiveTab(); }
                    if (changeVisibility) {
                        slide();
                    }
                }
            }
        });
    }
    function init(conf) {

        config = conf;
        container = $(config.container);
        
        console.log('SideNavi: Initializing', {
            container: config.container,
            itemsCount: $(config.item, container).length,
            tabsCount: $(config.tab, container).length,
            dataExists: $(config.data, container).length > 0
        });

        // Ensure side-navi-data is always block for width calculation
        var $sideNaviData = $(config.data, container);
        $sideNaviData.css({
            'display': 'block',
            'z-index': '30001',
            'pointer-events': 'auto'
        });
        
        // Ensure side-navi-items are clickable
        $(config.item, container).css({
            'pointer-events': 'auto',
            'z-index': '30002'
        });

        eventListener();
        
        // Set initial position - container should be at posStart (hidden)
        // Do this after eventListener to ensure config is set
        setTimeout(function() {
            // Reset posStart and posEnd to recalculate
            posStart = null;
            posEnd = null;
            
            var initialPos = getPosStart();
            container.css('transform', 'translateX(-' + initialPos + 'px)');
            
            // Hide side-navi-data initially (container is at posStart)
            $sideNaviData.css({
                'display': 'none',
                'visibility': 'hidden',
                'opacity': '0'
            });
            
            console.log('SideNavi: Initial position set', {
                initialPos: initialPos,
                containerRight: container.css('right'),
                posStart: getPosStart(),
                posEnd: getPosEnd(),
                dataWidth: $sideNaviData.width(),
                dataDisplay: $sideNaviData.css('display')
            });
            
            // Activate first tab (popular filters) by default if not already active
            var $firstItem = $(config.item + ':first', container);
            var $firstTab = $(config.tab + ':first', container);
            
            if ($firstItem.length && $firstTab.length) {
                if (!$firstItem.hasClass('active')) {
                    activeIndex = 0;
                    setActiveItem($firstItem);
                }
                if (!$firstTab.hasClass('active')) {
                    setActiveTab();
                }
                
                console.log('SideNavi: First tab activated', {
                    itemActive: $firstItem.hasClass('active'),
                    tabActive: $firstTab.hasClass('active'),
                    containerRight: container.css('right'),
                    dataDisplay: $sideNaviData.css('display'),
                    posStart: getPosStart(),
                    posEnd: getPosEnd()
                });
            }
        }, 150);
    }
    
    return {
        init: init
    }

})();

$(function() {
        SideNavi.init({
            container: '#sideNavi',
            item: '.side-navi-item',
            data: '.side-navi-data',
            tab: '.side-navi-tab',
            active: '.active'
        });
    });
