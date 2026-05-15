---
name: meta-skill
description: Expert skill creation assistant that guides you through building new OpenCode and Claude Code skills using latest documentation, best practices, and proven patterns from existing skills. Automatically validates structure, enforces naming conventions, and generates production-ready skill files.
license: MIT
compatibility: opencode
metadata:
  version: "1.0.0"
  author: "261906"
  category: "development"
  tags: "skill-creation,documentation,best-practices"
---

# Meta-Skill: OpenCode & Claude Code Skill Creator

**Purpose**: Expert assistant for creating new OpenCode/Claude Code skills. Guides you through the entire skill creation process using the latest documentation, proven patterns, and best practices from existing skills.

---

## 🎯 What This Skill Does

This skill helps you create **production-ready OpenCode/Claude Code skills** by:

1. **Guiding you through the skill creation process** with targeted questions
2. **Validating all requirements** (naming, frontmatter, structure)
3. **Providing templates and examples** from proven skills
4. **Ensuring documentation best practices** are followed
5. **Auto-generating the complete skill file** with proper formatting
6. **Testing and validating** the skill before deployment

---

## 🚀 When to Use This Skill

Invoke this skill when you want to:
- Create a new OpenCode or Claude Code skill from scratch
- Document reusable workflows or patterns
- Standardize repetitive tasks across projects
- Build domain-specific expert assistants
- Convert manual processes into automated skills
- Create skills that integrate with MCP servers, APIs, or tools

**Trigger phrases**:
- "Create a new skill"
- "Help me build a skill for..."
- "I want to make a skill that..."
- "Generate a skill for..."

---

## 📚 Latest OpenCode Skills Documentation Summary

### File Structure Requirements

**Location Options** (OpenCode searches these paths):
```
Project-local:
  .opencode/skills/<skill-name>/SKILL.md
  .claude/skills/<skill-name>/SKILL.md
  .agents/skills/<skill-name>/SKILL.md

Global:
  ~/.config/opencode/skills/<skill-name>/SKILL.md
  ~/.claude/skills/<skill-name>/SKILL.md
  ~/.agents/skills/<skill-name>/SKILL.md
```

**Discovery**: OpenCode walks up from current directory to git worktree root, loading all matching skill files.

---

### YAML Frontmatter Requirements

**Required Fields**:
- `name` (string, 1-64 chars, must match directory name)
- `description` (string, 1-1024 chars, shown to agent for skill selection)

**Optional Fields**:
- `license` (string, e.g., "MIT", "Apache-2.0")
- `compatibility` (string, e.g., "opencode", "claude")
- `metadata` (object with string key-value pairs)

**Example**:
```yaml
---
name: my-skill-name
description: Brief description of what this skill does (1-1024 chars)
license: MIT
compatibility: opencode
metadata:
  version: "1.0.0"
  author: "your-name"
  category: "productivity"
---
```

---

### Naming Convention Rules

**Skill Name Must**:
- Be 1-64 characters long
- Use lowercase letters (a-z) and numbers (0-9) only
- Use single hyphens (-) as separators between words
- NOT start or end with a hyphen
- NOT contain consecutive hyphens (--)
- Match the directory name containing SKILL.md

**Valid Examples**: `git-release`, `code-reviewer`, `mitchell-data-interactor`, `store-query`

**Invalid Examples**: `Git-Release`, `code_reviewer`, `--skill`, `skill-`, `my--skill`

**Regex Pattern**: `^[a-z0-9]+(-[a-z0-9]+)*$`

---

### Description Best Practices

The description field is **critical** because:
1. It's shown to the agent in the `skill` tool's available skills list
2. The agent uses it to decide when to invoke the skill
3. It should be **specific enough** for correct skill selection

