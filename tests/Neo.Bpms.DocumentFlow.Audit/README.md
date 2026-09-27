# Document-flow acceptance audit

Runs requirements against linked production source. Failures are intentionally
reported as failures (exit 1) until the implementation is corrected. This is a
separate opt-in audit, not a green release gate or an end-to-end certification.

See [the Persian audit report](../../docs/DOCUMENT-FLOW-AUDIT.fa.md) for findings,
source fingerprints, scope, boundary doubles and the remaining real-server checks.

```powershell
dotnet run --project tests/Neo.Bpms.DocumentFlow.Audit --disable-build-servers -p:UseSharedCompilation=false
node --test tests/Neo.Bpms.UI.MVC.Tests/Frontend/DocumentFlow.audit.test.cjs
```

.NET 10, NuGet restore, and sibling Neo and Hyper/Backend checkouts are required.
Override `NeoRoot` / `HyperRoot` MSBuild properties for a different checkout layout.
No application build outputs, credentials or database are used. MinIO SDK requests
are intercepted by an in-memory HTTP transport; this is not a real MinIO server.
Only a unique temporary directory is written, including traversal probes contained
within that audit directory. Small files are retained there for inspection.
