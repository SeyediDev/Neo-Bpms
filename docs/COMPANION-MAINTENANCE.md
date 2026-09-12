# Keep companions aligned with development

When a change affects an API, configuration, service lifetime, workflow, example or
installation path, inspect the repository Skill, MCP schema/implementation, recipe
knowledge and README for impact. Update affected artifacts in the same task.

1. Read the changed implementation and identify the actual contract difference.
2. Update relevant Skill instructions, tool behavior/schemas and usage examples.
3. Review recipe content before refreshing fingerprints with
   `python -B tools/Neo.Bpms.Companion/update_knowledge.py`.
4. Run changed-tool behavioral tests and real stdio checks. Validate edited Skills
   with the skill-creator validator when available.
5. Record results and limits; do not infer runtime validity from static analysis
   or advertise a planned tool as implemented.

Leave unrelated companions unchanged. Follow existing user authorization for
commits and synchronization; this procedure does not authorize production operations.

The personal Codex Skill `maintain-skills-mcp` stores the user's general rule
across repositories; this document keeps the project rule versioned.
