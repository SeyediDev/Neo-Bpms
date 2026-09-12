#!/usr/bin/env python3
"""Neo.Bpms Companion 0.2: bounded UTF-8 JSON-RPC over stdio."""
import argparse
import json
from pathlib import Path
import sys
from bpms_tools import Companion, ToolError, MAX_FILE

PROTOCOLS = ('2025-06-18', '2024-11-05')
MAX_MESSAGE = 2 * 1024 * 1024

def tool(name, description, fields, required=()):
    return {'name': name, 'description': description,
            'annotations': {'readOnlyHint': True, 'destructiveHint': False,
                            'idempotentHint': True, 'openWorldHint': False},
            'inputSchema': {'type': 'object', 'additionalProperties': False,
                            'properties': fields, 'required': list(required)}}

ROOT = {'type': 'string', 'minLength': 1, 'maxLength': 4096,
        'description': 'Optional; must equal the server startup root.'}
TOOLS = [
    tool('bpms_inspect_project', 'Read project/framework declarations without evaluating MSBuild.', {'projectRoot': ROOT}),
    tool('bpms_search_docs', 'Search reviewed source-pinned recipes, not arbitrary project documents.',
         {'projectRoot': ROOT, 'query': {'type': 'string', 'minLength': 1, 'maxLength': 200}}, ['query']),
    tool('bpms_validate_process', 'Check BPMN XML structure and direct sequence references; no runtime validation.',
         {'document': {'type': 'string', 'minLength': 1, 'maxLength': MAX_FILE}}, ['document']),
    tool('bpms_find_registration', 'Find lexical candidates for direct generic DI registrations.',
         {'projectRoot': ROOT, 'serviceName': {'type': 'string', 'minLength': 1, 'maxLength': 200}}, ['serviceName']),
    tool('bpms_get_recipe', 'Get a recipe with current-vs-baseline source evidence.',
         {'projectRoot': ROOT, 'topic': {'type': 'string', 'enum': ['new-process', 'dynamic-form', 'human-task', 'integration']}}, ['topic']),
]

def error(identifier, code, message):
    return {'jsonrpc': '2.0', 'id': identifier, 'error': {'code': code, 'message': message}}

def arguments_valid(schema, args):
    if not isinstance(args, dict) or set(args) - schema['properties'].keys():
        return False
    if any(key not in args for key in schema['required']):
        return False
    for key, value in args.items():
        spec = schema['properties'][key]
        if not isinstance(value, str) or not spec.get('minLength', 0) <= len(value) <= spec.get('maxLength', MAX_FILE):
            return False
        if 'enum' in spec and value not in spec['enum']:
            return False
    return True

class Server:
    def __init__(self, root):
        self.companion = Companion(root)
        self.initialized = False

    def handle(self, message):
        if not isinstance(message, dict) or message.get('jsonrpc') != '2.0' or not isinstance(message.get('method'), str):
            return error(None, -32600, 'Invalid request')
        identifier = message.get('id')
        if 'id' not in message:
            return None
        if not isinstance(identifier, (str, int)) or isinstance(identifier, bool):
            return error(None, -32600, 'Invalid request id')
        params = message.get('params', {})
        if not isinstance(params, dict):
            return error(identifier, -32602, 'Invalid params')
        method = message['method']
        if method == 'initialize':
            if not isinstance(params.get('protocolVersion'), str) or not isinstance(params.get('capabilities'), dict) or not isinstance(params.get('clientInfo'), dict):
                return error(identifier, -32602, 'Invalid initialize params')
            version = params['protocolVersion']
            result = {'protocolVersion': version if version in PROTOCOLS else PROTOCOLS[0],
                      'capabilities': {'tools': {}},
                      'serverInfo': {'name': 'neo-bpms-companion', 'version': '0.2.0'}}
            self.initialized = True
        elif method == 'ping':
            result = {}
        elif method not in ('tools/list', 'tools/call'):
            return error(identifier, -32601, 'Method not found')
        elif not self.initialized:
            return error(identifier, -32000, 'Initialize the server first')
        elif method == 'tools/list':
            if params.get('cursor'):
                return error(identifier, -32602, 'Invalid cursor')
            result = {'tools': TOOLS}
        else:
            definition = next((item for item in TOOLS if item['name'] == params.get('name')), None)
            if definition is None:
                return error(identifier, -32602, 'Unknown tool')
            args = params.get('arguments', {})
            if not arguments_valid(definition['inputSchema'], args):
                return error(identifier, -32602, 'Invalid tool arguments')
            try:
                data = self.companion.call(definition['name'], args)
                result = {'content': [{'type': 'text', 'text': json.dumps(data, ensure_ascii=False)}],
                          'structuredContent': data, 'isError': False}
            except ToolError as exc:
                result = {'content': [{'type': 'text', 'text': str(exc)}], 'isError': True}
            except Exception:
                return error(identifier, -32603, 'Tool execution failed')
        return {'jsonrpc': '2.0', 'id': identifier, 'result': result}

def serve(server, stream, output):
    while True:
        line = stream.readline(MAX_MESSAGE + 1)
        if not line:
            return
        if len(line) > MAX_MESSAGE:
            while line and not line.endswith(b'\n'):
                line = stream.readline(MAX_MESSAGE + 1)
            response = error(None, -32600, 'Message exceeds 2 MiB')
        else:
            try:
                message = json.loads(line.decode('utf-8'))
            except (ValueError, UnicodeError, RecursionError):
                response = error(None, -32700, 'Parse error')
            else:
                response = server.handle(message)
        if response is not None:
            output.write(json.dumps(response, ensure_ascii=False) + '\n')
            output.flush()

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project-root', type=Path, default=Path(__file__).resolve().parents[2])
    options = parser.parse_args()
    sys.stdout.reconfigure(encoding='utf-8')
    try:
        server = Server(options.project_root)
    except (ToolError, OSError):
        print('Cannot open companion knowledge or configured project root.', file=sys.stderr)
        return 1
    serve(server, sys.stdin.buffer, sys.stdout)
    return 0

if __name__ == '__main__':
    raise SystemExit(main())
