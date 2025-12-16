var ConfigConfig = function () {
	var constSubmitUri = 'Report/SetConfig';
	var cloneUri = 'Report/CloneConfig';
	var constChangeParentUri = 'Folder/ChangeConfigParent';

	var eConfigAccessLevel = {
		'private': '0',
		'public': '1',
		'userGroup': '2'
	};

	var acquireSubmittableObject = function () {
		var result = {};
		result["NamespaceId"] = window.PageAddressManager.getNamespaceId();
		result["EntityId"] = window.PageAddressManager.getEntityId();
		result["ReportId"] = window.PageAddressManager.getPageId();
		result["ConfigId"] = $('#config-config-modal input[name="ConfiggingConfigId"]').val();
		var acccessLevel = $('#config-config-modal input[name="access"]:checked').val();
		result["IsPublic"] = acccessLevel !== eConfigAccessLevel['private'];
		//       result["FolderId"] = $('#config-config-modal select[name="folder"]').val(); //todo
		result["IsDefault"] = $('#config-config-modal input[name="isdefault"]').is(':checked');
		result["UserGroupId"] = acccessLevel === eConfigAccessLevel['userGroup'] ? $('#config-config-modal select[name="userGroup"]').val() : 0;
		result["Name"] = $('#config-config-modal input[name="alias"]').val();
		return result;
	};

	var showOrHideUserGroupsCombo = function () {
		if ($('#config-config-modal input[name=access][value=' + eConfigAccessLevel['userGroup'] + ']')
			.prop('checked')) {
			$('#config-config-modal .user-group-div').removeClass('hidden');
		} else {
			$('#config-config-modal .user-group-div').addClass('hidden');
		}
	};

	var showModal = function (configId) {
		var $modal = $('#config-config-modal');
		var prevConfig = window.configsJson[configId];

		if (!prevConfig) {
			console.error('Config with id ' + configId + ' not found!');
			return;
		}

		$modal.find('input[name="ConfiggingConfigId"]').val(configId);
		$modal.find('input[name="alias"]').val(prevConfig.alias);
		$modal.find('select[name="folder"]').val(prevConfig.folderId).trigger('change');//trigger change is for select2
		$modal.find('input[name="isdefault"]').prop('checked', prevConfig.isdefault);

		if (window.jsCanPublishConfigs) {
			$modal.find('input[name=access][value=' + prevConfig.access + ']').prop('checked', true);
			$modal.find('select[name="userGroup"]').val(prevConfig.userGroup).trigger('change');
			showOrHideUserGroupsCombo();
		}

		$modal.modal('show');
	};

	var changeParent = function (nodeId, parentId, isFolder) {
		var changeParentModel = {
			newFolderId: parentId,
			configType: window.PageAddressManager.getPageType()
		};
		changeParentModel[isFolder ? 'folderId' : 'configId'] = nodeId;
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + constChangeParentUri,
			data: changeParentModel,
			headers: AddAntiForgeryToken(),
			success: function () {
				console.debug('success');
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
	};

	var submitForm = function () {
		var configObject = acquireSubmittableObject();

		$.ajax({
			type: "POST",
			url: window.top.rootUrl + constSubmitUri,
			data: configObject,
			headers: AddAntiForgeryToken(),
			success: function (res) {
				$('#configs-tree').jstree('rename_node', res, configObject.Name);
				$('#config-config-modal').modal('hide');
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
		//todo refresh? dismiss modal? tell it saved successfully?
	};
	var clone = function (configId, cb) {
		$.ajax({
			type: "POST",
			url: window.top.rootUrl + cloneUri + '?' + window.PageAddressManager.getQueryParameters(),
			data: {
				configId: configId
			},
			headers: AddAntiForgeryToken(),
			success: function (res) {
				if (cb)
					cb(res);
			},
			error: function () {
				window.toast.error(window.tetaI18n.t('Error Occured'));
			}
		});
	};
	return {
		showModal: showModal,
		changeParent: changeParent,
		submitForm: submitForm,
		showOrHideUserGroupsCombo: showOrHideUserGroupsCombo,
		clone: clone
	};
}();
var ConfigsTreeManager = function () {

	var selectCurrentNode = function (event, data) {
		var configId = $('#ConfigId').val();
		var $configsTree = $('#configs-tree');
		if (configId) {
			$configsTree.jstree("deselect_all");
			$configsTree.jstree('select_node', configId);
			// Open tree to show active config
			data.instance._open_to(configId);
			// Add active class for visual difference
			setTimeout(function() {
				var $activeNode = $configsTree.jstree('get_node', configId);
				if ($activeNode && $activeNode.length) {
					$('#' + configId).addClass('jstree-active-config');
				}
			}, 100);
		}
	};

    var goToConfig = function (configId) {
        var currentFilterId = FilterConfig.getCurrentPageFilterId();
		window.PageAddressManager.navigateTo('Report?' +
			window.PageAddressManager.getQueryParameters() +
			'&ConfigId=' +
            configId +
            (currentFilterId ? ('&FilterId=' + currentFilterId) : ''));
	};

	var addRemoveToContextMenu = function(items, node) {
		if (node.data.isMeta)
			return;
		items['remove'] = {
			'icon': "fa fa-times",
			'label': window.tetaI18n.t('Remove'),
			'action': function () {
				if (node.type === "default")
					FolderManager.deleteFolder(node.id, function () {
						$('#configs-tree').jstree('delete_node', node.id);
					});
				else
					deleteConfigId(node.id, function () {
						$('#configs-tree').jstree('delete_node', node.id);
					});
			}
		};
	};

	var addCloneToContextMenu = function(items, node) {
		if (node.type !== "default") {
			items['clone'] = {
				'icon': "fa fa-copy",
				'label': window.tetaI18n.t('Copy'),
				'action': function() {
					ConfigConfig.clone(node.id, goToConfig);
				}
			};
		}
	};

	var addSettingsToContextMenu = function(items, node) {
		if (node.data.isMeta)
			return;
		items['settings'] = {
			'icon': "fa fa-cogs",
			'label': window.tetaI18n.t('Settings'),
			'action': function() {
				if (node.type === "default")
					FolderManager.showModal(true,
						node.parent,
						{
							"Id": node.id,
							"Name": node.text,
							"IsForConfig": node.data.isForConfig, //todo
							"IsPublic": node.data.isPublic,
							"ParentFolderId": node.data.parentFolderId
						});
				else
					ConfigConfig.showModal(node.id);
			}
		};
	};

	var newFolder = function (parentId) {
		FolderManager.registerSubmissionCallback(createFolderNode);
		FolderManager.showModal(true, parentId);
	};

	var showAddConfigModal = function () {
		console.log('showAddConfigModal called');
		
		// Always use top window for modal operations since report-designs-modal might be in iframe
		var targetWindow = (window.top && window.top !== window) ? window.top : window;
		var target$ = (window.top && window.top !== window && window.top.$) ? window.top.$ : $;
		
		// Try to find modal in top window first
		var $modal = target$('#reportConfigModal');
		
		if ($modal.length === 0) {
			console.warn('reportConfigModal not found in top window, trying current window');
			// Try to find in current window
			$modal = $('#reportConfigModal');
			if ($modal.length === 0) {
				console.error('reportConfigModal not found in any window');
				alert('Modal not found. Please refresh the page.');
				return;
			}
			target$ = $;
			targetWindow = window;
		}
		
		console.log('Modal found:', $modal.length > 0, 'in window:', targetWindow === window.top ? 'top' : 'current');
		
		// Get rootUrl and PageAddressManager from appropriate scope
		var rootUrl = '';
		if (window.top && window.top.rootUrl) {
			rootUrl = window.top.rootUrl;
		} else if (targetWindow.rootUrl) {
			rootUrl = targetWindow.rootUrl;
		} else if (window.rootUrl) {
			rootUrl = window.rootUrl;
		}
		
		var queryParams = '';
		if (window.top && window.top.PageAddressManager && window.top.PageAddressManager.getQueryParameters) {
			queryParams = window.top.PageAddressManager.getQueryParameters();
		} else if (targetWindow.PageAddressManager && targetWindow.PageAddressManager.getQueryParameters) {
			queryParams = targetWindow.PageAddressManager.getQueryParameters();
		} else if (window.PageAddressManager && window.PageAddressManager.getQueryParameters) {
			queryParams = window.PageAddressManager.getQueryParameters();
		}
		
		console.log('Opening modal with URL:', rootUrl + 'Report/AddNewConfig?' + queryParams);
		
		// Set iframe content
		$modal.find('.modal-body').html(
			"<iframe src='" +
			rootUrl +
			"Report/AddNewConfig?" +
			queryParams +
			"' style='width:100%; height:100%;' frameborder='0' allowtransparency='true'></iframe>");
		
		// Ensure modal has high z-index - higher than report-designs-modal (10000)
		// Set z-index before showing to ensure it's applied immediately
		$modal.css('z-index', '10050');
		
		// Show modal using Bootstrap
		try {
			if (typeof $modal.modal === 'function') {
				$modal.modal('show');
				// After modal is shown, ensure z-index is correct and backdrop is above report-designs-modal
				setTimeout(function() {
					$modal.css('z-index', '10050');
					// Update backdrop z-index - should be above report-designs-modal (10000) but below modal (10050)
					var $backdrops = target$('.modal-backdrop');
					if ($backdrops.length > 0) {
						$backdrops.last().css('z-index', '10049');
					}
					console.log('Modal shown with z-index:', $modal.css('z-index'));
				}, 100);
			} else {
				// Fallback if Bootstrap modal not available
				$modal.addClass('show').css({
					'display': 'block',
					'z-index': '10050'
				});
				target$(targetWindow.document.body).addClass('modal-open');
				// Create backdrop if needed
				var $backdrop = target$('.modal-backdrop').last();
				if ($backdrop.length === 0) {
					$backdrop = target$('<div class="modal-backdrop fade show"></div>').appendTo(targetWindow.document.body);
				}
				$backdrop.css('z-index', '10049');
				console.log('Modal shown (fallback) with z-index:', $modal.css('z-index'));
			}
		} catch (error) {
			console.error('Error showing modal:', error);
			alert('Error opening modal: ' + error.message);
		}
	};
	
	var addNewFolderToContextMenu = function(items, node) {
		if (window.jsCanDesign && node.type === "default") {
			items['newFolder'] = {
				'icon': "fa fa-folder-open",
				'label': window.tetaI18n.t('New Folder'),
				'action': function() {
					newFolder(node.id);
				}
			};
		}
	};

	var contextMenu = function (node) {
		var items = {};
		addNewFolderToContextMenu(items, node);
		if (window.jsCanDesign && (!node.data.isPublic || window.jsCanPublishConfigs)) {
			addSettingsToContextMenu(items, node);
			addCloneToContextMenu(items, node);
			addRemoveToContextMenu(items, node);
		}
		return items;
	};

	// Helper function to get icon for ChartType
	var getChartTypeIcon = function(chartType) {
		var iconMap = {
			"Column": "fa fa-bar-chart",
			"Bar": "fa fa-bar-chart",
			"Line": "fa fa-line-chart",
			"Area": "fa fa-area-chart",
			"Pie": "fa fa-pie-chart",
			"Spline": "fa fa-line-chart",
			"Areaspline": "fa fa-area-chart",
			"Scatter": "fa fa-dot-circle-o",
			"Treemap": "fa fa-sitemap",
			"Gauge": "fa fa-tachometer",
			"MetricBox": "fa fa-cube",
			"IranMap": "fa fa-map",
			"WorldMap": "fa fa-globe",
			"BpmnDiagram": "fa fa-project-diagram"
		};
		return iconMap[chartType] || "fa fa-bar-chart"; // Default icon
	};

	// Helper function to get icon for ChartType
	var getChartTypeIcon = function(chartType) {
		if (!chartType) return "fa fa-bar-chart"; // Default
		var iconMap = {
			"Column": "fa fa-bar-chart",
			"Bar": "fa fa-bar-chart",
			"Line": "fa fa-line-chart",
			"Area": "fa fa-area-chart",
			"Pie": "fa fa-pie-chart",
			"Spline": "fa fa-line-chart",
			"Areaspline": "fa fa-area-chart",
			"Scatter": "fa fa-dot-circle-o",
			"Treemap": "fa fa-sitemap",
			"Gauge": "fa fa-tachometer",
			"MetricBox": "fa fa-cube",
			"IranMap": "fa fa-map",
			"WorldMap": "fa fa-globe",
			"BpmnDiagram": "fa fa-project-diagram"
		};
		return iconMap[chartType] || "fa fa-bar-chart"; // Default icon
	};

	var types = {
		"default": { //folder
			"icon": window.top.rootUrl + "Content/common-assets-includes/icons/folder.svg",
			"valid_children": ["default", "Chart", "ReportList", "GroupByList"]
		},
		"Chart": {
			"icon": "fa fa-bar-chart", // Default icon, will be overridden in model.jstree event
			"valid_children": []
		},
		"ReportList": {
			"icon": "fa fa-list",
			"valid_children": []
		},
		"GroupByList": {
			"icon": "fa fa-table",
			"valid_children": []
		}
	};

	var nodeMoved = function (e, data) {
		if (data['old_parent'] !== data['parent'])
			ConfigConfig.changeParent(data.node.id, data.parent, data.node.type === 'default');
		//            console.warn('Must save ' + data.node.id + '\'s parent as ' + data['parent']);
	};

	var instantiateTree = function () {
		$('#configs-tree')
			.jstree({
				'core': {
					'data': window.configsTree,
					"check_callback": true
				},
				"contextmenu": {
					"select_node": false,
					'items': contextMenu
				},
				"types": types,
				"plugins": [
					"dnd", "contextmenu", "types", "conditionalselect", "wholerow",
					"search", "state"
				]
			})
			.on('model.jstree', function (e, data) {
				// Set icon based on chartType in data
				if (data.node.type === 'Chart' && data.node.data && data.node.data.chartType) {
					var icon = getChartTypeIcon(data.node.data.chartType);
					data.node.icon = icon;
				}
			})
			.on('move_node.jstree', nodeMoved)
			.on('ready.jstree', function(e, data) {
				// Set icons for Chart nodes based on chartType
				var tree = data.instance;
				tree.get_json(null, {flat: true}).forEach(function(node) {
					if (node.type === 'Chart' && node.data && node.data.chartType) {
						var icon = getChartTypeIcon(node.data.chartType);
						tree.set_icon(node.id, icon);
					}
				});
				selectCurrentNode(e, data);
			})
			.on('search.jstree before_open.jstree', function (e, data) {
				if (data.instance.settings.search.show_only_matches) {
					data.instance._data.search.dom.find('.jstree-node')
						.show().filter('.jstree-last').filter(function () {
							return this.nextSibling;
						}).removeClass('jstree-last')
						.end().end().end().find(".jstree-children").each(function () {
							$(this).children(".jstree-node:visible").eq(-1).addClass("jstree-last");
						});
				}
			})
			.off('keydown');
		$("#configs-search-input").keyup(function () {
			$('#configs-tree').jstree('search', $(this).val());
		});
	};

	var createFolderNode = function (nodeId, parentNodeId, text, isUpdate) {
		if (isUpdate) {
			$('#configs-tree').jstree('rename_node', nodeId, text);
			//todo data should also change
		}
		else
			$('#configs-tree')
				.jstree('create_node',
					parentNodeId ? parentNodeId.toString() : '#',
					{
						"id": nodeId,
						"text": text,
						//				    "type": "default",
						"data": {
							"isPublic": /*parentNode ? parentNode.data.isPublic :*/ false,
							"parentFolderId": parentNodeId ? parentNodeId : null
						}
					},
					"first");
	};

	$(instantiateTree);
	var manager = {
		newFolder: newFolder,
		showAddConfigModal: showAddConfigModal,
		goToConfig: goToConfig,
		getContextMenu: contextMenu
	};
	
	// Expose to window for global access (both current window and top window if in iframe)
	if (typeof window !== 'undefined') {
		window.ConfigsTreeManager = manager;
		// Also expose to top window if we're in an iframe
		if (window.top && window.top !== window) {
			try {
				window.top.ConfigsTreeManager = manager;
				console.log('ConfigsTreeManager exposed to top window');
			} catch (e) {
				// Cross-origin iframe, can't access top window
				console.warn('Cannot access top window:', e);
			}
		}
		console.log('ConfigsTreeManager initialized:', {
			hasShowAddConfigModal: typeof manager.showAddConfigModal === 'function',
			hasNewFolder: typeof manager.newFolder === 'function',
			hasGoToConfig: typeof manager.goToConfig === 'function',
			hasGetContextMenu: typeof manager.getContextMenu === 'function'
		});
	}
	
	// Also expose to global scope for backward compatibility
	if (typeof ConfigsTreeManager === 'undefined') {
		window.ConfigsTreeManager = manager;
	}
	
	return manager;
}();
