---
name: grill-with-docs
description: Interview the user about a plan or design within a repo, writing resolved terms to CONTEXT.md and hard decisions as ADRs in docs/adr/ as they resolve. AUTOMATICALLY INVOKE when user says "/grill-with-docs", "grill me with docs", "interview me about this repo", or "document my plan".
---

# Grill With Docs

Interview the user relentlessly about a plan or design — like `grill-me`, but repo-aware and stateful. Resolved vocabulary lands in `CONTEXT.md` immediately. Decisions that pass all three gates land as ADRs under `docs/adr/`. Everything else stays in the conversation only.

## Rules

- Ask questions **one at a time**, with a **recommended answer** for each
- If a question can be answered by **reading the codebase**, read the codebase instead of asking
- Do not move to the next branch until the current one is resolved
- Be relentless — push back on vague or incomplete answers
- Push back when answers conflict with earlier decisions or existing `CONTEXT.md` terms

## Glossary Writing (CONTEXT.md)

When a term resolves (your project's own word for a thing, agreed on), write it to `CONTEXT.md` at the repo root immediately. Format:

```
## <Term>
<One tight definition in the project's own language. No implementation detail. No spec prose.>
```

- If a `CONTEXT-MAP.md` exists at the root marking a multi-context repo, write to the relevant context's `CONTEXT.md` instead.
- Create `CONTEXT.md` lazily — only when the first term resolves.
- The glossary is vocabulary only: no implementation details, no spec-like prose, no scratch notes.
- If a term you hear conflicts with an existing `CONTEXT.md` entry, challenge it before accepting it.

## ADR Writing (docs/adr/)

Write an ADR only when a decision passes **all three gates**:
1. Hard to reverse
2. Surprising without context
3. A real trade-off was made

If it fails any gate, the decision stays in the conversation only. Most sessions produce zero ADRs. That is correct behavior.

ADR format (`docs/adr/NNNN-<slug>.md`):
```markdown
# NNNN. <Title>

Date: <YYYY-MM-DD>

## Status
Accepted

## Context
<Why this decision needed to be made.>

## Decision
<What was decided.>

## Consequences
<What changes as a result. What becomes harder or easier.>
```

Create `docs/adr/` lazily — only when the first ADR qualifies.

## Process

1. Ask: "What plan, change, or area of the repo do you want me to grill you on?" (if not provided)
2. Read relevant codebase to answer what can be answered without asking
3. Identify the top-level decision branches
4. For each branch, drill down one question at a time until resolved; write resolved terms to `CONTEXT.md` immediately
5. At session end, write any decisions that passed all three ADR gates
6. Close: report terms written to `CONTEXT.md`, ADRs written (or none), and next step (`/to-spec` for multi-session work, `/implement` for single-session)
