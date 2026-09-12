"""Serve a synthetic toolbar comparison on loopback; no BPMS app or database starts."""
from http.server import BaseHTTPRequestHandler, HTTPServer
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
CSS = ROOT / 'src/Neo.Bpms.UI.MVC/wwwroot/css/neo-ux-polish.css'


def page():
    panels = []
    for direction, language, title, action in [('rtl', 'fa', 'کارتابل درخواست‌های من', 'نمایش فیلترها'),
                                                ('ltr', 'en', 'My process requests', 'Show filters')]:
        for after in [False, True]:
            for dark in [False, True]:
                panels.append(f'''<section lang="{language}" dir="{direction}" class="sample {'dark' if dark else ''}">
<p class="caption">{'After' if after else 'Before'} · {direction.upper()} · {'Dark' if dark else 'Light'}</p>
<div class="top-page col-md-12 col-sm-12 code main-box {'neo-ux-toolbar' if after else ''}">
<div class="icons float-end"><button type="button">{action}</button></div>
<h5 class="text-secondary mb-0 mt-2 {'neo-ux-heading' if after else ''}" {'' if after else 'style="padding-top:10px"'}>
<span class="fixture-title"><svg class="page-icon" aria-hidden="true" viewBox="0 0 24 24"><path d="M4 5h16v14H4zM8 9h8M8 13h5" fill="none" stroke="currentColor" stroke-width="1.5"/></svg><span {'class="neo-ux-title"' if after else 'style="margin-right:8px"'}>{title}</span></span></h5>
</div><p class="fixture-description">{'اطلاعات نمونه؛ بدون اتصال به سامانه' if direction == 'rtl' else 'Synthetic data; no application connection'}</p>
<input aria-label="Unchanged control" placeholder="Unchanged control"><button disabled>Disabled</button>
</section>''')
    return '''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Neo BPMS toolbar comparison</title>
<style>
*{box-sizing:border-box}body{margin:0;background:#f1f5f9;font:16px/1.7 Tahoma,Arial,sans-serif;color:#1e293b}
main{max-width:1300px;margin:auto;padding:24px}h1{font-size:24px;margin:0}header p{margin:0 0 24px;color:#475569}
.samples{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:20px}.sample{padding:20px;background:#fff;border:1px solid #e2e8f0;border-radius:12px;min-width:0}
.dark{background:#172033;color:#e2e8f0}.caption{font:12px/1.5 Tahoma,sans-serif;opacity:.8;margin:0 0 16px;text-align:start}
.top-page{padding:12px 16px;border:1px solid #cbd5e1;min-height:96px}.dark .top-page{border-color:#64748b}
.float-end{float:inline-end}.text-secondary{color:inherit}h5{font:inherit;font-weight:bold;margin:0}.mt-2{margin-top:8px}.page-icon{width:24px;height:24px;vertical-align:middle}.fixture-title{overflow-wrap:anywhere}
button,input{font:inherit;border:1px solid #94a3b8;border-radius:4px;background:transparent;color:inherit;padding:4px 8px}button{cursor:pointer}button:disabled{cursor:default;opacity:.5}input{max-width:65%}
.fixture-description{font-size:13px;opacity:.8}.dark input::placeholder{color:#cbd5e1}@media(max-width:700px){.samples{grid-template-columns:1fr}main{padding:12px}.sample{padding:12px}.top-page{padding:8px}}
</style><link rel="stylesheet" href="/polish.css"><main><header><h1>Neo BPMS · shared toolbar</h1><p>Representative markup · compare spacing and keyboard focus. This is not a live application screenshot.</p></header><div class="samples">''' + ''.join(panels) + '</div></main></html>'


class Preview(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == '/':
            data, content_type = page().encode(), 'text/html; charset=utf-8'
        elif self.path == '/polish.css':
            data, content_type = CSS.read_bytes(), 'text/css; charset=utf-8'
        else:
            self.send_error(404)
            return
        self.send_response(200)
        self.send_header('Content-Type', content_type)
        self.send_header('Content-Length', str(len(data)))
        self.end_headers()
        self.wfile.write(data)


if __name__ == '__main__':
    print('Synthetic UI preview: http://127.0.0.1:8876', flush=True)
    HTTPServer(('127.0.0.1', 8876), Preview).serve_forever()
