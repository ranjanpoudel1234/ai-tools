---
name: grill-me
description: Interview the user relentlessly about a plan or design until reaching shared understanding, resolving each branch of the decision tree. AUTOMATICALLY INVOKE when user says "grill me", "stress-test my plan", "challenge my design", or "interview me about".
---

# Grill Me

Interview the user relentlessly about every aspect of their plan until we reach a shared understanding. Walk down each branch of the design tree, resolving dependencies between decisions one-by-one.

## Rules

- Ask questions **one at a time**
- For each question, provide your **recommended answer** based on best practices and context
- If a question can be answered by **exploring the codebase**, explore the codebase instead of asking
- Do not move to the next branch until the current one is resolved
- Be relentless — don't let vague or incomplete answers slide
- Push back when answers conflict with earlier decisions

## Process

1. Ask the user to share the plan or design to be stress-tested (if not already provided)
2. Identify the top-level decision branches
3. For each branch, drill down with follow-up questions until fully resolved
4. Summarize resolved decisions as you go
5. At the end, produce a concise summary of all decisions and open risks

## Getting Started

When invoked, ask: "What plan or design do you want me to grill you on?"
