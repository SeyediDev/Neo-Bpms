import io
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch

from bpms_tools import (BPMN, MAX_FILE, Companion, Scan, ToolError,
                        find_registration, inspect_project, validate_process)
from neo_bpms_mcp import Server, serve, MAX_MESSAGE

HERE = Path(__file__).resolve().parent
INIT = {'jsonrpc': '2.0', 'id': 1, 'method': 'initialize', 'params': {
    'protocolVersion': '2025-06-18', 'clientInfo': {'name': 'test', 'version': '1'}, 'capabilities': {}}}


def bpmn(body):
    return f'<b:definitions xmlns:b="{BPMN}"><b:process id="p">{body}</b:process></b:definitions>'


HEALTHY = bpmn('<b:startEvent id="s"/><b:userTask id="t"/><b:endEvent id="e"/>'
               '<b:sequenceFlow id="f" sourceRef="s" targetRef="t"/>'
               '<b:sequenceFlow id="g" sourceRef="t" targetRef="e"/>')


class CompanionTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)

    def write(self, path, text):
        target = self.root / path
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_text(text, encoding='utf-8')
        return target

    def test_inspection_prunes_generated_trees_and_does_not_evaluate_projects(self):
        self.write('src/App/App.csproj', '<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup><Target Name="Build"><Exec Command="never run me"/></Target></Project>')
        self.write('node_modules/secret.csproj', '<bad>')
        self.write('obj/generated.csproj', '<bad>')
        report = inspect_project(self.root)
        self.assertEqual([p['name'] for p in report['projects']], ['App'])
        self.assertEqual(report['projects'][0]['declaredFrameworks'], ['net10.0'])
        self.assertEqual(report['filesRead'], 1)

    def test_di_recognizes_multiline_qualified_registration_without_comment_or_string_hits(self):
        self.write('Setup.cs', '// services.AddScoped<IThing, Wrong>();\n'
                   'var s = "services.AddScoped<IThing, Wrong>()";\n'
                   'services.TryAddScoped<\n N.IThing, N.Thing\n>();')
        report = find_registration(self.root, 'IThing')
        self.assertEqual(len(report['findings']), 1)
        self.assertEqual(report['findings'][0]['implementation'], 'N.Thing')
        self.assertEqual(report['findings'][0]['evidence']['line'], 3)

    def test_unsupported_di_is_explicitly_incomplete(self):
        self.write('Setup.cs', '#if NEVER\nservices.AddSingleton<IThing, Thing>();\n#endif')
        report = find_registration(self.root, 'IThing')
        self.assertEqual(report['status'], 'incomplete')
        self.assertEqual(report['findings'], [])

    def test_di_with_simple_configuration_interpolation(self):
        self.write('Setup.cs', 'var key = $"{nameof(DomainProvider.Domain)}Connection";\n'
                   'var fake = $"services.AddScoped<IThing, Wrong>(); {key}";\n'
                   'services.AddSingleton<IThing, Thing>();')
        report = find_registration(self.root, 'IThing')
        self.assertEqual([f['implementation'] for f in report['findings']], ['Thing'])
        self.assertEqual(report['findings'][0]['evidence']['line'], 3)

    def test_complex_interpolation_is_not_reported_as_di(self):
        self.write('Setup.cs', 'var text = $"{map["name"]}"; services.AddScoped<IThing, Thing>();')
        report = find_registration(self.root, 'IThing')
        self.assertEqual(report['status'], 'incomplete')
        self.assertEqual(report['findings'], [])

    def test_file_and_entry_limits_report_incomplete(self):
        self.write('Huge.cs', 'x' * (MAX_FILE + 1))
        self.assertEqual(find_registration(self.root, 'IThing')['status'], 'incomplete')
        for n in range(3):
            self.write(f'{n}.cs', '')
        with patch('bpms_tools.MAX_ENTRIES', 1):
            scan = Scan(self.root)
            list(scan.files())
            self.assertIn('entry-limit', scan.gaps)

    def test_links_cannot_escape_root(self):
        with tempfile.TemporaryDirectory() as outside:
            external = Path(outside)
            (external / 'Secret.cs').write_text('services.AddScoped<IThing, Secret>();')
            try:
                (self.root / 'linked').symlink_to(external, target_is_directory=True)
            except OSError:
                self.skipTest('OS does not permit creating symlinks')
            self.assertEqual(find_registration(self.root, 'IThing')['findings'], [])
            with self.assertRaises(ToolError):
                Companion(self.root / 'linked')

    def test_caller_cannot_select_a_different_root(self):
        with tempfile.TemporaryDirectory() as outside:
            with self.assertRaises(ToolError):
                Companion(self.root).call('bpms_inspect_project', {'projectRoot': outside})

    def test_doc_search_never_reads_arbitrary_project_markdown(self):
        self.write('docs/secrets.md', 'needle password=do-not-output')
        report = Companion(self.root).call('bpms_search_docs', {'query': 'needle'})
        self.assertEqual(report['hits'], [])
        self.assertNotIn('do-not-output', json.dumps(report))

    def test_recipes_detect_source_drift(self):
        companion = Companion(self.root)
        recipe = companion.recipe('integration')
        self.assertTrue(recipe['evidence'])
        self.assertTrue(all(not e['matchesBaseline'] for e in recipe['evidence']))
        self.assertTrue(any('returns false' in step for step in recipe['steps']))

    def test_reviewed_recipe_sources_match_this_checkout(self):
        companion = Companion(HERE.parents[1])
        for topic in companion.knowledge['recipes']:
            with self.subTest(topic=topic):
                self.assertTrue(all(e['matchesBaseline'] for e in companion.recipe(topic)['evidence']),
                                'Review the changed implementation and recipe before refreshing evidence.')

    def test_bpmn_accepts_namespace_alias_and_default_namespace(self):
        self.assertEqual(validate_process(HEALTHY)['findings'], [])
        default = HEALTHY.replace('xmlns:b=', 'xmlns=').replace('b:', '')
        self.assertEqual(validate_process(default)['findings'], [])
        self.assertEqual(validate_process(HEALTHY)['status'], 'review-required')

    def test_bpmn_rejects_malformed_dtd_and_oversized_inputs(self):
        for document in ('<definitions>', '<!DOCTYPE x><x/>', 'x' * (MAX_FILE + 1), '<definitions/>'):
            with self.subTest(document=document[:25]), self.assertRaises(ToolError):
                validate_process(document)

    def test_bpmn_ignores_comments_and_finds_duplicate_dangling_and_unreachable_nodes(self):
        doc = bpmn('<!-- <b:startEvent id="fake"/> -->'
                   '<b:startEvent id="s"/><b:userTask id="t"/><b:endEvent id="e"/>'
                   '<b:sequenceFlow id="duplicate" sourceRef="s" targetRef="missing"/>'
                   '<b:sequenceFlow id="duplicate" sourceRef="t" targetRef="e"/>')
        codes = {f['code'] for f in validate_process(doc)['findings']}
        self.assertTrue({'BPMS-BPMN001', 'BPMS-BPMN005', 'BPMS-BPMN006'} <= codes)
        implicit = validate_process(bpmn('<!-- <b:startEvent/> --><b:task id="t"/>'))
        self.assertIn('BPMS-BPMN002', {f['code'] for f in implicit['findings']})

    def test_bpmn_findings_do_not_echo_document_values(self):
        report = validate_process(bpmn('<b:task id="password=private"/>'))
        self.assertNotIn('private', json.dumps(report))

    def test_protocol_errors_preserve_id_and_notifications_are_silent(self):
        server = Server(self.root)
        server.handle(INIT)
        self.assertIsNone(server.handle({'jsonrpc': '2.0', 'method': 'notifications/initialized'}))
        for method, params, code in [('unknown', {}, -32601),
                                     ('tools/call', {'name': 'missing'}, -32602),
                                     ('tools/call', {'name': 'bpms_find_registration', 'arguments': {'serviceName': 12}}, -32602)]:
            response = server.handle({'jsonrpc': '2.0', 'id': 'request', 'method': method, 'params': params})
            self.assertEqual(response['id'], 'request')
            self.assertEqual(response['error']['code'], code)

    def test_protocol_recovers_after_bad_json_and_oversized_frame(self):
        lines = b'bad json\n' + b'x' * (MAX_MESSAGE + 1) + b'\n' + json.dumps(INIT).encode() + b'\n'
        output = io.StringIO()
        serve(Server(self.root), io.BytesIO(lines), output)
        responses = [json.loads(line) for line in output.getvalue().splitlines()]
        self.assertEqual([r['error']['code'] for r in responses[:2]], [-32700, -32600])
        self.assertEqual(responses[2]['result']['serverInfo']['version'], '0.2.0')

    def test_real_stdio_exercises_all_five_tools(self):
        self.write('Setup.cs', 'services.AddScoped<IExample, Example>();')
        self.write('Example.csproj', '<Project><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>')
        requests = [INIT, {'jsonrpc': '2.0', 'method': 'notifications/initialized'},
                    {'jsonrpc': '2.0', 'id': 2, 'method': 'tools/list'}]
        calls = [('bpms_inspect_project', {}), ('bpms_search_docs', {'query': 'فرم'}),
                 ('bpms_validate_process', {'document': HEALTHY}),
                 ('bpms_find_registration', {'serviceName': 'IExample'}),
                 ('bpms_get_recipe', {'topic': 'integration'})]
        for n, (name, arguments) in enumerate(calls, 3):
            requests.append({'jsonrpc': '2.0', 'id': n, 'method': 'tools/call', 'params': {'name': name, 'arguments': arguments}})
        proc = subprocess.run([sys.executable, str(HERE / 'neo_bpms_mcp.py'), '--project-root', str(self.root)],
                              input='\n'.join(json.dumps(r) for r in requests) + '\n',
                              capture_output=True, text=True, encoding='utf-8', timeout=30)
        self.assertEqual(proc.returncode, 0, proc.stderr)
        responses = [json.loads(line) for line in proc.stdout.splitlines()]
        self.assertEqual(len(responses), 7)
        self.assertEqual(len(responses[1]['result']['tools']), 5)
        self.assertTrue(all(not r['result']['isError'] for r in responses[2:]))
        self.assertTrue(responses[3]['result']['structuredContent']['hits'])
        self.assertEqual(responses[5]['result']['structuredContent']['findings'][0]['implementation'], 'Example')


if __name__ == '__main__':
    unittest.main()
