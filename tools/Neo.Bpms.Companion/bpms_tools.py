"""Bounded source inspection. No application assemblies, scripts or database access."""
import hashlib
import json
import os
from pathlib import Path
import re
import stat
import xml.etree.ElementTree as ET

MAX_FILE = 512 * 1024
MAX_TOTAL = 8 * 1024 * 1024
MAX_ENTRIES = 15000
SKIP = {'.git', '.agents', '.codex', '.vs', 'bin', 'obj', 'node_modules',
        'dist', 'wwwroot', 'artifacts', '__pycache__', 'commonassets'}
BPMN = 'http://www.omg.org/spec/BPMN/20100524/MODEL'


class ToolError(ValueError):
    """Safe error message that may be returned to the client."""


def linked(path):
    info = path.lstat()
    return stat.S_ISLNK(info.st_mode) or bool(getattr(info, 'st_file_attributes', 0) & 0x400)


def safe_path(path):
    path = Path(os.path.abspath(path))
    for part in (path, *path.parents):
        if linked(part):
            raise ToolError('Linked paths are not supported.')
    return path


def read_text(path):
    path = safe_path(path)
    if not path.is_file():
        raise ToolError('Expected a regular file.')
    with path.open('rb') as stream:
        data = stream.read(MAX_FILE + 1)
    if len(data) > MAX_FILE:
        raise ToolError('File exceeds the 512 KiB limit.')
    try:
        return data.decode('utf-8-sig')
    except UnicodeError:
        raise ToolError('File is not UTF-8.') from None


def parse_xml(document):
    if len(document.encode('utf-8')) > MAX_FILE:
        raise ToolError('XML exceeds the 512 KiB limit; input was not truncated.')
    if re.search(r'<!\s*(DOCTYPE|ENTITY)\b', document, re.I):
        raise ToolError('DTD and entity declarations are not supported.')
    try:
        return ET.fromstring(document)
    except ET.ParseError:
        raise ToolError('Malformed XML.') from None


class Scan:
    def __init__(self, root):
        self.root = root
        self.entries = 0
        self.bytes_read = 0
        self.files_read = 0
        self.gaps = set()

    def files(self):
        stack = [(self.root, 0)]
        while stack:
            folder, depth = stack.pop()
            try:
                safe_path(folder)
                with os.scandir(folder) as entries:
                    for entry in entries:
                        self.entries += 1
                        if self.entries > MAX_ENTRIES:
                            self.gaps.add('entry-limit')
                            return
                        if entry.name.lower() in SKIP:
                            continue
                        path = Path(entry.path)
                        if linked(path):
                            self.gaps.add('linked-entry')
                            continue
                        if entry.is_dir(follow_symlinks=False):
                            if depth < 20:
                                stack.append((path, depth + 1))
                            else:
                                self.gaps.add('depth-limit')
                        elif entry.is_file(follow_symlinks=False):
                            if path.suffix in {'.cs', '.csproj', '.props'} and not path.name.endswith(('.g.cs', '.Designer.cs', '.generated.cs')):
                                yield path
            except (OSError, ToolError):
                self.gaps.add('unreadable-directory')

    def read(self, path):
        if self.bytes_read >= MAX_TOTAL:
            self.gaps.add('total-byte-limit')
            return None
        try:
            text = read_text(path)
            size = len(text.encode('utf-8'))
            if self.bytes_read + size > MAX_TOTAL:
                self.gaps.add('total-byte-limit')
                return None
            self.bytes_read += size
            self.files_read += 1
            return text
        except (OSError, ToolError):
            self.gaps.add('skipped-file')
            return None

    def coverage(self):
        return {'entriesVisited': self.entries, 'filesRead': self.files_read,
                'bytesRead': self.bytes_read, 'coverageGaps': sorted(self.gaps),
                'status': 'incomplete' if self.gaps else 'review-required'}


def evidence(root, path, text, offset=0):
    return {'path': path.relative_to(root).as_posix(),
            'line': text.count('\n', 0, offset) + 1}


