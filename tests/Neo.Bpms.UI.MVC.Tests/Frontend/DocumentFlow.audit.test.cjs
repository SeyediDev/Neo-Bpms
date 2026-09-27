const {test}=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const vm=require('node:vm');
const source=path.resolve(__dirname,'../../../src/Neo.Bpms.UI.MVC/CommonAssets/Scripts/Features/Form');
function manager() {
  const fields={}, labels=[];
  const $=selector=>selector?.__jq?selector:({__jq:true,
    val(value){if(arguments.length){fields[selector]=value;return this;}return typeof selector==='object'?selector.value:fields[selector];},
    attr(name,value){if(arguments.length===2)return this;return selector[name];},
    closest(){return this;},find(){return this;},html(value){labels.push(value);return this;},toggleClass(){return this;}
  });
  class FileReader {
    readAsDataURL(file){if(!file)throw new TypeError('No file');queueMicrotask(()=>this.onload({target:{result:'data:text/plain;base64,'+Buffer.from(file.content).toString('base64')}}));}
  }
  const context={window:{toast:{error(){}},tetaI18n:{t:x=>x}},$,FileReader};
  const js=fs.readFileSync(path.join(source,'Form.js'),'utf8');const start=js.indexOf('window.FormFileManager = function()');const end=js.indexOf('}();',start);
  assert.ok(start>=0&&end>start);vm.runInNewContext(js.slice(start,end+4),context);
  return {manager:context.window.FormFileManager,fields,labels};
}
function fine(uploads){
  let io;const context={window:{ControlBindingsManager:{registerJsControlIO(name,value){io=value;}},FormFileManager:{eSubmitAction:{Move:'Move'}},tetaI18n:{t:x=>x}},$:()=>({fineUploader:()=>uploads})};
  vm.runInNewContext(fs.readFileSync(path.join(source,'ControlsIO/FineUploaderIO.js'),'utf8'),context);return io;
}
test('ordinary uploader submits its documented data-URL and filename wire format',async()=>{
 const x=manager();x.manager.fileValueChanged({files:[{name:'sample.txt',content:'sample'}],value:'C:\\fakepath\\sample.txt',ownername:'File'});await new Promise(queueMicrotask);
 assert.equal(x.fields["input[name='File']"],'data:text/plain;base64,c2FtcGxl|sample.txt');assert.equal(x.fields['input[name="File__Action"]'],'Upload');
});
test('remove and undo retain their form action contract',()=>{const x=manager();x.manager.removeFile('File');assert.equal(x.fields['input[name="File__Action"]'],'Remove');x.manager.undoRemove('File');assert.equal(x.fields['input[name="File__Action"]'],'Nothing');});
test('pending chunk upload blocks form submission',()=>{assert.throws(()=>fine([{status:'uploading'}]).obtainOutputData('File'));});
test('successful chunk upload submits UUID and Move action',()=>{const data=fine([{status:'upload successful',uuid:'sample'}]).obtainOutputData('File');assert.equal(data.uuid,'sample');assert.equal(data.action,'Move');});
test('cancelling native file selection does not throw',()=>{const x=manager();assert.doesNotThrow(()=>x.manager.fileValueChanged({files:[],value:'',ownername:'File'}));});
test('forbidden extension is rejected without a JavaScript exception',()=>{const x=manager();assert.doesNotThrow(()=>x.manager.fileValueChanged({files:[{name:'sample.exe',type:'application/x-msdownload'}],value:'C:\\fakepath\\sample.exe',ownername:'File'}));});
test('filename markup is displayed as text',async()=>{const x=manager();x.manager.fileValueChanged({files:[{name:'<b>x.txt',content:'sample'}],value:'C:\\fakepath\\<b>x.txt',ownername:'File'});await new Promise(queueMicrotask);assert.ok(!x.labels.some(x=>x.includes('<b>')),'Filename passed unescaped to jQuery.html');});
