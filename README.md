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

## Agent Creation from MCP Tools

The [`AgentCreation/`](AgentCreation/) folder contains documentation and samples demonstrating how to create custom agents that leverage MCP (Model Context Protocol) tools.

### Workflow Overview

1. **Discover available MCP tools** - Ask Claude to list tools from your configured MCP servers (e.g., ElevenLabs, Firecrawl)
2. **Identify target tools** - Select the specific MCP tools you want your agent to use
3. **Create agent via meta-agent** - Provide a prompt describing what you want the agent to do and which tools it should use
4. **Customize the output** - Tweak the generated agent definition to fit your specific needs

### Folder Contents

| File | Description |
|------|-------------|
| `ReadMe.md` | Step-by-step guide with screenshots |
| `meta-agent-creator.md` | Sample meta-agent that creates new sub-agents |
| `work-summary-provider-agent.md` | Example agent using ElevenLabs MCP tools for TTS summaries |

### Example: Creating an Audio Summary Agent

The included `work-summary-provider-agent.md` demonstrates creating an agent that:
- Uses `mcp__ElevenLabs__text_to_speech` to generate audio
- Uses `mcp__ElevenLabs__play_audio` to play the result
- Provides concise work completion summaries via audio

This pattern can be applied to any MCP server tools available in your configuration.

### Key Tips

- Use **"Proactively"** and **"IMPORTANT"** keywords in agent descriptions for automatic delegation
- Be concise in agent prompts - aim for 2 sentences max when passing context
- The meta-agent reads the latest Claude Code documentation to ensure proper format

**Source**: [Claude Code Hooks Mastery](https://github.com/disler/claude-code-hooks-mastery)

## MCP Context Bloat Alternatives

MCP servers can **bleed context tokens** through auto-discovery before your agent starts working. The [`MCPContextBloatAlternative/`](MCPContextBloatAlternative/) folder documents four approaches to agent tooling with different context/complexity trade-offs.

| Approach | Context Cost | Best For |
|----------|-------------|----------|
| **MCP Server** | High | Multi-agent scale, standardized protocols |
| **CLI as Tools** | Medium | Build once, use everywhere |
| **Scripts (Progressive Disclosure)** | Low (~90% savings) | Context-critical workloads |
| **Skills** | Low | Claude Code ecosystem |

**The 80/10/10 Rule**: Start with CLI (80%), extend to scripts when context matters (10%), wrap in MCP only for multi-agent scale (10%).

**Source**: [IndyDevDan - Beyond MCP](https://github.com/disler/beyond-mcp)

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
- [`AgentCreation/`](AgentCreation/) - Guide for creating agents from MCP tools
- [`MCPContextBloatAlternative/`](MCPContextBloatAlternative/) - Alternatives to MCP for context-efficient tooling

## Repository Structure

```
ai-tools/
├── AgentCreation/              # Agent creation guide & samples
├── MCPContextBloatAlternative/ # MCP alternatives for context efficiency
├── ClaudeCode/configuration/
│   ├── agents/                 # Custom sub-agents
│   ├── commands/               # Workflow commands
│   ├── skills/                 # User-invocable skills
│   ├── plans/                  # Planning documents
│   └── prompts/                # Prompt templates
├── CLAUDE.md                   # Claude Code guidance
└── README.md                   # This file
```

---

**Note**: This is a personal development repository focused on Claude Code customization and AI learning experimentation.
