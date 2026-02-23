# Meta-Command Generator

Create concise, effective OpenCode commands using latest best practices.

## Instructions

1. **Ask for Command Purpose:**
   - What should the command do?
   - What inputs does it need?
   - What outputs should it produce?

2. **Fetch Latest OpenCode Docs:**
   - Use webfetch to get current documentation from https://opencode.ai/docs
   - Check relevant sections (commands, slash commands, variables, etc.)

3. **Generate Command File:**
   - Name: `{{COMMAND_NAME}}.md`
   - Location: Ask user where to save (default: `~/.opencode/commands/`)
   - Structure:
     ```markdown
     # {{COMMAND_TITLE}}
     
     {{BRIEF_DESCRIPTION}}
     
     ## Variables (if needed)
     - {{VAR_NAME}}: Description
     
     ## Instructions
     
     {{CONCISE_STEPS}}
     ```

4. **Optimization Rules:**
   - **Be concise** - No fluff, straight to the point
   - **Use variables** - For reusable paths/values (format: `{{VAR_NAME}}`)
   - **Clear steps** - Numbered or bulleted, actionable
   - **Skip obvious** - Don't explain basic concepts
   - **Examples only if needed** - Most commands don't need them

5. **Command Patterns:**
   
   **Simple Task:**
   ```markdown
   # Title
   Brief description.
   
   {{INSTRUCTIONS}}
   ```
   
   **With Variables:**
   ```markdown
   # Title
   Description.
   
   ## Variables
   - {{VAR}}: What it is
   
   Instructions using {{VAR}}.
   ```
   
   **Multi-Step:**
   ```markdown
   # Title
   Description.
   
   ## Steps
   1. Do this
   2. Do that
   3. Final step
   ```

6. **Save and Confirm:**
   - Write to specified location
   - Show command name for user to invoke
   - Confirm file path

## Command Categories

**Code Generation:**
- Ask what to generate
- Specify patterns/conventions
- Output location

**Documentation:**
- Ask for topic
- Reference existing docs if available
- Structure and format requirements

**Analysis:**
- What to analyze
- What insights to extract
- Output format

**Integration:**
- What systems/APIs
- Authentication approach
- Data transformation needs

**Automation:**
- Workflow to automate
- Trigger conditions
- Success criteria

## Example Interaction

**User:** "Create command to analyze API performance"

**Assistant:** 
- Command name? → `analyze-api-performance`
- Location? → `~/.opencode/commands/`
- What metrics? → Response time, error rate, throughput
- Data source? → Log files
- Output format? → Markdown report

*Creates concise command file with variables and clear steps*

## Anti-Patterns (Avoid)

❌ Long explanations
❌ Redundant instructions
❌ Over-documentation
❌ Multiple examples for simple tasks
❌ Obvious pre-requisites

## Best Practices

✅ One clear purpose per command
✅ Variables for reusable values
✅ Minimal but complete instructions
✅ Action-oriented language
✅ Reference docs when needed
