---
name: to-spec
description: Turn an agreed conversation into a written spec and publish it as an issue (GitHub or local .scratch/). Use after grilling is done and the work is too large for one session. AUTOMATICALLY INVOKE when user says "/to-spec", "write a spec", "turn this into a spec", or "spec out this feature".
---

# To Spec

Turn the conversation you have just had into a **spec** — a decision record that survives context-window boundaries — and publish it as a single issue to the configured tracker (GitHub Issues or local `.scratch/` markdown files).

Do **not** interview the user. The deciding is already done. Synthesise what is known from the thread, the codebase, `CONTEXT.md`, and any ADRs in `docs/adr/`. The spec records decisions already made; it does not make new ones.

Use this when the work spans several sessions. If it fits one context window, skip the spec and implement directly.

## Process

1. **Read** the conversation, `CONTEXT.md`, and relevant ADRs in `docs/adr/`
2. **Sketch the seams** — the boundaries the feature will be tested at. Prefer existing seams over new ones. Aim for the fewest seams possible (ideally one). Present these to the user and confirm before writing the spec.
3. **Write the spec** using the template below, in the project's own vocabulary from `CONTEXT.md`
4. **Publish** it as a single issue with the `ready-for-agent` label

## Spec Template

```markdown
# <Feature/Change Title>

## Summary
<One paragraph. What this is and why it matters, in the project's own words.>

## Background
<Decisions and context that led here. Reference ADRs by filename if relevant.>

## Agreed Seams
<The test boundaries agreed with the user. List each seam and what it covers.>

## Implementation Decisions
<The specific technical choices made during grilling. Not prose — decisions.>
- <Decision 1>
- <Decision 2>

## Testing Decisions
<What will be tested at each agreed seam, and how.>

## Out of Scope
<What was explicitly refused. This section is as important as the rest.>

## Vocabulary
<Any project-specific terms used in this spec, with brief definitions if not in CONTEXT.md.>
```

## Rules

- Start writing immediately — do not open a fresh round of questions
- Use the project's nouns from `CONTEXT.md`, not generic product-management boilerplate
- Every decision in the spec must be one the user can remember making — nothing invented to fill a section
- The out-of-scope section must have real entries: things explicitly refused are the most useful lines on the page
- Confirm seams with the user before writing
- Do not link or cite ADRs inline; just respect what they say
- Do not search the tracker for related work — tell the user to do that themselves if the area is busy

## Publishing

- **GitHub**: Create a single issue with the spec body and label `ready-for-agent`
- **Local fallback**: Write to `.scratch/<slug>.md` if no GitHub tracker is configured

The `ready-for-agent` label means "no further triage needed" — not a work order.

After publishing, tell the user the issue URL or file path, and that `/to-tickets` is next.
