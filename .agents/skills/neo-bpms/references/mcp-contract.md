# Neo.Bpms Companion 0.2 contract

Five read-only tools run through stdio with a root fixed at startup.

| Tool | Required input | Optional input |
| --- | --- | --- |
| bpms_inspect_project | none | projectRoot |
| bpms_search_docs | query | projectRoot |
| bpms_validate_process | document (BPMN XML text) | none |
| bpms_find_registration | serviceName | projectRoot |
| bpms_get_recipe | topic | projectRoot |

topic is new-process, dynamic-form, human-task or integration. projectRoot must match
the startup root. Search returns reviewed recipes, not arbitrary files.

Source evidence uses path/line and recipes include SHA-256 hashes with matchesBaseline.
XML evidence uses elementIndex; do not label it a source line. DI certainty is
lexical-candidate, not proof of runtime registration. Tool reports carry limitations
and scan coverage; they do not assert application health.

Input/schema failures are JSON-RPC errors; execution/input-document errors use isError.
Tools do not execute code, connect to databases, submit forms or mutate the project.
The maintainer-only update_knowledge.py command writes knowledge.json and is not an MCP tool.

See the repository tools/Neo.Bpms.Companion/README.md for limits and verification commands.
