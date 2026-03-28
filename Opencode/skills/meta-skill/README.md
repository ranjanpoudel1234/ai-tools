# Meta-Skill: OpenCode & Claude Code Skill Creator

## Overview

The meta-skill is your expert assistant for creating new OpenCode and Claude Code skills. It guides you through the entire process using the latest documentation, proven patterns, and best practices.

## Installation

**Location**: `~/.opencode/skills/meta-skill/SKILL.md`

This is a **global skill** available in all your projects.

## What It Does

- ✅ Guides you through skill creation with targeted questions
- ✅ Validates naming conventions and requirements
- ✅ Provides templates from 4 proven patterns
- ✅ Auto-generates production-ready skill files
- ✅ Tests and validates before deployment
- ✅ Teaches best practices along the way

## Usage

### Invoke the Skill

Simply ask:
```
Create a new skill for querying databases
```

Or use the skill tool directly:
```
Load the meta-skill
```

### Trigger Phrases

- "Create a new skill"
- "Help me build a skill for..."
- "I want to make a skill that..."
- "Generate a skill for..."

## The Process

The meta-skill will guide you through:

1. **Understand the Goal** - What you want to automate
2. **Determine Pattern** - Which of 4 patterns fits best
3. **Generate Name** - Following strict naming rules
4. **Write Description** - Specific and actionable
5. **Build Content** - Section by section
6. **Add Metadata** - Version, author, tags
7. **Generate File** - Complete SKILL.md
8. **Choose Location** - Global or project-local
9. **Validate** - All requirements checked
10. **Test** - Verify it works
11. **Document** - Optional README

## Skill Patterns

The meta-skill knows 4 proven patterns:

### 1. Domain Expert Pattern
For API integrations and specialized knowledge domains.

**Examples**: mitchell-data-interactor, store-query

**Best for**: External API wrappers, specialized tools

### 2. Code Review/Analysis Pattern
For code quality, review, or analysis tasks.

**Examples**: backend-code-reviewer, react-ux-code-reviewer

**Best for**: Code analysis, PR reviews, quality checks

### 3. Data Transformation Pattern
For converting data between formats.

**Examples**: business-objective, competency-tracker

**Best for**: Documentation generation, data formatting

### 4. Quick Reference Pattern
For simple tools and quick operations.

**Examples**: store-query, fork-terminal

**Best for**: Tool wrappers, quick lookups

## Key Features

### Naming Validation
- Automatic conversion to lowercase-hyphen format
- Validates against regex: `^[a-z0-9]+(-[a-z0-9]+)*$`
- Ensures 1-64 character length

### Description Optimization
- Enforces 1-1024 character limit
- Ensures specificity for agent selection
- Includes auto-invoke keywords when needed

### Structure Enforcement
- Required YAML frontmatter
- Consistent section headers
- Proper markdown formatting
- Complete examples

### File Management
- Creates directories automatically
- Saves to correct location (global or local)
- Generates companion README
- Validates file structure

## Requirements

### Required Frontmatter Fields
```yaml
name: skill-name          # Must match directory name
description: What it does # 1-1024 characters
```

### Optional Frontmatter Fields
```yaml
license: MIT
compatibility: opencode
metadata:
  version: "1.0.0"
  author: "your-name"
  category: "productivity"
  tags: "tag1,tag2"
```

### Naming Rules
- ✅ Lowercase letters and numbers only
- ✅ Single hyphens as separators
- ✅ 1-64 characters
- ❌ No uppercase
- ❌ No underscores or spaces
- ❌ No leading/trailing hyphens
- ❌ No consecutive hyphens

## Location Options

### Global Installation
```
~/.config/opencode/skills/<skill-name>/SKILL.md
~/.claude/skills/<skill-name>/SKILL.md
~/.agents/skills/<skill-name>/SKILL.md
```

### Project-Local Installation
```
.opencode/skills/<skill-name>/SKILL.md
.claude/skills/<skill-name>/SKILL.md
.agents/skills/<skill-name>/SKILL.md
```

## Validation Checklist

The meta-skill validates:
- [x] Name follows naming convention
- [x] Description is 1-1024 characters
- [x] Frontmatter complete
- [x] Directory name matches skill name
- [x] File named exactly `SKILL.md` (all caps)
- [x] Saved to correct location
- [x] Content follows proven pattern
- [x] Examples included
- [x] Getting Started section present

## Examples

### Example 1: Creating a Database Query Skill

**User**: "Create a new skill for querying our internal customer database"

**Meta-skill**:
1. Asks about use case and API details
2. Recommends Domain Expert pattern
3. Suggests name: `customer-database-query`
4. Guides through sections
5. Generates complete SKILL.md
6. Validates and saves
7. Tests invocation

### Example 2: Creating a Code Formatter Skill

**User**: "I want a skill that formats TypeScript code according to our team's style guide"

**Meta-skill**:
1. Identifies as Code Review pattern
2. Suggests name: `typescript-formatter`
3. Collects style guide details
4. Builds structured content
5. Adds auto-invoke keywords
6. Creates file and validates
7. Shows test commands

## Best Practices

The meta-skill teaches you:

1. **Start Simple**: You can add complexity later
2. **Use Proven Patterns**: They work!
3. **Include Examples**: Make it easy to use
4. **Document Edge Cases**: Save future headaches
5. **Test Early**: Verify as you build
6. **Version Your Skills**: Track changes
7. **Share with Team**: Great skills help everyone

## Common Pitfalls Avoided

The meta-skill prevents:
- ❌ Invalid naming (uppercase, underscores, etc.)
- ❌ Vague descriptions
- ❌ Wrong file names (`skill.md` vs `SKILL.md`)
- ❌ Missing frontmatter
- ❌ Incomplete content
- ❌ Wrong installation location

## Version History

- **v1.0.0** (2024-02-13) - Initial release
  - 4 proven patterns
  - Complete validation
  - Auto-generation
  - Testing support

## Resources

- [OpenCode Skills Documentation](https://opencode.ai/docs/skills/)
- [OpenCode Agents Documentation](https://opencode.ai/docs/agents/)
- [OpenCode Custom Tools](https://opencode.ai/docs/custom-tools/)

## Contributing

Found a way to improve this skill? Update the SKILL.md file and bump the version!

## License

MIT

---

**Ready to create amazing skills? Just ask me to "create a new skill"!** 🚀
