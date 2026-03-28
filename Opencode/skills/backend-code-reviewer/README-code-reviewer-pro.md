# Code Reviewer Pro Skill - Usage Guide

## Overview

The **Code Reviewer Pro** skill is a custom code review assistant based on real review patterns from 20+ PRs and 100+ code review comments analyzed from your actual PR reviews in the rome-repairorder-service repository (August-November 2025).

## How to Use

### Method 1: Automatic Invocation
Claude Code will automatically suggest using this skill when appropriate contexts are detected (code reviews, architectural discussions, etc.).

### Method 2: Manual Invocation
Type the following in your Claude Code prompt:
```
/code-reviewer-pro
```

Or simply say:
```
Use the code-reviewer-pro skill to review this code
```

### Method 3: Within a Conversation
While reviewing code, you can say:
```
Review this using my code-reviewer-pro skill
```

## What It Reviews

The skill systematically checks for:

1. **Architecture & Clean Code** - DDD, layering, encapsulation, record types
2. **Dependency Injection** - Primary constructors, service lifetimes, null validation
3. **Performance** - Caching, connection pooling, memory optimization
4. **API Design** - Breaking changes, REST standards, backward compatibility
5. **Error Handling** - Defensive validation, domain exceptions
6. **Testing Quality** - Proper test structure, dependency management
7. **Code Reusability** - DRY principle, constants, factories
8. **Documentation** - ADRs, inline comments, knowledge sharing
9. **Security** - Stale data prevention, secrets management
10. **Modern C#** - Records, primary constructors, clear naming

## Example Usage Scenarios

### Scenario 1: Pre-Commit Review
```
I've made changes to the RepairOrder service. Use code-reviewer-pro to review before I commit.
```

### Scenario 2: PR Review
```
Can you review PR #156 using the code-reviewer-pro skill?
```

### Scenario 3: Architecture Discussion
```
I'm considering adding caching to this endpoint. Use code-reviewer-pro to evaluate the approach.
```

### Scenario 4: Code Refactoring
```
Review this refactored code with code-reviewer-pro to ensure we're following best practices.
```

## Review Output Format

The skill provides structured feedback in these categories:
- Architecture & Design
- Performance & Resources
- API Design
- Code Quality
- Testing
- Documentation
- Security & Data Integrity
- Summary with action items prioritized

## Customization

You can customize the skill by editing:
```
C:\Users\261906\.claude\skills\code-reviewer-pro.md
```

Add new patterns, adjust priorities, or refine the review checklist based on evolving team standards.

## Tips for Best Results

1. **Provide context**: Mention what you're trying to achieve
2. **Be specific**: Point to specific files or areas of concern
3. **Ask questions**: Use the skill to discuss trade-offs
4. **Iterate**: Use feedback to refine your approach

## Skill Strengths

Based on your actual review patterns, this skill is particularly strong at:
- Detecting breaking changes in APIs
- Identifying architecture violations
- Spotting performance issues (caching, lifetimes)
- Ensuring modern C# patterns
- Catching naming ambiguities
- Enforcing DDD/Clean Architecture principles

## Integration with Your Workflow

This skill complements:
- Your existing ADR documentation practices
- Your focus on Clean Architecture
- Your emphasis on team learning through code reviews
- Your pragmatic balance between ideals and delivery

---

**Created**: November 21, 2025
**Based on**: 20+ PRs, 100+ review comments (August-November 2025)
**Repository**: rome-repairorder-service
