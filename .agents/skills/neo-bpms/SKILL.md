---
name: neo-bpms
description: Build, inspect and troubleshoot Neo.Bpms processes, forms, tasks and integrations using the repository's real modules and conventions.
---

# Neo.Bpms development

Use this skill for work inside the Neo.Bpms repository. Start by identifying the affected module under `src`, its project file, and one existing implementation with the same responsibility.

Read the relevant csproj, central package versions and existing tests before generating code. Preserve BPMN identifiers, form validation/localization, task authorization transitions, API module enablement, and existing persistence conventions.

For diagnostics, report evidence paths and line numbers, distinguish missing evidence from a proven defect, and never expose connection strings, tokens or form data. For implementation, build the affected project and run the smallest relevant test set.

For MCP-assisted work, use `bpms_inspect_project` first, then `bpms_search_docs`; use `bpms_validate_process` only for a supplied BPMN/JSON document. Mutation tools are intentionally excluded from v1.

Read [references/mcp-contract.md](references/mcp-contract.md) when implementing or consuming the companion MCP.
