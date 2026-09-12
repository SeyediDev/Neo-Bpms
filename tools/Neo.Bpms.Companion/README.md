# Neo.Bpms Companion 0.2

Read-only MCP over newline-delimited UTF-8 stdio, using Python's standard library.
Run `python -B tools/Neo.Bpms.Companion/neo_bpms_mcp.py --project-root <absolute-repo-path>`.
The default root is the checkout containing the script, independent of client cwd.

## Tool arguments

| Tool | Arguments |
| --- | --- |
| `bpms_inspect_project` | Optional `projectRoot` |
| `bpms_search_docs` | `query`, optional `projectRoot` |
| `bpms_validate_process` | `document` containing BPMN XML |
| `bpms_find_registration` | `serviceName`, optional `projectRoot` |
| `bpms_get_recipe` | `topic`, optional `projectRoot` |

Topics: `new-process`, `dynamic-form`, `human-task`, `integration`.
An optional `projectRoot` must match the startup root; a model cannot switch roots.
Search is over reviewed recipes in `knowledge.json`, not arbitrary Markdown or
configuration. Source evidence contains paths, one-based lines and hashes;
XML findings use one-based element indices instead of source lines.

## Bounds and limitations

- 512 KiB per source file/XML document, 8 MiB per scan, 15,000 visited entries,
  depth 20; prunes build/vendor/assets directories and linked entries.
- Maximum 100 project results or XML findings, 50 DI candidates.
- A 2 MiB protocol frame is rejected rather than silently truncated.
- Unknown tools/invalid arguments receive JSON-RPC errors. Tool input failures use
  `isError`; request IDs are preserved and notifications are silent.
- BPMN namespace-aware XML parsing checks duplicates, direct sequence references
  and basic reachability. No XSD validation, nested subprocess execution, expressions,
  JSON or DMN. Missing start/end events are review warnings, not conformance failures.
- DI recognizes direct generic Add/TryAdd Scoped/Singleton/Transient calls. Comments
  and ordinary strings are excluded. Simple identifier/nameof interpolations are
  masked too; preprocessor/raw/complex-interpolation files are skipped with incomplete
  coverage. No symbol resolution, typeof/generic-type
  registration analysis, factory-body interpretation or runtime ordering.
- Filesystem link checks are conservative; this is not a sandbox against concurrent
  hostile filesystem replacement. Run on a trusted local checkout.

## Development

```powershell
python -B -m unittest discover -s tools/Neo.Bpms.Companion -p test_companion.py -v
# After reviewing source and recipe changes:
python -B tools/Neo.Bpms.Companion/update_knowledge.py
```

Refreshing evidence does not validate recipe semantics. Review `knowledge.json`
and run tests before committing. See [VALIDATION.md](VALIDATION.md).

Protocol references: [MCP tools](https://modelcontextprotocol.io/specification/2025-06-18/server/tools)
and [stdio transport](https://modelcontextprotocol.io/specification/2025-06-18/basic/transports).
