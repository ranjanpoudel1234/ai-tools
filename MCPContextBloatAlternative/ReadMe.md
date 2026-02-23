# Beyond MCP: Alternatives to Prevent Context Bloat

When MCP servers automatically expose all their tools, they can **bleed context tokens** before your AI agent even starts working. This guide covers four proven approaches to connecting AI agents to external tools, with trade-offs for each.

**Source**: [IndyDevDan - Beyond MCP Video](https://www.youtube.com/watch?v=OIKTsVjTVJE) | [GitHub Repository](https://github.com/disler/beyond-mcp)

---

## The Problem: Context Bleeding

MCP servers use auto-discovery to expose tools to LLMs. While convenient, this means:
- All tool definitions are loaded into context immediately
- Token consumption happens before any work begins
- Complex MCP servers with many tools can consume significant context budget

## The Four Approaches

### 1. MCP Server (Traditional)

**Architecture:** `Claude/LLM → MCP Protocol → MCP Server → subprocess → CLI → API`

| Pros | Cons |
|------|------|
| Standardized integration across LLM clients | Instant context loss on each invocation |
| Auto-discovery of all tools | High token consumption |
| Clean protocol abstractions | Subprocess overhead |
| Multi-agent ready | Limited customization |

**Best for:** Multi-client support, standardized protocols, when context degradation is acceptable

---

### 2. CLI as Tools

**Architecture:** `Claude → subprocess → CLI → Direct HTTP → API`

| Pros | Cons |
|------|------|
| Single source of truth | Requires subprocess calls |
| Dual output (human + JSON) | Medium context consumption |
| Smart caching support | Manual tool registration |
| ~50% less context than MCP | |
| Build once, use everywhere | |

**Best for:** Direct API control, CLI + programmatic hybrid access, caching requirements

**Example structure:**
```
cli/
├── kalshi_cli.py      # 552 lines, 13 commands
├── cache/             # 6-hour TTL pandas cache
└── README.md
```

---

### 3. File System Scripts (Progressive Disclosure)

**Architecture:** `Claude → Read tool → Individual script → Embedded HTTP → API`

| Pros | Cons |
|------|------|
| Progressive disclosure | Code duplication |
| Minimal token consumption | More files to maintain |
| Complete isolation | Manual script selection |
| Maximum portability | |
| ~90% context savings | |

**Best for:** Context preservation critical, maximum portability, progressive disclosure needs

**Example structure:**
```
scripts/
├── status.py          # 200-300 lines each
├── markets.py         # Standalone, embedded HTTP
├── market.py          # No shared dependencies
├── orderbook.py
├── trades.py
├── search.py
├── events.py
├── event.py
├── series_list.py
└── series.py
```

**Key Pattern:** Each script is completely self-contained with its own HTTP client. Claude only reads the script it needs, preserving context.

---

### 4. Skills as Tools (Claude Code Ecosystem)

**Architecture:** `Claude (detects trigger) → Loads SKILL.md → Runs scripts → API`

| Pros | Cons |
|------|------|
| Autonomous skill discovery | Claude Code specific |
| Low context consumption | Platform lock-in |
| Team git collaboration | Requires skill setup |
| Trigger-based loading | |

**Best for:** Claude Code environments, autonomous skill discovery, team collaboration

**Example structure:**
```
.claude/skills/kalshi-markets/
├── SKILL.md           # Description & instructions
└── scripts/           # File system scripts
    ├── status.py
    ├── markets.py
    └── ...
```

---

## Trade-off Comparison Matrix

| Dimension | MCP Server | CLI | Scripts | Skills |
|-----------|------------|-----|---------|--------|
| **Agent-Invoked** | Yes | No | No | Yes |
| **Context Consumption** | High | Medium | Low | Low |
| **Customizable** | Limited | Yes | Yes | Yes |
| **Portability** | Low | Medium | High | High |
| **Simplicity** | High | Medium | Medium | Medium |
| **Multi-Agent Ready** | Yes | No | No | Partial |

---

## The 80/10/10 Rule

### For Using External Tools
- **80% MCP servers** - When tools already exist
- **15% CLI** - When you need more control
- **5% Scripts/Skills** - When context is critical

### For Building New Tools
- **80% CLI + Prime Prompt** - Start here, most flexible
- **10% Wrapped MCP** - When multi-agent scale needed
- **10% Scripts/Skills** - When context preservation critical

---

## Recommended Progression

```
Start Simple                    Scale When Needed
     │                                │
     ▼                                ▼
┌─────────┐    Context      ┌──────────────┐    Multi-Agent    ┌─────────┐
│   CLI   │ ──────────────► │   Scripts    │ ────────────────► │   MCP   │
└─────────┘    Matters      └──────────────┘      Scale        └─────────┘
```

1. **Start with CLI** - Build once, use everywhere (you, team, agents)
2. **Extend to Scripts** - When context preservation matters
3. **Wrap in MCP** - Only when you need multi-agent scale

---

## Implementation Tips

### Context Engineering > Context Window Size
Managing what goes into context matters more than having a larger window.

### Progressive Disclosure Pattern
```python
# Instead of loading all tools at once:
# BAD: Load 15 tool definitions (high token cost)

# Load only what's needed:
# GOOD: Agent reads single script when needed (minimal tokens)
```

### Caching Strategy for Search-Heavy Operations
```python
# Cache API responses locally (e.g., pandas DataFrame)
# - First call: 2-5 minutes to fetch and index
# - Subsequent calls: Instant from cache
# - TTL: 6 hours before auto-refresh
```

### Path Resolution for Cross-Directory Invocation
```python
from pathlib import Path
script_dir = Path(__file__).resolve().parent
```

---

## Key Takeaways

1. **Most engineers overthink agent tooling** - Start simple
2. **Context bleeding is real** - MCP auto-discovery has costs
3. **CLI tools are underrated** - Maximum flexibility, good context efficiency
4. **Scripts enable progressive disclosure** - Only load what you need
5. **Skills are powerful but lock you in** - Consider portability needs

![
    
](image.png)
---

## Resources

- [GitHub: disler/beyond-mcp](https://github.com/disler/beyond-mcp) - Full implementation examples
- [Anthropic: Code execution with MCP](https://www.anthropic.com/engineering)
- [Mario Zechner: What if you don't need MCP Server?](https://mariozechner.at/posts/2025-11...)
- [Tactical Agentic Coding](https://agenticengineer.com/tactical-agentic-coding/)
- [Vitalik: From prediction markets to info finance](https://vitalik.eth.limo/general/2024/11/09/infofinance.html)
