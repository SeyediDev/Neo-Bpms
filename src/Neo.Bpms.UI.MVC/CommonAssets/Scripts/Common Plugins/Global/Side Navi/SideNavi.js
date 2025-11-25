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
            posStart = $(config.item + ':eq(0)', container).height() * 1;
        }
        return posStart;
    }
    function getPosEnd() {
        if (posEnd === null) {
            posEnd = $(config.item + ':eq(0)', container).height() * 1;
            posEnd += $(config.data, container).width() * 1;
        }
        return posEnd;
    }
    function getPos() {
        return container.css('right').replace('px', '');
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
        
        // Ensure side-navi-data is always visible (for width calculation)
        $(config.data, container).css({
            'display': 'block',
            'z-index': '30001'
        });

        if (isVisible && pos < getPosEnd() || !isVisible && pos > getPosStart()) {

            pos = (isVisible) ? pos + posStep : pos - posStep;

            if (isVisible && pos + posStep >= getPosEnd() || !isVisible && pos - posStep <= getPosStart()) {

                pos = (isVisible) ? getPosEnd() : getPosStart();
                container.css('right', pos + 'px');
                isSlideing = false;

            } else {
                container.css('right', pos + 'px');
                setTimeout(function () { slideEvent() }, 20);
            }

        } else {
            isSlideing = false;
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

        $(config.item, container).on('click', function (event) {

            event.preventDefault();
            event.stopPropagation();
            
            console.log('SideNavi: Tab clicked', {
                item: $(this).text().trim(),
                isVisible: isVisible,
                activeIndex: activeIndex
            });
            
            setEventParam($(this));

            if (isVisible) { 
                setActiveTab();
                // Ensure side-navi-data is visible
                $(config.data, container).css({
                    'display': 'block',
                    'z-index': '30001'
                });
            }

            if (changeVisibility) {
                // Ensure side-navi-data is visible before sliding
                $(config.data, container).css({
                    'display': 'block',
                    'z-index': '30001'
                });
                slide();
            }
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
            'z-index': '30001'
        });
        
        eventListener();
        
        // Set initial position - container should be at posStart (hidden)
        // Do this after eventListener to ensure config is set
        setTimeout(function() {
            var initialPos = getPosStart();
            container.css('right', initialPos + 'px');
            
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
