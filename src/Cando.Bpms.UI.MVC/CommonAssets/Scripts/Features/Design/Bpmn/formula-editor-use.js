//ace.require("ace/ext/language_tools");
//var editorId = 'teta-editor';
//var editor = ace.edit(editorId);
//
//function setAceEditor() {
//	editor.setTheme("ace/theme/monokai");
//	editor.getSession().setMode("ace/mode/javascript");
//	editor.setOptions({
//		enableBasicAutocompletion: true,
//		enableSnippets: true,
//		enableLiveAutocompletion: false
//	});
//}
//
//function copyToClipboard() {
//	var copyTextarea = document.querySelector('#clipboard-content');
//	copyTextarea.value = editor.getValue();
//	copyTextarea.select();
//	document.execCommand('copy');
//	copyTextarea.value = "";
//	alert("Copied To The ClipBoard");
//}
//
//function clearEditorContent() {
//	editor.setValue("");
//}