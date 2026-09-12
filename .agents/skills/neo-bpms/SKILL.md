---
name: neo-bpms
description: Build, inspect and troubleshoot Neo.Bpms processes, forms, tasks and integrations using its source contracts and companion tools.
---

# Neo.Bpms development

Identify the affected module, csproj and one relevant implementation before changing it.
The current repository targets net10.0 and references a sibling Neo checkout; recheck
Directory.Build.props and package versions instead of relying on a README badge.

## Source contracts and recipes

When MCP is connected, use bpms_inspect_project for declarations, then bpms_get_recipe
for the relevant topic. bpms_search_docs searches those reviewed recipes only.
Check matchesBaseline on each source; re-read changed sources before implementing.
Without MCP, read the implementation directly using the paths below.

- Process: Infrastructure/Features/MetaLoader/MetaProcess/ProjectBpmn.cs loads
  Bpmn/bpmn.xml and merges root elements by ID. Preserve IDs and version behavior;
  Load/Save are mutations, not validation methods.
- Forms: Infrastructure/Features/Cmmn/Forms/FormStructures/FormStructures.cs defines
  field metadata and save/navigation variants. Keep labels, parent IDs, validation
  and localization aligned with the render path.
- Cartables: Infrastructure/Features/Bpms/Processes/Managers/WorkItemManager.cs combines
  user/culture/filter, counts and paging. Preserve owner restrictions and query cleanup.
  Displaying a task does not prove permission to complete it.
- Integration: Infrastructure/Features/SendFormCommand.cs currently returns false;
  dispatch is commented out. Do not recommend it as a working integration until the
  implementation and focused runtime tests establish that behavior.

All abbreviated paths above are under src/Neo.Bpms.* with the indicated layer.

## Diagnosis and frontend changes

bpms_find_registration returns lexical candidates for generic DI calls, not proof of
runtime resolution. bpms_validate_process accepts BPMN XML only; it is not JSON/DMN
validation or execution certification. Report coverage gaps and evidence accurately.

For UI work, distinguish the MVC/CommonAssets layer from separate TypeScript apps.
Prefer scoped components and logical CSS properties for RTL/LTR. Preserve form field
names, event targets, request payloads, validation and loading behavior. Verify static
asset delivery, keyboard use and representative layouts; a build alone is not visual QA.

Read [references/mcp-contract.md](references/mcp-contract.md) for exact tool arguments.
When development changes companion contracts or examples, update the affected Skill,
MCP knowledge/tests and README in the same task. Review source before refreshing hashes.