**Good Descriptions**:
- ✅ "Query CarMax store databases using kmxstore-dotnet MCP server for repair orders, vehicles, and inventory data"
- ✅ "Reviews code for architecture, performance, API design, and maintainability. AUTOMATICALLY INVOKE when user mentions code review"
- ✅ "Interact with Mitchell1 Data Services API to retrieve vehicle maintenance schedules, labor times, and parts information"

**Poor Descriptions**:
- ❌ "Database helper" (too vague)
- ❌ "Does stuff with APIs" (not specific)
- ❌ "Utility skill" (doesn't explain when to use)

---

## 🎨 Skill Structure Patterns

Based on analysis of existing skills, here are proven patterns:

### Pattern 1: Domain Expert Skill (e.g., mitchell-data-interactor)

**Structure**:
```markdown
---
frontmatter
---

# Skill Name

## Description
Brief overview of what the skill does

## System Prompt
You are a [ROLE] assistant that helps users...

### Core Capabilities
- Capability 1
- Capability 2

### Configuration/Setup
- API endpoints
- Authentication details
- Required credentials

### Workflow Patterns
#### Pattern 1: User asks for X
1. Step 1
2. Step 2

### Response Formatting
How to format responses

### State Management
What context to maintain

### Error Handling
Common errors and solutions

### User Interaction Guidelines
1. Guideline 1
2. Guideline 2

### Example Interactions
Example scenarios

### Tools You Can Use
Which tools to use and how

### Important Reminders
- Checklist of key points

### Getting Started
Initial flow when skill is invoked
```

---

### Pattern 2: Code Review/Analysis Skill (e.g., backend-code-reviewer)

**Structure**:
```markdown
---
frontmatter
---

# Skill Name

## Communication Style
How to communicate with users

## When to Invoke (Automatic Triggers)
List of keywords and phrases

## Review Philosophy
Core principles

## Output Format
Detailed structure of feedback files

## N Key Focus Areas
### 1. Focus Area Name
**Check for:**
- Item 1
- Item 2

**Feedback Pattern:**
Example structure

**Red Flags:**
- Warning sign 1
- Warning sign 2

## Process Workflow
### Step 1: [Name]
Details

## Quick Reference
Tables and checklists

## Final Notes
Summary of principles
```

---

### Pattern 3: Data Transformation Skill (e.g., business-objective)

**Structure**:
```markdown
---
frontmatter
---

# Skill Name

## Your Role
What you help users accomplish

## Framework/Model
Detailed framework being used

## Process Flow
### Step 1: Collect Information
Questions to ask

### Step 2: Parse and Map
How to process input

### Step 3: Transform
How to transform data

### Step 4: Generate Output
Output format with examples

### Step 5: Save and Verify
Where to save, how to verify

### Step 6: Iterate
Follow-up process

## Important Guidelines
### Examples of Mapping
Concrete examples

### Handling Edge Cases
Special situations

## Ready to Begin
Starting instructions
```

---

### Pattern 4: Quick Reference/Tool Skill (e.g., store-query)

**Structure**:
```markdown
---
frontmatter
---

# Skill Name

Brief overview

## Available Tools
- Tool 1
- Tool 2

## Quick Format
Essential syntax/usage

## Common Use Cases
- Case 1
- Case 2

## Known Issues & Workarounds
### Issue Name
**Issue:** Description
**Workaround:** Solution

## Quick Workflows
### Workflow 1
Steps

## Examples
Multiple examples

## Troubleshooting
Common problems

## Security & Limits
Important restrictions

## Quick Reference
Table or checklist
```

---

## 🔧 Skill Creation Process

When user invokes this skill, follow this process:

### Step 1: Understand the Goal

**Ask the user**:
1. "What task or workflow do you want this skill to automate or assist with?"
2. "Who will use this skill? (You, your team, anyone with the repo)"
3. "Should this skill be automatic or manual invocation?"
4. "Does this skill integrate with any tools, APIs, or MCP servers?"

**Gather context**:
- Problem being solved
- Current manual process (if any)
- Expected inputs/outputs
- Integration points

---

### Step 2: Determine Skill Pattern

Based on user input, recommend one of these patterns:

1. **Domain Expert Pattern** - For API integrations, specialized knowledge domains
   - Example: "Your skill sounds like a Domain Expert pattern since you're integrating with an external API"

2. **Code Review/Analysis Pattern** - For code quality, review, or analysis tasks
   - Example: "This matches the Code Review pattern since you're analyzing code"

3. **Data Transformation Pattern** - For converting data between formats, documentation generation
   - Example: "This fits the Data Transformation pattern since you're converting notes to structured docs"

4. **Quick Reference Pattern** - For simple tools, lookups, or quick operations
   - Example: "This is best as a Quick Reference pattern since it's a straightforward tool wrapper"

Explain why this pattern fits and show the user the structure.

---

### Step 3: Generate Skill Name

**Rules**:
1. Ask user for preferred name or suggest one based on purpose
2. Convert to lowercase
3. Replace spaces/underscores with hyphens
4. Validate against regex: `^[a-z0-9]+(-[a-z0-9]+)*$`
5. Ensure it's descriptive and memorable

**Validation**:
- Length: 1-64 characters
- Format: lowercase, hyphens only
- No leading/trailing hyphens
- No consecutive hyphens

**Examples**:
- "ERO Data Helper" → `ero-data-helper`
- "Vehicle Lookup API" → `vehicle-lookup-api`
- "Test Automation Helper" → `test-automation-helper`

---

### Step 4: Write Description

**Requirements**:
- Length: 1-1024 characters
- Must be specific enough for agent selection
- Include key verbs: "Query", "Analyze", "Generate", "Review", etc.
- Mention when to auto-invoke (if applicable)

**Template**:
```
[ACTION] [WHAT] using [HOW/TOOL]. Use when [SCENARIO]. [AUTO-INVOKE KEYWORDS if applicable]
```

**Examples**:
```
Query CarMax store databases using kmxstore-dotnet MCP server. Use when user asks to query store data, repair orders, vehicles, inventory, or any store database tables.

Reviews code for architecture, performance, API design, and maintainability. AUTOMATICALLY INVOKE when user mentions "review", "PR", "pull request", or "code review".

Interact with Mitchell1 Data Services API to retrieve vehicle maintenance schedules, labor times, fluid specifications, and parts information.
```

---

### Step 5: Build Skill Content

Based on the chosen pattern, guide user through filling out each section:

**For each major section**:
1. Show the section template
2. Ask targeted questions to extract details
3. Fill in the section with user's information
4. Show preview and ask for approval

**Example for Domain Expert Pattern**:

Section: Core Capabilities
- "What are the main things this skill can do? (3-5 bullet points)"
- User answers: "Query repair orders, look up vehicles, get inventory data"
- Generated:
  ```markdown
  ### Core Capabilities
  - Query repair orders with filtering and sorting
  - Look up vehicle information by various identifiers
  - Retrieve inventory data across store locations
  ```

Continue through all sections in the pattern.

---

### Step 6: Add Metadata (Optional but Recommended)

**Ask user**:
1. "Would you like to add optional metadata?"
2. If yes, collect:
   - `version` (e.g., "1.0.0")
   - `author` (username or name)
   - `category` (e.g., "productivity", "development", "data")
   - `tags` (comma-separated)
   - `last_updated` (auto-generate current date)

---

### Step 7: Generate Complete Skill File

Assemble the complete SKILL.md file with:

1. **Frontmatter** with all required/optional fields
2. **Skill content** following chosen pattern
3. **Examples** (at least 2-3 realistic examples)
4. **Troubleshooting section** (common issues)
5. **Getting Started section** (initial invocation flow)

**Format**:
- Use proper markdown formatting
- Include clear section headers (##, ###)
- Use code blocks with language tags
- Include emoji section markers if appropriate (🎯, 📚, 🔧, ✅)
- Keep consistent indentation and spacing

---

### Step 8: Determine Installation Location

**Ask user**:
```
Where should this skill be installed?

1. Global (available everywhere)
   → ~/.config/opencode/skills/<name>/SKILL.md

2. Project-local (only in current project)
   → .opencode/skills/<name>/SKILL.md

3. Both (I'll create it in both locations)
```

Create the directory and save the file.

---

### Step 9: Validate the Skill

**Run validations**:
1. ✅ Frontmatter contains required fields (name, description)
2. ✅ Name matches directory name
3. ✅ Name follows naming convention regex
4. ✅ Description is 1-1024 characters
5. ✅ File saved to correct location
6. ✅ File is named exactly `SKILL.md` (all caps)

**Show validation results**:
```
✅ Skill Validation Results:
  ✅ Name valid: my-new-skill
  ✅ Description length: 234 characters
  ✅ Frontmatter complete
  ✅ File location: /home/ranjan-unix/.opencode/skills/my-new-skill/SKILL.md
  ✅ Ready to use!
```

---

### Step 10: Test the Skill

**Guide user to test**:
1. "Let's verify OpenCode can discover this skill"
2. Show command: Run `/skills` or check agent tool descriptions
3. "Try invoking it with a test scenario"
4. Provide example invocation based on skill purpose

---

### Step 11: Document Usage

Create a companion README.md (optional but recommended):

**Template**:
```markdown
# [Skill Name]

## Purpose
Brief purpose statement

## Installation
Where it's installed and how to enable

## Usage Examples

### Example 1: [Scenario]
\`\`\`
User prompt here
\`\`\`

Expected behavior

### Example 2: [Scenario]
\`\`\`
User prompt here
\`\`\`

Expected behavior

## Configuration
Any configuration options

## Troubleshooting
Common issues

## Version History
- v1.0.0 - Initial release
```

---

## 📋 Quick Templates

### Minimal Skill Template
```markdown
---
name: skill-name
description: What this skill does in 1-2 sentences
---

# Skill Name

## What I Do
- Thing 1
- Thing 2
- Thing 3

## When to Use Me
Describe scenarios when this skill should be invoked.

## How I Work
1. Step 1
2. Step 2
3. Step 3

## Examples
### Example 1
Scenario and expected behavior
```

### Full-Featured Skill Template
```markdown
---
name: skill-name
description: Comprehensive description with auto-invoke keywords
license: MIT
compatibility: opencode
metadata:
  version: "1.0.0"
  author: "your-name"
  category: "category"
  tags: "tag1,tag2"
---

# Skill Name

## Purpose
Detailed purpose

## System Prompt
You are a [ROLE] that...

### Core Capabilities
- Capability 1
- Capability 2

### Configuration
Setup details

### Workflow
Step-by-step process

### Examples
Concrete examples

### Troubleshooting
Common issues

### Best Practices
Guidelines

### Getting Started
Initial flow
```

---

## 🛡️ Permissions & Security

**Teach users about permission controls**:

```json
{
  "permission": {
    "skill": {
      "*": "allow",           // Allow all skills
      "my-skill": "allow",     // Specific skill always allowed
      "internal-*": "deny",    // Block internal skills
      "experimental-*": "ask"  // Prompt before loading
    }
  }
}
```

**Permission levels**:
- `allow` - Skill loads immediately
- `deny` - Skill hidden, access rejected
- `ask` - User prompted for approval

**Per-agent overrides** (in agent frontmatter):
```yaml
---
permission:
  skill:
    "private-skill": "allow"
---
```

---

## ✅ Best Practices Checklist

When creating a skill, ensure:

- [ ] **Name** follows lowercase-hyphen convention
- [ ] **Description** is specific and actionable (1-1024 chars)
- [ ] **Frontmatter** includes name and description (minimum)
- [ ] **Directory name** matches skill name exactly
- [ ] **File name** is exactly `SKILL.md` (all caps)
- [ ] **Content structure** follows a proven pattern
- [ ] **Examples** are realistic and helpful
- [ ] **Getting Started** section guides initial use
- [ ] **Auto-invoke keywords** included if automatic
- [ ] **Tools/APIs** properly documented if used
- [ ] **Troubleshooting** section covers common issues
- [ ] **File saved** to correct location
- [ ] **Tested** by invoking in OpenCode

---

## 🎓 Learning from Existing Skills

**Study these for patterns**:

1. **mitchell-data-interactor** - Perfect Domain Expert pattern
   - Clear API documentation
   - Authentication workflows
   - Multiple usage patterns
   - State management

2. **backend-code-reviewer** - Comprehensive Review pattern
   - Auto-invoke keywords
   - Structured feedback format
   - Multiple focus areas
   - Process workflows

3. **business-objective** - Data Transformation pattern
   - Detailed framework
   - Step-by-step process
   - Proactive questions
   - Validation checklist

4. **store-query** - Clean Quick Reference pattern
   - Minimal but complete
   - Known issues with workarounds
   - Quick examples
   - Troubleshooting

**Key Takeaways**:
- Start with clear purpose statement
- Use consistent section headers
- Provide concrete examples
- Include troubleshooting
- Document when to use
- Show expected outputs

---

## 🚨 Common Pitfalls to Avoid

1. **❌ Invalid naming**
   - Using uppercase, underscores, or spaces
   - Starting/ending with hyphens
   - Consecutive hyphens

2. **❌ Vague descriptions**
   - Too generic: "Helper skill"
   - Missing use cases
   - No auto-invoke keywords when needed

3. **❌ Wrong file name**
   - `skill.md` instead of `SKILL.md`
   - `README.md` instead of `SKILL.md`

4. **❌ Missing frontmatter**
   - No YAML at top
   - Missing required fields
   - Invalid YAML syntax

5. **❌ Incomplete content**
   - Missing examples
   - No getting started section
   - Unclear when to invoke

6. **❌ Wrong location**
   - Saved outside skills directory
   - Wrong path structure

---

## 🔄 Skill Iteration & Updates

**When user wants to update existing skill**:

1. Read current SKILL.md file
2. Show current structure
3. Ask what needs to change
4. Make updates preserving structure
5. Validate again
6. Show diff of changes
7. Save updated version

**Version tracking**:
```yaml
metadata:
  version: "1.1.0"
  last_updated: "2024-02-13"
  changelog: "Added new workflow patterns"
```

---

## 📚 Resources & References

**Official Documentation**:
- OpenCode Skills: https://opencode.ai/docs/skills/
- OpenCode Config: https://opencode.ai/docs/config/
- OpenCode Agents: https://opencode.ai/docs/agents/

**Related Topics**:
- Custom Tools: https://opencode.ai/docs/custom-tools/
- MCP Servers: https://opencode.ai/docs/mcp-servers/
- Agent Configuration: https://opencode.ai/docs/agents/

---

## 🎯 Ready to Create Skills!

When this skill is invoked:

1. **Greet the user**: "I'll help you create a new OpenCode/Claude Code skill! Let's build something awesome together."

2. **Start with Step 1**: Ask about the goal and context

3. **Guide through each step**: Be conversational and helpful

4. **Validate continuously**: Check requirements as you go

5. **Generate the skill**: Create complete, production-ready file

6. **Test together**: Help user verify it works

7. **Celebrate**: Acknowledge the new skill creation!

---

## 💡 Pro Tips

- **Start simple**: You can always add complexity later
- **Use examples from existing skills**: They're proven patterns
- **Test early and often**: Verify each section as you build
- **Document liberally**: Future you (or others) will thank you
- **Version your skills**: Track changes over time
- **Share with team**: Great skills benefit everyone

---

**Let's create some amazing skills! 🚀**
