# Neo BPMS

Business process, dynamic form and human-task tooling built on Neo Framework.
Private, proprietary software by Javad Seyedi.

## Project map

| Project | Responsibility |
| --- | --- |
| Neo.Bpms.Domain | Domain entities and BPMN/CMMN models |
| Neo.Bpms.Application | Application contracts |
| Neo.Bpms.Infrastructure | Process runtime, metadata loading, form structures, persistence and DI |
| Neo.Bpms.UI.MVC | Razor views, forms, cartables and shared assets |
| Neo.Bpms.Api | API modules and options |
| Neo.Bpms.UI.Resources | Localized UI resources |
| Neo.UI.AdminPanel.Core, Neo.UI.EditableGrid, Neo.UI.Monitoring | Separate TypeScript frontend projects |

The current shared target is **.NET 10** in `Directory.Build.props`. Several projects
reference Neo source through a sibling `../Neo` checkout. Set up that checkout and
the configured private package feeds before building. Frontend packages have their
own `package.json`; use the scripts in the package being changed.

```powershell
dotnet restore Neo.Bpms.sln
dotnet build Neo.Bpms.sln --no-restore
```

Runtime applications also need their normal host configuration, authentication and
data services. The Companion below runs independently of those services.

## AI development companion

### Repository Skill: neo-bpms

[The Skill](.agents/skills/neo-bpms/SKILL.md) guides process, form, cartable and
integration development using actual source contracts. It checks versions, service
lifetimes, authorization, examples and tests before suggesting changes.

Example requests:

- Use `$neo-bpms` to trace how my cartable is filtered and paginated.
- Explain the actual SendFormCommand implementation before adding an integration.
- فرم و کارتابل را بررسی کن و تغییرات را با Skill و MCP هماهنگ نگه دار.

The Skill works with source files even when MCP is not configured. Its presence in
the repository does not automatically install a server in every client.

### MCP server: Neo.Bpms Companion 0.2

Five tools are implemented in [tools/Neo.Bpms.Companion](tools/Neo.Bpms.Companion/README.md):

| Tool | Result |
| --- | --- |
| `bpms_inspect_project` | Static project/framework declarations and coverage limits |
| `bpms_search_docs` | Search of reviewed, source-pinned recipes |
| `bpms_validate_process` | BPMN XML parsing, duplicate IDs, direct sequence references and basic reachability |
| `bpms_find_registration` | Direct generic DI registration candidates with file/line evidence |
| `bpms_get_recipe` | Process, form, human-task and integration guidance with source hashes |

Run with **Python 3.10+** (tested locally with 3.12); no extra Python packages,
database, API key or hosted server are required:

```powershell
python -B tools/Neo.Bpms.Companion/neo_bpms_mcp.py --project-root E:/SJVS/Projects/Neo-Bpms
```

This starts a stdio server, not a website. An MCP client launches it and sends
JSON-RPC requests. Use [the example configuration](tools/Neo.Bpms.Companion/mcp-config.example.json)
with absolute paths matching your checkout.

Tools read only the configured root and reviewed knowledge. They do not execute
BPMS workflows, send forms, evaluate MSBuild or query databases. DI findings are
lexical candidates, not proof of the runtime container. BPMN checks are structural,
not full BPMN conformance or executable-process certification; JSON/DMN validation
is not implemented. A report with no findings still requires review.

Recipes flag source drift. For example, the integration recipe records that
`SendFormCommand.Send` returns `false` while dispatch code is commented out;
it is not presented as a functioning integration.

## Verification and UI scope

```powershell
python -B -m unittest discover -s tools/Neo.Bpms.Companion -p test_companion.py -v
```

The suite exercises all five tools through a real stdio process, XML failures,
limits, DI evidence, root restrictions and recipe drift. See the
[validation record](tools/Neo.Bpms.Companion/VALIDATION.md) for results and limitations.

The first UI refinement is limited to the shared TopBar: subtle surface styling,
logical spacing for RTL/LTR and visible keyboard focus. It removes two inline style
declarations and preserves JavaScript handlers and form behavior. The stylesheet
is shipped through the Razor library's `wwwroot` assets. See the
[UI review](docs/UI-REVIEW.fa.md) for scope and remaining issues.

The [UI and active-theme roadmap](docs/UI-THEME-PLAN.fa.md) records the source
audit, shared theme contract proposal, phased rollout and acceptance checks across
MVC, React, generated controls, overlays, charts and iframes. The first implementation is included in this repository. Browser regression
checks cover the shared controls; authenticated Hyper acceptance is recorded separately.
See the [theme integration guide and validation status](docs/UI-THEME-IMPLEMENTATION.fa.md).

The shared `NeoTheme` bridge maps the host palette to semantic CSS tokens used by
MVC adapters and React surfaces. Same-origin embedded pages inherit the parent
palette. Standalone pages support light, dark and system preferences using the
existing `neo_theme` storage key. Host colors take precedence over local preferences.

```javascript
window.NeoTheme.set({ mode: 'system' }, true); // standalone preference
window.addEventListener('neo:theme-changed', event => {
  console.log(event.detail.mode);
});
```

Run `npm run test:theme` in the existing frontend test directory after installing
its dependencies. The Edge regression checks passed: 14 column-filter tests and 12 theme tests,
including the shipped Bootstrap CSS. Two additional production-output tests
validate asset loading, hydration and live iframe theme inheritance. These
fixtures are not evidence of
authenticated Hyper acceptance.
The local Hyper Development host also has three passing preview tests for light/dark
contrast, mobile menu/scroll behavior and period navigation (`npm run test:hyper-theme`).
They use the existing sample-data preview, never an authentication bypass. Host layouts
that override `CommonIncludes` must explicitly include the canonical theme assets.
The five Companion MCP tools retain their current contracts.

## Column-header filters

The header funnel opens and focuses the existing generated filter control. It
does not submit the form or introduce a separate query. Hidden advanced controls
and their containing tabs are revealed first. Repeated activation keeps the panel
and combo open; columns without a matching filter are disabled until a control is
available. Enter and Space activate the native header button.

Run the browser regression suite (Node.js 20+):

```powershell
Set-Location tests/Neo.Bpms.UI.MVC.Tests/Frontend
npm ci
npx playwright install chromium
npm run test:column-filters
```

To use an installed Microsoft Edge instead, set `$env:NEO_TEST_BROWSER = 'msedge'`
before running the test command; downloading Chromium is then unnecessary.

The suite loads the actual ColumnFilter script, report toggle/outside-click
handlers, both multi-select implementations, Bootstrap and Persian datepicker
into representative generated markup. It checks click/keyboard behavior, hidden
filters, tabs, repeated refresh, dynamic controls, selection preservation and
the absence of unintended sorting/submission. These are browser regression tests,
not authenticated application or database end-to-end tests.

The host must serve the updated ColumnFilter.js and filter CSS through its existing
`/Content/common-assets-includes/` mapping. The test fixture reads files directly
and cannot certify that deployment mapping or browser cache invalidation.

## Maintenance rule

When code changes affect Skills, MCP contracts, recipes or usage, update them in the
same development task. Review changed source before refreshing hashes; keep README
examples aligned and run relevant behavioral checks. Unaffected companions do not
need artificial changes. [Maintenance procedure](docs/COMPANION-MAINTENANCE.md).

[راهنمای فارسی Skill و MCP](docs/NEO-BPMS-COMPANION.fa.md)

## License

Proprietary — all rights reserved by Javad Seyedi. This repository and its Companion
knowledge are private. Unauthorized copying, distribution or use is prohibited.