def inspect_project(root):
    scan = Scan(root)
    projects, properties = [], []
    for path in scan.files():
        if path.suffix not in {'.csproj', '.props'}:
            continue
        text = scan.read(path)
        if text is None:
            continue
        try:
            tree = parse_xml(text)
        except ToolError:
            scan.gaps.add('unparsed-project')
            continue
        frameworks = []
        for elem in tree.iter():
            if elem.tag.split('}')[-1] in {'TargetFramework', 'TargetFrameworks'}:
                # Only return framework identifiers, never arbitrary property values.
                value = elem.text or ''
                if re.fullmatch(r'net[a-zA-Z0-9.;-]+', value):
                    frameworks.extend(value.split(';'))
                else:
                    scan.gaps.add('unresolved-framework-property')
        item = {'evidence': evidence(root, path, text), 'declaredFrameworks': frameworks}
        if path.suffix == '.csproj':
            item['name'] = path.stem
            projects.append(item)
        elif frameworks:
            properties.append(item)
        if len(projects) + len(properties) >= 100:
            scan.gaps.add('result-limit')
            break
    return {**scan.coverage(), 'projects': projects, 'frameworkPropertyFiles': properties,
            'limitations': ['Static declarations only; MSBuild imports, conditions and installed packages are not evaluated.']}


def mask_csharp(text):
    # Conservative lexical support. Reject constructs whose interpolation/preprocessing
    # would require a C# parser instead of making false registration claims.
    if re.search(r'^\s*#', text, re.M) or '"""' in text:
        return None
    pattern = r'//[^\n]*|/\*[\s\S]*?\*/|(?:\$@|@\$|@)"(?:""|[^"])*"|\$?"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
    def mask(match):
        token = match.group()
        if token.startswith(('$"', '$@"', '@$"')):
            # Support common configuration names, but not arbitrary interpolation
            # expressions/nested strings. The entire literal remains excluded.
            body = token[token.index('"') + 1:-1].replace('{{', '').replace('}}', '')
            body = re.sub(r'\{(?:nameof\([A-Za-z_][\w.]*\)|[A-Za-z_][\w.]*)\}', '', body)
            if '{' in body or '}' in body:
                raise ToolError('Unsupported interpolation.')
        return re.sub(r'[^\n]', ' ', token)
    try:
        return re.sub(pattern, mask, text)
    except ToolError:
        return None


def find_registration(root, service):
    if not re.fullmatch(r'[A-Za-z_][A-Za-z0-9_.]{0,199}', service):
        raise ToolError('serviceName must be a simple or qualified C# type name.')
    scan, findings = Scan(root), []
    pattern = re.compile(r'\b((?:Try)?Add(?:Scoped|Singleton|Transient))\s*<\s*([\w.]+)\s*(?:,\s*([\w.]+)\s*)?>\s*\(')
    for path in scan.files():
        if path.suffix != '.cs':
            continue
        text = scan.read(path)
        if text is None:
            continue
        code = mask_csharp(text)
        if code is None:
            scan.gaps.add('unsupported-csharp-syntax')
            continue
        for match in pattern.finditer(code):
            registered = match[2]
            if registered != service and ('.' in service or registered.rsplit('.', 1)[-1] != service):
                continue
            findings.append({'code': 'BPMS-DI001', 'certainty': 'lexical-candidate',
                             'registration': match[1], 'service': registered,
                             'implementation': match[3], 'evidence': evidence(root, path, text, match.start())})
            if len(findings) >= 50:
                scan.gaps.add('result-limit')
                return {**scan.coverage(), 'findings': findings, 'limitations': DI_LIMITS}
    return {**scan.coverage(), 'findings': findings, 'limitations': DI_LIMITS}


DI_LIMITS = ['Only direct generic Add/TryAdd Scoped/Singleton/Transient calls are candidates.',
             'No symbol resolution, typeof registrations, runtime ordering or container validation; zero matches does not prove missing DI.']


