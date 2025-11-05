function tooltipFormatter(tooltipsObj) {
	var tooltipHtml =
		"<div style='direction: rtl; z-index:100; background-color:white; margin:-8px; padding:5px;'>";
	for (var field in tooltipsObj) {
		if (tooltipsObj.hasOwnProperty(field)) {
			tooltipHtml += tooltipFormatterItem(field, tooltipsObj[field]);
		}
	}
	tooltipHtml += "</div>";
	return tooltipHtml;
}

function tooltipFormatterItem(field, fieldValue) {
	return '<span class="float-right teta-bidi" style="direction: rtl;">' +
		field +
		': ' +
		fieldValue +
		'</span><br/>';
}

function grabReportData(key) {
	if (!window.top.reportsData)
		window.top.reportsData = {};
	window.top.reportsData[key] = [];
	return window.top.reportsData[key];
}