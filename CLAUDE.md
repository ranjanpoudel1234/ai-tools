# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Purpose

This repository serves as a configuration and experimentation hub for Claude Code CLI tooling, custom skills, agents, and AI learning projects. It contains:

- Claude Code configuration files (settings, skills, agents, commands)
- Custom skills and sub-agents for specialized tasks
- AI learning plans and experimental projects
- PowerShell automation scripts for Claude Code integration

## Repository Structure

```
ai-tools/
├── ClaudeCode/configuration/        # Main configuration directory
│   ├── agents/                      # Custom sub-agent definitions
│   │   ├── meta-agent.md           # Agent for creating new agents
│   │   └── unit-test-writer.md     # Unit test generation agent
│   ├── commands/                    # Slash command definitions
│   │   ├── agent-commands/         # Commands for agent creation/management
│   │   ├── create-adr.md           # Architecture Decision Records
│   │   ├── plan-work.md            # Planning and phased approach
│   │   ├── implement-plan.md       # Plan implementation workflow
│   │   └── [other commands]        # Various workflow commands
│   ├── skills/                      # User-invocable skills
│   │   ├── backend-code-reviewer/  # C#/.NET code review skill
│   │   ├── react-ux-code-reviewer/ # React/TypeScript review skill
│   │   ├── domain-driven-design-guru/ # DDD/Clean Architecture skill
│   │   ├── competency-tracker/     # Competency documentation
│   │   ├── fork-terminal/          # Terminal session forking
│   │   └── read-avro-files/        # Apache Avro file reader
│   ├── plans/                       # Planning documents
│   │   └── ai-learning.md          # Comprehensive AI learning plan
│   ├── prompts/                     # Prompt templates
│   │   └── PRDs/                   # Product requirement documents
│   ├── settings.json               # Claude Code settings
│   ├── statusline.ps1              # PowerShell statusline script
│   └── notify-task-complete.ps1    # Notification hook script
└── README.md                        # Repository readme (minimal)
```

## Key Configuration Files

### Settings (ClaudeCode/configuration/settings.json)
- Contains permissions, hooks, and statusline configuration
- Defines approved Bash commands and web access domains
- Configures PowerShell hooks for task completion notifications

### Custom Agents (ClaudeCode/configuration/agents/)
Custom sub-agents follow the frontmatter format:
```markdown
---
name: agent-name
description: When to use this agent
tools: Read, Write, Grep, Glob, Bash
model: sonnet | haiku | opus
color: red | blue | green | yellow | purple | orange | pink | cyan
---
```

### Skills (ClaudeCode/configuration/skills/)
Skills are user-invocable commands that users trigger with `/skill-name` syntax. Each skill directory should contain documentation and implementation details.

### Commands (ClaudeCode/configuration/commands/)
Markdown files defining workflow commands. These are similar to skills but focus on specific workflows like planning, code review, ADR creation, etc.

## Common Workflows

### Creating New Agents
Use the meta-agent to create new Claude Code sub-agents:
1. The meta-agent reads Claude Code documentation
2. Takes user requirements
3. Generates a complete agent definition file in `.claude/agents/`
4. Follows the standard frontmatter format

### Planning and Implementation
Two-phase approach for complex tasks:
1. **Planning Phase** (`/plan-work`): Create detailed implementation plan in `claude/tasks/TASK_NAME.md`
2. **Implementation Phase** (`/implement-plan`): Execute the plan with continuous updates

### Code Review Workflows
- **Backend Review** (`/backend-code-reviewer`): C#/.NET code review against Microsoft standards
- **UX Review** (`/react-ux-code-reviewer`): React/TypeScript accessibility and UX review
- **DDD Review** (`/domain-driven-design-guru`): Domain-Driven Design and Clean Architecture review

### Documentation
- **ADR Creation** (`/create-adr`): Creates Architecture Decision Records following template at `docs/adrs/template.md`
- **C4 Diagrams** (`/c4-documenter`): Creates C4 architecture diagrams

## AI Learning Context

This repository supports an AI learning journey (see `ClaudeCode/configuration/plans/ai-learning.md`):
- Phase 1: GenAI & Enterprise Integration (Months 1-3)
- Phase 2: Foundational AI & ML (Months 4-9)
- Phase 3: Advanced Topics & Specialization (Months 10-12)

### AI Learning Project Organization
All AI learning work should be organized under `claude/tasks/ai-learning-progress/phaseN/`:

```
claude/tasks/ai-learning-progress/
└── phaseN/
    ├── *.md                    # Documentation, notes, results (root level)
    ├── experiments/            # Small experimental code projects
    │   └── ExperimentName/
    └── projects/               # Full implementation projects
        └── ProjectName/
```

**Important Rules:**
- Never create experiment or project folders at the repository root
- Keep documentation at phase root for easy discovery
- Use `experiments/` for proof-of-concept and learning exercises
- Use `projects/` for complete, production-ready implementations

## PowerShell Integration

### Statusline Script (statusline.ps1)
Displays current session information in Claude Code statusline:
- Debug information
- Session details
- Last user message

### Notification Hook (notify-task-complete.ps1)
Triggers notifications when tasks complete (Stop and Notification events).

## Working with This Repository

### Before Starting Work
1. Review existing agents/skills/commands to avoid duplication
2. Check if a meta-agent or specialized agent can help
3. For AI learning projects, identify the appropriate phase folder

### Creating New Content
- **New Agents**: Use meta-agent (`/meta-agent`)
- **New Skills**: Follow existing skill structure in `skills/` directory
- **New Commands**: Create markdown files in `commands/` directory
- **AI Projects**: Create under appropriate `ai-learning-progress/phaseN/` folder

### Documentation Standards
- Use markdown for all documentation
- Include clear descriptions and usage instructions
- For agents: follow frontmatter format strictly
- For skills: include README and example usage
- For AI projects: document experiments and results

## Important Conventions

### Agent Creation
- Always specify tools needed (Read, Write, Edit, Bash, Grep, Glob, etc.)
- Choose appropriate model: haiku (simple tasks), sonnet (default), opus (complex)
- Write clear delegation descriptions for automatic invocation
- Include numbered instructions and best practices

### Planning Approach
- Create detailed plans before implementation
- Store plans in `claude/tasks/TASK_NAME.md`
- Include phases, goals, and checkboxes
- Update plans with progress as work proceeds

### Code Review Focus
- Backend: Architecture, performance, API design, maintainability
- Frontend: Accessibility, UX, component design, performance
- DDD: Domain boundaries, aggregates, value objects, repositories

## User Preferences

The user (ID: 261906) prefers:
- Detailed implementation plans before starting work
- Phased approaches with clear goals
- Checkboxes for tracking progress
- Code examples in plans
- MVP focus to avoid over-planning
- Approval before proceeding with implementation
- Continuous plan updates during implementation

## Notes

- This repository is primarily for configuration and experimentation
- No build/test commands needed (configuration files only)
- PowerShell scripts integrate with Windows environment
- Git hooks and statusline provide rich IDE integration
