#!/usr/bin/env python3
"""Dependency-free read-only Neo.Bpms MCP server over stdio."""
import json, re, sys
from pathlib import Path
MAX=512*1024
def out(x): return {"content":[{"type":"text","text":json.dumps(x,ensure_ascii=False)}]}
def inspect(root):
 p=Path(root).resolve(); fs=[]; mods=set()
 if not p.is_dir(): return out({"error":"projectRoot is not a directory"})
 for f in p.rglob('*'):
  if len(fs)>=5000 or not f.is_file() or any(x in f.parts for x in ('.git','bin','obj','node_modules','wwwroot')): continue
  fs.append(str(f.relative_to(p))); mods.update(x for x in f.parts if x.startswith('Neo.Bpms.'))
 return out({"projectRoot":str(p),"filesRead":len(fs),"modules":sorted(mods),"projects":[x for x in fs if x.endswith('.csproj')][:100]})
def search(root,q):
 p=Path(root).resolve(); hits=[]
 for f in p.rglob('*.md'):
  if len(hits)>=50 or any(x in f.parts for x in ('.git','node_modules')): continue
  try: ls=f.read_text(encoding='utf-8',errors='ignore').splitlines()
  except OSError: continue
  hits += [{"path":str(f.relative_to(p)),"line":i+1,"observation":l[:300]} for i,l in enumerate(ls) if q.lower() in l.lower()]
 return out({"query":q,"hits":hits[:50]})
def validate(doc):
 ids=re.findall(r'\bid=["\']([^"\']+)',doc); fs=[]
 if len(ids)!=len(set(ids)): fs.append({"code":"BPMS-BPMN001","severity":"error","message":"Duplicate element identifiers."})
 if not re.search(r'<(?:bpmn2?:)?startEvent\b',doc): fs.append({"code":"BPMS-BPMN002","severity":"error","message":"No start event found."})
 if not re.search(r'<(?:bpmn2?:)?endEvent\b',doc): fs.append({"code":"BPMS-BPMN003","severity":"error","message":"No end event found."})
 return out({"status":"review-required" if fs else "passed","findings":fs,"limitations":["Static structure only."]})
TOOLS=[{"name":"bpms_inspect_project","description":"Inspect project files and modules.","inputSchema":{"type":"object","properties":{"projectRoot":{"type":"string"}},"required":["projectRoot"]}},{"name":"bpms_search_docs","description":"Search Markdown documentation.","inputSchema":{"type":"object","properties":{"projectRoot":{"type":"string"},"query":{"type":"string"}},"required":["projectRoot","query"]}},{"name":"bpms_validate_process","description":"Validate basic BPMN XML structure.","inputSchema":{"type":"object","properties":{"document":{"type":"string"}},"required":["document"]}}]
for line in sys.stdin:
 try:
  m=json.loads(line); method=m.get('method'); p=m.get('params') or {}; r=None
  if method=='initialize': r={"jsonrpc":"2.0","id":m.get('id'),"result":{"protocolVersion":"2025-06-18","capabilities":{"tools":{}},"serverInfo":{"name":"neo-bpms-companion","version":"0.1.0"}}}
  elif method=='tools/list': r={"jsonrpc":"2.0","id":m.get('id'),"result":{"tools":TOOLS}}
  elif method=='tools/call':
   a=p.get('arguments') or {}; n=p.get('name'); v=inspect(a.get('projectRoot','.')) if n=='bpms_inspect_project' else search(a.get('projectRoot','.'),a.get('query','')) if n=='bpms_search_docs' else validate(a.get('document','')[:MAX])
   r={"jsonrpc":"2.0","id":m.get('id'),"result":v}
  if r: print(json.dumps(r,ensure_ascii=False),flush=True)
 except Exception: print(json.dumps({"jsonrpc":"2.0","id":None,"error":{"code":-32603,"message":"internal error"}}),flush=True)
