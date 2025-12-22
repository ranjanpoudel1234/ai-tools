# AI Tools & Claude Code Configuration

Personal repository for Claude Code CLI customization, AI experimentation, and enterprise development automation.

## What's Inside

### 🤖 Custom Claude Code Agents
Specialized sub-agents for complex tasks:
- **meta-agent** - Creates new sub-agents from descriptions
- **unit-test-writer** - Generates comprehensive unit tests

### 🛠️ Skills & Commands
User-invocable skills (invoke with `/skill-name`):
- **backend-code-reviewer** - C#/.NET code review with Microsoft standards
- **react-ux-code-reviewer** - React/TypeScript UX and accessibility review
- **domain-driven-design-guru** - DDD, Clean Architecture, and CQRS guidance
- **competency-tracker** - Professional competency documentation
- **read-avro-files** - Apache Avro file processing

Development workflows:
- `/plan-work` - Create detailed implementation plans
- `/implement-plan` - Execute plans with progress tracking
- `/create-adr` - Generate Architecture Decision Records
- `/c4-documenter` - Create C4 architecture diagrams
- `/new-worktree` - Git worktree management

### 📚 AI Learning Journey
Structured 12-month learning plan covering:
- **Phase 1**: GenAI & Enterprise Integration (LLMs, RAG, Agent frameworks)
- **Phase 2**: ML/DL Fundamentals (Neural networks, transformers)
- **Phase 3**: Advanced Topics (Multi-agent systems, AI safety)

See [`ClaudeCode/configuration/plans/ai-learning.md`](ClaudeCode/configuration/plans/ai-learning.md)

### ⚙️ Configuration
- **settings.json** - Permissions, hooks, and statusline config
- **statusline.ps1** - PowerShell statusline integration
- **notify-task-complete.ps1** - Task completion notifications

## Quick Start

### Using Custom Skills
```bash
claude /backend-code-reviewer    # Review C#/.NET code
claude /plan-work                # Plan implementation
claude /create-adr               # Create ADR
```

### Creating New Agents
```bash
claude /meta-agent               # Launch agent creator
```

### AI Learning Projects
Organize projects under:
```
claude/tasks/ai-learning-progress/phaseN/
├── experiments/         # Proof-of-concept code
├── projects/           # Production-ready implementations
└── *.md               # Documentation and notes
```

## Tech Stack Focus

- **Backend**: C#, .NET, Azure, Domain-Driven Design
- **Frontend**: React, TypeScript, Material-UI
- **AI/ML**: Semantic Kernel, LangChain, Azure OpenAI, ML.NET
- **Tools**: Claude Code CLI, PowerShell, Git

## Documentation

- [`CLAUDE.md`](CLAUDE.md) - Comprehensive guide for Claude Code instances
- [`ClaudeCode/configuration/`](ClaudeCode/configuration/) - All configuration files

## Repository Structure

```
ai-tools/
├── ClaudeCode/configuration/
│   ├── agents/          # Custom sub-agents
│   ├── commands/        # Workflow commands
│   ├── skills/          # User-invocable skills
│   ├── plans/          # Planning documents
│   └── prompts/        # Prompt templates
├── CLAUDE.md           # Claude Code guidance
└── README.md           # This file
```

---

**Note**: This is a personal development repository focused on Claude Code customization and AI learning experimentation.