def validate_process(document):
    tree = parse_xml(document)
    if tree.tag != '{' + BPMN + '}definitions':
        raise ToolError('Expected BPMN definitions in the BPMN MODEL namespace.')
    elements = list(tree.iter())
    if len(elements) > 5000:
        raise ToolError('XML exceeds the 5000-element limit.')
    findings = []
    def add(code, message, elem, severity='warning'):
        if len(findings) < 100:
            findings.append({'code': code, 'severity': severity, 'certainty': 'structural',
                             'message': message, 'evidence': {'elementIndex': elements.index(elem) + 1}})
    ids = set()
    for elem in elements:
        identifier = elem.get('id')
        if identifier:
            if identifier in ids:
                add('BPMS-BPMN001', 'Duplicate identifier.', elem, 'error')
            ids.add(identifier)
    processes = tree.findall('{' + BPMN + '}process')
    if not processes:
        add('BPMS-BPMN004', 'No direct process to inspect.', tree)
    node_types = {'startEvent', 'endEvent', 'intermediateCatchEvent', 'intermediateThrowEvent',
                  'boundaryEvent', 'task', 'userTask', 'serviceTask', 'manualTask', 'scriptTask',
                  'businessRuleTask', 'sendTask', 'receiveTask', 'callActivity', 'subProcess',
                  'exclusiveGateway', 'inclusiveGateway', 'parallelGateway', 'eventBasedGateway', 'complexGateway'}
    for process in processes:
        nodes = {el.get('id'): el for el in process if el.tag.startswith('{' + BPMN + '}') and el.tag.split('}')[-1] in node_types and el.get('id')}
        starts = [key for key, el in nodes.items() if el.tag.endswith('}startEvent')]
        if not starts:
            add('BPMS-BPMN002', 'No explicit start event; implicit-start semantics are not evaluated.', process)
        if not any(el.tag.endswith('}endEvent') for el in nodes.values()):
            add('BPMS-BPMN003', 'No explicit end event; this alone does not establish invalid BPMN.', process)
        edges = {key: [] for key in nodes}
        for flow in process.findall('{' + BPMN + '}sequenceFlow'):
            source, target = flow.get('sourceRef'), flow.get('targetRef')
            if source not in nodes or target not in nodes:
                add('BPMS-BPMN005', 'Sequence reference does not resolve to a recognized node in this process.', flow, 'error')
            else:
                edges[source].append(target)
        # Boundary events and subprocess semantics require runtime-aware analysis.
        complex_process = any(el.tag.endswith(('}boundaryEvent', '}subProcess')) for el in nodes.values())
        if complex_process:
            add('BPMS-BPMN007', 'Subprocess/boundary-event reachability is outside this validator.', process)
        elif starts:
            seen, queue = set(), list(starts)
            while queue:
                key = queue.pop()
                if key not in seen:
                    seen.add(key)
                    queue.extend(edges[key])
            for key in nodes.keys() - seen:
                add('BPMS-BPMN006', 'No sequence-flow path from an explicit start event.', nodes[key])
    return {'status': 'review-required', 'findings': findings, 'findingLimitReached': len(findings) == 100,
            'limitations': ['XML structure only, not XSD conformance or Neo runtime validity.',
                            'Only direct process sequence-flow graphs are inspected; no JSON, DMN, expressions or execution.',
                            'Evidence is a one-based XML element index, not a source line. User document values are not echoed.']}


class Companion:
    def __init__(self, root):
        self.root = safe_path(root)
        if not self.root.is_dir():
            raise ToolError('Configured project root must be a directory.')
        self.knowledge = json.loads(read_text(Path(__file__).parent / 'knowledge.json'))

    def check_root(self, requested):
        if requested is not None and safe_path(requested) != self.root:
            raise ToolError('projectRoot must match the root configured when the server started.')

    def recipe(self, topic):
        recipe = self.knowledge['recipes'].get(topic)
        if recipe is None:
            raise ToolError('Unknown recipe topic.')
        source = []
        for item in recipe['evidence']:
            record = dict(item)
            try:
                data = read_text(self.root / item['path']).replace('\r\n', '\n')
                record['matchesBaseline'] = hashlib.sha256(data.encode()).hexdigest() == item['sha256']
            except (OSError, ToolError):
                record['matchesBaseline'] = False
            source.append(record)
        return {'topic': topic, 'baseline': self.knowledge['baseline'], 'steps': recipe['steps'],
                'evidence': source, 'status': 'review-required',
                'limitations': ['Recipe is source guidance, not an executed feature. Re-read changed sources before implementation.']}

    def call(self, name, args):
        self.check_root(args.get('projectRoot'))
        if name == 'bpms_inspect_project':
            return inspect_project(self.root)
        if name == 'bpms_find_registration':
            return find_registration(self.root, args['serviceName'])
        if name == 'bpms_validate_process':
            return validate_process(args['document'])
        if name == 'bpms_get_recipe':
            return self.recipe(args['topic'])
        if name == 'bpms_search_docs':
            query = args['query'].casefold().strip()
            if not query:
                raise ToolError('query must not be blank.')
            hits = [self.recipe(topic) for topic, item in self.knowledge['recipes'].items()
                    if query in (topic + ' ' + ' '.join(item['steps'])).casefold()]
            return {'scope': 'reviewed companion knowledge only', 'hits': hits,
                    'baseline': self.knowledge['baseline']}
        raise ToolError('Unknown tool.')
