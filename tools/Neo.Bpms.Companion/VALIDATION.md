# Validation record — 2026-09-12

## Companion 0.2

Run from repository root:

```powershell
python -B -m unittest discover -s tools/Neo.Bpms.Companion -p test_companion.py -v
```

The suite covers real UTF-8 stdio initialization/discovery and calls to all five
tools; malformed/oversized requests; XML namespaces, duplicates and invalid paths;
DI comment/string exclusions; file/traversal bounds; startup root restriction;
reviewed-doc search; recipe drift and current source fingerprints.

Local Python 3.12 run: 18 tests, 17 passed, one skipped because Windows did not
permit creating a symlink. Link handling is implemented but that OS-level case has
not been demonstrated locally. Initial sandbox execution failed to access temporary
fixture directories; the successful run used approved command access.

The independent Companion CI workflow runs the same suite on Windows and Linux.
Its remote result is separate from the local results recorded here.

## MVC UI

```powershell
dotnet build src/Neo.Bpms.UI.MVC/Neo.Bpms.UI.MVC.csproj --no-restore -m:1 -nr:false -p:UseSharedCompilation=false -p:BuildProjectReferences=false --verbosity quiet
```

Succeeded with zero warnings/errors against existing local dependency outputs.
This is a targeted MVC/Razor build, not a clean full-solution build. The generated
static-web-assets manifest contains css/neo-ux-polish under
_content/Neo.Bpms.UI.MVC with PreserveNewest publication.

Synthetic browser comparison checked RTL/LTR, light/dark, default desktop width and
390px mobile. No horizontal overflow was observed; input/button heights and text
colors matched before/after. Keyboard focus on the scoped button showed a 2px
currentColor outline with a 3px offset. No live authenticated BPMS application,
production data, dynamic form submission or workflow completion was exercised.

Both the repository neo-bpms Skill and personal maintain-skills-mcp Skill passed
the skill-creator structural validator. That validator is not a behavioral eval.
