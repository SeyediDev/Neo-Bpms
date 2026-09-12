# Neo.Bpms Companion MCP v1

The companion is a read-only, source-evidence tool server. It must not start the application, connect to a database, invoke a process, submit a form or modify files.

Tools: `bpms_inspect_project` (module map and bounded evidence), `bpms_search_docs` (pinned knowledge search), `bpms_validate_process` (BPMN structure), `bpms_find_registration` (static DI evidence), and `bpms_get_recipe` (process/form/task/integration recipes).

All inputs are bounded. Responses include `code`, `severity`, `certainty`, `message`, `suggestion` and evidence with one-based line numbers. Secrets, connection strings and form data are redacted.
